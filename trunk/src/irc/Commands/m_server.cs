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
		public virtual void m_server(IRCConnection connection, string[] ar)
		{
			// SERVER test.oulu.fi 1 :[tolsun.oulu.fi]
			// test.oulu.fi	== Server-Name
			// 1			== Hop-Count
			// [*]			== IP/DNS-Name

			Console.WriteLine(this + ": server message");
			foreach (string s in ar)
			{
				Console.WriteLine("server-msg: " +s);
			}
			if (connection is IRCUserConnection)
			{
				connection.SendLine("ERROR :already registered as client");
				return;
			}
			if (ar.Length < 4)
			{
				connection.SendLine("ERROR :SERVER needs more params");
				this.CloseLink(connection);
				return;
			}

			/*
			** The SERVER message must only be accepted from either (a) a connection
			** which is yet to be registered and is attempting to register as a
			** server, or (b) an existing connection to another server, in  which
			** case the SERVER message is introducing a new server behind that
			** server.
			*/
			if (connection is IRCServerConnection) // neu Server; wird uns von einen anderen Server mitgeteilt
			{
				SimpleServer rser = new SimpleServer();
				rser.HopCount = Convert.ToInt32(ar[3]);
				rser.UpLink = (IRCServerConnection)connection;
			}
			else // ein neuer Server versucht sich direkt zu registrieren
			{
				if (this.HasServer(connection.Socket.RemoteEndPoint)) // <-- HACK
				{
					connection.SendLine("ERROR :Server already exits; Potential Damage To Network Intergrity");
					this.CloseLink(connection);
					return;
				}
/*				if (this.HasServer(ar[2])) veraltet
				{
					connection.SendLine("hostmask allrady in use");
					this.CloseLink(connection);
					return;
				}*/
				if (!((IRCConnection)connection).PassSet)
				{
					connection.SendLine("ERROR :no permission");
					this.CloseLink(connection);
					return;
				}
				// anmerkung: wird nur gemacht wenn er sich direkt registriert
				if (!this.HasServerAccess(ar[2]))
				{
					connection.SendLine("ERROR :no c/n lines");
					this.CloseLink(connection);
					return;
				}

				// all fine, register the new server
				SimpleServer ser = new SimpleServer(); // ausfüllen
				ser.HopCount = Convert.ToInt32(ar[3]);
				this.RegisterServer((IRCConnection)connection, ser);
			}
		}

		private bool HasServerAccess(string servername) // TODO: dummy, implement
		{
			//SettingsHost.Settings.ServerLines.CanConnect();
			return true;
		}
	}
}
