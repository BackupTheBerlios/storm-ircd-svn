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
using System.Net;
using System.Xml;
using System.Diagnostics;
using System.Collections;

namespace Other
{// TODO: Addressbereiche zulassen

enum BlackListState
{
Banned
}

	/// <summary>
	/// TODO - Add class summary
	/// </summary>
	/// <remarks>
	/// 	created by - Josef Schmeier
	/// 	created on - 25.09.2004 12:24:44
	/// </remarks>
	public class BlackList : object, System.Collections.IEnumerator
	{
		private ArrayList list; // hashtable
		private IEnumerator enumerator;

		/// <summary>
		/// Default constructor - initializes all fields to default values
		/// </summary>
		public BlackList()
		{
			this.list = new ArrayList();
			this.enumerator = this.list.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public bool Check(IPAddress address)
		{
			foreach (IPAddress check in this.list)
			{
				if (address.Equals(check)) // TODO: !!! ACHTUNG
					return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public void SaveToFile(string name) // TO/DO: SaveToFile
		{
		//	throw new NotImplementedException();
			XmlDocument doc = new XmlDocument();
		//	doc.LoadXml("<?xml version=\"1.0\"?><List />");
			doc.AppendChild(doc.CreateElement("List"));

			foreach (IPAddress address in this.list)
			{
				XmlElement element = doc.CreateElement("Address");
				element.InnerText = address.ToString();
				doc.DocumentElement.AppendChild(element);
				Trace.WriteLine("BlackList::Save: " + address.ToString());
			}

			doc.Save(name);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		public void LoadFromFile(string name)
		{
		//	throw new NotImplementedException();
			list.Clear();

			XmlDocument doc = new XmlDocument();
			doc.Load(name);
			foreach (XmlElement element in doc.DocumentElement.ChildNodes)
			{
				this.list.Add(IPAddress.Parse(element.InnerText));
				Trace.WriteLine("BlackList::Load: " + element.InnerText);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public void AddIP(IPAddress address)
		{
			foreach (IPAddress check in this.list)
			{
				if (address.Equals(check)) // TODO: !!! ACHTUNG
					return;
			}
			this.list.Add(address);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Interface property from IEnumerator
		/// 	- read only
		/// 
		/// </remarks>
		public virtual object Current
		{
			get
			{
				return this.enumerator.Current;
			}
		}
		
		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IEnumerator
		/// 
		/// </remarks>
		public virtual void Reset()
		{
			this.enumerator.Reset();
		}
		
		/// <summary>
		/// TODO - add method description
		/// </summary>
		/// <remarks>
		/// Interface method from IEnumerator
		/// 
		/// </remarks>
		public virtual bool MoveNext()
		{
			return this.enumerator.MoveNext();
		}
	}
}
