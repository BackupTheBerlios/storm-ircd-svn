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
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;

// compressing
//using ICSharpCode.SharpZipLib.GZip;
//using ICSharpCode.SharpZipLib.BZip2;

// intern
using Other;
using Service;

namespace Network
{
//	internal delegate void HandlerProc(string text);
	//public delegate void ReceivedEvent(string text); //überholt

	public class StateObject
	{
		public ManualResetEvent allDone = new ManualResetEvent(false);

		public const int BufferSize = 1024;

		public byte[] buffer = new byte[BufferSize];

		public StringBuilder sb = new StringBuilder();
	}

	/// <summary>
	/// Zusammenfassung fuer Connection.
	/// </summary>
	public class BaseConnection : MarshalByRefObject, IConnection, IDisposable // iCloseable
	{
		#region Fields
//		private Encoding _encoding;

		private Random rnd = new Random();
		private bool available = false;
		//private int _id;
		protected int _id;
		private IServer _server;
		private Socket handler;
		private bool disposed = false;
		#endregion

		#region Events
		public event ReceivedEvent Received;
		#endregion

		///
		static long traffic;
		bool run = true;
		///

		#region Construct
		/// <summary>
		/// Add an socket to "server"-Server-Instance and register it
		/// </summary>
		/// <param name="ep">Socket to register</param>
		/// <param name="server">Server-Instance</param>
		public BaseConnection(Socket client, IServer server)
		{
			Debug.WriteLine("IRCConnectin Konstruktor");
			if (client == null)
				throw new ArgumentNullException("client");

			if (server == null)
				throw new ArgumentNullException("server");

//			this._encoding = Encoding.GetEncoding(28591);// iso_8859-1
			this.handler = client;
			this._server = server;
/*
			int id = this.rnd.Next();
			while (server.HasID(id) || server.SeedID(id)) // new seed id
			{
				id = this.rnd.Next();
			}

			this._id = id;

#if UNSTABLE
			SocketManager socketManager = ((BaseServer)this._server).SocketManager;
			socketManager.AddConnection(this);
#else
			ThreadPool.QueueUserWorkItem(new WaitCallback(this.RecieveProc), null); // to/do: 25 Thread Limit!!!!
#endif

			this.available = true;
			
			Console.WriteLine("new BaseConnection with id: {0}, is availbale", id);*/
			this.Initialize();
		}

		/// <summary>
		/// Create a TCP/IP Connection
		/// </summary>
		/// <param name="ep">IPEndPoint to connect</param>
		/// <param name="server">Server-Instance</param>
		protected BaseConnection(IPEndPoint ep, IServer server)
		{
			Console.WriteLine("new baseconnection from IPEndPoint");

			if (ep == null)
				throw new ArgumentNullException("ep");

			if (server == null)
				throw new ArgumentNullException("server");

			this._server = server;

			Socket tmp = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

// geht nicht			tmp.Blocking = false;
			tmp.Connect(ep);
			this.handler = tmp;

			this.Initialize();
		}		private void Initialize()
		{
			int id = this.rnd.Next();
			while (this._server.HasID(id) || this._server.SeedID(id)) // new seed id
			{
				id = this.rnd.Next();
			}

			this._id = id;

			SocketManager socketManager = ((BaseServer)this._server).SocketManager;
			socketManager.AddConnection(this);

			this.available = true;
			
			Console.WriteLine("new " + this + " with id: {0}, is availbale", id);
		}

		~BaseConnection()
		{
			Console.WriteLine(this + ": Destruktor [ OK ]");
		}
		#endregion

		/*
		public static virtual BaseConnection ConnectTo(IPAddress ip, IServer server)
		{
			return new this(null, server);
		}
*/
		#region Methods
		#region private
		// TODO: denn Dispose zweig nochmal überprüfen
		private void Dispose(bool disposing)
		{
			lock (this)
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.run = false; // 100 % Bugfix
						this.handler.Close();
					}
				}
				this.disposed = true;
			}
			Console.WriteLine("end of disposing: " + this.ID);
		}

		public void Execute(string text)
		{
			// TODO: dekompriemieren wenn Z-flag
			// wenn fehler beim dekompriemieren schauen ob es ohne so läuft siehe ERROR message
			// use the standard-interpretor
//			Debug.WriteLine(this + "Execute("+text+")");
			// bool ProcessCommand // true wenn funktioniert
			// TODO: Flood check hier einbauen evt. eine Ausnahme für ping.

#if UNSTABLE
			if (true) // TODO: H.P.
			{

			}
			// todo: string dekomprimieren wenn Z-Flag
			if (!this._server.Interpretor.ProcessCommand(this, text))
			{
				// TODO: nochmal unkomprimiert versuchen
				Debug.WriteLine(this + ": drop message: " + text);
			}
#else
			this._server.Interpretor.ProcessCommand(this, text);
#endif
		}

		private void InvokeReceived(IConnection connection, string text)
		{
			if (this.Received != null)
				this.Received(connection, text);
		}

//#if UNSTABLE
//#else
		internal virtual void RecieveProc(object dat)
		{
			Console.WriteLine("RecieveProc");
			StateObject state = new StateObject();

			try
			{
				while (this.run && handler.Connected) // 100 % Bugfix
				{
					Console.WriteLine("receiveproc()");
					state.allDone.Reset();

					handler.BeginReceive(state.buffer, 0, StateObject.BufferSize,
						0, new AsyncCallback(ReadCallback), state);

					state.allDone.WaitOne();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
//#endif

		#endregion

		#region protected
		protected virtual void HandleRecieved(string text)
		{
		Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!WArnig");
			//this.InvokeReceived(text);
		}

		//public virtual void SendChat(string message
		// TO/DO: BUGFIX: imported TicTacToeConnection Methode
		// TicTacToeConnection source:
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected virtual void ReadCallback(IAsyncResult ar)
		{
			try
			{
				string text1 = string.Empty;
				StateObject obj1 = (StateObject) ar.AsyncState;
				obj1.allDone.Set();
				int num1 = this.Socket.EndReceive(ar);

///
stat.traffic += num1;
///
				if (num1 == 0)
				{
					Console.WriteLine("Client has sent null bytes -> run = false");
					this.run = false;
					// todo: set_removeable 
				//	this._server.RemoveConnection(this as IConnection); // <-- server crash
				}
				if (num1 > 0)
				{
					obj1.sb.Append(this.Encoding.GetString(obj1.buffer, 0, num1));

					text1 = obj1.sb.ToString();
					if (num1 < 0x400)
					{
						Console.WriteLine("Read {0} bytes form socket. \nData: {1}", text1.Length, text1);
		                        obj1.sb = new StringBuilder();
						this.HandleRecieved(text1);
					}
					else
					{
						this.Socket.BeginReceive(obj1.buffer, 0, 0x400, SocketFlags.None, new AsyncCallback(this.ReadCallback), obj1);
					}
				}
			}
			catch (SocketException exception1)
			{
				Console.WriteLine(exception1.ToString());
			}
		}
		// end

		protected void SendCallback(IAsyncResult ar)
		{
			try
			{
				Socket handler = (Socket)ar.AsyncState;
				int lenght = handler.EndSend(ar);
				
				Console.WriteLine("{0} Bytes gesendet", lenght);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}
		#endregion

		#region public
		public virtual void Close()
		{
			this.Dispose();
		}

		public virtual void Dispose()
		{
			Console.WriteLine("BaseConnection Dispose()");
			lock (this)
			{
				this.Dispose(true);
			}

			GC.Collect();
		}

		public virtual void SendALT(string text)
		{
			Console.WriteLine("Send Buffer: " + text);
			try
			{
			// TO/DO: ersetzen
			//Encoding enc = Encoding.GetEncoding(28591);//1252
			//stringData = enc.GetString(data, 0, recv);

			//	byte[] data = Encoding.UTF8/*.ASCII*/.GetBytes(text);
				byte[] data = this.Encoding.GetBytes(text);

				this.handler.BeginSend(data, 0, data.Length,
						0, new AsyncCallback(this.SendCallback), handler);
			}
			catch (SocketException e)
			{
				Console.WriteLine("Socket Exception: " + e.ErrorCode);
				if (e.ErrorCode == 10053)
				{
					Console.WriteLine("Send Error call Dispose()");
					//this.Dispose();
				//	((IRCServer)this._server).RemoveConnection(this as IRCConnection); // todo: change
				}
			}
		}

// TODO: nochmal alles überprüfen
#if UNSTABLE
		// TODO: kompriemieren wenn Z flag
		public virtual void Send(string text)
		{
			// TODO: if kompriemier
			this.Send(text, this._server.SocketManager.Priority);
		}

		public virtual void Send(string text, MessagePriority priority)
		{
			if (priority.Equals(MessagePriority.RealTime))
			{
				this.SendSync(text);
			}
			else
			{
				this._server.SocketManager.Send(this, text, priority); // socketmanager
			}
		}

		public virtual void SendLine(string text)
		{
			Console.WriteLine("new sender");
			this.SendLine(text, this._server.SocketManager.Priority);
		}

		public virtual void SendLine(string text, MessagePriority priority)
		{
			// to/do: EOL
			//text += "\n";// this._config.EOL;
			text += ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.EndOfLine;
			this.Send(text, priority);
		}
#else
		public void Send(string text)
		{
			Console.WriteLine("!!!!!!!!!!!!! Use IRCConnection.SendLine() !!!!!!!!!!!!!");
			this.SendSync(text);
		}
#endif

		protected void SendSync(string text)
		{
			// never use SendSync direktly!
			Console.WriteLine("SendSync: " + text);
			try
			{
				byte[] data = this.Encoding.GetBytes(text);
///
stat.traffic += data.Length;
///
				this.handler.Send(data);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				// error -> remove connection
				this._server.RemoveConnection(this as IConnection);
			}
		}
		#endregion
		#endregion

		#region Properties
		public virtual bool Disposing
		{
			get
			{
				return this.disposed; // todo: change
			}
		}

		public virtual Encoding Encoding
		{
			get
			{
			//	lock (this._encoding)
			//	{
					//return this._encoding;
					return ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding;
			//	}
			}
	/*		set // TODO: nicht mehr erlauben
			{
				lock (this)
				{
					this._encoding = value;
					((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.Encoding = value;
				}
			}*/
		}

		public virtual IServer Server
		{
			get
			{
				return this._server;
			}
		}

		public virtual int ID
		{
			get
			{
				return this._id;
			}
		}

		public virtual Socket Socket
		{
			get
			{
				return this.handler;
			}
		}
		#endregion
	}
}
