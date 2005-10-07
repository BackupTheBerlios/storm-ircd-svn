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
		bool nickset = false;
		bool userset = false;
		bool passset = false;
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
		private IRCUserMode _mode;
		private DateTime _lastping; // todo
		// end

		private Hashtable _channels;

		#region Construct
		public IRCUserConnection(IRCConnection bas) : base(bas.Socket, bas.Server)
		{
			// TODO: sollte gleiche id haben: int id = bas.ID;
			Socket handler = bas.Socket;
			this._channels = new Hashtable();

//			this.SetNick(bas.NickName);
//			this.SetUser(bas.ClientName, bas.HostName, "", bas.RealName);
			if (!(bas.SimpleClient is SimpleUser))
				throw new ArgumentException("bas.SimpleClient musst be a SimpleUser");

			this._simpleClient = bas.SimpleClient;
			this.SimpleUser.UpLink = this;

			// TODO: nachfolgendes ist noch nötig soll sich änder
			this.SetNick(this.SimpleUser.NickName);
			this.SetUser(this.SimpleUser.ClientName, this.SimpleUser.HostName,
							"", this.SimpleUser.RealName);
		}
		#endregion // Construct

		#region Methods
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

			if ((client == "*") || (this.ClientName == client))
				bclient = true;

//TODO			if (hnm == '*' || this.???)
//				bother = true;

			if ((host == "*") || (this.HostName == host))
				bhost = true;

			ret = bname && bclient && bother && bhost;
			Debug.WriteLine(this+": match 1="+bname+" 2="+bclient+" 3="+bother+" 4="+bhost+" ret="+ret);
			return ret;
		}

		public void Join(Channel chan)
		{
			chan.do_join(this, true);
			this._channels.Add(chan.Name, chan);
		}

		public void Part(Channel chan)
		{
			chan.do_part(this, true);
		}

		public void Part(Channel chan, bool confirmation)
		{
			chan.do_part(this, confirmation);
		}

		public virtual void Notice(string nick, string message)
		{
			// :blackdragon!~aa@127.0.0.1 NOTICE blackdragon :message
			// old: this.SendLine(this.Ident() + "NOTICE " + nick + " :" + message);
			// new:
			this.SendCommand("NOTICE", new string[] {nick, message});
		}

		private void Intro()
		{
			lock (this)
			{
			string server = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName;
				this.SendLine(":" + server + " 001 " +
				              this.NickName + " :Welcome " + this.NickName);//, MessagePriority.RealTime);
				this.SendLine(":" + server + " 002 " +
				              this.NickName + " :Your host is " +
				              server +
				              ", running on " + ((IRCServer)this.Server).devName + ", version: " +
				              Assembly.GetExecutingAssembly().GetName().Version.ToString());//, MessagePriority.RealTime);
				this.SendLine(":" + server + " 003 " +
				              this.NickName + " :Platform: " +
				              Environment.OSVersion.ToString() +
				              ", CLR version: " + Environment.Version.ToString());//, MessagePriority.RealTime);
#if UNSTABLE
				this.SendLine(":" + server + " 004 " +
				              this.NickName + " :Warning: Debug Build");//, MessagePriority.RealTime);
#else
				this.SendLine(":" + server + " 004 " +
				              this._nickName + " :mono: http://www.mono-project.com");//, MessagePriority.RealTime);
#endif
				this.SendLine(":" + server + " 005 " +
				              this.NickName + " :Josef.Schmeisser@freenet.de");//, MessagePriority.RealTime);
			}
// todo: implement			this.Motd();
		}

		public string Ident()
		{
			// :blackdragon!~aa@127.0.0.1
			// TODO: this.SendLine(":" + this._nickName + "!~" + this._clientName + "@" + this._hostName + ); // TODO: ausbessern
			return (":" + this.NickName + "!" +
				this.ClientName + "@" + this.RealHostName + " ");
		}

		public string ServerIdent()
		{
			return (":" + ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName + " ");
		}

		public void SendCommand(string command, string[] args)
		{
			Console.WriteLine(this + ": SendCommand warning use Client");
			this.SendCommand(MessageType.Client, command, args);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <param name="args"></param>
		public void SendCommand(MessageType type, string command, string[] args)
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
				this.SendLine(":" + oldNick + "!~" + this.ClientName +
					"@" + this.HostName + " NICK :" + newNick);
			}

#if DEC // erledigt jetzt m_nick
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

		public override void SetUser(string clientName, string host,
		                         string host2, string realName)
		{
			Console.WriteLine("SetUser({0},{1},{2},{3})", clientName, host, host2, realName);
			base.SetUser(clientName, host, host2, realName);

			if (this.nickset && !this.userset)
				this.Intro();
			this.userset = true;
			Console.WriteLine("User set [ OK ]");
		}

		/// <summary>
		/// Is User in Channel...
		/// </summary>
		/// <param name="name">Name of Channel</param>
		/// <returns>True if this user in channel "name".</returns>
		public bool IsInChannel(string name)
		{
			Console.WriteLine("TODO: impelement isinchannel");
#if DEC
			lock (this._channels)
			{
				if (this._channels.ContainsKey(name))
					return true;
			}
#endif
			return false;
		}
		#endregion // Methods

		#region Properities

//#if DEC
		/// Properities
		public string ClientName
		{
			get
			{
				return this.SimpleUser.ClientName;
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
//#endif
		public SimpleUser SimpleUser
		{
			get
			{
				return (SimpleUser)this.SimpleClient;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("SimpleUser can't be null");
				if (value.UpLink != this)
					throw new ArgumentException("SimpleUser.UpLink must be this");

				this.SimpleClient = value;
			}
		}
		#endregion // Properities
	}
}
