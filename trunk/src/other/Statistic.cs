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
using System.Threading;
using System.Diagnostics;

using Service;

namespace Tools
{
	/// <summary>
	/// Description of Statistic.
	/// </summary>
	[Serializable]
	public sealed class Statistic : IService
	{
		private static Statistic _instance = new Statistic();
		private DateTime starttime;
		private bool _loaded = false;

		public static Statistic Instance
		{
			get
			{
				return _instance;
			}
		}

		private int errors;
		private int traffic;

		private Statistic()
		{
			Debug.WriteLine(this + ": create new Statistic() ...");
			this.errors = 0;
			this.traffic = 0;
		}


		public void Load()
		{
			this.starttime = DateTime.Now;
			Debug.WriteLine(this + " is avaible");
		}


		public void Unload()
		{
			// save
		}

		public bool Loaded
		{
			get
			{
				return this._loaded;
			}
		}

		public int Erros
		{
			get
			{
				return this.errors;
			}
			set
			{
				this.errors = value;
			}
		}

		public int Traffic
		{
			get
			{
				return this.traffic;
			}
			set
			{
				this.traffic = value;
			}
		}

		public TimeSpan UpTime
		{
			get
			{
				return DateTime.Now - this.starttime;
			}
		}

		// return the number of worker Threads
		public int AvailableCompletionPortThreads
		{
			get
			{
				int workerThreads = 0;
				int completionPortThreads = 0;
				ThreadPool.GetAvailableThreads(out workerThreads,
												out completionPortThreads);
				return completionPortThreads;
			}
		}

		public int AvailableWorkerThreads
		{
			get
			{
				int workerThreads = 0;
				int completionPortThreads = 0;
				ThreadPool.GetAvailableThreads(out workerThreads,
												out completionPortThreads);
				return workerThreads;
			}
		}

		public int CompletionPortThreads
		{
			get
			{
				int workerThreads = 0;
				int completionPortThreads = 0;
				ThreadPool.GetMaxThreads(out workerThreads,
											out completionPortThreads);

				return completionPortThreads - this.AvailableCompletionPortThreads;
			}
		}
		public Type[] Dependences
		{
			get
			{
				return null;
			}
		}

		public int WorkerThreads
		{
			get
			{
				int workerThreads = 0;
				int completionPortThreads = 0;
				ThreadPool.GetMaxThreads(out workerThreads,
											out completionPortThreads);

				return workerThreads - this.AvailableWorkerThreads;
			}
		}

		public int Threads
		{
			get
			{
				return this.CompletionPortThreads + this.WorkerThreads;
			}
		}
	}
}
