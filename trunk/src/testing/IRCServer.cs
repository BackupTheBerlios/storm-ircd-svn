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

namespace IRC
{
	public partial class IRCServer
	{
		private ArrayList _users = new ArrayList();
		private ArrayList _servers = new ArrayList();

		public void AddCommandHandler(string command)
		{
		// AddCommandHandler("/user", func);
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
			int limit = 500000; // TODO: secunden
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
