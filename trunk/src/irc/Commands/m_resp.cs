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
		private void m_resp(IConnection connection, string[] par)
		{
#if DEC // TODO: look at m_chall
			Console.WriteLine(this + ": resp message");
			IRCConnection src = (IRCConnection)connection;
			if (src.IsPerson())
			{
				if (src.IsServer())
				{
					throw new Exception("is a hacked server");
				}
				else
				{
					src.SendLine(":ERROR");//Command("error");
				}
			}
			// TODO: hauptaugenmerk ist die serverunterstüzung!
//			if (this.HasServer(src.HostName))
//			{
//				src.SendLine(":ERROR");//Command("error");
//			}

			// todo: nur ein beispiel!; läuft auch nur in meinen netz!!!
			//send_capabilities
			// SERVER test.oulu.fi 1 1
			src.SendLine("SERVER 192.168.2.5 1 1");
#endif
		}
	}
}
#endif
