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
using System.Collections;

using Network;

namespace IRC
{
	public delegate void CommandHandler(IRCConnection connection, string[] args);

	public partial class IRCServer
	{
		// simple*
		private ArrayList _users = new ArrayList();
		private ArrayList _servers = new ArrayList();

		private Hashtable _commands = new Hashtable();

		private void InitializeHandlers()
		{
			this.AddHandler("chall", new CommandHandler(this.m_chall));
//			this.AddHandler("connect", new CommandHandler(this.m_connect));
			this.AddHandler("error", new CommandHandler(this.m_error));
			this.AddHandler("id", new CommandHandler(this.m_id));
			this.AddHandler("join", new CommandHandler(this.m_join));
			this.AddHandler("list", new CommandHandler(this.m_list));
			this.AddHandler("mode", new CommandHandler(this.m_mode));
			this.AddHandler("names", new CommandHandler(this.m_names));
			this.AddHandler("nick", new CommandHandler(this.m_nick));
			this.AddHandler("part", new CommandHandler(this.m_part));
			this.AddHandler("pass", new CommandHandler(this.m_pass));
			this.AddHandler("ping", new CommandHandler(this.m_ping));
			this.AddHandler("privmsg", new CommandHandler(this.m_privmsg));
			this.AddHandler("quit", new CommandHandler(this.m_quit));
			this.AddHandler("rehash", new CommandHandler(this.m_rehash));
			this.AddHandler("resp", new CommandHandler(this.m_resp));
			this.AddHandler("restart", new CommandHandler(this.m_restart));
			this.AddHandler("server", new CommandHandler(this.m_server));
			this.AddHandler("stat", new CommandHandler(this.m_stat));
			this.AddHandler("user", new CommandHandler(this.m_user));
			this.AddHandler("who", new CommandHandler(this.m_who));
		}

		/// <summary>
		/// </summary>
		public void AddHandler(string command, CommandHandler pfunc)
		{
			// AddCommandHandler("/user", pfunc);
			lock (this._commands)
			{
				this._commands.Add(command, pfunc);
			}
		}

		/// <summary>
		/// Returns a "CommandHandler" for the specified "command"-string
		/// </summary>
		public CommandHandler GetHandler(string command)
		{
			return (CommandHandler)this._commands[command];
		}

		/// <summary>
		/// Determines whether the current instance contains an "CommandHandler" for the specified "command".
		/// </summary>
		public bool HasHandler(string command)
		{
			return this._commands.ContainsKey(command);
		}

		/// <summary>
		/// Removes the handler with the specified "command" from the current instance. 
		/// </summary>
		public bool RemoveHandler(string command)
		{
			lock (this._commands)
			{
				if (this._commands.ContainsKey(command))
				{
					this._commands.Remove(command);
					return true;
				}
				return false;
			}
		}

		public void CreateChannel(string name)
		{
			lock (this._channels)
			{
				if (!this._channels.ContainsKey(name))
				{
					Channel channel = new Channel(name, this, null);
					this._channels.Add(name, channel);
				}
			}
		}

		public bool RemoveChannel(Channel channel)
		{
			Console.WriteLine("RemoveChannel <{0}>", channel.Name);
			lock (this._channels)
			{
				if (this._channels.ContainsKey(channel.Name))
				{
					this._channels.Remove(channel.Name);
					return true;
				}
				return false;
			}
			//throw new NotImplementedException();
		}

		private void _worker() // TODO: evtl schon in BaseServer implementieren
		{
			bool stop = false; // todo: global
			while (!stop)
			{
				this.pinger();
			}
		}

		private void pinger()
		{
			int limit = 500000; // TODO: secs
			foreach (IRCConnection client in this.Clients)
			{
				TimeSpan c = DateTime.Now - client.LastPing;
				Console.WriteLine(c.Ticks);
				if (c.Seconds  > limit)
				{
					// src.quit("ping timeout")
					//this.RemoveConnection("ping timeout");
				}
				if (client is IRCServerConnection)
				{
					client.SendLine("PING :test");
				}
			}
		}

		/// <summary>
		/// Add a SimpleUser object
		/// </summary>
		/// <param name="user">SimpleUser to add</param>
		public virtual void AddUser(SimpleUser user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			lock (this._users)
			{
				if (this._users.Contains(user))
					return;

				this._users.Add(user);
			}
		}

		/// <summary>
		/// Remove a SimpleUser object
		/// </summary>
		/// <param name="user">SimpleUser to remove</param>
		public virtual void RemoveUser(SimpleUser user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			lock (this._users)
			{
				if (this._users.Contains(user))
					return;

				this._users.Remove(user);
			}
		}

		/// <summary>
		/// Add a SimpleServer object
		/// </summary>
		/// <param name="server">SimpleServer to add</param>
		public virtual void AddServer(SimpleServer server)
		{
			if (server == null)
				throw new ArgumentNullException("server");

			lock (this._servers)
			{
				if (this._servers.Contains(server))
					return;

				this._servers.Add(server);
			}
		}

		/// <summary>
		/// Remove a SimpleServer object
		/// </summary>
		/// <param name="server">SimpleServer to remove</param>
		public virtual void RemoveServer(SimpleServer server)
		{
			if (server == null)
				throw new ArgumentNullException("server");

			lock (this._servers)
			{
				if (this._servers.Contains(server))
					return;

				this._servers.Remove(server);
			}
		}

		/// <summary>
		/// </summary>
		public bool IsMyClient(SimpleUser usr)
		{
			if (usr.UpLink.GetType() == typeof(IRCUserConnection))
				return true;
			return false;
		}

		/// <summary>
		/// Returns true if "usr" has mode 'o'|'O'
		/// </summary>
		/// <param name="usr>SimpleUser object to check</param>
		public bool IsOper(SimpleUser usr)
		{
			return (usr.UserMode.HasMode(IRCUserModes.MODE_OP) | usr.UserMode.HasMode(IRCUserModes.MODE_LOCAL_OP));
		}

		/// <summary>
		/// </summary>
		public OperData GetOperData(string name, SimpleUser usr)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Search Channels
		/// </summary>
		/// <param name="pattern">pattern for match()</param>
		public Channel[] SearchChannel(string pattern)
		{
			throw new NotImplementedException();
		}

		// only for debugging
		public void WriteArgs(string[] ar)
		{
			int i = 0;
			foreach (string str in ar)
			{
				Console.WriteLine("ar[{0}] = {1}", i, ar[i]);
				i++;
			}
		}

		/// <summary>
		/// Check if "con" is a IRCServerConnection-object
		/// </smmary>
		public bool IsServer(IRCConnection con)
		{
			return (con.GetType() == typeof(IRCServerConnection));
		}

		/// <summary>
		/// Check if "con" is a IRCUserConnection-object
		/// </smmary>
		public bool IsUser(IRCConnection con)
		{
			return (con.GetType() == typeof(IRCUserConnection));
		}

		/// <summary>
		/// List of all SimpleUser objects
		/// </summary>
		public ArrayList Users
		{
			get
			{
				return this._users;
			}
		}

		/// <summary>
		/// List of all SimpleServer objects
		/// </summary>
		public ArrayList Servers
		{
			get
			{
				return this._servers;
			}
		}
	}
}
