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
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Diagnostics;

namespace Service
{
	public class ConsoleInterface : IService
	{
		private bool _loaded = false;
		private Thread _thread;

		public ConsoleInterface()
		{
		}

		public void Load()
		{
			if (this._loaded)
				return;

			this._loaded = true;

			if (SettingsHost.Instance.Settings.UseConsoleInterface)
			{
				ThreadStart myThreadDelegate = new ThreadStart(this.loop);
				this._thread = new Thread(myThreadDelegate);
				this._thread.IsBackground = true;
				this._thread.Start();
			}
		}

		public void Unload()
		{
			if (!this._loaded)
				return;

			this._loaded = false;

			this._thread.Abort();
			Debug.WriteLine(this + ".Unload()");
		}

		private Socket sock;
		private void loop()
		{
#if UNIX
			Debug.WriteLine(this + ": starting ConsoleInterface ...");
			string name = System.Net.Dns.GetHostName();

			try
			{
				EndPoint endPoint = new Mono.Unix.UnixEndPoint(((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]).Settings.SocketFile);
				Socket sock = new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
				sock.Connect(endPoint);

				while (true)
				{
					Console.Write("storm@" + name + "# ");
					string cmd = Console.ReadLine();
					cmd.Trim();
					if (cmd == "help")
					{
						Console.WriteLine("shutdown restart status");
					}
					else
					{
						sock.Send(Encoding.ASCII.GetBytes(cmd));

						if (cmd == "shutdown")
						{
							Debug.WriteLine(this + ": exit loop ...");
							sock.Close();
							return;
						}
					}
				}
			}
			catch (Exception e)
			{
				//Console.WriteLine(this + ": " + e.ToString());
			}
			Debug.WriteLine(this + ": exit loop ...");
#endif
		}

		public Type[] Dependences
		{
			get
			{
				return new Type[]{typeof(SettingsHost), typeof(ApplicationHost)};
			}
		}

		public bool Loaded
		{
			get
			{
				return this._loaded;
			}
		}
	}
}
