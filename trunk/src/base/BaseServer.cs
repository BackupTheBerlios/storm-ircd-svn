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
using System.Net;
using System.Text;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.Net.Sockets;
using System.Collections;

// intern
using Other;
using Service;

namespace Network
{
	public delegate void AcceptedConnectionHandler(BaseConnection connection);

	public class AcceptObject
	{
		public Socket listener;
		public ManualResetEvent allDone = new ManualResetEvent(false);
	}

	/// <summary>
	/// Zusammenfassung fuer BaseServer.
	/// </summary>
	public abstract class BaseServer : /*test: MarshalByRefObject,*/ IServer, IDisposable
	{
		#region Fields
		private ArrayList clientsToRemove = new ArrayList(); // TODO: entfernen
		private ArrayList _clients; // all clients , servers and users 
		private ManualResetEvent allDone = new ManualResetEvent(false);
		private Timer checkTimer;
		private bool disposed = false;

		// new
		private IInterpretor _interpretor;
		private ArrayList _idseed = new ArrayList();
		#endregion

		///new 
		#if UNSTABLE
		public SocketManager _socketManager;
		private ArrayList _listeners = new ArrayList();
		#endif
		///

		#region Events
//		public event ReceivedEvent Received;
		public event AcceptedConnectionHandler AcceptedConnection;
		#endregion

		#region Construct
		public BaseServer()
		{
#if UNSTABLE
// alle encoding von Settings lesen -> alle gleich und ein runtime switch ist möglich
			this.InternalConstruct(new SocketManager());
#else
			this._clients = new ArrayList();

//			this._encoding = Encoding.GetEncoding(28591);// iso_8859-1
#endif
		//	this.checkTimer = new Timer(new TimerCallback(this.CheckStatus), null, 20000, 20000);
		}

		~BaseServer()
		{
			Console.WriteLine(this + ": Destrukor [ OK ]");
		}
#if UNSTABLE
		public BaseServer(SocketManager sm)
		{
			this.InternalConstruct(sm);
		}
#endif
		#endregion

#if UNSTABLE
		private void InternalConstruct(SocketManager sm)
		{
			this._clients = new ArrayList();
			this._socketManager = sm;
			this._socketManager.AddListener(this);
			this._socketManager.Received += new ReceivedEvent(this.HandleRecieved);
		}
#endif

		public virtual void HandleRecieved(IConnection connection, string text)
		{
			connection.Execute(text);
		}

		#if UNSTABLE
		#else
		// nur um als Release kompilieren zu können
		public virtual void HandleRecieved(object obj)
		{
			HandleData dat = (HandleData)obj;
			this.HandleRecieved(dat.worker, dat.text);
		}
		#endif

		public void errorhandler()
		{
		//create new StartListening
		//socket.shutdown in dispose
		}

		#region Methods
		public virtual void RemoveConnection(IConnection connection)
		{
			Console.WriteLine("BaseServer RemoveConnection()");

			lock (this._clients)
			{
				this._clients.Remove(connection); //remove all handels
				this._socketManager.RemoveConnection(connection); // neu sollte fehler behen
//				connection.Dispose(); // TODO: soll hier raus; RemoveConnection dient nur dazu das object aus der strucktur zu entfernen
			}
		}
		#region private
/*		private void client_Received(string text)
		{
			Console.WriteLine("event:"+text);

			if (this.Received != null)
				this.Received(text);
		}
*/
		public bool SeedID(int id)
		{
			if (this._idseed.Count == Int32.MaxValue)
			{
				Debug.WriteLine(this + ": Clear ID-Seed...");
				this._idseed.Clear();
				return false;
			}
			foreach (int seed in this._idseed)
			{
				if (id == seed)
					return true;
			}
			return false;
		}

		private void OnAcceptedConnection(BaseConnection connection)
		{
			if (this.AcceptedConnection != null)
			{
				this.AcceptedConnection(connection);
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
#if UNSTABLE
					this.StopListening();
					this._socketManager.Dispose();
#endif
				}
			}
			this.disposed = true;
		}
		#endregion

		#region protected
		protected virtual void AcceptCallback(IAsyncResult ar)
		{
//			if (true)
//				;
			// todo
			// it's a base implemtation and virtual
			AcceptObject obj = ((AcceptObject)ar.AsyncState);
			obj.allDone.Set();
			Socket handler = obj.listener.EndAccept(ar);

			Console.WriteLine("Accept connection from: " + handler.RemoteEndPoint.ToString());
			lock (this._clients)
			{
				BaseConnection connection = new BaseConnection(handler, this); // use the same interpretor

				this.AddID(connection.ID);
				// OnConnected
				this._clients.Add(connection);
				this.OnAcceptedConnection(connection);
			}
		}

		protected void AddID(int id)
		{
			lock (this._idseed)
			{
				this._idseed.Add(id);
			}
		}
		#endregion

		#region public
		public void CheckStatus(object value)
		{
			Console.WriteLine("CheckStatus");
			lock (this.clientsToRemove)
			{
				lock (this._clients)
				{
					if (this.clientsToRemove.Count > 0)
					{
						foreach (IConnection client in this.clientsToRemove)
						{
							Console.WriteLine("Key: " + client.ID);
							this._clients.Remove(client);
						}
						this.clientsToRemove.Clear();
					}
				}
			}

			Console.WriteLine("End CheckStatus");
		}

		public virtual void Close()
		{
			this.Dispose();
		}

		public virtual void Dispose()
		{
			lock (this._clients)
			{
				try
				{
					IEnumerator enumerator = ((ArrayList)this._clients.Clone()).GetEnumerator();
					while (enumerator.MoveNext())
					{
						IConnection con = (IConnection)enumerator.Current;
						this.RemoveConnection(con);
						con.Dispose();
					}
#if UNSTABLE
					this._socketManager.RemoveListener(this);
#endif
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
// TODO: überprüfen
#if UNSTABLE
			this.Dispose(true);
#endif
		}

		public bool HasConnectin(IConnection con)
		{
			foreach (IConnection client in this._clients)
			{
				if (client == con)
					return true;
			}

			return false;
		}

		public bool HasID(int id)
		{
			foreach (IConnection client in this._clients)
			{
				if (client.ID == id)
					return true;
			}

			return false;
		}

		public void Send(string text)
		{
			lock (this._clients)
			{
				try
				{
					IEnumerator enumerator = this._clients.GetEnumerator();
					while (enumerator.MoveNext())
					{
						this.SendTo(text, ((IConnection)enumerator.Current).ID);
					}
				}
				catch (InvalidOperationException)
				{
				}
				catch (Exception e)
				{
					Console.WriteLine(e.ToString());
				}
			}
		}

		public void SendTo(string text, int id)
		{
			lock (this._clients)
			{
				foreach (BaseConnection client in this._clients)
				{
					if (client.ID == id)
					{
						try
						{
							client.Send(text);
						}
						catch (SocketException e)
						{
							lock (this.clientsToRemove)
							{
								this.clientsToRemove.Add(client);
							}
							Console.WriteLine(e.ToString());
						}
					}
				}
			}
		}

		public void StartListening(EndPoint ep)//int port)
		{
			if (ep == null)
				throw new ArgumentNullException("ep");

			if (ep is IPEndPoint)
				Console.WriteLine("BaseServer.StartListening on {0}:{1}", ((IPEndPoint)ep).Address, ((IPEndPoint)ep).Port);

			//IPEndPoint localEndPoint = new IPEndPoint(/*IPAddress.Any*/ IPAddress.Parse("10.0.1.3"), port);

			Socket listener = new Socket(AddressFamily.InterNetwork,
					SocketType.Stream, ProtocolType.Tcp);

			try
			{
				listener.Bind(ep);
				listener.Listen(500); // TODO: überprüfen

#if UNSTABLE
				// blockt nicht!
				this._listeners.Add(listener);
				this._socketManager.UpdateListener(this);
#else

				AcceptObject obj = new AcceptObject();
				obj.listener = listener;

				while (true)
				{
					obj.allDone.Reset();

					Console.WriteLine("Waiting for a Connection ...");
					listener.BeginAccept(new AsyncCallback(this.AcceptCallback), obj);

					obj.allDone.WaitOne();
				}
#endif
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		// Close all sockets in this._listeners
		public void StopListening()
		{
			lock (this._listeners)
			{
				foreach (Socket sock in this._listeners)
				{
					sock.Close();// TODO: Socket.Listener
				}
				this._listeners.Clear();
				this._socketManager.UpdateListener(this);
			}
		}

		public void StopListening(EndPoint ep)
		{
			lock (this._listeners)
			{
				foreach (Socket sock in ((ArrayList)this._listeners.Clone()))
				{
					if (ep == sock.LocalEndPoint)
					{
						// Cleanup
						this._listeners.Remove(sock);
						this._socketManager.UpdateListener(this);
						sock.Close(); // TODO: BUG: Socket.Listener liefert einen fehler weil sock disposed ist
					}
				}
			}
		}
		#endregion
		#endregion

		#region Properties
		protected virtual ArrayList Clients
		{
			get
			{
				return this._clients;
			}
		}

		protected virtual ArrayList ClientsToRemove
		{
			get
			{
				return this.clientsToRemove;
			}
		}

		public virtual Encoding Encoding
		{
			get
			{
/*				Trace.WriteLine("BaseServer.Encoding is being obsolete: use Settings.Encoding in future versions.");
				lock (this._encoding)
				{
					return this._encoding;
				}*/
				return ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding;
			}
			set
			{
/*				lock (this)
				{
					this._encoding = value;
				}*/
				((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding = value;
			}
		}

		public SocketManager SocketManager
		{
			get
			{
				return this._socketManager;
			}
		}

		public ArrayList Listeners // TODO: protected machen
		{
			get
			{
				return this._listeners;
			}
		}

		public virtual void AddConnection(Socket client)
		{
			// throw new NotImplementedException();
			Debug.WriteLine(this + ".AddConnection from socket");

//			Socket handler = client.Accept();

			Console.WriteLine("Accept connection from: " + client.RemoteEndPoint.ToString());
			lock (this._clients)
			{
				BaseConnection connection = new BaseConnection(client, this); // use the same interpretor

				this.AddID(connection.ID);
				// OnConnected
				this._clients.Add(connection);
				this.OnAcceptedConnection(connection);
			}
		}

		public virtual void AddConnection(IConnection connection)
		{
			if (this._clients.Contains(connection))
			{
				throw new InvalidOperationException("connetion bereits vorhanden!");
			}

			lock (this._clients)
			{
				if (!this.HasID(connection.ID))
				{
					this.AddID(connection.ID);
					this._clients.Add(connection);
					this.OnAcceptedConnection((BaseConnection)connection);
				}
				else
				{
					throw new InvalidOperationException("ID bereits vorhanden!");
				}
			}
//
		}

		public virtual void RemoveConnection(Socket client)
		{
			Console.WriteLine("BaseServer RemoveConnection(Socket)");

			lock (this._clients)
			{
				foreach (IConnection connection in this._clients)
				{
					if (connection.Socket == client)
					{
						this.RemoveConnection(connection);
						return;
					}
				}
			}
		}

		public int[] IDSeed
		{
			get
			{
				return (int[])this._idseed.ToArray(typeof(int));
			}
		}

		/// <summary>
		/// Return or set the current interpretor
		/// </summary>
		public IInterpretor Interpretor
		{
			get
			{
				return this._interpretor;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("Interpretor");

				if (this._interpretor == null)
				{
					this._interpretor = value;
					return;
				}
				lock (this._interpretor)
				{
					this._interpretor = value;
				}
			}
		}
		#endregion
	}
}
