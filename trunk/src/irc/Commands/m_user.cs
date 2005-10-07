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
/* alte version		public virtual void m_user(IConnection connection, string[] par)
		{

			if (connection.GetType().Equals(typeof(IRCConnection)))
			{
				if (par.Length == 5)
				{
					((IRCConnection)connection).SetUser(par[1], par[2], par[3], par[4]);
				}
			}
		}
*/
		public virtual void m_user(IConnection connection, string[] par)
		{
			// TODO: H.P.
			Console.WriteLine(this + ": user message");
			if (!((connection is IRCConnection) || (connection is IRCUserConnection)))
			{
				connection.SendLine("ERROR: connetion type must be client!");
				this.CloseLink(connection);
				return;
			}

			IRCConnection src = (IRCConnection)connection;
			if (!(src.SimpleClient is SimpleUser))
			{
				src.SendLine("ERROR: internal error, drop connection");
				this.CloseLink(src);
				return;
			}
			//if (RFC2812.IsValidUser());
			if (par.Length == 6)
			{
				src.SetUser(par[1], par[2], par[3], par[4]);
				if (((SimpleUser)src.SimpleClient).NickName != string.Empty)
					this.RegisterUser(src);
			}
			else
				src.SendLine("ERROR: not enough parameters");
		}
	}
}
#endif