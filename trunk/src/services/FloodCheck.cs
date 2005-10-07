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

namespace Service
{
	public sealed class FloodCheck : IService
	{
		private bool _loaded = false;

		public void Load()
		{
		}

		public void Unload()
		{
		}
		public Type[] Dependences
		{
			get
			{
				return null;
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
}
