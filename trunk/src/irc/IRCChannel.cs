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
// todo: nur noch IRCConnection.SendLine() nutzen !
// BUGFIX: lock (members)
namespace IRC
{
	internal class ChannelState
	{
		public IRCConnection connection;
		public Privilegs privilegs;
	}

	/// <summary>
	/// Class for Channels
	/// </summary>
	/// <remarks>
	/// 	created by - Josef Schmeisser
	/// 	created on - 06.03.2004 22:39:45
	/// </remarks>
	public class Channel
	{
		private bool disposed = false;
		private string _name;
		///////////////////new
		private string _topic;
		private string _topicnick;
		private DateTime _topicset;
		///////////////////////

		private Hashtable _members;// todo: soll _nicks ersetzen
		/////////
//		private IRCConnection chatMaster = null;
		private IRCServer _ircServer = null;
///		private Configuration _config;

		// new
		private IRCChannelModes _modes;
		// end

		private ArrayList _channeloperators;

		/// <summary>
		/// Create a new Channel
		/// </summary>
		public Channel(string name, /*Configuration config,*/
		               IRCServer server, IRCConnection chatMaster)
		{
///			this._config = config;
			this._name = name;
			this._topic = String.Empty;
//			this.chatMaster = chatMaster;

			this._members = new Hashtable();
//			ChannelState state = new ChannelState();
//			state.connection = chatMaster;
//			state.privilegs = Privilegs.Master;
//			this._nicks.Add(chatMaster.ID, state);

			this._ircServer = server;
		}

		/// <summary>
		/// </summary>
		~Channel()
		{
			Console.WriteLine("Channel: Destrukor [ OK ]");
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
			Console.WriteLine("Channel: Pre-Destrukor1");
			this.Dispose(true);

			GC.Collect();
		}

		/// <summary>
		/// Dispose object internal
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{
//			lock (this)
//			{
				if (!this.disposed)
				{
//					if (disposing)
//					{
//						this.Send("left");
//						this.Socket.Close();
//					}
					// TO/DO: Channle add dispose code
					Console.WriteLine("Channel: Pre-Destrukor");
					this.disposed = true;
				}
//			}
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
		
		public virtual bool CanJoin(IRCConnection connection)
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

		public virtual void do_join(IRCUserConnection connection, bool confirmation)
		{
			Console.WriteLine("do_join() new; id: {0}", connection.ID);
			lock (this._members)
			{
				lock (connection)
				{
					if (this.CanJoin(connection))
					{
						this._members.Add(connection.ID, connection);

						if (confirmation)
						{
							Console.WriteLine("Send Join command to client");
							// TODO: SendCommand auch in do_part
							connection.SendLine(":" + connection.NickName + "!~" + connection.ClientName + "@" +
					      	          connection.HostName + " JOIN :" + this.Name);

							//connection.Names(this.Name); // TODO
						}
					}
				}
			}
		}

		public virtual void do_part(IRCUserConnection connection, bool confirmation)
		{
			lock (this._members)
			{
				lock (connection)
				{
					this._members.Remove(connection.ID);
					Console.WriteLine("Count nicks: " + this._members.Count);

					if (confirmation)
					{
						connection.SendLine(":" + connection.NickName + "!~" +
						                connection.ClientName + "@" +
						                connection.HostName + " PART " + this.Name);

						this.SendChat(connection, "left"); // wird übersprungne -> da nicht im channel
					}
				}
			}
		}

		/// <summary>
		/// Is parameter nick in channel?
		/// </summary>
		/// <param name="nick"></param>
		/// <returns>True if the user in channel</returns>
		public virtual bool HasNick(string nick) // TODO: auch bei anderen servern anfragen
		{
			nick = nick.ToLower();
			lock (this._members)
			{
				foreach (DictionaryEntry myDE in this._members)
				{
					IRCUserConnection irccon = (IRCUserConnection)myDE.Value;
					if (irccon.NickName == nick)
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual bool HasConnection(IConnection connection) // TODO: auch bei anderen servern anfragen
		{
			if (this._members.Contains(connection.ID))
				return true;
			return true;
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
		public virtual void SendChat(IRCUserConnection sender, string text)
		{
			// TODO: wenn eine object (Socket) nicht zugreifbar ist --> absturz
			if (this.HasConnection(sender))
			{
				foreach (DictionaryEntry myDE in this._members)
				{
					if (myDE.Value == sender)
						continue;

					IRCUserConnection connection = (IRCUserConnection)myDE.Value;
					// TODO: SendCommand
					connection.SendLine(":" + sender.NickName + // BUGFIX: SendLine
						                "!~" + connection.ClientName +
						                "@" + connection.HostName + " " +
						                "PRIVMSG " + this.Name + " " + text);
					// connection.Commamd("Primsg" + this.Name + " " + text);
				}
			}
			else
			{
			//sender.SendLine(404)
			}
		}

		public int StringToChannelState(string state)
		{
			return 0;
		}

		public string CahnnelModesToString()
		{
			return "";
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
		/// String array of nick's
		/// </summary>
/*		public string[] Nicks
		{
			get
			{
				string[] nicks = new string[this._nicks.Count];
				IDictionaryEnumerator enumerator = this._nicks.GetEnumerator();

				int i = 0;
				while (enumerator.MoveNext())
				{
					nicks[i] = (string)enumerator.Key;
					i++;
				}
				return nicks;
			}
		}*/

		public string[] Nicks
		{
			get
			{
				lock (this._members)
				{
					string[] nicks = new string[this._members.Count];
					IDictionaryEnumerator enumerator = this._members.GetEnumerator();

					int i = 0;
					while (enumerator.MoveNext())
					{
						nicks[i] = ((IRCUserConnection)enumerator.Value).NickName;
						i++;
					}
					return nicks;
				}
			}
		}
	}
}
