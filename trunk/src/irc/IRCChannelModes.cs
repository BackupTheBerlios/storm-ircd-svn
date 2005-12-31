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

namespace IRC
{
/*
	public enum IRCChannelModes : int
	{
		MODE_SECRET,
		MODE_MODERATED,
		MODE_NOCOLOR,
	}
*/
	public class IRCChannelModes
	{
		public static readonly string modes = "soc";//"soc";
		public static readonly char MODE_SECRET = 's'; // TODO: stimmen nicht!
		public static readonly char MODE_MODERATED = 'o';
		public static readonly char MODE_NOCOLOR = 'c';
	}

	public class IRCChannelMode
	{
		private Hashtable mode;

		public IRCChannelMode()
		{
			this.mode = new Hashtable();
			this.Initialize();
		}

		private void Initialize()
		{
			foreach (char c in IRCChannelModes.modes)
			{
				this.mode.Add(c, false);
			}
		}

		public bool HasMode(char c)
		{
			if (this.mode.ContainsKey(c))
			{
				return (bool)this.mode[c];
			}
			return false;
		}

		public bool SetMode(char c, bool state) // liefert false wenn die mode nicht vorhanden ist
		{
			if (this.mode.ContainsKey(c))
			{
				this.mode[c] = state;
				return true;
			}
			return false; // key nicht vorhanden!
		}

		public bool FromString(string modes) // false und abbruch bei fehler
		{
			foreach (char c in modes)
			{
				if (!this.SetMode(c, true))
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			string ret = String.Empty;
			foreach (char c in IRCChannelModes.modes)
			{
				if (HasMode(c))
					ret += c;
			}
			return ret;
		}
	}
}
