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
using Tools;
using Service;
using Network;

#if UNSTABLE
namespace IRC
{
	public partial class IRCServer
	{
/*		public virtual void m_stat(IConnection connection, string[] ar)
		{
			Console.WriteLine("===============================");
			Console.WriteLine("= Server State:");
			Console.WriteLine("= TotalMemory: {0} bytes", GC.GetTotalMemory(true));
			Console.WriteLine("= Clients: {0}", this.Clients.Count);
			Console.WriteLine("= Errors: {0}", stat.errors);
			Console.WriteLine("= Traffic: {0} bytes", stat.traffic);
			Console.WriteLine("= Threads: {0}",0);
			Console.WriteLine("===============================");
		}
*/
		public virtual void m_stat(IRCConnection connection, string[] ar)
		{
#if false
			IRCConnection src = (IRCConnection)connection;

			if (true)//src.UserMode.get('D')) // only for testing
			{
				if (ServiceManager.Services.HasService(typeof(Statistic)))
				{
					Statistic service = ((Statistic)ServiceManager.Services[typeof(Statistic)]);

					src.Notice("server", this.ServerName + " stats: uptime=" +
								service.UpTime + "; Clients=" + this.Clients.Count +
								"; Traffic=" + stat.traffic +/* TODO: stat durch Statistic ersetzen */
								" bytes ; Threads=" + service.Threads +
								"; TotalMemory in use=" + GC.GetTotalMemory(true) +
								" bytes ; Errors=" + stat.errors);
								
				}
				else
					src.Notice("server", "Stats not Available.");
			}
			else
			{
				src.Notice("server", "Permission denied, you have no D-umode");
			}
#endif
		}
	}
}
#endif
