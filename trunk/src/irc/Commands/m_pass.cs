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
/*
pass-msg: 
pass-msg: pass
pass-msg: password
pass-msg: 0211000001
pass-msg: IRC|aEFJKMRTu
pass-msg: P
*/
		public virtual void m_pass(IRCConnection connection, string[] ar)
		{
			// TODO: pass ist auch vom clienten möglich!
			Console.WriteLine(this + ": pass message");
			foreach (string s in ar)
			{
				Console.WriteLine("pass-msg: " +s);
			}

			if ((!(connection.GetType() == typeof(IRCConnection))) || ((IRCConnection)connection).PassSet)
			{
				connection.SendLine("ERROR: ERR_ALREADYREGISTRED");//connection.CloseLink(); // TO/DO: CloseLink soll RemoveConnetion in der Server-Klasse ersetzen/erweitern; RemoveConnation bleibt bestehen ruft aber nicht mehr connection.Dispose auf.
				return;
			}
			if (ar.Length < 3)
			{
				connection.SendLine("ERROR: ERR_NEEDMOREPARAMS");
				return;
			}

			if (false) // wenn passwort falsch
			{
				connection.SendLine("ERROR: Wrong Password, clossing link ...");
				this.CloseLink(connection); // CloseLink() kümmert sich um alles
				return;
			}
			((IRCConnection)connection).PassSet = true;

			if (ar.Length > 3)
			{
				// TODO: TS-Server
				;
			}
		}
	}
}
