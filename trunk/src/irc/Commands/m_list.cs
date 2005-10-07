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

#if UNSTABLE
namespace IRC
{
	public partial class IRCServer
	{
/*
		client.Send(":" +  this.ServerName/* todo: this._config.ServerName*//* + " 321 " +
			            client.NickName + " Channel :Users Name");

		lock (this._channels)
		{
			foreach (DictionaryEntry entry in this._channels)
			{
				string name = (string)entry.Key;
				Channel ch = ((Channel)entry.Value);

				client.Send(":" +  this.ServerName/*todo._config.ServerName +
				            " 322 " + client.NickName + " " + name +
				            " " + ch.MemberCount + " :" + ch.Topic);

				Console.WriteLine(":" +  this.ServerName/*todo._config.ServerName +
				            " 322 " + client.NickName + " " + name +
				            " " + ch.MemberCount + " :" + ch.Topic);
			}
		}

			// end of /list
		client.Send(":" +  this.ServerName/*todo._config.ServerName + " 323 " +
		            client.NickName + " :End of /LIST");
*/
		private void m_list(IConnection connection, string[] par) // TODO
		{
			Console.WriteLine(this + ": TODO: list message");
#if DEC
			IRCConnection src = (IRCConnection)connection;

			if (par.Length == 2)
			{
				if (par[0] == String.Empty && src.IsPerson())
				{
					src.SendCommand(MessageType.Server, "321"/*TODO: nummern definieren*/,
						new string[]{src.NickName, "Channel", "topic"});

					lock (this._channels)
					{
						foreach (DictionaryEntry entry in this._channels)
						{
							string name = (string)entry.Key;
							Channel ch = ((Channel)entry.Value);

							src.SendCommand(MessageType.Server, "322",
								new string[]{src.NickName, name, ch.MemberCount.ToString(), "test" + ch.Topic});
						}
					}
					
					src.SendCommand(MessageType.Server, "323",
						new string[]{src.NickName, "End of /LIST"});
				}
			}
#endif
		}
	}
}
#endif