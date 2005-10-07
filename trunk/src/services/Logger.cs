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

using System.Diagnostics;

using Service;

public class Logger : IService
{
	private static Logger _instance;
	private bool _loaded = false;
	private FileStream _fileStream;
	private StreamWriter _writer;

	public static Logger Instance
	{
		get
		{
			if (_instance == null)
				_instance = new Logger();

			return _instance;
		}
	}

	public static void LogMsg(params string[] msg)
	{
		Instance.InternalLogMsg(msg);
	}

	private Logger()
	{
		Debug.WriteLine(this + ": initialize new Logger()");
	}

	public void Load()
	{
		if (this._loaded)
			return;

		this._fileStream = new FileStream(SettingsHost.Instance.Settings.LogFile,
											FileMode.OpenOrCreate, FileAccess.Write);
		this._writer = new StreamWriter(this._fileStream);
		this._writer.BaseStream.Seek(0, SeekOrigin.End); // Zeiger auf das Dateiende setzen

		this.InternalLogMsg(DateTime.Now +
			": Start Logfile\nstorm-ircd version: " +
			System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
		this._loaded = true;
	}

	public void Unload()
	{
		if (!this._loaded)
			return;

		this.InternalLogMsg(DateTime.Now + ": end\n");
		this._writer.Flush();
		this._fileStream.Flush();
		this._writer.Close();
		this._fileStream.Close();
		this._loaded = false;
	}

	private void InternalLogMsg(params string[] msg)
	{
		string line = string.Empty;
		foreach (string st in msg)
		{
			line += st;
		}
		this._writer.WriteLine(line);
		this._writer.Flush();
	}

	public Type[] Dependences
	{
		get
		{
			return new Type[]{typeof(SettingsHost)};
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
