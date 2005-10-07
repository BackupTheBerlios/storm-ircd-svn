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
using Network;

namespace IRC
{
	/// <summary>
	/// Description of RFC2813.	
	/// </summary>
	public class RFC2813
	{
		private string nick = @"(?<nick>[^ ]+) (?<name>)"; //(?<user>[^@]+)@(?<host>.+)
	//	private string nick = @"^nick *";
		private string ping = @"ping *";

		/// <summary>
		/// 
		/// </summary>
		public string ERR_NOSUCHSERVER;

		public static string SendPass(IRCConnection cn, string /*password*/moresecretpassword)
		{
			// erros reply: ERR_NEEDMOREPARAMS ERR_ALREADYREGISTRED
		//	cn.SendCommand(MessageType.
			return "PASS" + moresecretpassword  + "0210010000 IRC|aBgH$ Z";
		}

		public static string Server()
		{
			// error reply: ERR_ALREADYREGISTRED
			return "SERVER " + "test.oulu.fi" + " 1 1 :Experimental server";
		}

		public static string Nick()
		{
			return "NICK syrk 5 kalt millennium.stealth.net 34 +i :Christophe Kalt";
		}

		public static bool IsValidServerName(string command)
		{
			return true;
		}
	}

#if UNSTABLE
	// neu: der code für die verarbeitung wird verteilt
	public partial class IRCServer // RFC2813 extension in IRCServer
	{
		public void SendPass(IConnection f)
		{
			//PASS moresecretpassword 0210010000 IRC|aBgH$ Z
		}

		public void SendServer(IConnection f)
		{
			//SERVER test.oulu.fi 1 1 :Experimental server ; New server
            //                       test.oulu.fi introducing itself and
            //                       attempting to register.
		}
		
		private void nick1()
		{
		}

		private void nick2()
		{
		}
	}
#endif
}
