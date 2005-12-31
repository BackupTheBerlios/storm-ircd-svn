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

using IRC;
using Network;

using Service;

namespace IRC
{
	public partial class IRCServer
	{
		// Command: NAMES
		// Parameters: [ <channel> *( "," <channel> ) [ <target> ] ]
		private void m_names(IRCConnection connection, string[] par)
		{
			if (connection.GetType() != typeof(IRCUserConnection))
			{
				// ERR
				connection.SendLine("ERROR");
				return;
			}
			if (par.Length > 4)
			{
				connection.SendLine("to much parameters"); // 
				return;
			}
			IRCUserConnection src = (IRCUserConnection)connection;

			if (par.Length == 4)
			{
				throw new NotImplementedException("NAMES forward");

				// forward the request to another server
				IRCServerConnection[] srvs = this.SearchServer(par[3]);
				if (srvs.Length > 0)
				{
					// [server name] :No such server
					src.SendCommand(ReplyCodes.ERR_NOSUCHSERVER, src.NickName, par[3], ":No such server");
				}
				else
				{
					//srvs[0].SendCommand(srvs.ServerName, src, "NAMES", par[2], par[3]);
				}
				return;
			}
			if (par.Length == 3)
			{
				Channel chan;
				if (par[2].IndexOf(',') != -1)
				{
					chan = this.GetChannel(par[2]);
					if (chan != null)
					{
						chan.SendNames(src);
					}
				}
				else
				{
					string[] chans = par[2].Split(new char[',']);
					foreach (string schan in chans)
					{
						chan = this.GetChannel(schan);
						if (chan == null)
						{
							continue;
						}
						chan.SendNames(src);
#if false
						src.SendCommand(ReplyCodes.RPL_NAMREPLY, src.NickName, "=", channel.Name, line);
						src.SendCommand(ReplyCodes.RPL_ENDOFNAMES, src.NickName, channel.Name, ":End of NAMES list");
#endif
					}
				}
			}
			else
			{
				foreach (DictionaryEntry entry in this._channels)
				{
					Channel chan = (Channel)entry.Value;
					chan.SendNames(src);
				}

#if false
				// TODO: Nun noch alle Clients ausgeben, die in keinem Channel sind 
				//////
				src.SimpleClient.disadvantage++;
				src.SendCommand(ReplyCode.RPL_ENDOFNAMES, src.NickName, "*", ":End of NAMES list");
#endif
			}
		}
	}
}
