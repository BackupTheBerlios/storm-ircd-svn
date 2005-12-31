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
using System.Net;

using Network;

namespace IRC
{
	public class IRCServerConnection : IRCConnection
	{
		private string _serverName;
//old		private SimpleServer _info;

		public IRCServerConnection(IRCConnection bas, SimpleServer ser) : base(bas.Socket, bas.Server)
		{
			if (ser == null)
				throw new ArgumentNullException("ser");

//			this._info = ser;
//			ser.UpLink = this;
//			if (!(bas.SimpleClient is SimpleServer))
//				throw new ArgumentException("bas.SimpleClient musst be a SimpeServer");

//			this._simpleClient = bas.SimpleClient;
			this._simpleClient = ser;
			this.SimpleServer.UpLink = this;
		}

		public override void Dispose()
		{
			((IRCServer)this.Server).RemoveServer(this.SimpleServer);
			base.Dispose();
		}

		// HACK: minimaler code zum testen
		public static bool ConnectToServer(IRCServer server, IPEndPoint ep) // todo. parsen
		{
			Console.WriteLine("Verbinde zu " + ep.Address +":"+ep.Port);
			IRCConnection tmp = null;
			try
			{
				//IPEndPoint ep = new IPEndPoint(/*IPAddress.Loopback*/IPAddress.Parse("10.0.1.1"), 9000); // HACK: connect to dancer
				tmp = new IRCConnection(ep, server); // TODO: hängt wenn es die zielip nicht gibt zb "10.0.1.1"
			}
			catch (Exception e)
			{
				Console.WriteLine("konnte keine verbindung zu server herstellen: "+e.Message);
				return false;
			}

			server.AddConnection(tmp);
			// TODO: nachsehen um welchen typ von server es sich handelt
			IRCServerConnection.SendServer(tmp);
			return true;
		}
/*
		private string UserPrefix(SimpleUser usr)
		{
			return (":" + usr.NickName + "!" +
				usr.UserName + "@" + usr.??? + " "); // TODO
		}

		public void SendCommand(SimpleUser usr, string command, params string[] args)
		{
			if (sender == null)
				throw new ArgumentNullException("sender");

			this.SendCommand(this.UserPrefix(usr), command, args);
		}
*/
		// registrierungsmethoden
		public static void SendChall()
		{
		}

		public static void SendServer(IConnection con)
		{
			// only for testing
			Console.WriteLine("SEND SERVER COMMAND");
			con.SendLine("PASS password 0211010001 IRC|aEFJKMRTu P");
			con.SendLine("SERVER host3.irc.org 1 000C :Example Geographic L");
		}

		public SimpleServer SimpleServer
		{
			get
			{
				return (SimpleServer)this.SimpleClient;
			}
			set
			{
				if (value == null)
					throw new ArgumentNullException("SimpleServer can't be null");
				if (value.UpLink != this)
					throw new ArgumentException("SimpleServer.UpLink must be \"this\"");

				this.SimpleClient = value;
			}
		}
	}
}
