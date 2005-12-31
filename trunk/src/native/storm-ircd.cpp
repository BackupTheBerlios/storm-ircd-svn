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

#include <iostream>
#include "Client.h"
#include "Starter.h"

using std::cout;
using std::endl;

void print_usage_and_exit()
{
	cout << "storm-ircd: Copyright 2005 Josef Schmeisser\n" <<
		"Released WITHOUT ANY WARRANTY under the terms of the GNU General Public license.\n\n" <<
		"Usage: storm-ircd [OPTIONS]\n\n" <<
		"Options:\n" <<
		"  -c\tConfigfile.\n" <<
		"  -h\tPrint this help message.\n" <<
		"  -n\tDon't fork.\n" <<
		"  -s\tSends a shutdown-request." << endl;

	exit(EXIT_SUCCESS);
}

int main(int argc, char *argv[])
{
	// parse commandline
	for (int i = 1; i < argc; i++) {
		if (std::string(argv[i]).compare("-h") == 0) {
			print_usage_and_exit();
		}
		else if (std::string(argv[i]).compare("-s") == 0) {
			Client client("../tmp/storm-interface");
			client.send_request("shutdown");
			return EXIT_SUCCESS;
		}
	}

	std::string commandline;
	bool fork = true;
	for (int i = 1; i < argc; i++) {
		if (std::string(argv[i]).compare("-c") == 0) {
			commandline.append(argv[i]);
			i++;
			if (argc > i) {
				commandline.append(" ");
				commandline.append(argv[i]);
			}
			else {
				print_usage_and_exit();
			}
		}
		else if (std::string(argv[i]).compare("-n") == 0) {
			fork = false;
		}
		else {
			print_usage_and_exit();
		}
	}

	Starter starter("storm-ircd.exe", fork);
	starter.Run(commandline);

	return EXIT_SUCCESS;
}
