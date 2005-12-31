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
/*
<< WHO #test
>> :irc.localhost 353 test = #test :test blackdrag 
>> :irc.localhost 366 test #test :End of NAMES list.
>> :irc.localhost 324 test #test + 
>> :irc.localhost 352 test #test ~aa localhost irc.localhost test H :0 aa
>> :irc.localhost 352 test #test ~aa localhost irc.localhost blackdrag H :0 aa
>> :irc.localhost 315 test #test :End of WHO list.

<< WHO #test
>> :irc.localhost 352 test #test ~aa localhost irc.localhost test H :0 aa
>> :irc.localhost 352 test #test ~aa localhost irc.localhost blackdrag H :0 aa
>> :irc.localhost 315 test #test :End of WHO list.
*/
		private const int MAXWHOREPLIES = 10;
		public virtual void m_who(IRCConnection connection, string[] ar)
		{
			// 0 == prefix
			// 1 == mask
			// 2 == z.B nur ops anzeigen
			if (this.IsServer(connection))
			{
				throw new NotImplementedException("remote /who");
			}
			else if (!this.IsUser(connection))
			{
				// ERR
				connection.SendLine("ERROR :Not registered");
				return;
			}

			if (ar.Length > 3) // zuviel parameter
			{
				connection.SendLine("ERROR :To much parameters");
				return;
			}

			Channel chan = null;
			bool onlyops = false;
			IRCUserConnection src = (IRCUserConnection)connection;

			if (ar.Length == 3)
			{
				// only ops
				if (ar[2] == "o")
					onlyops = true;
//				else
//					connection.SendLine("ERR_NEEDMOREPARAMS_MSG");

			}
			if (ar.Length >= 2)
			{
				// TO/DO: Search Channel
				chan = GetChannel(ar[2]); // TODO: rfc
				if (chan != null)
				{
					chan.SendWho(src, onlyops);

					//src.SendCommand(ReplyCodes.RPL_ENDOFWHO, src.NickName, chan.Name, "End of /WHO list."); // jetzt in IRCChannel
					return;
				}
			}

			foreach (IRCConnection client in this.Clients)
			{
				throw new NotImplementedException();
			}
			if (ar.Length == 1)
			{
				//return "*"
			}
			else
			{
				//return ar[1]
			}
		}
	}
}
#endif
