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
using System.Net.Sockets;

using Other;

namespace Network
{
	public delegate void ReceivedEvent(IConnection connection, string text);

	public interface IConnection :
#if UNSTABLE
IWorker
#else
IDisposable
#endif
// end
	{
//		event ReceivedEvent Received;

		void Send(string text);

#if UNSTABLE
		void Send(string text, MessagePriority priority);
		void SendLine(string text);
		void SendLine(string text, MessagePriority priority);
#endif

//		void SendChat(string text, string sender);

// obsolete		void Receive(); // use for IRCServer.cs

/* obsolete
		bool DataAvaible
		{
			get;
		}
*/
		IServer Server
		{
			get;
		}

		int ID
		{
			get;
		}

#if UNSTABLE
#else
		Socket Socket
		{
			get;
		}

		void Execute(string text); // nach IWorker verschoben
#endif

		// ermöglicht es kommandos auch manuel auszuführen
		// z.B. "join #test", wird behandelt wie wenn es vom client
		// selbst geschickt würde.
		// besser als ein eigener interpretor
		/*
		

		IInterpretor Interpretor
		{
			get;
			set;
		}*/
		///
	}
}
