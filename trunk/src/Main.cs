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
using System.IO;
using System.Xml;
using System.Text;
using System.Reflection;
using System.Diagnostics;

using System.Net;
using System.Net.Sockets;

using System.Threading;

using IRC;
using Service;
using Tools;
using Other;

// TODO: nant support
public class MainClass
{
	//static IRCServer server;
	private static ManualResetEvent _wait = new ManualResetEvent(false);
//	static Configuration config;

	public static void Main(string[] args)
	{
		try
		{
			Console.WriteLine("storm-ircd: Copyright 2005 Josef Schmeisser\nReleased WITHOUT ANY WARRANTY under the terms of the GNU General Public license.");
#if UNSTABLE
			Console.WriteLine("storm-ircd-revolution: this build is only for testing!");
#endif
			/// signal handler
			//new storm_signal();
			///
////			Stream logFile = File.Open(Assembly.GetExecutingAssembly().GetName().Name + ".log", FileMode.Create);//.Append);
////			byte[] header = Encoding.UTF8.GetBytes(DateTime.Now + " Start Logfile\r\n\r\n");
////			logFile.Position = logFile.Length;
////			logFile.Write(header, 0, header.Length);

			Trace.Listeners.AddRange(new System.Diagnostics.TraceListener[] {
////				new TextWriterTraceListener(logFile),
				new TextWriterTraceListener(Console.Out)
			});

			Debug.AutoFlush = true;
			Debug.WriteLine("Start debuging...");
			Trace.WriteLine("Start tracing...");
			Debug.Indent();
			Debug.WriteLine("storm-ircd version: " +
								System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());

			// Standard dienste starten
			ServiceManager.Services.AddService(SettingsHost.Instance);
			ServiceManager.Services.AddService(Logger.Instance);
			ServiceManager.Services.AddService(Statistic.Instance);
			ServiceManager.Services.AddService(IRCService.Instance); // load the server
			((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Show();

			// Application host set up
			ApplicationHost aph = new ApplicationHost("storm-interface", // TODO: in einen service packen
				((IRCService)ServiceManager.Services[typeof(IRCService)]).Server.SocketManager);

			// Verbindung zu ApplicationHost aufbauen
			ServiceManager.Services.AddService(new ConsoleInterface());

// DEBUG: ((IRCService)ServiceManager.Services[typeof(IRCService)]).Server.SocketManager.Dispose();

			_wait.WaitOne();
			aph.Dispose(); // aufräumen
			GC.Collect();

			Debug.Unindent();
			Trace.WriteLine(DateTime.Now + ":  Shutdown complete"); // TODO: x-augen m00-kuh ;)

			Debug.Flush();
			Debug.Close();
			Trace.Flush();
			Trace.Close();
		}
		catch(Exception e)
		{
			Trace.WriteLine("Main" + ": in :" + e.ToString());
		}
		Console.WriteLine("End Main ...");
	}

	public static void shutdown()
	{
		_wait.Set();
	}

	[ObsoleteAttribute("test",false)]
	public static void obsolete()
	{
	}
}
