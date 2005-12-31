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
using Service;
using Network;

namespace IRC
{
	public partial class IRCServer
	{
		/*
		* m_restart:
		*/
		private void m_restart(IRCConnection src, string[] ar)
		{
			if (!this.IsUser(src) || !((IRCUserConnection)src).UserMode.HasMode(IRCUserModes.MODE_OP))
			{
				// :Permission Denied- You're not an IRC operator
				src.SendCommand(ReplyCodes.ERR_NOPRIVILEGES,
						((IRCUserConnection)src).NickName,
						":Permission Denied- You're not an IRC operator");
				return;
			}

			// Close all links
			foreach (IRCConnection con in this.Clients)
			{
				if (this.IsServer(con))
				{
					con.SendCommand(((IRCUserConnection)src).SimpleUser, "ERROR", ":Restarted by " + src);
					this.CloseLink(con);
				}
				else if (this.IsUser(con))
				{
					((IRCUserConnection)con).Notice("Server Restarting. " + src);
					this.CloseLink(con, "Server Restarting");
				}
				else
				{
					this.CloseLink(con, "Server Restarting");
				}
			}

			Logger.LogMsg("RESTART by " + src);
			MainClass.Restart();
		}
	}
}
