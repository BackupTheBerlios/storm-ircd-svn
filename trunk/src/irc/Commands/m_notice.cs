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

using IRC;
using Network;

#if UNSTABLE
namespace IRC
{
	public partial class IRCServer
	{
		private void m_notice(IConnection connection, string[] par)
		{
			System.Console.WriteLine("NOTICE dummy");
			if (par[1] == "AUTH")
				return; // eine AUTH Zeile; ensteht dadurch das storm-ircd einen verbingung zu einen anderen server herstellt -> ignorieren
		}
	}
}
#endif
