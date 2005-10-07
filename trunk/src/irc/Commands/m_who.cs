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
<< JOIN #test
>> :test!~aa@localhost JOIN :#test
<< MODE #test
<< WHO #test
>> :irc.localhost 353 test = #test :test blackdrag 
>> :irc.localhost 366 test #test :End of NAMES list.
>> :irc.localhost 324 test #test + 
>> :irc.localhost 352 test #test ~aa localhost irc.localhost test H :0 aa
>> :irc.localhost 352 test #test ~aa localhost irc.localhost blackdrag H :0 aa
>> :irc.localhost 315 test #test :End of WHO list.

// zweiter:
<< WHO #test
>> :irc.localhost 352 test #test ~aa localhost irc.localhost test H :0 aa
>> :irc.localhost 352 test #test ~aa localhost irc.localhost blackdrag H :0 aa
>> :irc.localhost 315 test #test :End of WHO list.
*/
		private const int MAXWHOREPLIES = 10;
		public virtual void m_who(IConnection connection, string[] ar)
		{
			// 0 == prefix
			// 1 == mask
			// 2 == z.B nur ops anzeigen
			Console.WriteLine("who dummy");
			string channel;
			IRCConnection src = (IRCConnection)connection;

			if (false)//!this.IsMyClient(src)) // TODO: ismyclient implementieren
			{
				return;
			}

			if (ar[1] != String.Empty)
			{
			}
			else
				channel = ar[0];
		}
	}
}
#endif
