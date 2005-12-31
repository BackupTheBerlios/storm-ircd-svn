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

using System.Reflection;
using System.Collections;

using Service;

namespace IRC
{
	// stellt clients und services da
	public class IRCUserConnection : IRCConnection
	{
		// nickset
		private bool nickset = false;
		private bool userset = false;
		private bool passset = false;

		private int disadvantage = 0; // TODO
/*
		// client data
		private string _clientName;
		private string _hostName;
		private string _nickName = string.Empty;
		private string _realName;
*/
		// real data
//		private string _realhostName;
		private string[] _aliases;
		private IPAddress _address;

		// neu
		private DateTime _lastping; // todo
		// end

		private Hashtable _channels;

		#region Construct
		public IRCUserConnection(IRCConnection bas) : base(bas.Socket, bas.Server)
		{
			Socket handler = bas.Socket;
			this._channels = new Hashtable();

			if (!(bas.SimpleClient is SimpleUser))
				throw new ArgumentException("bas.SimpleClient musst be a SimpleUser");

			this._simpleClient = bas.SimpleClient;
			this.SimpleUser.UpLink = this;

			// Host data
			this._realhostName = bas.RealHostName;
			this._aliases = bas.Aliases;
			this._address = bas.Address;

			this.SetNick(this.SimpleUser.NickName);
			this.SetUser(this.SimpleUser.UserName, this.SimpleUser.HostName,
							"", this.SimpleUser.RealName);
		}
		#endregion // Construct

		#region Methods
#if false
		/// <summary>
		/// Does 
		/// </summary>
		/// <param name="ident">ident</param>
		/// <returns>True if ident matches this users ident.</returns>
		public bool Match(string ident) // TODO
		{
			// TODO: string vorher überprüfen in get_person
			// sample
			// *!~*@*.host
			// *!~*@domain


			// *!~cl@domain
			// name!~cl@host.de

			string name;
			string client;
			string hnm;
			string host;

			// get name
			int pos = ident.IndexOf('!'); // TODO: '!' nur einmal erlauben
			pos--;
			name = ident.Substring(0, pos);

			if (ident[0] == '~')
				pos++;

			pos = pos + 2;
			ident = ident.Substring(pos, ident.Length-pos);

			// get client
			pos = ident.IndexOf('@');
			pos--;
			client = ident.Substring(0, pos);
			pos = pos + 2;
			ident = ident.Substring(pos, ident.Length-pos);

			// get other
			pos = ident.LastIndexOf('.');
			pos--;
			hnm = ident.Substring(0, pos);
			pos = pos + 2;
			ident = ident.Substring(pos, ident.Length-pos);
			
			// get host
			host = ident;
			ident = String.Empty;

			Console.WriteLine(name+"\n"+client+"\n"+hnm+"\n"+host);

			bool ret = false;
			bool bname = false;
			bool bclient = false;
			bool bother = false;
			bool bhost = false;

			if ((name == "*") || (this.RealName == name)) // TODO: case-sens
				bname = true;

			if ((client == "*") || (this.UserName == client))
				bclient = true;

//TODO			if (hnm == '*' || this.???)
//				bother = true;

			if ((host == "*") || (this.HostName == host))
				bhost = true;

			ret = bname && bclient && bother && bhost;
			Debug.WriteLine(this+": match 1="+bname+" 2="+bclient+" 3="+bother+" 4="+bhost+" ret="+ret);
			return ret;
		}
#endif
		public void Join(Channel chan)
		{
			chan.do_join(this, true);
			this._channels.Add(chan.Name, chan);
		}

#if false
		public void Part(Channel chan)
		{
			chan.do_part(this, true);
		}

		public void Part(Channel chan, bool confirmation)
		{
			chan.do_part(this, confirmation);
		}
#endif

		public virtual void Notice(string message)
		{
			// :blackdragon!~aa@127.0.0.1 NOTICE blackdragon :message
			// old: this.SendLine(this.Ident() + "NOTICE " + nick + " :" + message);
			// new:
			this.SendCommand("NOTICE", this.NickName, ":" + message);
		}

		private void Intro() // TODO: rewrite
		{
			lock (this)
			{
				string server = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName;
				string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
				this.SendCommand(ReplyCodes.RPL_WELCOME, this.NickName, String.Format(":Welcome to the Internet Relay Network {0}", this));
				this.SendCommand(ReplyCodes.RPL_YOURHOST, this.NickName, String.Format(":Your host is {0}, running version {1}",
						server, String.Format("{0}-{1}", MainClass.ProjectName, version)));
				this.SendCommand(ReplyCodes.RPL_CREATED, this.NickName, String.Format(":This server was created {0}", MainClass.BuildDate));
				this.SendCommand(ReplyCodes.RPL_MYINFO, this.NickName, ":TODO");

#if false
				/* nachfolgendes kommt dann in das default-motd */
				this.SendLine(":" + server + " 001 " +
				              this.NickName + " :Welcome " + this.NickName);
				this.SendLine(":" + server + " 002 " +
				              this.NickName + " :Your host is " +
				              server +
				              ", running on " + ((IRCServer)this.Server).devName + ", version: " +
				              Assembly.GetExecutingAssembly().GetName().Version.ToString());
				this.SendLine(":" + server + " 003 " +
				              this.NickName + " :Platform: " +
				              Environment.OSVersion.ToString() +
				              ", CLR version: " + Environment.Version.ToString());
#if DEBUG
				this.SendLine(":" + server + " 004 " +
				              this.NickName + " :Warning: Debug Build");
#else
				this.SendLine(":" + server + " 004 " +
				              this._nickName + " :mono: http://www.mono-project.com");
#endif
				this.SendLine(":" + server + " 005 " +
				              this.NickName + " :Ideas and proposals to blackdragon@bat.berlios.de");
#endif
			}
			this.SendMotd();
		}

		/// <summary>
		/// 
		/// </summary>
		public string UserPrefix()
		{
			// :blackdragon!~aa@127.0.0.1
			// prefix = nickname [ [ "!" user ] "@" host ]
			return (":" + this.NickName + "!" +
				this.UserName + "@" + this.RealHostName + " ");
		}

		private void SendMotd()
		{
			//throw new NotImplementedException();
			Console.WriteLine("Implement SendMotd()");
		}

#if false
// methode entfernen
		/// <summary>
		///
		/// </summary>
		/// <param name="sender">IRCUserConnection</param>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public void SendCommand(IRCUserConnection sender, string command, params string[] args)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			base.SendCommand(sender.UserPrefix(), command, args);
		}
#endif

		public override void SetNick(string newNick)
		{
			Console.WriteLine("SetNick({0})", newNick);

			string oldNick;
			lock (this.NickName)
			{
				oldNick = this.NickName;
				//this._nickName = newNick;
				base.SetNick(newNick);
			}

			if (this.nickset)
			{
				this.SendLine(":" + oldNick + "!" + this.UserName +
					"@" + this.HostName + " NICK :" + newNick);
			}

#if false // erledigt jetzt m_nick
			Console.WriteLine(this._channels.GetEnumerator().GetType().ToString());
			foreach (DictionaryEntry entry in this._channels)
			{
				((Channel)entry.Value).ChangeNickNotice(this, oldNick, newNick);
			}
#endif
			Console.WriteLine(this + ": " + oldNick + " heisst jetzt " + newNick);

			if (this.userset && !this.nickset)
				this.Intro();
			this.nickset = true;
			Console.WriteLine("Nick set [ OK ]");
		}

		// Parameters: <user> <mode> <unused> <realname>
		public override void SetUser(string user, string mode,
		                         string something, string realname)
		{
			// TODO: Parse <mode>
			Console.WriteLine("SetUser({0},{1},{2},{3})", user, mode, something, realname);
			base.SetUser(user, mode, something, realname);

			if (this.nickset && !this.userset)
				this.Intro();
			this.userset = true;
			Console.WriteLine("User set [ OK ]");
		}

		/// <summary>
		/// Is User in Channel...
		/// </summary>
		/// <param name="name">Name of Channel</param>
		/// <returns>True if this user in channel "chan".</returns>
		public bool IsInChannel(Channel chan)
		{
			return chan.HasConnection(this);
		}

		public bool IsRegistered()
		{
			return (this.userset & this.nickset);
		}

		public override string ToString()
		{
			if (this._simpleClient != null)
			{
				return (this.NickName + "!" + this.UserName + "@" + this.RealHostName);
			}
			else
			{
				return base.ToString();
			}
		}
		#endregion // Methods

		#region Properities

		/// Properities
		public string UserName
		{
			get
			{
				return this.SimpleUser.UserName;
			}
		}

		public string HostName
		{
			get
			{
				return this.SimpleUser.HostName;
			}
		}

		public virtual string NickName
		{
			get
			{
				return this.SimpleUser.NickName;
			}
		}

		public virtual string RealName
		{
			get
			{
				return this.SimpleUser.RealName;
			}
		}

		public IRCUserMode UserMode
		{
			get
			{
				return this.SimpleUser.UserMode;
			}
			set
			{
				this.SimpleUser.UserMode = value;
			}
		}

		public IRCUserMode ChanUserMode
		{
			get
			{
				return this.SimpleUser.ChanUserMode;
			}
		}

		public SimpleUser SimpleUser
		{
			get
			{
				return (SimpleUser)this.SimpleClient;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("SimpleUser can't be null!");
				if (value.UpLink != this)
					throw new ArgumentException("SimpleUser.UpLink must be this");

				this.SimpleClient = value;
			}
		}
		#endregion // Properities
	}
}
