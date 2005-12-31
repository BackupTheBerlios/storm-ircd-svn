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
using System.Net.Sockets;
using System.Diagnostics;
using System.Collections;

using Network;
using Service;

namespace IRC
{
/*
	public enum MessageType
	{
		Server,
		Client
	}
*/
	public class IRCConnection : BaseConnection
	{
		protected SimpleClient _simpleClient;

		// real data
		protected string _realhostName;
		protected string[] _aliases;
		protected IPAddress _address;

		private DateTime _lastping; // todo
		private Hashtable _channels;

		public IRCConnection(Socket sock, IServer server) : base(sock, server)
		{
			Debug.WriteLine("IRCConnection Konstruktor");
			this._lastping = DateTime.Now; // direkt nach verbindung
		}

		// HACK:
		public IRCConnection(IPEndPoint ep, IRCServer server) : base(ep, server)
		{
			Debug.WriteLine("IRCConnection Konstruktor");
			this._lastping = DateTime.Now;
		}

		public void LookupHostName()
		{
			this.NoticeAuth("Looking up your hostname...");
			this.NoticeAuth("Checking ident..."); // TODO: ident sys ...

			if (!this.ResolveHost())
				this.NoticeAuth("Couldn't look up your hostname");
			else
				this.NoticeAuth("Found your hostname");
		}

		public virtual void NoticeAuth(string message)
		{
			this.SendLine("NOTICE AUTH :*** " + message);
		}

		private bool ResolveHost()
		{
			try
			{
				EndPoint endPoint = this.Socket.RemoteEndPoint;
				IPEndPoint ipendPoint;
				if (endPoint is IPEndPoint)
				{
					ipendPoint = (IPEndPoint)endPoint;

					IPHostEntry hostInfo = Dns.GetHostByAddress(ipendPoint.Address);

					Console.WriteLine("Connection HostName: " + hostInfo.HostName);
					
					Console.WriteLine("Aliases:");
					foreach (string name in hostInfo.Aliases)
					{
						Console.WriteLine(name);
					}

					Console.WriteLine("IP address list:");
					foreach (IPAddress address in hostInfo.AddressList)
					{
						Console.WriteLine(address.ToString());
					}

					// set data
					this._realhostName = hostInfo.HostName;
					this._address = ipendPoint.Address;
					this._aliases = hostInfo.Aliases;

					return true;
				}
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine("Problem during ResolveHost(), use ip ...");
				this._address = ((IPEndPoint)this.Socket.RemoteEndPoint).Address;
				this._realhostName = this._address.ToString(); // notfallplan ;-)
				return false;
			}
		}

		/// <summary>
		///
		/// </summary>
		private string UserPrefix(SimpleUser usr)
		{
			return (":" + usr.NickName + "!" +
				usr.UserName + "@" + usr.HostName + " ");
		}

		/// <summary>
		///
		/// </summary>
		private string ServerPrefix()
		{
			// prefix = servername
			return (":" + ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName + " ");
		}

		/// <summary>
		///
		/// </summary>
		public virtual void SendCommand(SimpleUser usr, string command, params string[] args)
		{
			if (usr == null)
				throw new ArgumentNullException("usr");

			this.SendCommand(this.UserPrefix(usr), command, args);
		}

		/// <summary>
		///
		/// </summary>
		public virtual void SendCommand(string command, params string[] args)
		{
			//Console.WriteLine(this + ": SendCommand warning use Server");
			this.SendCommand(this.ServerPrefix(), command, args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		private void SendCommand(string prefix, string command, params string[] args)
		{
			string toSend;
			toSend = prefix + command;

			foreach (string arg in args)
			{
				toSend += " ";
				toSend += arg;
			}

			Console.WriteLine(this + ".SendCommand(): " + toSend);
			this.SendLine(toSend);
		}

		public virtual void SetNick(string nick)
		{
//			this.CreateSimpleUser();
			if (this.SimpleClient is SimpleUser)
			{
				((SimpleUser)this.SimpleClient).NickName = nick;
			}
		}

		/// <summary>
		/// Fill the SimpleUser struct
		/// </summary>
		public virtual void SetUser(string user, string mode,
		                         string something, string realname)
		{
//			this.CreateSimpleUser();
			if (this.SimpleClient is SimpleUser)
			{
				SimpleUser usr = (SimpleUser)this.SimpleClient;
				usr.UserName = user;
				usr.RealName = realname;

				//usr.UserMode = new IRCUserMode(Convert.ToInt32(mode));
			}
		}

		private void CreateSimpleUser()
		{
			if (this.SimpleClient == null)
			{
				this.SimpleClient = new SimpleUser(this);
			}
		}

		public override string ToString()
		{
			return this.RealHostName;
		}

/*
		public virtual string RealName
		{
			get
			{
				return this._realName;
			}
		}
*/
		public Hashtable Channels
		{
			get
			{
				if (this._channels == null)
					this._channels = new Hashtable();

				return this._channels;
			}
		}

		public DateTime LastPing
		{
			get
			{
				return this._lastping;
			}
			set
			{
				this._lastping = value;
			}
		}

		public SimpleClient SimpleClient
		{
			get
			{
				return this._simpleClient;
			}
			set
			{
				this._simpleClient = value;
			}
		}

		public string RealHostName
		{
			get
			{
				return this._realhostName;
			}
		}

		public string[] Aliases
		{
			get
			{
				return this._aliases;
			}
		}

		public IPAddress Address
		{
			get
			{
				return this._address;
			}
		}

		private bool _passSet = false;
		public bool PassSet
		{
			get
			{
				return this._passSet;
			}
			set
			{
				this._passSet = true;
				//Console.WriteLine("TODO: PassSet");
			}
		}
	}
}
