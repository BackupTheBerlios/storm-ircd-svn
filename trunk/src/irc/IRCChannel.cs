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
/*	internal class ChannelState
	{
		public IRCConnection connection;
		public Privilegs privilegs;
	}
*/
	public enum ExitType {Quit, Part, Kick}

	public class Channel
	{
		private bool disposed = false;
		private string _name;

		// new
		private string _topic;
		private string _topicnick;
		private DateTime _topicset;
		//

		private ArrayList _members; // IRCConnection objects
		private ArrayList _users; // SimpleUser objects
		private IRCServer _server = null;

		// new
		private IRCChannelMode _modes;
		// end

		private ArrayList _channeloperators;

		/// <summary>
		/// Create a new Channel
		/// </summary>
		public Channel(string name, IRCServer server, SimpleUser op)
		{
			Console.WriteLine("new {0} with name: {1}, is available", this, name);
			this._modes = new IRCChannelMode();
			this._name = name;
			this._topic = String.Empty;
			this._members = new ArrayList();
			this._users = new ArrayList();
			this._server = server;
		}

		/// <summary>
		/// </summary>
		~Channel()
		{
			Console.WriteLine("{0}<{1}> Destrukor", this, this.Name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="oldNick"></param>
		/// <param name="newNick"></param>
		public virtual void ChangeNickNotice(IRCConnection connection,
		                                     string oldNick, string newNick)
		{
			// TODO: implement ChangeNickNotice
		}

		/// <summary>
		/// Dispose object
		/// </summary>
		public virtual void Dispose()
		{
			Console.WriteLine("{0}<{1}>.Dispose()", this, this.Name);
			this.Dispose(true);

			GC.Collect();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
//				if (disposing)
//				{
//				}
				this.disposed = true;
			}
		}

//		/// <summary>
//		/// Kick user from channel
//		/// </summary>
//		/// <param name="name">Nick to kick</param>
//		/// <param name="privilegs">Pivilegs</param>
//		public virtual void KickUser(string name, IPrivilegs privilegs)
//		{
//			// TODO: KickUser
//			if (privilegs.Privilegs <= Privilegs.Master)
//			{
//				foreach (ChannelState state in this._nicks)
//				{
//					if (state.connection.NickName == name)
//					{
////						if (state.connection.Privilegs < privilegs)
////						{
////							state.connection.Send("/KICK " + this._name);
////						
//					//	state.connection.Message(Type.kick);
//					}
//				}
//			}
//		}

		public virtual void KickUser(string nick, IPrivilegs privilegs)
		{
		// getperson
			// todo: nick tolower
//			if (this._nicks.ContainsKey(nick))
//			{
				//if (privilegs.Privilegs.UserModes == null)

//				this._ircServer.Part(null, this.Name);
			//	this._nicks
//			}
		}
		
		public virtual void KickUser(IRCConnection connection)
		{
		//	if (this.
		}
		
		public virtual bool CanJoin(IRCUserConnection usr)
		{
		// if (connection.Channels.Contains(this))
		//  return false;
	//	if (this.HasConnection(connection))
	//		return false;
		// if (this.max == this.client.count)
		//	return false;
		// if (this.modes.contain(EINGELADNE)
		//   if (connection.geteingeladen)
		//     return false;
		// else
		//  return false;
			return true;
		}

		private void AddUser(SimpleUser usr)
		{
			lock (this._users)
			{
				if (this._users.Contains(usr))
				{
					return;
				}
				this._users.Add(usr);
				this._members.Add(usr.UpLink);
				usr.UpLink.Channels.Add(this.Name, this); // TODO: wird auch von server benutzt um bei einen netspliet schneller zu aggieren
			}
		}

		public void RemoveUser(SimpleUser usr, ExitType type) // TODO
		{
			if (usr == null)
				throw new ArgumentNullException("usr");

			lock (this._users)
			{
				if (this._users.Contains(usr))
				{
					this._users.Remove(usr);
					this._members.Remove(usr.UpLink);
					usr.UpLink.Channels.Remove(this.Name);
/* TODO: nachrichten senden
					switch (type)
					{
						case ExitType.Quit:
						case ExitType.Kick:
						case ExitType.Part:
							/*
							* Server informieren <-- nicht notwendig, sieht Part()
							*
					}*/
				}
			}

			if (this.HasMode('P')) // pre-defined channel
				return;

			if (this.MemberCount == 0)
			{
				this._server.RemoveChannel(this);
				this.Dispose();
			}
		}

		public virtual void do_join(IRCUserConnection connection, bool confirmation)
		{
			Console.WriteLine("do_join() new; id: {0}", connection.ID);
			lock (connection)
			{
				if (this.CanJoin(connection))
				{
					this.AddUser(connection.SimpleUser);

					if (confirmation)
					{
						Console.WriteLine("Send Join command to client");
						// TODO: SendCommand auch in do_part
						connection.SendLine(":" + connection.NickName + "!" + connection.UserName + "@" +
				      	        connection.HostName + " JOIN :" + this.Name);
						this.SendNames(connection);
					}
				}
			}
		}

		public virtual void Part(SimpleUser usr, string message) // TODO: SimpleUser
		{
			if (usr == null)
				throw new ArgumentNullException("usr");

			this.RemoveUser(usr, ExitType.Part);
			// TODO: server informieren
#if false
			//connection.SendLine(":" + connection.NickName +
			//			"!" + connection.UserName +
			//			"@" + connection.HostName + " PART " + this.Name);

//			if( ! Remove_Client( REMOVE_PART, chan, Client, Origin, Reason, true)) return false;
//				else return true;
#endif
		}

		/// <summary>
		/// Is "nick" in channel?
		/// </summary>
		/// <param name="nick"></param>
		/// <returns>True if the user in channel</returns>
		public virtual bool HasNick(string nick)
		{
			nick = nick.ToLower();
			lock (this._users)
			{
				foreach (SimpleUser usr in this._users)
				{
					if (usr.NickName.ToLower() == nick)
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool HasConnection(IRCConnection connection)
		{
			if (this._members.Contains(connection))
				return true;
			return false;
		}

		public virtual bool HasUser(SimpleUser usr)
		{
			if (this._users.Contains(usr))
				return true;
			return false;
		}
/*
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="text"></param>
		public virtual void SendChat(IRCConnection sender, string text)
		{// TODO: wenn eine object (Socket) nicht zugreifbar ist --> absturz
			foreach (DictionaryEntry myDE in this._nicks)
			{
				if (((ChannelState)myDE.Value).connection == sender)
					continue;

				if (sender.IsInChannel(this))
				{
					IRCConnection connection = ((ChannelState)myDE.Value).connection;
					connection.SendLine(":" + sender.NickName + // BUGFIX: SendLine
					                "!~" + connection.ClientName +
					                "@" + connection.HostName + " " +
					                "PRIVMSG " + this.Name + " " + text);

					Console.WriteLine(":" + sender.NickName +
					                "!~" + connection.ClientName +
					                "@" + connection.HostName + " " +
					                "PRIVMSG " + this.Name + " " + text);
				}
				else
				{
					// send (404):
				}
			}
		}
		*/

		public virtual void SendChat(IRCConnection sender, SimpleUser usr, string text)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			if (usr == null)
				throw new ArgumentNullException("usr");

			if (this.HasConnection(sender))
			{
				foreach (IRCConnection connection in this._members)
				{
//					if (myDE.Value == sender)
//						continue;

//					IRCUserConnection connection = (IRCUserConnection)myDE.Value;
					// TODO: SendCommand
// :igraltist!n=igraltis@p54A15473.dip.t-dialin.net PRIVMSG #gentoo.de :-kein probelm das tut meine auchohne arts
					connection.SendLine(":" + usr.NickName + // TODO: usr.SendCommand(); sender findet dann keine verwengung mehr
						                "!" + usr.UserName +
						                "@" + usr.HostName + " " +
						                "PRIVMSG " + this.Name + " " + text);
					//connection.SendCommamd("Primsg" + this.Name + " " + text);
				}
			}
			else
			{
			//sender.SendLine(404)
			}
		}

		public virtual void SendWho(IRCUserConnection client, bool only_ops)
		{
			Console.WriteLine(this + ".SendWho()");
			bool is_member, is_visible;

			if (!this.HasConnection(client))
				is_member = false;
			else
				is_member = true;
#if false
			if (!is_member && this.HasMode('s')) // secret chan
				return; // user is not in the channel -> return
#endif
			if (is_member || !this.HasMode('s'))
			{
				foreach (SimpleUser usr in this._users)
				{
//					IRCUserConnection usr = (IRCUserConnection)dict.Value; // TODO: MUSS mal ein SimpleUser sein
				
					if (usr.UserMode.HasMode(IRCUserModes.MODE_INVISIBLE))
						is_visible = false;
					else
						is_visible = true;

					if (is_visible || is_member)
					{
					//	if (only_ops && !usr.ChanUserMode.get(this, IRCUserModes.MODE_OP)) // TODO
					//		continue;

					//:irc.localhost 352 test #test ~aa localhost irc.localhost test H :0 aa
//[channel] [user] [host] [server] [nick]( "H" / "G" > ["*"] [ ( "@" / "+" ) ] :[hopcount] [real name]
//:amd                    352 blackdragon #test    aa    127.0.0.1      amd              blackdragon H :0 aa
//:example.irc.org 352 blackdragon #test ~aa localhost.tux example.irc.org blackdragon H* :0 aa

						client.SendCommand(ReplyCodes.RPL_WHOREPLY, client.NickName, this.Name, usr.UserName, "127.0.0.1" /*usr.HostName/*todo*/, this._server.ServerName, usr.NickName, "H"/*( "H" / "G" > ["*"] [ ( "@" / "+" ) ]*/, String.Format("{0} {1}", usr.HopCount, usr.RealName));//, true); // true ein ":" beim letzen parameter
					}
				}
			}
			client.SendCommand(ReplyCodes.RPL_ENDOFWHO, client.NickName, this.Name, ":End of /WHO list.");
		}

		public virtual void SendNames(IRCUserConnection client)
		{
			string line = ":";
			string[] nicks = this.Nicks;
			for (int j = 0; j < nicks.Length; j++)
			{
				if (j == nicks.Length-1)
					line += nicks[j];
				else
					line += nicks[j] + ' ';
			}

			client.SendCommand(ReplyCodes.RPL_NAMREPLY, client.NickName, "=", this.Name, line); // TODO: liste kann zu lang werden
			client.SendCommand(ReplyCodes.RPL_ENDOFNAMES, client.NickName, this.Name, ":End of NAMES list");
		}

		public int StringToChannelState(string state)
		{
			throw new NotImplementedException();
			return 0;
		}

		public string CahnnelModesToString()
		{
			throw new NotImplementedException();
			return "";
		}

		public bool HasMode(char mode)
		{
			return this._modes.HasMode(mode);
		}

		/// <summary>
		/// Count of users
		/// </summary>
		public int MemberCount
		{
			get
			{
				return this._members.Count;
			}
		}

		/// <summary>
		/// Name of the Channel
		/// </summary>
		public string Name
		{
			get
			{
				return this._name;
			}
		}

		/// <summary>
		/// Get or set the topic of this Channel
		/// </summary>
		public string Topic
		{
			get
			{
				return this._topic;
			}
			set
			{
				this._topic = value;
				// ontopicchanged
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public string[] Nicks
		{
			get
			{
				lock (this._users)
				{
					string[] nicks = new string[this._users.Count];

					int i = 0;
					foreach (SimpleUser usr in this._users)
					{
						nicks[i] = usr.NickName;
						i++;
					}
					return nicks;
				}
			}
		}
	}
}
