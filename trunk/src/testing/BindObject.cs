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

[XmlRoot("BindTo")]
public class BindObject
{
	private string _hostName;
	private int _port;

	public BindObject()
	{
		this.Port = 6667;
		this.HostName = "0.0.0.0";
	}

	public BindObject(int port, string hostName)
	{
		this.Port = port;
		this.HostName = hostName;
	}

	public string HostName
	{
		get
		{
			return this._hostName;
		}
		set
		{
			// TODO: validate
			this._hostName = value;
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
			if ((value <= IPEndPoint.MaxPort) && (value >= IPEndPoint.MinPort))
				this._port = value;
		}
	}
}
