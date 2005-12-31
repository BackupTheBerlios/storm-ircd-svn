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
/* alte version
		private void m_nick(IConnection connection, string[] par)
		{
			if (connection.GetType().Equals(typeof(IRCConnection)))
			{
				if (RFC2812.IsValidNick(par[1]))
				{
				    if (par.Length == 2)
				    {
				    	((IRCConnection)connection).SetNick(par[1]);
					}
					else
					{
						
					}
				}
				else
				{
					connection.Send("");
				}
			}
		}
*/
		private void m_nick(IRCConnection connection, string[] par)
		{
			if (connection is IRCServerConnection) // server teilt uns mit das ein user seinen nick geändert hat
			{
				throw new NotImplementedException("remote nick message; TODO: IMPLEMENT");
				return;
			}
			else
			{
				if (connection is IRCConnection)
				{
					IRCConnection src = (IRCConnection)connection;
					if (src.SimpleClient == null)
					{
						src.SimpleClient = new SimpleUser(src);
					}
					else
					{
						if (!(src.SimpleClient is SimpleUser))
						{
							src.SendLine("ERROR :Illegal nick usage");
							this.CloseLink(src);
							return;
						}
					}
				}
/*
				IRCUserConnection usr;
				if (connection is IRCConnection)
				{
					IRCConnection src = (IRCConnection)connection;
					this.RemoveConnection(src);
					usr = new IRCUserConnection(src);
					this.AddConnection(usr);
				}
				else
					usr = (IRCUserConnection)connection;
*/
				IRCConnection usr = (IRCConnection)connection;
				if (par.Length == 3)
				{
					if (par[0] == String.Empty)
					{
						if (RFC2812.IsValidNick(par[1]))
						{
						    usr.SetNick(par[2]);
						}
						else
						{
							usr.SendLine("ERROR :kein gültiger nick!");
						}
					}
					else
					{
						usr.SendLine("ERROR :du bist kein server!");
					}
				}
			}
		}

		private void NickTaken(IRCUserConnection connection)
		{
			// nick already in use
			// 433
		}

		private void InformUsers(IRCUserConnection connection, string rquested_nick)
		{
		}
	}
}
#endif
