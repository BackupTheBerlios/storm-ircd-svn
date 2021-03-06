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

namespace IRC
{
	public class SimpleUser : SimpleClient
	{
		public string NickName;
		public string HostName;
		public string UserName;
		public string RealName;

		private int _hopCount;
		private IRCConnection _upLink;
		private IRCUserMode _mode = new IRCUserMode();
		private IRCUserMode _chanmode = new IRCUserMode(); // TODO: neue modedefinietion

		public SimpleUser(IRCConnection uplink)
		{
			if (uplink == null)
				throw new ArgumentNullException();

			this._upLink = uplink;
		}

		public void SendCommand(string command, params string[] args)
		{
			throw new NotImplementedException();
		}

		public int HopCount
		{
			get
			{
				return _hopCount;
			}
			set
			{
				this._hopCount = value;
			}
		}

		public IRCConnection UpLink
		{
			get
			{
				return this._upLink;
			}
			set
			{
				this._upLink = value;
			}
		}

		public IRCUserMode UserMode
		{
			get
			{
				return this._mode;
			}
			set
			{
				this._mode = value;
			}
		}

		public IRCUserMode ChanUserMode
		{
			get
			{
				return this._chanmode;
			}
		}
	}
}
