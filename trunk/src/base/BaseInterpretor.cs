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

namespace Network
{
	/// <summary>
	/// 
	/// </summary>
	public class BaseInterpretor : IInterpretor
	{
		private IInterpretor _next;

		/// <summary>
		/// 
		/// </summary>
		public BaseInterpretor()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="next"></param>
		public BaseInterpretor(IInterpretor next)
		{
			this._next = next;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="connection"></param>
		/// <param name="cmd"></param>
		public virtual bool ProcessCommand(IConnection connection, string cmd)
		{
			if (this._next != null)
			{
				return this.Next.ProcessCommand(connection, cmd);
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		public IInterpretor Next
		{
			get
			{
				return this._next;
			}
			set
			{
				this._next = value;
			}
		}
	}
}
