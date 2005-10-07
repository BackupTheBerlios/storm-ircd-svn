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
{/*
	// usermodes form unrealirc
	public enum IRCUserModes : char
	{
		MODE_OP			=	'O',
		MODE_INVISINLE	=	'i',
		MODE_READWALLOP	=	'w',
		MODE_RWOPS		=	'g'
	}
*/
	public class IRCUserModes
	{
		private ArrayList list;
		public IRCUserModes()
		{
			this.list = new ArrayList();
			this.list.AddRange(new char[] {'O', 'i', 'w', 'g', 'c', 's'});
		}

		public static readonly char MODE_OP	=	'O';
		public static readonly char MODE_INVISIBLE	=	'i';
		public static readonly char MODE_READWALLOP	=	'w';
		public static readonly char MODE_RWOPS	=	'g';
		public static readonly char CAP_SERVICES = 'c';
		public static readonly char MODE_STATUS = 's';

		public ArrayList List
		{
			get
			{
				return this.list;
			}
		}
	}
/*
	O = Local IRC Operator
	i = Invisible (Wird nicht in der /WHO suche angezeigt)
	w = kann Wallop Nachrichten lesen
	g = Kann GlobOps und LocOps Nachrichten sowohl lesen als auch schreiben
	h = Verfügbar um zu helfen (Help Operator)
	s = Bekommt ServerNachrichten
	k = Sieht /KILL's
	S = Nur für die Services . (Schützt sie)
	a = Services Administrator
	A = Server Administrator
	N = Network Administrator
	T = Technical Administrator
	C = Co Administrator
	c = Sieht alle Connects/Disconnects auf dem lokalen Server
	f = Bekommt alle FLOOD-Benachrichtigungen
	r = Zeichnet den Nick als registriert aus
	x = Gibt den Benutzer versteckte HOSTs
	e = Kann alle Server Nachrichten lesen die zu +e Users (Eyes) geschickt wurden
	b = kann ChatOps lesen und senden
	W = Dadurch sieht man wer ein /WHOIS auf dich macht (IRC Operators only)
	q = Nur U:lines könnenDich noch kicken (Services Admins only)
	B = Markiert Dich als BOT
	F = Lets you recieve Far and Local connect notices)
	I = Unsichtbare Join/Part. Bist in Channel versteckt/Unsichtbar
	H = Versteckt den IRCOP-Status in den /WHO und /WHOIS whois abfragen . (IRC Operators only)
	d = Lässt keine ChannelNachrichten/PrivatNachrichten mehr empfangen
	v = Empfängt abgelehnten infizierte DCC Sendungen
	t = Sagt aus , dass du einen /VHOST benutzt
	G = Filtert alle BÖSEN-WÖRTER aus Deinen NAchrichten raus .
	z = MArkiert den Klienten als "Secure Connection (SSL) "
	*/

// mit "& and" und "^ xor" überprüfen
	public class IRCUserMode
	{
		private int mode;
		private IRCUserModes um;

		public IRCUserMode()
		{
			this.mode = 0;
			this.um = new IRCUserModes();
		}
/* nicht erlauben!
		public IRCUserMode(int modes)
		{
			this.mode = modes;
			this.um = new IRCUserModes();
		}
*/
		public IRCUserMode(string modes)
		{
			this.mode = 0;
			this.um = new IRCUserModes();
			this.FromString(modes); // BUGFIX: war vor "this.um = new IRCUserModes();" > nullreferenz
		}

		public bool FromString(string modes) // false und abbruch bei fehler
		{
			foreach (char c in modes)
			{
				if (!this.set(c, true)) // alle true setzen
					return false;
			}
			return true;
		}

		public override string ToString()
		{
			string ret = String.Empty;
			foreach (char c in this.um.List)
			{
				if (get(c))
					ret += c;
			}
			return ret;
		}

		public bool get(char c)
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

		public bool set(char c, bool state) // liefert false wenn die mode nicht vorhanden ist
		{
			int ret = this.FromChar(c);
			if (ret == -1)
				return false; // fehler, "mode" ist nicht vorhanden

			if (this.get(c) == state)
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
