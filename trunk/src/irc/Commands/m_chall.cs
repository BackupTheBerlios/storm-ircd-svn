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
		private void m_chall(IRCConnection connection, string[] par)
		{
			Console.WriteLine(this + ": chall message; TODO");
			foreach (string s in par)
			{
				Console.WriteLine("chall: " +s);
			}

#if DEC
			if (!(connection is IRCConnection))
			{
				// TODO: CloseLink()
				return;
			}
			IRCConnection src = (IRCConnection)connection;

			if (src.IsPerson())
			{
				if (src.IsServer())
				{
					throw new Exception("is a hacked server");
				}
				else
				{
					src.SendLine(":ERROR unknown command");

					this.send_ops_flag();

				}
			}

			if (src.IsServer() || src.IsPerson()) // wenn wir das bereits wissen sollten wir kein CHALL bekommen
				return;

			if (!this.EqualHosts(src.RealHostName, this.ServerName))
			{
				src.SendLine("ERROR :I am " + this.ServerName + " not " + src.RealHostName);
				//this.SendToOps(UMODE_SERVCONNECT)
				this.CloseLink(src, "Sorry");
				return;
			}

			if (this.HasServer(src.HostName))
			{
				src.SendLine("ERROR :Server " + src.RealHostName + "already exists");
				if (src.UserMode.get(IRCUserModes.CAP_SERVICES))
					this.send_ops_flag();

				this.CloseLink(src, "Server already exists");
				return;
			}

			// sample:
			src.SendLine("CHALL 192.168.2.5 localhost. " + par[4] + " password :TS");
#endif
		}
	}
}
#endif
