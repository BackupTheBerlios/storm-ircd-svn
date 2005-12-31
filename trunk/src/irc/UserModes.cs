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
/* modes from rfc
      The available modes are as follows:

           a - user is flagged as away;
           i - marks a users as invisible;
           w - user receives wallops;
           r - restricted user connection;
           o - operator flag;
           O - local operator flag;
           s - marks a user for receipt of server notices.
*/

	public class IRCUserModes
	{
		private ArrayList list;
		public IRCUserModes()
		{
			this.list = new ArrayList();
			this.list.AddRange(new char[] {'a', 'i', 'w', 'r', 'o', 'O', 's', 'D'});
		}

		public static readonly char MODE_AWAY		= 'a';
		public static readonly char MODE_INVISIBLE	= 'i';
		public static readonly char MODE_REW_WALLOP	= 'w';
		public static readonly char MODE_RESCONNETION	= 'r';
		public static readonly char MODE_OP		= 'o';
		public static readonly char MODE_LOCAL_OP	= 'O';
		public static readonly char MODE_REC_SNOTICE	= 's';

		public ArrayList List
		{
			get
			{
				return this.list;
			}
		}
	}

	public class IRCUserMode
	{
		private int mode;
		private IRCUserModes um;

		public IRCUserMode()
		{
			this.mode = 0;
			this.um = new IRCUserModes();
		}

		public IRCUserMode(int modes)
		{
			throw new NotImplementedException("IRCUserMode(int modes)");
			this.mode = modes;
			this.um = new IRCUserModes();
		}

		public IRCUserMode(string modes)
		{
			this.mode = 0;
			this.um = new IRCUserModes();
			this.FromString(modes);
		}

		public bool FromString(string modes) // false und abbruch bei fehler
		{
			foreach (char c in modes)
			{
				if (!this.SetMode(c, true)) // alle true setzen
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			string ret = String.Empty;
			foreach (char c in this.um.List)
			{
				if (HasMode(c))
					ret += c;
			}
			return ret;
		}

		public bool HasMode(char c)
		{
			int ret = this.FromChar(c);
			
			if (ret == -1)
				return false; // fehler, "mode" ist nicht vorhanden

			if ((this.mode & ret) > 0) // BUGFIX: > 0
			{
				return true;
			}
			return false;
		}

		public bool SetMode(char c, bool state) // liefert false wenn die mode nicht vorhanden ist
		{
			int ret = this.FromChar(c);
			if (ret == -1)
				return false; // fehler, "mode" ist nicht vorhanden

			if (this.HasMode(c) == state)
				return true; // nothing to do
			else
				this.mode = this.mode ^ ret; // xor; gegenteil

			return true;
		}

		private int FromChar(char c)
		{
			int i = this.um.List.IndexOf(c);

			if (i != -1)
			{
				int ret = Convert.ToInt32(Math.Pow(2, i));

				return ret;
			}
			return -1;
		}
	}
}
