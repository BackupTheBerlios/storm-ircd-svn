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

#if UNIX
using Mono.Unix.Native;

public static class storm_signal
{
	private static void Handler(int signal)
	{
		System.Console.WriteLine("Signal received: " + signal);
		Logger.LogMsg("Handling singnal " + signal);

		switch (signal)
		{
			case (int)Signum.SIGINT:
				System.Console.WriteLine("SIGINT");
				MainClass.Shutdown();
				break;
			case (int)Signum.SIGTERM:
				System.Console.WriteLine("SIGTERM");
				MainClass.Shutdown();
				break;
			case (int)Signum.SIGQUIT:
				System.Console.WriteLine("SIGQUIT");
				MainClass.Shutdown();
				break;
			case (int)Signum.SIGHUP:
				System.Console.WriteLine("SIGHUP"); // reread config
				break;
		}
	}

	public static void SetupHandlers()
	{
		// Set up handlers
		Stdlib.signal(Signum.SIGINT, new SignalHandler(storm_signal.Handler));
		Stdlib.signal(Signum.SIGTERM, new SignalHandler(storm_signal.Handler));
		Stdlib.signal(Signum.SIGQUIT, new SignalHandler(storm_signal.Handler));
	}
}#endif // UNIX
