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
using System.Reflection;
using System.Net;
using System.Net.Sockets;

using System.Diagnostics;

using Network;
using Other;

using Service;

namespace IRC
{

	/// <summary>
	/// </summary>
	public delegate void ClientConnectHandler(string nick);

	/// <summary>
	/// Core server
	/// </summary>
	public partial class IRCServer : BaseServer // new partial
	{
		private int _SoftLimit;
		private BlackList _blacklist;
		private Hashtable _channels;

		public readonly string devName = "storm-ircd";
		private readonly char[] _nickPrefixes = {'@', '%', '+'}; // TODO: mit regex
		private readonly char[] _channelPrefixes = {'#', '&'};

		/// <summary>
		/// Default Constructor
		/// </summary>
		public IRCServer()
		{
			this._blacklist = new BlackList();
			this._channels = new Hashtable();
			base.Interpretor = new IRCInterpretor(this);

			// TODO: ThreadPool.QueueUserWorkItem(new WaitCallback(this.pinger));
		}

		public void RegisterUser(IRCConnection con)
		{
			if (con == null)
				throw new ArgumentNullException("con");

			lock (con)
			{
				if (con is IRCConnection)
				{
					IRCUserConnection usr;
					this.RemoveConnection(con);
					usr = new IRCUserConnection(con);
					this.AddConnection(usr);
					// TODO: this.AddUser(usr.SimpleUser);
				}
				else
				{
					con.SendLine("ERROR: you could not register!: internal error");
				}
			}
		}

		public void RegisterServer(IRCConnection con, SimpleServer ser)
		{
			if (con == null)
				throw new ArgumentNullException("con");
			if (ser == null)
				throw new ArgumentNullException("ser");

	// TODO: ircservice mitteilen das zu diesem server keine verbindung mehr hergestellt werden muss
			lock (con)
			{
				if (con is IRCConnection)
				{
					IRCServerConnection srv;
					this.RemoveConnection(con);
					srv = new IRCServerConnection(con, ser);
					this.AddConnection(srv);
					// TODO: mal sehen evtl. simpleserver erzeugen und mit AddServer hinzufügen
					//this.AddServer(ser);
					IRCServerConnection.SendServer(srv);
				}
				else
				{
					con.SendLine("ERROR: you could not register!; internal error");
				}
			}
		}

		protected override void AcceptCallback(IAsyncResult ar)
		{
		///todo: base.AcceptCallback() ausführen
			AcceptObject obj = ((AcceptObject)ar.AsyncState);
			obj.allDone.Set();
			Socket handler = obj.listener.EndAccept(ar);

			Console.WriteLine("Accept connection from: " + handler.RemoteEndPoint.ToString());
			lock (base.Clients)
			{
				IRCConnection connection = new IRCConnection(handler, this);//neu, true);

				Console.WriteLine("End Accept");
				base.AddID(connection.ID); // wichtig!

				// OnConnected
				base.Clients.Add(connection);

				// sample for exexute
				//connection.Execute("join #test");
			}
			///

		//	this.OnClientConnect(connection.NickName);
		}

		/// <summary>
		/// </summary>
		public override void Close()
		{
			Console.WriteLine("Shuting down Server...");
			lock (this.Clients)
			{
				foreach (BaseConnection connection in this.Clients)
				{
					if (connection is IRCConnection)
					{
						IRCConnection ic = (IRCConnection)connection;
					//	ic.Quit();
					//	this.RemoveConnection(ic);
						this.CloseLink(ic);
					}
				}
			}
			base.Close(); // cleanup in BaseServer
		}

		// neu
		public void SendToOps() // TODO: fertig machen
		{
			Console.WriteLine(this + "SendToOps() not implemented");
			//throw new NotImplementedException();
			/*foreach (IConnection client in this.Client)
			{
				if (client.UserModes.get(IRCUserModes.op)
				{
					client.SendCommand("");
				}
			}*/
		}

		// überprüft zwei aliase ips oä auf gleichheit
		public bool EqualHosts(string hosta, string hostb)
		{
			IPHostEntry hostInfoa = Dns.Resolve(hosta);
			IPHostEntry hostInfob = Dns.Resolve(hostb);

			if (hostInfoa.HostName == hostInfob.HostName)
				return true;
			return false;
		}
// end neu

		/// <summary>
		/// Handel RemoveMe Request
		/// </summary>
		/// <param name="obj">object to Remove</param>
		public virtual void HandleRemoveMe(object obj) // TODO: obsolete
		{
			if (obj is IRCConnection)
			{
				//IRCConnection connection = (IRCConnection)obj;
				//this.RemoveConnection((IRCConnection)obj);BUG !!!!!!!!!!
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nick"></param>
		/// <returns></returns>
		public virtual Channel GetChannel(string name)
		{
			foreach (DictionaryEntry entry in this._channels)
			{
				Channel ch = (Channel)entry.Value;
				if (ch.Name == name)
					return ch;
			}

			return null;
		}

		public virtual IRCConnection[] SearchPersons(string alias)
		{
		// für bann nick und whois etc.
		// auf mehrer typen zu prüfen
		//	return this.Clients[
		
		// blackdragon!~aa@127.0.0.1
		//	if 
			return null;
		}

		public bool HasNick(string nick)
		{
/*			foreach (IRCConnection client in this.Clients)
			{
				if (client.NickName == nick)
					return true;
			}*/
			foreach (IRCUserConnection user in this._users)
			{
				if (user.NickName == nick)
					return true;
			}

			return false;
		}

// new !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
		// HACK:
		public bool HasServer(EndPoint ep)
		{
			foreach (IRCConnection server in this._servers)
			{
				if (server.Socket.RemoteEndPoint.Equals(ep))
					return true;
			}
			return false;
		}

		public IConnection GetServer(string hostName)
		{
			throw new NotImplementedException("GetServer");
		}

		/// <summary>
		/// 
		/// </summary>
		internal virtual void OnClientConnect(string nick) // TODO: base event
		{
			if (this.ClientConnect != null)
				this.ClientConnect(nick);
		}

		/// <summary>
		/// Join a channel
		/// </summary>
		/// <param name="client">Connection</param>
		/// <param name="channel">Name of channel to join</param>
		public virtual void Join(IRCUserConnection client, string channel)
		{
			lock (this._channels)
			{
				if (!this._channels.Contains(channel))
				{
					if (RFC2812.IsValidChannelName(channel)) // channel prefix(&,#), lenght, etc.
					{
						Console.WriteLine("Create new Channel: {0}", channel);
						Channel ch = new Channel(channel, this, null);
						this._channels.Add(channel, ch);
						//connection.channel umode +creator
					}
				}
				lock (client)
				{
					Channel ch = (Channel)this._channels[channel];
					if (ch.CanJoin(client))
					{
						Console.WriteLine("{0} can join {1}", client.ID, channel);
						client.Join(ch);
					}
				}
			}
		}

		public virtual void Part(IRCUserConnection client, string channel)
		{
			this.Part(client, channel, true);
		}

		public virtual void Part(IRCUserConnection client, string channel, bool confirmation)
		{
			Console.WriteLine("TO/DO: part");
#if DEC
			lock (client)
			{
				if (client.IsInChannel(channel))
				{
					/// new
					Channel ch = ((Channel)client.Channels[channel]);
					client.Part(ch, confirmation);
					//if (!ch.GetMode (/*unkaputbar*/)
					if (ch.MemberCount == 0)
					{
						Console.WriteLine("Call Channel.Dispose()");
						ch.Dispose();
					}
				}
				Console.WriteLine("end part()");
			}
#endif
		}


// TODO:
// new
		public virtual IRCConnection[] get_servers(string ident)
		{
			foreach (IRCConnection src in this._servers)
			{
			}
			return null;
		}

		public virtual IRCUserConnection[] get_services(string ident) // TODO
		{
//			foreach (IRCConnection src in this._services)
//			{}
			return null;
		}

		public virtual IRCUserConnection[] get_persons(string ident)
		{
			// sample *!~*@host*
			// *!~*@host.de
			// *!~cl@host.de
			// name!~cl@host.de

			ArrayList list = new ArrayList();

			// TODO if (!RFC2812.isvalidident(ident)
			// return null
			foreach (IRCUserConnection src in this._users) // TODO: liste ist leer
			{
				if (src.Match(ident))
					list.Add(src); // TODO: invisible system
			}

			IRCUserConnection[] all = new IRCUserConnection[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				all[i] = (IRCUserConnection)list[i];
			}
			
			return all;
		}
// end



		/// <summary>
		/// Check the Prefix of "name"
		/// </summary>
		/// <param name="name">Name of channel</param>
		/// <returns>True if "name" a channel</returns>
		private bool IsChannel(string name)
		{
			// TODO: RFC.isvalidechannelname
			foreach (char pre in this._channelPrefixes)
			{
				if (name[0] == pre)
				{
					return true;
				}
			}
			return false;
		}

//		/// <summary>
//		/// Execute a Command
//		/// </summary>
//		/// <param name="cmd">command</param>
//		public void Execute(string cmd)
//		{
//			
//		}
/*
		public void SendTo(string text, string nick)
		{
			lock (this.Clients)
			{
				foreach (IRCConnection client in this.Clients)
				{
					if (client.NickName == nick)
					{
						try
						{
							client.SendLine(text); // BUGFIX: SendLine
						}
						catch (SocketException e)
						{
							lock (this.ClientsToRemove)
							{
								this.ClientsToRemove.Add(client);
							}
							Console.WriteLine(e.ToString());
						}
					}
				}
			}
		}
*/
		public void SendTo(string text, string nick)
		{
			lock (this.Clients)
			{
				foreach (IRCUserConnection client in this.Clients)
				{
					if (client.NickName == nick)
					{
						client.SendLine(text);
					}
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="client"></param>
		/// <param name="msg"></param>
		public void Quit(IRCConnection client, string msg) // TODO: nach IRCConnection verschieben und den RemoveConnection call entfernen.
		{
			lock (client)
			{
				client.Send(":" + client.RealHostName/*.NickName*/ + "a@b.c" + "@" + "Hostname" + "QUIT :quitter\n\n");

				// TO/DO: RemoveConnection
				//this.RemoveConnection(client);
				this.CloseLink(client);
			}
		}

		public virtual void CloseLink(IConnection con)
		{
			Console.WriteLine(this+".CloseLink("+con.GetType()+")");
			this.RemoveConnection(con);
			con.Dispose();
		}
		public virtual void RemoveConnection(IRCConnection connection, string msg) // TODO: msg senden
		{
			this.RemoveConnection(connection);
		}

		/// <summary>
		/// Remove a Connection
		/// </summary>
		/// <param name="connection">Connection to remove</param>
		public override void RemoveConnection(IConnection connection)
		{
			Console.WriteLine("IRCServer RemoveConnection()");

			lock (connection)
			{				try
				{
					if (connection is IRCUserConnection)
					{
						Console.WriteLine("Connection is a IRCUserConnction");
						Console.WriteLine("TODO: remove von channels");
#if DEC
						IRCUserConnection irccon = (IRCUserConnection)connection;
						if (!(irccon.Channels == null))
						{
							IDictionaryEnumerator enu = ((Hashtable)(irccon.Channels.Clone())).GetEnumerator(); // workaround to modify the Hashtable
							while (enu.MoveNext())
							{
								Channel ch = (Channel)enu.Value;

								Console.WriteLine("remove connection from: {0}", ch.Name);

								this.Part(irccon, ch.Name, false);
							}
						}
#endif
					}
					base.RemoveConnection(connection);
				}
				catch (Exception e)
				{
					StackTrace st = new StackTrace(true);

					Console.WriteLine(e);
					Console.WriteLine(st);
				}
			}
		}

		public override void AddConnection(Socket client)
		{
			Debug.WriteLine(this + ".AddConnection from socket");
			Debug.WriteLine(this + ": Accept connection from: " + client.RemoteEndPoint.ToString());

// new
//canconnect(client); //TODO schauen ob ein bann
//
			lock (base.Clients)
			{
				IRCConnection connection = new IRCConnection(client, this);//neu, true);
				connection.LookupHostName();
				Console.WriteLine("End Accept");
				base.AddID(connection.ID); // wichtig!

				// OnConnected
				base.Clients.Add(connection);
			}
		}

		/// <summary>
		/// string[] of Nicks on this Server
		/// </summary>
		public string[] Nicks
		{
			get
			{
				// BUGFIX: 1.04.05 -> invalide operation exception
				string[] nicks = null;
				lock (this.Clients)
				{
					nicks = new string[this.Clients.Count];

					for (int i = 0; i < this.Clients.Count; i++)
					{
						nicks[i] = ((IRCUserConnection)Clients[i]).NickName;
					}
				}
				return nicks;
			//	return (string[])base.Clients.ToArray(typeof(string));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public BlackList BlackList
		{
			get
			{
				return this._blacklist;
			}
		}

		/// <summary>
		/// Count of users
		/// </summary>
		public int UserOnServer
		{
			get
			{
				this.CheckStatus(null);
				return this.Clients.Count;// todo: users
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int SoftLimit // TODO: aus settings
		{
			get
			{
				return this._SoftLimit;
			}
			set
			{
				this._SoftLimit = value;
			}
		}

		public string ServerName
		{
			get
			{
				return ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName;
			}
			set
			{
				throw new NotImplementedException(this + ".set_ServerName");
			}
		}

		/// <summary>
		/// </summary>
		public bool LimitReached
		{
			get
			{
				return !(this.UserOnServer<this.SoftLimit); // TO/DO: implement LimitReached
			}
		}

		/// <summary>
		/// All Channels on this Server
		/// </summary>
		internal Hashtable Channels
		{
			get
			{
				return this._channels;
			}
		}

		/// <summary>
		/// </summary>
		public event ClientConnectHandler ClientConnect;
	}
}
