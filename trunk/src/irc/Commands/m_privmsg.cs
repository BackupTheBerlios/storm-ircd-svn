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

#if UNSTABLE
namespace IRC
{
	public partial class IRCServer
	{
		private void m_privmsg(IRCConnection connection, string[] par)
		{
			Console.WriteLine("TODO privmsg");
			IRCConnection src = (IRCConnection)connection;
//#if false
			par[2] = par[2].ToLower();

			if (par.Length == 4)
			{
				// channel prefixes
				if (par[2][0] == '#')
				{
//					Channel ch = ((IRCConnection)connection).GetChannel(par[2]);
					Channel ch = this.GetChannel(par[2]);

					if (ch != null)
						ch.SendChat(src, ((SimpleUser)src.SimpleClient), par[3]);
				}
				else
				{
					if (connection is IRCUserConnection)
					{
						IRCUserConnection irccon = connection as IRCUserConnection;
						if (this.HasNick(par[2]))
						{
							this.SendTo(":" + irccon.NickName +
										"!" + irccon.UserName +
										"@" + irccon.HostName + " " +
										"PRIVMSG " + par[2] + " " + par[3], par[2]);
						}
					}
				}
			}
//#endif
		}
	}
}
#endif
