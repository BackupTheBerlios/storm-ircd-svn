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
		* m_oper:
		*  ar[0] = prefix
		*  ar[1] = command
		*  ar[2] = oper name
		*  ar[3] = password
		*/
		private void m_oper(IRCConnection src, string[] ar)
		{
/*
           ERR_NEEDMOREPARAMS              RPL_YOUREOPER
           ERR_NOOPERHOST                  ERR_PASSWDMISMATCH
*/
			if (!this.IsUser(src))
			{
				// do something
				return;
			}

			if (ar > 3)
			{
				IRCUserConnection usr = (IRCUserConnection)src;
				if (this.IsOper(usr.SimpleUser))
				{
					//<< oper blackdragon pass
					//>> :blackdragon MODE blackdragon :+o
					//>> :example.irc.org 381 blackdragon :You are now an IRC Operator
					// :You are now an IRC operator
					usr.SendCommand(ReplyCodes.RPL_YOUREOPER, usr.NickName, ":You are now an IRC operator");
				}

				// search operator ...
				OperData od = this.GetOperData(ar[2], usr.SimpleUser);
				if (od != null)
				{
					if (od.Password != ar[3])
					{
						// :Password incorrect
						usr.SendCommand(ReplyCodes.ERR_PASSWDMISMATCH, usr.NickName, ":Password incorrect");
						return;
					}
					if (true)
					{
						// :No O-lines for your host
						usr.SendCommand(ReplyCodes.ERR_NOOPERHOST, usr.NickName, ":No O-lines for your host");
						return;
					}
					/* TODO */
				}
				else
				{
					// :No O-lines for your host
					usr.SendCommand(ReplyCodes.ERR_NOOPERHOST, usr.NickName, ":No O-lines for your host");
					return;
				}
			}
			else
			{
				// <command> :Not enough parameters
				src.SendCommand(ReplyCodes.ERR_NEEDMOREPARAMS, ar[1], ":Not enough parameters");
			}
		}
	}
}
