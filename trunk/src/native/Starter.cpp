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

#include "Starter.h"

Starter::Starter(char *assembly, bool fork)
{
	szassembly = assembly;
	ffork = fork;
	frunning = false;
}

Starter::~Starter()
{
}

void Starter::Run(std::string commandline)
{
	if (!frunning) {
		if (ffork) {
			daemonize();
		}
		call_mono(commandline);
	}
}

void Starter::daemonize()
{
	pid_t pid, sid;
	pid = fork();
	if (pid < 0) {
		exit(EXIT_FAILURE);
	}
	else if (pid > 0) {
		exit(EXIT_SUCCESS);
	}

	umask(0);
	sid = setsid();

	if (sid < 0) {
		exit(EXIT_FAILURE);
	}
/*	if (chdir("/") < 0) {
		exit(EXIT_FAILURE);
	}
*/
	close(STDIN_FILENO);
	close(STDOUT_FILENO);
	close(STDERR_FILENO);

	// redirect stdout, stderr and stdin, this task is important for mono
	freopen("/dev/null", "w", stdout);
	freopen("/dev/null", "w", stderr);
	freopen("/dev/null", "r", stdin);
}

void Starter::call_mono(std::string commandline)
{
	cout << "exec mono" << endl;

	int ret = execl("/usr/bin/mono", "/usr/bin/mono", this->strAssembly, commandline.c_str(), NULL);
	if (ret == -1)
	{
		cout << "exec failed!" << endl;
	}
	exit(EXIT_FAILURE);
}
