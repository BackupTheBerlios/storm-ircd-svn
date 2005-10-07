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

#if UNSTABLE
namespace IRC
{
	public partial class IRCServer
	{
		private void m_ping(IConnection connection, string[] par)
		{
			// :amd.tux PONG amd.tux :506855393
			if (par.Length < 2)
			{
				connection.SendLine(":ERROR ERR_NOORIGIN");
				return;
			}

			string name = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.ServerName;

			if (par.Length == 4)
				connection.SendLine(":"+name+" PONG " + par[3] + " :"+par[2]);
			else
				connection.SendLine(":"+name+" PONG " + name + " :"+par[2]);

			((IRCConnection)connection).LastPing = DateTime.Now; // reset timer
		}
	}
}
#endif
