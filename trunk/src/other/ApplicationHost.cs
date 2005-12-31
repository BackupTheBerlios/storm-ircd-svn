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
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Net.Sockets;

using Mono.Unix;

using IRC;
using Tools;
using Other;
using Network;

namespace Service
{
	public class ApplicationHostInterpretor : BaseInterpretor
	{
		public ApplicationHostInterpretor()
		{
			Debug.WriteLine(this + ": initialize ...");
		}

		public override bool ProcessCommand(IConnection connection, string cmd)
		{
#if false
			// TODO:
			Request req = Deserialize(cmd);
#endif
			if (cmd == string.Empty)
				return false;

			Debug.WriteLine(this + ": has received: " + cmd);
			if (cmd == "shutdown")
			{
				//ServiceManager.Services[typeof(IRCService)].Unload(); // save
				//ServiceManager.Services.UnloadAll();

				Console.WriteLine("MainClass.shutdown();");
				MainClass.Shutdown();
			}
			else if (cmd == "restart") // TODO: bug
			{
				Debug.WriteLine("restarting server ...");
				if (ServiceManager.Services[typeof(IRCService)].Loaded)
				{
					ServiceManager.Services[typeof(IRCService)].Unload();
					ServiceManager.Services[typeof(IRCService)].Load();
				}
			}
			else if (cmd == "collect")
			{
				Debug.WriteLine(this + ": collect; mem: " + GC.GetTotalMemory(true));
				GC.Collect();
				GC.WaitForPendingFinalizers();
				Debug.WriteLine(this + ": collect; mem: " + GC.GetTotalMemory(true));
			}
			else if (cmd == "send")
			{
			}
			else if (cmd == "status")
			{
				if (ServiceManager.Services[typeof(IRCService)].Loaded)
				{
					IRCServer server = ((IRCService)ServiceManager.Services[typeof(IRCService)]).Server;

					Console.WriteLine("===============================");
					Console.WriteLine("= Server State:");
					Console.WriteLine("= TotalMemory: {0} bytes", GC.GetTotalMemory(true));
					Console.WriteLine("= Clients: {0}", server.UserOnServer);//.Clients.Count);
					Console.WriteLine("= Errors: {0}", stat.errors);
					Console.WriteLine("= Traffic: {0} bytes", stat.traffic);


					if (ServiceManager.Services.HasService(typeof(Statistic)))
					{
						Statistic service = ((Statistic)ServiceManager.Services[typeof(Statistic)]);
						
						Console.WriteLine("= AvailableCompletionPortThreads: {0}",service.AvailableCompletionPortThreads);
						Console.WriteLine("= AvailableWorkerThreads: {0}",service.AvailableWorkerThreads);
						Console.WriteLine("= CompletionPortThreads: {0}",service.CompletionPortThreads);
						Console.WriteLine("= WorkerThreads: {0}",service.WorkerThreads);

						Console.WriteLine("= Threads: {0}",service.Threads);
					}
					Console.WriteLine("===============================");
				}
			}

			return true;
		}
	}

	public class ApplicationHost : IListener, IService
	{
		private IInterpretor _interpretor;
		private SocketManager _socketManager;
		private ArrayList _listeners = new ArrayList();
		private ArrayList _clients = new ArrayList();
		private string _fileName;
		private bool _loaded = false;

		public ApplicationHost(SocketManager sm) : this(SettingsHost.Instance.Settings.SocketFile, sm)
		{
		}

		// note: SocketManager.AddListener erstellt eine kopie das _listeners array
		// darum gibt es jetzt eine neue funktion SocketManager.UpdateListener
		public ApplicationHost(string fileName, SocketManager sm)
		{
			if (fileName == null)
				throw new ArgumentNullException("fileName");

			if (sm == null)
				throw new ArgumentNullException("sm");

			this._fileName = fileName;
			this._socketManager = sm;
			this._socketManager.AddListener(this);

			this._interpretor = new ApplicationHostInterpretor();
		}

		~ApplicationHost()
		{
			Console.WriteLine(this + ": Destrukor [ OK ]");
		}

		private bool disposed = false;

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._socketManager.RemoveListener(this);
				foreach (Socket sock in this._listeners)
				{
					sock.Close();
				}
				foreach (IWorker worker in this._clients)
				{
					worker.Dispose();
				}
				this.disposed = true;
			}
		}

		public void Load()
		{
			if (this._loaded)
				return;

			this._loaded = true;
			this.Initialize();
		}

		public void Unload()
		{
			if (!this._loaded)
				return;

			this._loaded = false;
			this.Dispose();
		}

		public void Initialize()
		{
			try
			{
#if UNIX
				EndPoint endPoint = new UnixEndPoint(this._fileName);

				if (File.Exists(this._fileName))
				{
					File.Delete(this._fileName);
				}

				Debug.WriteLine(this + " listen on " + this._fileName);
				Socket socket = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
				socket.Bind(endPoint);
				socket.Listen(5);

				// add "socket" to this._listeners to manage it by SocketManager
				this._listeners.Add(socket);
				this._socketManager.UpdateListener(this); // important to register the new socket in SocketManager
#endif
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		public Type[] Dependences
		{
			get
			{
				return new Type[]{typeof(SettingsHost)};
			}
		}

		public SocketManager SocketManager
		{
			get
			{
				return this._socketManager;
			}
		}

		public IInterpretor Interpretor
		{
			get
			{
				return this._interpretor;
			}
			set
			{
				this._interpretor = value;
			}
		}

		public bool Loaded
		{
			get
			{
				return this._loaded;
			}
		}

		public ArrayList Listeners
		{
			get
			{
				return this._listeners;
			}
		}

		public void AddConnection(Socket client)
		{
			if (client == null)
				throw new ArgumentNullException(this + "client");
// bug:
//			if (client.Connected)
//				throw new InvalidOperationException("socket problem");

			this._clients.Add(new ApplicationWorker(client, this));
		}

		public void AddConnection(IConnection connection) // TODO: braucht nicht im interface vorhanden zu sein!
		{
			throw new NotSupportedException("AddConnection");
		}

		public void RemoveConnection(Socket client)
		{
			if (client == null)
				throw new ArgumentNullException(this + "client");

			lock (this._clients)
			{
				foreach (IWorker worker in this._clients)
				{
					if (worker.Socket == client)
					{
						this._clients.Remove(worker);
						worker.Dispose();
						return;
					}
				}
			}
		}

		public void RemoveConnection(IConnection connection)
		{
			throw new NotSupportedException("RemoveConnection");
		}
	}

	public class ApplicationWorker : IWorker
	{
		Socket _socket;
		IListener _listener;
		private bool disposed = false;

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._socket.Close();
				this.disposed = true;
			}
		}

		public ApplicationWorker(Socket client, IListener listener)
		{
			Debug.WriteLine(this + ": konstruktor");

			if (client == null)
				throw new ArgumentNullException(this + "client");

			if (listener == null)
				throw new ArgumentNullException(this + "listerner");

			this._socket = client;
			this._listener = listener;

			this._listener.SocketManager.AddConnection(this);
		}

		~ApplicationWorker()
		{
			Console.WriteLine(this + ": Destrukor [ OK ]");
		}

		public Socket Socket
		{
			get
			{
				return this._socket;
			}
		}

		public void Execute(string text)
		{
			Debug.WriteLine(this + ".Execute()");
			this._listener.Interpretor.ProcessCommand(null, text);
		}
	}
}

