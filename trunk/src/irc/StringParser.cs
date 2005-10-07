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
using System.Text;
using System.Collections;

namespace IRC
{
	/// <summary>
	/// StringParser
	/// </summary>
	public class StringParser
	{
		// TO/DO: für server unterstüzung erweitern!
		/// <summary>
		/// Parse the CMD string
		/// </summary>
		/// <param name="string">string to split</param>
		/// <returns>Array of strings</returns>
		public string[] ParseString(string text)
		{
			Console.WriteLine(this + ".ParseString({0})", text);
			string[] lines;

			Console.WriteLine(text.Length + " : " + text.LastIndexOf('\n'));

			if ((text.Length - 1) == text.LastIndexOf('\n'))
			{
				int i = text.LastIndexOf('\n');
				if (i == 0)
					return null;

				text = text.Substring(0, i - 1); 
			}

			if (text.IndexOf('\n') != -1)
			{
				lines = text.Split('\n');
			}
			else
			{
				lines = new string[1];
				lines[0] = text;
			}

			return lines;
		}

		/// <summary>
		/// Parse the CMD line
		/// </summary>
		/// <param name="line"></param>
		/// <returns>Array of strings</returns>
		public string[] ParseLine(string line)
		{
			try
			{
				if (line.Length == 0)
					throw new ArgumentNullException("line");

				line = line.Trim(new char[]{'\r'}); // Windows Zeilenende, nur manchmal erforderlich
				ArrayList res = new ArrayList();

				if (line[0] == ':') // server-prefix
				{
					// Beispiel: :tolsun.oulu.fi SERVER ...
					int pos = line.IndexOf(' ');
					string prefix;
					if (pos != -1)
					{
						prefix = line.Substring(0, pos);
						prefix = prefix.Trim();
						line = line.Substring(pos, line.Length-pos);
						line = line.Trim();
					}
					else
					{
						prefix = line;
						line = String.Empty;
					}
					res.Add(prefix); // erstes argument == prefix
				}
				else
				{
					res.Add(String.Empty); // damit ar[0] leer ist wenn kein prefix vorhanden ist
				}
				Console.WriteLine(this + ": nach prefix " + line);

				while (line.Length != 0)
				{
					int pos = line.IndexOf(' '); // space
					if (pos != -1)
					{
						if (line[0] == ':') // last parameter
						{
							res.Add(line);
							break;
						}

						string part = line.Substring(0, pos);

						part = part.Trim();
						line = line.Substring(pos, line.Length-pos);
						line = line.Trim();
						res.Add(part);
					}
					else
					{
						res.Add(line);
						line = String.Empty;
					}
				}

				string[] param = new string[res.Count];

				for (int i = 0; i < res.Count; i++)
				{
					param[i] = (string)res[i];
				}

				Console.WriteLine(this + ": param[0]="+param[0]);
				if (param.Length > 1)
					Console.WriteLine(this + ": param[1]="+param[1]);

				return param;
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
				return new string[]{}; // drop message
			}
		}
	}
}
