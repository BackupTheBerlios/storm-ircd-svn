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
	public class Privilegs
	{
		bool god = false; //
		int UserModes;
		Hashtable ChannelUModes = new Hashtable();

		public Privilegs(int modes)
		{
			this.UserModes = modes;
		}
		
		public void AddChannel(string name, int umode)
		{
			this.ChannelUModes.Add(name, umode);
		}
		
		public void RemoveChannel(string name)
		{
			this.ChannelUModes.Remove(name);
		}

		public bool GetFlag(string channel, int flag)
		{
			return true;
		}
		
		public void SetFlag(string channel, int flag, bool value)
		{
		}
	}

	/// <summary>
	/// Privilegs Interface
	/// </summary>
	public interface IPrivilegs
	{
		/*Privilegs*/object GetPrivilegsChannel(Channel ch);

		/// <description>
		/// Privileg level
		/// </description>
		Privilegs Privilegs
		{
			get;
			set;
		}
	}
}
