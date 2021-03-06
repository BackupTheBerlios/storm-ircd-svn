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
/* orginal m_join		public virtual void m_join(IConnection connection, string[] par)
		{
			if (par.Length == 2)
			{
				if (par[1].Length > 1) // BUFIX emule "#" -channel
					this.Join(connection as IRCConnection, par[1]);
			}
		}
*/

		private void m_join(IRCConnection connection, string[] par)
		{
			Console.WriteLine("TO/DO: join command");
			if (connection is IRCUserConnection)
			{
				// TODO: if (RFC2812.IsJoinCommand);
				if (par.Length == 3)
				{
					if (par[1].Length > 2) // TODO: regex
						this.Join((IRCUserConnection)connection, par[2]);
				}
			}
			else
				connection.SendLine("ERROR: type !IRCUserConnection");
		}
	}
}
#endif
