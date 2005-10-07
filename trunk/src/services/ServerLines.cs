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
using System.Collections;
using System.Xml;
using System.Xml.Serialization;

using System.Net;

[XmlRoot("ServerLines")]
public class ServerLines
{
	private ArrayList _acceptLines = new ArrayList();
	private ArrayList _connectLines = new ArrayList();

	public ServerLines()
	{
/*		AcceptServerObject t = new AcceptServerObject();
		t.HostName = "text1";
		t.ServerName ="text2";
		t.Password = "secrect!";
		this._acceptLines.Add(t);
*/
/*		ConnectServerObject o = new ConnectServerObject();
		o.HostName = "10.0.1.1";
		o.ServerName ="host1.irc.org";
		this._connectLines.Add(o);*/
	}

	public ArrayList GetServerList()
	{
		ArrayList servers = new ArrayList();
		foreach (ConnectServerObject cso in this._connectLines)
		{
			IPEndPoint ep = cso.ToIPEndPoint();
			if (ep == null)
			{
				Console.WriteLine("invalide connectline");
				Logger.LogMsg("invalide connectline");
				continue;
			}
			servers.Add(ep);
		}
		return servers;
	}

	public bool CanConnect(IPAddress ip, string servername, string password)
	{
		lock (this._acceptLines)
		{
			foreach (AcceptServerObject aso in this._acceptLines)
			{
				if ((aso.ServerName == servername) && (aso.Password == password)) // TODO: ip bzw. hostname
					return true;
			}
		}
		return false;
	}

	[XmlArrayItem(typeof(AcceptServerObject))]
	public ArrayList AcceptLines
	{
		get
		{
			return this._acceptLines;
		}
		set
		{
			this._acceptLines = value;
		}
	}

	[XmlArrayItem(typeof(ConnectServerObject))]
	public ArrayList ConnectLines
	{
		get
		{
			return this._connectLines;
		}
		set
		{
			this._connectLines = value;
		}
	}
}

[XmlRoot("Server")]
public class AcceptServerObject
{
	private string _hostName = string.Empty;
	private string _serverName = string.Empty;
	private string _password = string.Empty;

	public string HostName
	{
		get
		{
			return _hostName;
		}
		set
		{
			this._hostName = value;
		}
	}

	public string ServerName
	{
		get
		{
			return _serverName;
		}
		set
		{
			this._serverName = value;
		}
	}

	public string Password
	{
		get
		{
			return this._password;
		}
		set
		{
			this._password = value;
		}
	}

string key;
int options;
}

[XmlRoot("Server")]
public class ConnectServerObject
{
	private int _port = 6667;
	private string _password = string.Empty;
	private string _hostName = string.Empty;
	private string _serverName = string.Empty;

	public IPEndPoint ToIPEndPoint()
	{
		//return new IPEndPoint(IPAddress.Parse("10.0.1.1"), this._port);
		return new IPEndPoint(Dns.Resolve(this._hostName).AddressList[0], this._port);
	}

	public string HostName
	{
		get
		{
			return _hostName;
		}
		set
		{
			this._hostName = value;
		}
	}

	public string ServerName
	{
		get
		{
			return _serverName;
		}
		set
		{
			this._serverName = value;
		}
	}

	public int Port
	{
		get
		{
			return this._port;
		}
		set
		{
			Console.WriteLine(this +"setPort "+value);
			this._port = value;
		}
	}

	public string Password
	{
		get
		{
			return this._password;
		}
		set
		{
			this._password = value;
		}
	}

string key;
int options;
}
