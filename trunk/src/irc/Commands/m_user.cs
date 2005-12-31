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
		* m_user:
		*  ar[0] = prefix
		*  ar[1] = command
		*  ar[2] = username
		*  ar[3] = user modes
		*  ar[4] = unused
		*  ar[5] = real name
		*/
		public virtual void m_user(IRCConnection src, string[] ar)
		{
			if (this.IsUser(src))
			{
				if (((IRCUserConnection)src).IsRegistered())
				{
					src.SendCommand(ReplyCodes.ERR_ALREADYREGISTRED, ":Unauthorized command (already registered)");
					return;
				}
			}

			if (!(src.SimpleClient is SimpleUser))
			{
				src.SendCommand("ERROR", ":Internal error");
				this.CloseLink(src);
				return;
			}

			if (!RFC2812.IsValidUserName(ar[2]))
			{
				return; // ignore message
			}
			if (ar.Length > 5)
			{
				src.SetUser(ar[2], ar[3], ar[4], ar[5]);
				Logger.LogMsg("Got valid USER command from " + src);
				if (((SimpleUser)src.SimpleClient).NickName != string.Empty)
					this.RegisterUser(src);
			}
			else
			{
				// <command> :Not enough parameters
				src.SendCommand(ReplyCodes.ERR_NEEDMOREPARAMS, ar[1], ":Not enough parameters");
			}
		}
	}
}
