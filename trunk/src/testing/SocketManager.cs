// Copyright (c) 2005 Josef Schmeisser
//
// This program is free software; you can reditribute it and/or modify
// it under the terms of the GNU General Public License Version 2 as published by
// the Free Software Foundation.
//
// This program is distrubuted in the hope that it will be useful,
// but WITHOUT ANY WRRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

using System;
using System.Threading;
using System.Collections;

using System.Net.Sockets;
using System.Diagnostics;
using IRC;

using System.Text;
using Network;
using Service;

namespace Other
{

	public enum MessagePriority
	{
		Low = 1,
		Normal = 2,
		High = 3,
		RealTime = 4
	}

	/// <summary>
	/// Description of SocketManager.
	/// </summary>
	public class SocketManager : MarshalByRefObject, IDisposable // TODO: error handler für select()
	{
		private Hashtable listeners = Hashtable.Synchronized(new Hashtable());
		private Hashtable workers = Hashtable.Synchronized(new Hashtable());
		private ManualResetEvent listen = new ManualResetEvent(false);

		private MessagePriority _standardPriority;
		private bool worke = false;
		private bool run = true;

		private Listener listener_thread;
		private Writer writer_thread;
		private Reader reader_thread;

		// state for "writer"
//		private object writerstate = new object();
//		private object readerstate = new object();
//		private object listenerstate = new object(); // veraltet

		#region Events
		public event ReceivedEvent Received;
		#endregion

		public SocketManager()
		{
			this._standardPriority = MessagePriority.Normal;/*/MessagePriority.RealTime;*/

			// listener
			this.listener_thread = new Listener(this);
			this.listener_thread.Start();

			// reader
			this.reader_thread = new Reader(this);
			this.reader_thread.Start();

			// writer
			this.writer_thread = new Writer(this);
			this.writer_thread.Start();
		}
/* obsolete
		private void InvokeReceived(IWorker worker, string text)
		{
			if (this.Received != null)
				this.Received(worker, text);
		}
*/
		public void Dispose()
		{
			Debug.WriteLine(this + ".Dispose()");
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			try
			{
				this.workers.Clear();
				this.listeners.Clear();

				this.listener_thread.Stop();
				this.reader_thread.Stop(); // TO/DO: bug
				this.writer_thread.Stop();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public IListener GetListener(Socket sock)
		{
			if (this.listeners.ContainsKey(sock))
				return (IListener)this.listeners[sock];
			return null;
		}

		public void AddListener(IListener listener)
		{
			if (listener.Listeners == null)
				throw new ArgumentNullException("listener.Listeners");

			if (listener.Listeners.Count == 0)
				Debug.WriteLine(this + ".AddListener: warning, adding empty list ...");

			lock (this.listeners)
			{
				if (this.listeners.ContainsValue(listener))
					throw new InvalidOperationException("listener bereits in liste!");

				foreach (Socket sock in listener.Listeners)
				{
					this.listeners.Add(sock, listener);
				}
				Monitor.PulseAll(this.listeners);
			}
		}

		public void UpdateListener(IListener listener)
		{
			Debug.WriteLine(this +".UpdateListerner("+listener.GetType()+")");
			lock (this.listeners)
			{
				foreach (Socket sock in listener.Listeners)
				{
					//BUGFIX
					if (this.listeners.ContainsKey(sock))
						continue;
					//
					this.listeners.Add(sock, listener);
				}
			}

			lock (this.listeners)
			{
				foreach (DictionaryEntry entry in ((Hashtable)this.listeners.Clone()))
				{
					if (entry.Value == listener)
					{
						if (!listener.Listeners.Contains(entry.Key))
							this.listeners.Remove(entry.Key);
					}
				}
			}
		}

		public void RemoveListener(IListener listener) // TO/DO: sicherheitsfunktion einbauen das RemoveListener solange blockt bis Listener._Worker durch
		{
			Debug.WriteLine(this +".RemoveListener("+listener.GetType()+")");
			try
			{
			this.listen.WaitOne(); // neu: siehe oben, sollte das problem beheben
			lock (listener)
			{
				lock (this.listeners)
				{
					IDictionaryEnumerator enu = ((Hashtable)this.listeners.Clone()).GetEnumerator(); // wir können nur eine Kopie durchlaufen

					while(enu.MoveNext())
					{
						if (enu.Value == listener)
						{
							this.listeners.Remove(enu.Key);
							return;
						}
					}
				}
			}
			}
			catch (Exception e)
			{
				StackTrace st = new StackTrace(true);

				Console.WriteLine(e);
				Console.WriteLine(st);
			}
		}

		public void AddConnection(IWorker worker)
		{
			if (worker is IConnection)
				Debug.WriteLine(this + ": AddConnection, ID: " + ((IConnection)worker).ID);

			worker.Socket.Blocking = false; // set all new sockets nonblocking
			lock (this.workers)
			{
				this.workers.Add(worker.Socket, worker);
				Monitor.PulseAll(this.workers);
			}			
/*			lock (this.readerstate)
			{
				Monitor.PulseAll(this.readerstate);
			}
			lock (this.writerstate)
			{
				Monitor.PulseAll(this.writerstate);
			}*/
		}

		public void RemoveConnection(IWorker worker)
		{
			if (worker is IConnection)
				Debug.WriteLine(this + ": RemoveConnection, ID: " + ((IConnection)worker).ID);
	
			lock (this.workers)
			{
				if (this.workers.ContainsKey(worker.Socket))
				{
					this.workers.Remove(worker.Socket);
				}
			}
		}

		public void Send(IWorker worker, string text, MessagePriority priority)
		{
			if (worker == null)
				throw new ArgumentNullException("worker");

			byte[] data = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding.GetBytes(text);
			this.Send(worker, data, priority);
		}

		public void Send(IWorker worker, byte[] data, MessagePriority priority)
		{
			if (priority == MessagePriority.RealTime)
			{
				this.InternalSend(worker, data);
			}
			else
			{
				HandleData handledata = new HandleData(worker, data);
				((Queue)this.writer_thread.SendBuffer[priority]).Enqueue(handledata);

				lock (this.writer_thread.SendBuffer)
				{
					Monitor.Pulse(this.writer_thread.SendBuffer);
					Monitor.Wait(this.writer_thread.SendBuffer);
					Monitor.Pulse(this.writer_thread.SendBuffer);
				}
			}
		}

		private void InternalSend(IWorker worker, byte[] data)
		{
			try
			{
				// statistics
				stat.traffic += data.Length;
				//

				worker.Socket.Send(data);
			}
			catch (Exception e)
			{
				Console.WriteLine(this + ": Exception! : " + e.ToString());
				// TODO: close link
				// OnError(worker) eg
			}
		}

		/// <summary>
		/// </summary>
		public virtual MessagePriority Priority
		{
			get
			{
				return this._standardPriority;
			}
			set
			{
				this._standardPriority = value;
			}
		}

		internal class Listener
		{
			SocketManager socketManager;
			Thread thread;

			private bool started = false;

			//
			private bool run = true;
			//

			public Listener(SocketManager sm)
			{
				if (sm == null)
					throw new ArgumentNullException("sm");

				this.socketManager = sm;
			}

			public void Start()
			{
				if (this.started)
					return;

				this.started = true;

				// start the _Woker loop
				this.thread = new Thread(new ThreadStart(this._Worker));
				this.thread.Name = this.ToString();
				this.thread.IsBackground = true;

				this.thread.Start();
			}

			public void Stop()
			{
				if (!this.started)
					return;

				this.thread.Abort();
				this.started = false;
			}

			private void _Worker()
			{
				try
				{
					while (run)
					{
						if (this.socketManager.listeners.Count < 1)
						{
							Monitor.Enter(this.socketManager.listeners);
							Monitor.Wait(this.socketManager.listeners);
							Monitor.Exit(this.socketManager.listeners);
							continue;
						}

						ArrayList readList = null;
						lock (this.socketManager.listeners)
						{
							readList = new ArrayList(this.socketManager.listeners.Count);
							foreach (DictionaryEntry entry in this.socketManager.listeners)
							{
								readList.Add(entry.Key);
							}
						}						try
						{
/*							ArrayList readList = new ArrayList(this.socketManager.listeners.Count);
							foreach (DictionaryEntry entry in this.socketManager.listeners)
							{
								readList.Add(entry.Key);
							}
*/
							if (readList.Count > 1)
							{
								Socket.Select(readList, null, null, 1000*1000);

								if (readList.Count == 0) // optimierung
									continue;

								foreach (Socket sock in readList)
								{
									Socket client = sock.Accept();
									Debug.WriteLine(this + ": Run AddConnction");
									((IListener)this.socketManager.listeners[sock]).AddConnection(client);
								}
							}
							else
							{
								Socket sock = (Socket)readList[0];

								if (!sock.Poll(1000*1000, SelectMode.SelectRead))
									continue;

								Socket client = sock.Accept();
								Debug.WriteLine(this + ": Run AddConnction");
								((IListener)this.socketManager.listeners[sock]).AddConnection(client);
							}

							Debug.WriteLine(this + ": " + readList.Count + " listeners(s) accept");
							this.socketManager.listen.Set(); // neu
						}
						catch (SocketException e)
						{
							Debug.WriteLine("Invalide descriptor!!!" + e.ToString()); // TODO: warum kommt das???, aber nur wenn dancer connected
						}
					}
				}
				catch (ThreadAbortException)
				{
					Debug.WriteLine(this + ": thread wurde abgebrochen!");
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}

		internal class Reader
		{
			private SocketManager socketManager;
			private Thread thread;

			private bool started = false;
			private bool run = false;
//			private static int runR; // TODO: man kann keine zwei Reader starten

			public Reader(SocketManager sm)
			{
				if (sm == null)
					throw new ArgumentNullException("sm");

				this.socketManager = sm;
			}

			public void Start()
			{
				if (this.started)
					return;

				this.started = true;
//				runR = 1;
				this.run = true;

				this.thread = new Thread(new ThreadStart(this._Worker));
				this.thread.Name = this.ToString();
				this.thread.Priority = ThreadPriority.Highest;

				this.thread.Start();
			}

			public void Stop()
			{
				if (!this.started)
					return;

//				Interlocked.Exchange(ref runR, 0);
				this.run = false;
				Monitor.Enter(this.socketManager.workers);
				Monitor.PulseAll(this.socketManager.workers);
				Monitor.Exit(this.socketManager.workers);

				this.started = false;
			}

			private void _Worker()
			{
            			//while (runR == 1)
				while (this.run)
				{
					try
					{
						//Console.WriteLine("READER loop");
/*						if (this.socketManager.workers.Count == 0)
						{
							Thread.Sleep(3000);
							continue;
						}*/
						if (this.socketManager.workers.Count == 0)
						{
							lock (this.socketManager.workers)
							{
								// Wait for for a new connection
								Monitor.Wait(this.socketManager.workers);
								continue;
							}
						}

/*
						Monitor.Enter(this.socketManager.workers);
						if (this.socketManager.workers.Count == 0)
						{
							if (!Monitor.Wait(this.socketManager.workers, 1000000))
								continue;
							else
								Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!");
						}
						Monitor.Exit(this.socketManager.workers);
*/
						ArrayList readList = new ArrayList(this.socketManager.workers.Count);
						foreach (DictionaryEntry entry in this.socketManager.workers)
						{
							readList.Add(entry.Key);
						}

						Socket.Select(readList, null, null, 1000000);

						if (readList.Count == 0) // optimierung
							continue;

						this._Handler(readList);
					}
					catch (Exception e)
					{
						Console.WriteLine(this + ": " + e);
					}
				}
				Console.WriteLine(this + ": end of _Worker");
			}

			private void _Handler(object obj)
			{
				ArrayList readList = (ArrayList)obj;
				foreach (Socket sock in readList)
				{
					try
					{
						Monitor.Enter(sock);
						StringBuilder sb = new StringBuilder();
						byte[] re = new byte[sock.Available]; // BUGFIX: 256 bytes to sock.Available

						if (sock.Available == 0)
						{
							continue;
						}
						while (sock.Available > 0)
						{
							// BUGFIX: 256 bytes to sock.Available
							sock.Receive(re, 0, sock.Available, SocketFlags.None);

							sb.Append(((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding.GetString(re));
						}
						Monitor.Exit(sock);

						if (this.socketManager.listeners.ContainsKey(sock))
							continue;

						// Wichtig: hier nicht einen Thread starten; z.B. für nick/user paare
						// wenn user als erstes ankommt kann es zu unberechenbaren situationen kommen
						((IWorker)this.socketManager.workers[sock]).Execute(sb.ToString());
					}
					catch (ObjectDisposedException od)
					{
						Debug.WriteLine(this + ": disposed socket; removing ...");

						IListener lt = this.socketManager.GetListener(sock);
						if (lt != null)
						{
							lt.RemoveConnection(sock);
							((IWorker)this.socketManager.workers[sock]).Dispose();
						}
						else
						{
							this.socketManager.workers.Remove(sock);
						}

						continue;
					}
				}
			}
		}

		// in baseconn eine SendLine func machen die die daten an den sm weitergibt
		// außer bei realtime
		// sm tut sie dan in die poll und arbeitet sie ab
		// TODO: bug bei vielen verbindungen > 100
		internal class Writer
		{
			SocketManager socketManager;
			Thread thread;

			internal Hashtable SendBuffer = Hashtable.Synchronized(new Hashtable());
			bool started = false;
			bool run = true;

			public Writer(SocketManager sm)
			{
				this.socketManager = sm;
				
				this.SendBuffer.Add(MessagePriority.High, Queue.Synchronized(new Queue()));
				this.SendBuffer.Add(MessagePriority.Normal, Queue.Synchronized(new Queue()));
				this.SendBuffer.Add(MessagePriority.Low, Queue.Synchronized(new Queue()));
			}

			public void Start()
			{
				if (this.started)
					return;

				this.started = true;
				this.run = true;

				// start the _Woker loop
				this.thread = new Thread(new ThreadStart(this._Worker));
				this.thread.Name = "Writer " + this;//this.socketManager.ID;
				this.thread.IsBackground = true;
				this.thread.Start();
			}

			public void Stop()
			{
				if (!this.started)
					return;

				this.run = false;
				Monitor.Enter(this.socketManager.workers);
				Monitor.PulseAll(this.socketManager.workers);
				Monitor.Exit(this.socketManager.workers);

				Monitor.Enter(this.SendBuffer);
				Monitor.PulseAll(this.SendBuffer);
				Monitor.Exit(this.SendBuffer);
				this.started = false;
			}

			private void _Worker()
			{
				try
				{
					while (this.run)
					{
						if (this.socketManager.workers.Count == 0)
						{
							lock (this.socketManager.workers)
							{
								// Wait for for a new connection
								Monitor.Wait(this.socketManager.workers);
							}
						}

						while (this.socketManager.workers.Count > 0)
						{
							this.Check(); // Check for status and send data

							// Wait for new data
							Monitor.Enter(this.SendBuffer);
							Monitor.Pulse(this.SendBuffer);
							Monitor.Wait(this.SendBuffer);
							Monitor.Exit(this.SendBuffer);
						}
					}
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
				Debug.WriteLine(this + "._Worker() exit");
			}

			private void Check()
			{
				if (((Queue)this.SendBuffer[MessagePriority.High]).Count > 0)
					this.SendQueue((Queue)this.SendBuffer[MessagePriority.High]);

				if (((Queue)this.SendBuffer[MessagePriority.Normal]).Count > 0)
					this.SendQueue((Queue)this.SendBuffer[MessagePriority.Normal]);

				if (((Queue)this.SendBuffer[MessagePriority.Low]).Count > 0)
					this.SendQueue((Queue)this.SendBuffer[MessagePriority.Low]);
			}

#if false
			private void InternalSend(IWorker worker, string text) // obsolete
			{
//				Trace.WriteLine(this + ".InternalSendLine: " + text);
				try
				{
 					byte[] data = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding.GetBytes(text);
 
					/// statistics
					stat.traffic += data.Length;
					///

					worker.Socket.Send(data);
				}
				catch (Exception e)
				{
					Console.WriteLine(this + ": Exception! : " + e.ToString());
				}
			}
#endif

			private void SendQueue(Queue queue)
			{
				while (queue.Count > 0)
				{
					HandleData handledata = (HandleData)queue.Dequeue();
					//this.InternalSend(data.worker, data.text);
					this.socketManager.InternalSend(handledata.worker, handledata.data);
				}
			}
		}
	}
}
