// Copyright (C) 2005 Josef Schmeisser
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Diagnostics;
using System.Collections;

// TO/DO: support für near types
// TODO: evt. extern nochmal testen
// TODO: autoload system implementieren; d.h. z.B. IRCService lädt automatisch SettingsHost nach
namespace Service
{
	public class ServiceManager
	{
	//	private ArrayList services = new ArrayList();
		private Hashtable services = Hashtable.Synchronized(new Hashtable());
		private static ServiceManager servicemanager = new ServiceManager();

		public static ServiceManager Services
		{
			get
			{
				return servicemanager;
			}
		}

		private ServiceManager()
		{
			Debug.WriteLine(this + ": Initialisiere Service-Subsystem");

			// Basis-System laden
			// this.AddService(new SettingsHost()); // jetzt in main
		}

		public bool AddService(IService service)
		{
			if (!this.CanLoad(service))
			{
				Debug.WriteLine(this + ": " + service + " konnte nicht geladen werden");
				return false;
			}
			Debug.WriteLine(this + ": lade " + service.GetType());
/*			foreach (IService ser in this.services)
			{
				if ((ser).GetType() == service.GetType())
				{
					Debug.WriteLine(typeof(this) + ": Service bereits geladen!");
					return;
				}
			}
*/
			if (this.services.ContainsKey(service.GetType()))
			{
				Debug.WriteLine(this + ": Service bereits geladen!");
				return false;
			}

			this.services.Add(service.GetType(), service);
			service.Load();
			// TODO: return service.Load()
			return true;
		}

		public bool Unload(IService service)
		{
			if (!this.CanUnload(service))
			{
				Debug.WriteLine(this + ": kann " + service + "nicht entladen");
				return false;
			}

			Debug.WriteLine(this + ": entlade " + service.GetType()  + "...");
			if (service.Loaded)
				service.Unload();

			this.services.Remove(service.GetType());
			return true;
		}

		public void UnloadAll() // DONE: TO/DO: durch canunload bleiben reste; this.services.Remove() direkt aufrufen
		{
			foreach (DictionaryEntry entry in this.services)
			{
				IService service = (IService)entry.Value;
				if (service.Loaded)
					service.Unload();
			}
			this.services.Clear(); // clear all
		}
/*
		public IService GetService(Type type)
		{
			foreach (IService ser in this.services)
			{
				if ((ser).GetType() == type)
				{
					return (IService)ser;
				}
			}
			return null;
		}*/

		public bool HasService(IService service)
		{
			return this.HasService(service.GetType());
		}

		public bool HasService(Type type)
		{
#if UNSTABLE
			if (this.services.ContainsKey(type))
				return true;

			foreach (DictionaryEntry entry in this.services)
			{
				if (this.SearchService(type, (IService)entry.Value))
				{
					return true;
				}
			}
			return false;
#else
			if (this.services.ContainsKey(type))
				return true;
			return false;
#endif
		}

		private bool CanLoad(IService service) // TODO: near type
		{
			if (service == null)
				throw new ArgumentNullException("service");

			if (service.Dependences == null)
				return true;

			Type[] adep = service.Dependences;
			foreach (Type dep in adep)
			{
				bool test = false;
				foreach (DictionaryEntry entry in this.services)
				{
					if (dep == entry.Key)
						test = true;
				}
				if (!test)
					return false;
			}
			return true;
		}

		private bool CanUnload(IService service) // TODO: near type
		{
			if (service == null)
				throw new ArgumentNullException("service");

//			Type type = service.GetType();

			foreach (DictionaryEntry entry in this.services)
			{
				if (((IService)entry.Value).Dependences == null)
					continue;

				foreach (Type dep in ((IService)entry.Value).Dependences)
				{
					if (this.SearchService(dep, service)) // TODO: testen
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool SearchService(Type type, IService service)
		{
			Type stype = service.GetType();

			foreach (Type ia in stype.GetInterfaces())
			{
				if (type == ia)
					return true;
			}

			while (stype != typeof(object))
			{
				if (type == stype)
					return true;
				stype = stype.BaseType;
			}
			return false;
		}

		public IService this[Type type] // TODO: testen
		{
			get
			{
#if UNSTABLE
				if (this.services.ContainsKey(type))
					return (IService)this.services[type];

				foreach (DictionaryEntry entry in this.services) // TODO: testen
				{
					if (this.SearchService(type, (IService)entry.Value))
					{
						this.services.Add(type, (IService)entry.Value); // auch unter neuem Typ bekanntmachen
						return (IService)entry.Value;
					}
				}

				return null;
#else
				return (IService)this.services[type];
#endif
			}
		}
	}
}
