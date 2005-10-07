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
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using System.Net;

namespace Service
{
	public enum EOL
	{
		unix,
		other
	}

	public class SettingsHost : IService
	{
		private static SettingsHost _instance = new SettingsHost();
		private readonly string configPath = "/etc/storm-ircd/";
		private readonly string configFile = "storm-ircd.xml";

		private Settings _settings;
		private bool _loaded = false;

		private SettingsHost()
		{
			Debug.WriteLine(this + ": Create new " + this);
		}

		public static SettingsHost Instance
		{
			get
			{
				return _instance;
			}
		}

		public Settings Settings
		{
			get
			{
				if (this._settings == null)
					this._settings = new Settings(); // neues leeres object
				return this._settings;
			}
		}

		public void Load()
		{
			if (this._loaded)
				return;

			this._loaded = true;
			/* TODO: Nachsehen ob schreibrecht im pfad vorhanden ist und
			 * ob der pfad vorhanden ist, ob das file existiert etc.
			 * abschließend deserialization der daten
			 */

			this.Deserialize();
		}

		public void Reload()
		{
			if (this._loaded)
			{
				lock (this)
				{
					this.Deserialize();
				}
			}
		}

		private void Serialize()
		{
			try
			{
				Debug.WriteLine(this + ": Serialize ...");

	 			using (StreamWriter sw = new StreamWriter(configFile))
	 			{
	 				XmlSerializer serializer = new XmlSerializer(typeof(Settings));

	 				// serialize the data
	 				serializer.Serialize(sw, this._settings);

	 				Debug.WriteLine(this + ": Serialized " + this._settings);
	 			}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		private void Deserialize()
		{
			try
			{
				Debug.WriteLine(this + ": Deserialize ...");

				if (!File.Exists(configFile))
					return;

				using (StreamReader sr = new StreamReader(configFile))
				{
					XmlSerializer serializer = new XmlSerializer(typeof(Settings));

					// deserialize the data 
					this._settings = (Settings)serializer.Deserialize(sr);

					Debug.WriteLine(this + ": Deserialized " + this._settings);
				}
			}
			catch (Exception e)
			{
				// new test
				Trace.WriteLine(this + ": create default settings");
				this._settings = new Settings(); // defaultzustand herstellen
				// end test
				Console.WriteLine(e.ToString());
			}
		}

		public void Unload()
		{
			if (!this._loaded)
				return;

			this._loaded = false;

			this.Serialize();
			Debug.WriteLine(this + ".Unload()");
		}

		public Type[] Dependences
		{
			get
			{
				return null;
			}
		}

		public bool Loaded
		{
			get
			{
				return this._loaded;
			}
		}

		public void Show()
		{
			Trace.WriteLine(this + ": configPath = " + this.configPath);
			Trace.WriteLine(this + ": configFile = " + this.configFile);
		}
	}

	[XmlRoot("Settings")]
	public class Settings
	{
		// kann durch die config verändert werden;
		// sollte aber _immer_ einen echten alias
		// für diese maschine entsprechen!
		private string _serverName;

		private EOL _endOfLineFormat;
		// xml stuff
		private Encoding _encoding;
		private ArrayList _ircports; // TODO alt
		private ServerLines _serverLines;
		private ArrayList _bind; // TODO neu
		
		// todo: ändern!!!
		[XmlElement("Encoding")]
		public int _codepage = 28591;

		private string _logFile = "storm-ircd.log"; //TODO"/var/log/storm-ircd/ircd";

		public Settings()
		{
			Debug.WriteLine(this + ": Create new Settings");

			try
			{
				this._encoding = Encoding.GetEncoding(28591);// iso_8859-1
				this._serverName = Dns.GetHostName(); // auf "local host name" setzen
				this._serverLines = new ServerLines();
				this._endOfLineFormat = EOL.unix;
				this._ircports = new ArrayList();
				this._bind = new ArrayList();
				
				// debug
				BindObject t = new BindObject();
				t.Port = 12;
				this._bind.Add(t);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		[XmlElement("ServerLines")]
		public ServerLines ServerLines
		{
			get
			{
				return this._serverLines;
			}
			set
			{
				this._serverLines = value;
			}
		}

		[XmlIgnore]
		public Encoding Encoding
		{
			get
			{
				return this._encoding;
			}
			set
			{
			}
		}

		[XmlElement("ServerName")]
		public string ServerName
		{
			get
			{
				//Debug.WriteLine(this + ": _get ServerName ret " + this._serverName);
				return this._serverName;
			}
			set
			{
			//	if (!IsValidServerName())
				//	Console.WriteLine(sorry the servername is not valide use the default);
				// else
				Debug.WriteLine(this + ": _set ServerName set " + value);
				this._serverName = value;
			}
		}

		[XmlElement("LogFile")]
		public string LogFile
		{
			get
			{
				return this._logFile;
			}
			set
			{
				this._logFile = value;
			}
		}

		[XmlElement("ConsoleInterface")]
		public bool UseConsoleInterface
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		[XmlElement("Language")]
		public string Language // für fehlermeldung und motd etc.
		{
			get
			{
				return "de";
			}
			set
			{
			}
		}

		[XmlElement("EndOfLine")]
		public EOL EndOfLineFormat
		{
			get
			{
				return this._endOfLineFormat; // TO/DO
			}
			set
			{
				this._endOfLineFormat = value;
			}
		}

		public string EndOfLine
		{
			get
			{
				if (this._endOfLineFormat == EOL.unix)
					return "\n";
				else
					return "\n\r";
			}
		}

		[XmlElement("blacklist")]
		public XmlElement BlackList
		{
			get
			{
				return null; // TODO
			}
			set
			{
			}
		}

		public bool UseIPv6
		{
			get
			{
				return false;
			}
		}

		[XmlArrayItem(typeof(BindObject))]
		public ArrayList EndPoints
		{
			get
			{
				return this._bind;
			}
			set
			{
				this._bind = value;
			}
		}

		[XmlElement("ircports")]
		public ArrayList IRCPorts
		{
			get
			{
				return this._ircports;
			}
			set
			{
				if (value == null)
					return;

				ArrayList tmp = new ArrayList();

				foreach (object obj in value)
				{
					if (obj.GetType() == typeof(int))
					{
						int p = (int)obj;
						if ((p <= System.Net.IPEndPoint.MaxPort) && (p >= System.Net.IPEndPoint.MinPort))
							tmp.Add(p);
					}
				}

				if (tmp.Count > 0)
				{
					Debug.WriteLine(this + ": change ports");
					this._ircports = tmp;
				}
			}
		}
	}
}
