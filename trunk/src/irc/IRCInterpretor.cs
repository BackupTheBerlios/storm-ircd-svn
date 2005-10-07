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


using System.Reflection;

using Network;

namespace IRC
{
	/// <summary>
	/// </summary>
	public class InterpretorException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public InterpretorException(string text) : base("InterpretorException: " + text)
		{
		}
	}

	/// <summary>
	/// IRCInterpretor
	/// </summary>
	public class IRCInterpretor : BaseInterpretor
	{
		private StringParser _parser;
		private IRCServer _server;

		/// <summary>
		/// </summary>
		public IRCInterpretor(IRCServer server)
		{
			this._parser = new StringParser();
			this._server = server;
		}

#if UNSTABLE
		// TODO: support für server strings
		// der erste parameter ist _immer_ das prefix fals keines vorhanden bleit der platz frei
		public override bool ProcessCommand(IConnection connection, string cmd)
		{
			if (cmd.Length == 0) // BUFIX: 1.04.05: "methode kann nicht in IRCServer gefunden werden"
			{
				Console.WriteLine(this + ": " + connection.ID + " has sent an null request, cmd.Length: " + cmd.Length);

				return true; // drop message
			}

			string[] lines = this._parser.ParseString(cmd);
			if (lines == null)
				return true; // drop message

			foreach (string line in lines)
			{
				if (line.Length  == 0)
					continue;

				Console.WriteLine(this.GetType().ToString() + " bearbeite " + line);// + ", Lenght: " + line.Length);

				if (RFC2812.IsValidCommand(line))
				{
					string[] ar = this._parser.ParseLine(line);
					if (ar.Length <= 1) // nur das prefix ist vorhanden
					{
						Console.WriteLine("drop invalide server message and sent an error");
						connection.SendLine(":ERROR invalide message");
						return true; // drop
					}

					ar[1] = ar[1].ToLower(); // neu für server

//					Console.WriteLine(this + ": browse: " + ar[0]);
/* TODO: to implement
if (ar[0] != String.Empty)
{
	if (connection.IsServer())
}


// security

if (isregiestred)
{
}
*/
					try
					{
						object[] obj = new object[2];
						obj[0] = connection;
						obj[1] = ar;

						Type srv = this._server.GetType();

						string search = "m";

						// ar[0] enthält entweder das prefix oder das commando
						// achtung prefix sample: ":krys NICK syrk"
						search += "_" + ar[1];

						Console.WriteLine(this + ": invoke " + search);
						srv.InvokeMember(search, // very fast
							BindingFlags.DeclaredOnly |
							BindingFlags.Public |
							BindingFlags.NonPublic |
							BindingFlags.Instance |
							BindingFlags.InvokeMethod, null, this._server, obj);
					}
					catch (MissingMethodException me)
					{
						Console.WriteLine(this + ": no match: " + me.Message);
						return false;
					}
					catch (TargetInvocationException ie)
					{
						Console.WriteLine(this + ": An Invoke Exception has thrown; command: {0}\nStack:\n{1}", ar[0], ie.ToString());
						///
						stat.errors++;
						///
					}
					catch (Exception e)
					{
						Console.WriteLine(this + "Fatal Error: " + e.ToString());
					}
				}
			}
			return true;
		}
#else
		public override void ProcessCommand(IConnection connection, string cmd)
		{
			if (cmd.Length == 0) // BUFIX: 1.04.05: "methode kann nicht in IRCServer gefunden werden"
			{
				Console.WriteLine(this + ": " + connection.ID + " has sent an null request, cmd.Length: " + cmd.Length);

				return;
			}

			string[] lines = this._parser.ParseString(cmd);
			foreach (string line in lines)
			{
				if (line.Length  == 0)
					continue;

				Console.WriteLine(this.GetType().ToString() + " bearbeite " + line + ", Lenght: " + line.Length);

				if (RFC2812.IsValidCommand(line))
				{
					string[] ar = this._parser.ParseLine(line);

					ar[0] = ar[0].ToLower();
					Console.WriteLine(this + ": browse: " + ar[0]);

					try
					{
						object[] obj = new object[2];
						obj[0] = connection;
						obj[1] = ar;

						Type srv = this._server.GetType();

						srv.InvokeMember("m_" + ar[0], // very fast
							BindingFlags.DeclaredOnly |
							BindingFlags.Public |
							BindingFlags.Instance |
							BindingFlags.InvokeMethod, null, this._server, obj);
					}
					catch (MissingMethodException me)
					{
						Console.WriteLine(this + ": no match: m_" + ar[0] + "\n: " + me.ToString());
					}
					catch (TargetInvocationException ie)
					{
						Console.WriteLine(this + ": An Invoke Exception has thrown; command: {0}\nStack:\n{1}", ar[0], ie.ToString());
						///
						stat.errors++;
						///
					}
					catch (Exception e)
					{
						Console.WriteLine(e.ToString());
					}
				}
			}
		}
#endif
	}
}
