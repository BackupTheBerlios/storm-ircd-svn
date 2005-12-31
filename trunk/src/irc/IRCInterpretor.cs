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

		// TODO: support für server strings
		// der erste parameter ist _immer_ das prefix fals keines vorhanden bleit der platz frei
		public override bool ProcessCommand(IConnection connection, string cmd)
		{
			if (cmd.Length == 0)
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
						connection.SendLine("ERROR :Invalide message");
						return true; // drop
					}
					ar[1] = ar[1].ToLower(); // neu für server

					if ((ar[0] != string.Empty) && (!this._server.IsServer((IRCConnection)connection)))
					{
						connection.SendLine("ERROR :Not allowed from a client");
					}

					// security
//					if (this._server.IsRegistred(connection)) // TODO
//					{
//					}

					// debugging
					this._server.WriteArgs(ar);

					if (this.InvokeHandler(connection, ar))
						continue;
					if (this.CallHandler(connection, ar))
						continue;

					return false; // drop group
				}
			}
			return true;
#if false
/*
					string search = string.Empty;
					try
					{
						object[] obj = new object[2];
						obj[0] = connection;
						obj[1] = ar;

						Type srv = this._server.GetType();

						search = "m";

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
						Console.WriteLine(this + ": InvokeMember() no match: " + search);
						try
						{
							CommandHandler handler = this._server.GetHandler(ar[0]);
							if (handler != null)
							{
								handler(connection as IRCConnection, ar);
							}
							else
							{
								Console.WriteLine(this + ": GetHandler() no match: " + search + "\n: ");
								return false;
							}
						}
						catch (Exception e)
						{
							Console.WriteLine("CommandHandler: " + e.ToString());
						}
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
					catch
					{
						Console.WriteLine("!!!!!!!!!!!!!!");
					}

				}
			}
			return true;
*/
#endif
		}

		private bool CallHandler(IConnection src, string[] ar)
		{
			try
			{
				CommandHandler handler = this._server.GetHandler(ar[1]);
				if (handler != null)
				{
					handler((IRCConnection)src, ar);
				}
				else
				{
					Console.WriteLine(this + ": GetHandler() no match: " + ar[1] + "\n: ");
					return false;
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(this + ".CallHandler(): " + e.ToString());
				stat.errors++;
			}
			return true;
		}

		private bool InvokeHandler(IConnection src, string[] ar)
		{
			string search = string.Empty;
			object[] obj = new object[2];
			obj[0] = (IRCConnection)src;
			obj[1] = ar;
			Type srv = this._server.GetType();
			search = "m_" + ar[1];

			try
			{
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
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine(this + ".InvokeHandler(): " + e.ToString());
				stat.errors++;
			}
			return true;
		}
	}
}
