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
using System.Collections;
using System.Diagnostics;

using Service;
using IRC;

namespace Service
{
	public class IRCService : IService
	{
		private static IRCService _instance;
		private IRCServer _server;
//		private ArrayList _ports;
		private ArrayList _bindObjects;

		private bool _loaded = false;
		private ArrayList _serversToConnect;

		public static IRCService Instance
		{
			get
			{
				if (_instance == null)
					_instance = new IRCService();

				return _instance;
			}
		}

		private IRCService()
		{
// new
			this._bindObjects = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.BindObjects;
			this._serversToConnect = SettingsHost.Instance.Settings.ServerLines.GetServerList();
//			this._ports = new ArrayList();

			if (this._bindObjects.Count == 0)
			{
				this._bindObjects.Add(new BindObject());
			}
/*			foreach (BindObject obj in this._bindObjects)
			{
//				this._ports.Add(obj.Port);
			}
// old
/*			ArrayList ports = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.IRCPorts;
			this._ports = new ArrayList();
			this._serversToConnect = SettingsHost.Instance.Settings.ServerLines.GetServerList();

			if (ports != null)
			{
				foreach (object obj in ports)
				{
					if (obj.GetType() == typeof(int))
					{
						int p = (int)obj;
						if ((p <= IPEndPoint.MaxPort) && (p >= IPEndPoint.MinPort))
							this._ports.Add(p);
					}
				}
			}
			if (this._ports.Count == 0)
			{
				this._ports.Add(6667);
			}*/
		}

		// get the list of "ConnectLines" and connect to every server 
		private void ConnectToServers()
		{
			try
			{
				lock (this._serversToConnect)
				{
					ArrayList servers = (ArrayList)this._serversToConnect.Clone();

					foreach (IPEndPoint ep in servers)
					{
						if (IRCServerConnection.ConnectToServer(this._server, ep))
						{
							this._serversToConnect.Remove(ep);
						}
						else
						{
//							Logger.LogMsg("Verbindung zu " + ep.ToString() + " gescheitert");
							Logger.LogMsg("Cannot connect to: " + ep.ToString());
						}
					}
				}
			}
			catch (Exception e)
			{
//				Console.WriteLine("kann nicht zu server verbinden: "+e);
				Console.WriteLine("Cannot connect to server: " + e.Message);
			}
			Console.WriteLine("end ConnectToServers");
		}

		public void Load()
		{
			try
			{
				if (this._loaded)
					return;

				this._server = new IRCServer();
				this._loaded = true;
/*
				foreach (int port in this._ports)
				{
					_server.StartListening(new IPEndPoint(IPAddress.Parse("127.0.0.1"),port)); // <-- hack
				}
*/
				foreach (BindObject obj in this._bindObjects)
				{
					IPHostEntry hostInfo = Dns.Resolve(obj.HostName);
					IPAddress[] addresses = hostInfo.AddressList;

					this._server.StartListening(new IPEndPoint(addresses[0], obj.Port));
				}
				this.ConnectToServers();
			}
			catch (Exception e)
			{
				Console.WriteLine("fatal error in "+ this +" :" + e.ToString() + "\n\naborting ...");
				Environment.Exit(1);
			}
		}

		public void Unload()
		{
			if (!this._loaded)
				return;

			this._loaded = false;
/*
			foreach (int port in this._ports)
			{
				_server.StopListening(port); // TODO auch auf EndPoint umschreiben
			}
*/
			this._server.Dispose();

//			((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.IRCPorts = this._ports;
			Debug.WriteLine(this + ".Unload()");
		}

		public Type[] Dependences
		{
			get
			{
				return new Type[]{typeof(SettingsHost)};
			}
		}

		public bool Loaded
		{
			get
			{
				return this._loaded;
			}
		}

		public IRCServer Server
		{
			get
			{
				return this._server;
			}
		}
	}
}
