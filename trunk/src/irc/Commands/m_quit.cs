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
		public virtual void m_quit(IRCConnection connection, string[] ar)
		{
			string quitMessage = "quit";
			if (this.IsServer(connection))
			{
				throw new NotImplementedException("remote /quit");
			}
			else if (!this.IsUser(connection))
			{
				connection.SendLine("ERROR :Not allowed");
				this.CloseLink(connection);
				return;
			}
			else
			{
				IRCUserConnection src = (IRCUserConnection)connection;

				if (ar.Length > 2)
				{
					quitMessage = ar[2];
				}
				this.RemoveFromChannels(src, quitMessage);
				this.CloseLink(src);
			}
#if false
			lock (connection)
			{
			// avaible auslesen
				this.CloseLink(connection); // TODO
			}
#endif
		}
	}
}
