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
using Network;
using System.Text.RegularExpressions;

namespace IRC
{
	public sealed class RFC2812 // TODO: regex
	{
		private string nick;

		public static bool IsValidCommand(string command)
		{
			return true;
		}

		public static bool IsValidNick(string nick)
		{
			return true;
		}

		public static bool IsValidUserName(string userName)
		{
			// user       =  1*( %x01-09 / %x0B-0C / %x0E-1F / %x21-3F / %x41-FF )
			//               ; any octet except NUL, CR, LF, " " and "@"
			return true;
		}

		public static bool IsValidChannelName(string channel)
		{
			if (channel == null)
				return false;
			if (channel.Trim().Length == 0)
				return false;

			return true;
		}
	}

}
