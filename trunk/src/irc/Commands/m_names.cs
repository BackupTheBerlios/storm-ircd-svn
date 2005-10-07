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

using Service;

namespace IRC
{
	public partial class IRCServer
	{
		private void m_names(IConnection connection, string[] par)
		{
			System.Console.WriteLine("names handler");
			if (connection.GetType() == typeof(IRCUserConnection))
			{
				string channelName = par[1];
				for (int i = 0; i< 10;i++) // TODO: siehe RFC1459 /NAMES
				{
					if ((par.Length == 3))
					{
						connection.SendLine("ERROR");
						return;
					}
						
					IRCUserConnection src = (IRCUserConnection)connection;
					Channel channel = this.GetChannel(channelName);
					if (channel == null)
					{
						src.SendLine("Error not found" +channelName);
						return;
					}
					string server = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName;

					//src.SendCommand(MessageType.Server, "324",
					//	new string[]{src.NickName, channel, '+'});
					src.SendLine(":" + server + " 324 " +
	              		src.NickName + " " + channel + "+");

					string command = string.Empty;
					string[] nicks = channel.Nicks;
					for (int j = 0; j < nicks.Length; j++)
					{
						if (j == nicks.Length-1)
							command += nicks[j];
						command += nicks[j] + ' ';
					}

					src.SendCommand(MessageType.Server, "353",
						new string[]{src.NickName, channel.Name, command});

					src.SendCommand(MessageType.Server, "366",
						new string[]{src.NickName, channel.Name, "end of /NAMES list"});
				}
			}
			else
				connection.Send("ERROR");
		}
	}
}
