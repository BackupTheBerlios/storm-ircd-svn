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
		private void m_mode(IConnection connection, string[] par)
		{
			System.Console.WriteLine("TODO mode dummy");
#if DEC
			IRCConnection src = (IRCConnection)connection;
			foreach (string l in par)
			{
				Console.WriteLine(l);
			}
			Console.WriteLine("Umodes for "+src.ID+": "+src.UserMode);
			//src.SendCommand("+"+src.UserModes.ToString())
#endif
		}
	}
}
#endif
