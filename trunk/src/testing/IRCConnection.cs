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
using Network;

namespace IRC
{
	public enum MessageType
	{
		Server,
		Client
	}

	public class IRCConnection : BaseConnection
	{
		protected object _simpleClient;

		// real data
		private string _realhostName;
		private string[] _aliases;
		private IPAddress _address;

		private DateTime _lastping; // todo

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
			this.NoticeAuth("Checking ident...");

			if (!this.gethost())
				this.NoticeAuth("Couldn't look up your hostname");
			else
				this.NoticeAuth("Found your hostname");
		}

		public virtual void NoticeAuth(string message)
		{
			this.SendLine("NOTICE AUTH :*** " + message);
		}

		private bool gethost()
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

					// setze daten
					this._realhostName = hostInfo.HostName;
					this._address = ipendPoint.Address;
					this._aliases = hostInfo.Aliases;

					return true;
				}
				else
				{
					Console.WriteLine("???? this should never happen!!!!");
				}
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine("Problem during gethost(), use ip ...");
				this._address = ((IPEndPoint)this.Socket.RemoteEndPoint).Address;
				this._realhostName = this._address.ToString(); // notfallplan ;-)
				return false;
			}
		}
#if DEC
		public string Ident(ISimpleClient client)
		{
			return (":" + this._nickName + "!" +
				this._clientName + "@" + this._realhostName + " ");
		}

		public string ServerIdent()
		{
			return (":" + ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName + " ");
		}

		public void SendCommand(string command, params string[] args)
		{
			Console.WriteLine(this + ": SendCommand warning use Client");
			this.SendCommand(MessageType.Client, command, args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public void SendCommand(MessageType type, string command, params string[] args)
		{
			string toSend = String.Empty;

			if (type == MessageType.Client)
				toSend = this.Ident() + command;
			else
				toSend = this.ServerIdent() + command;

			foreach (string arg in args)
			{
				toSend += " ";

				if (arg == args[args.Length-1])
					toSend += ":";

				toSend += arg;
			}

			Console.WriteLine(this + ".SendCommand(): " + toSend);
			this.SendLine(toSend);
		}
#endif
		private string _clientName;
		private string _hostName;
		private string _nickName = string.Empty;
		private string _realName;

		public virtual void SetNick(string nick)
		{
			//this._nickName = nick;
			if (this.SimpleClient is SimpleUser)
			{
				((SimpleUser)this.SimpleClient).NickName = nick;
			}
		}

		public virtual void SetUser(string clientName, string host,
		                         string host2, string realName)
		{
/*
			this._clientName = clientName;
			this._realName = realName;
			this._hostName = host;
*/
			if (this.SimpleClient is SimpleUser)
			{
				SimpleUser usr = (SimpleUser)this.SimpleClient;
				usr.ClientName = clientName;
				usr.HostName = host;
				usr.RealName = realName;
			}
		}

#if DEC // old
		public string ClientName
		{
			get
			{
				return this._clientName;
			}
		}

		public string HostName
		{
			get
			{
				return this._hostName;
			}
		}

		public virtual string NickName
		{
			get
			{
				return this._nickName;
			}
		}
#endif
		public virtual string RealName
		{
			get
			{
				return this._realName;
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

		public object SimpleClient
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
