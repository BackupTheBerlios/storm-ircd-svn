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

using IRC;
using Network;

namespace IRC
{
	public partial class IRCServer
	{
/*
           ERR_NEEDMOREPARAMS              ERR_NOSUCHCHANNEL
           ERR_NOTONCHANNEL
*/
		private void m_part(IRCConnection src, string[] ar)// TODO: SendCommand anpassen
		{
			SimpleUser usr = null; // TODO

			if (this.IsServer(src))
			{
				throw new NotImplementedException("remote /part");
				//usr = this.Search(ar[0]); // TODO
				if (usr == null)
				{
					return;
				}
			}
			else if (!this.IsUser(src))
			{
				// ERR
				src.SendLine("ERROR");
				return;
			}
			else
			{
				usr = (SimpleUser)src.SimpleClient;
			}

#if false
			if (ar.Length > 4) // zuviel parameter
			{
				src.SendLine("to much parameters");
				return;
			}
#endif
			if (ar.Length < 3)
			{
				// <command> :Not enough parameters
				src.SendCommand(ReplyCodes.ERR_NEEDMOREPARAMS, usr.NickName, ar[1], ":Not enough parameters");
				return;
			}

			Channel chan;
			string msg = string.Empty;
			if (ar.Length > 3)
				msg = ar[3];
			else
				msg = usr.NickName;

			string[] chans = ar[2].Split(new char[',']);
			foreach (string schan in chans)
			{
				chan = this.GetChannel(schan);
				if (chan == null)
				{
					// <channel name> :No such channel
					src.SendCommand(ReplyCodes.ERR_NOSUCHCHANNEL, usr.NickName, chan.Name, ":No such channel"); 
					continue;
				}
				else if (!chan.HasUser(usr))
				{
					// <channel> :You're not on that channel
					src.SendCommand(ReplyCodes.ERR_NOTONCHANNEL, usr.NickName, chan.Name, ":You're not on that channel");
					continue;
				}
				this.Part(usr, chan, msg);
			}
		}
	}
}
