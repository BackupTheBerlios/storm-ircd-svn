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

#ifndef STARTER_H
#define STARTER_H

#include <iostream>
#include <stdlib.h>
#include <sys/stat.h>

// for exec
#include <unistd.h>

// for open
#include <sys/types.h>
#include <sys/stat.h>
#include <fcntl.h>

using std::cout;
using std::endl;

class Starter
{
public:
	Starter(char *assembly, bool fork = true);
	~Starter();
	void Run(std::string commandline);
private:
	void daemonize();
	void call_mono(std::string commandline);
	int nfd;
	char *szassembly;
	bool ffork;
	bool frunning;
};

#endif // STARTER_H
