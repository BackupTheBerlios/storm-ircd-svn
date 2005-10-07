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

using System.Collections;
using System.Text;


using Other;
namespace Network
{
	/// <summary>
	/// IServer
	/// </summary>
	public interface IServer : IListener
	{
		/// <summary>
		/// </summary>
		void Close();

		/// <summary>
		/// </summary>
		bool HasID(int id);

		/// <summary>
		/// </summary>
		bool SeedID(int id);
/*
#if UNSTABLE
#else
		/// <summary>
		/// </summary>
		void RemoveConnection(IConnection connection);
#endif
*/
		/// <summary>
		/// </summary>
		void HandleRecieved(IConnection connection, string text);

		/// <summary>
		/// </summary>
/*		ArrayList Clients // TODO: protected machen
		{
			get;
		}
*/
		/// <summary>
		/// </summary>
		Encoding Encoding
		{
			get;
			set;
		}

		/// <summary>
		/// </summary>
		int[] IDSeed
		{
			get;
		}

		/// <summary>
		/// </summary>
		void AddConnection(IConnection connection);

		/// <summary>
		/// </summary>
		void RemoveConnection(IConnection connection);
	}
}
