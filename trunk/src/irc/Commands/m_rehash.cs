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

using IRC;
using Service;
using Network;

namespace IRC
{
	public partial class IRCServer
	{
		/*
		* m_rehash:
		*/
		public virtual void m_rehash(IRCConnection src, string[] ar)
		{
			if (!this.IsUser(src) || !((IRCUserConnection)src).UserMode.HasMode(IRCUserModes.MODE_OP))
			{
				src.SendCommand(ReplyCodes.ERR_NOPRIVILEGES,
						((IRCUserConnection)src).NickName,
						":Permission Denied- You're not an IRC operator");
				return;
			}

			Logger.LogMsg("REHASH command from " + src);
			SettingsHost settingsHost = ((SettingsHost)ServiceManager.Services[typeof(SettingsHost)]);
			// <config file> :Rehashing
			src.SendCommand(ReplyCodes.ERR_NOPRIVILEGES, ((IRCUserConnection)src).NickName,
					settingsHost.ConfigFile, ":Rehashing");
			// Rehash
			settingsHost.Reload();
		}
	}
}
