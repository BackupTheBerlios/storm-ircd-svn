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

/*
Possible signal values list:
Signal     Value     Action   Comment
-----------------------------------------------------------------------              
SIGHUP        1       Term    Hangup detected on controlling terminal 
                                     or death of controlling process
SIGINT        2       Term    Interrupt from keyboard
SIGQUIT       3       Core    Quit from keyboard
SIGILL        4       Core    Illegal Instruction
SIGABRT       6       Core    Abort signal from abort(3)
SIGFPE        8       Core    Floating point exception
SIGKILL       9       Term    Kill signal
SIGSEGV      11       Core    Invalid memory reference
SIGPIPE      13       Term    Broken pipe: write to pipe with no readers
SIGALRM      14       Term    Timer signal from alarm(2)
SIGTERM      15       Term    Termination signal
SIGUSR1   30,10,16    Term    User-defined signal 1
SIGUSR2   31,12,17    Term    User-defined signal 2
SIGCHLD   20,17,18    Ign     Child stopped or terminated
SIGCONT   19,18,25            Continue if stopped
SIGSTOP   17,19,23    Stop    Stop process
SIGTSTP   18,20,24    Stop    Stop typed at tty
SIGTTIN   21,21,26    Stop    tty input for background process
SIGTTOU   22,22,27    Stop    tty output for background process
*/     
// to be added
//using Mono.Posix;

/*

using System;
using Mono.Posix;
//using Mono.Unix;
using System.Runtime.InteropServices;

public sealed class storm_signal
{
	public void SignalCatcher(int v)
	{
		Console.WriteLine("Signal received: " + v);
		// socket.send(shutdown);
	}

/*	public static void Main(string[] args)
	{
		Test t = new Test();
		Console.Write("waiting for event...");
		string x = Console.ReadLine();
	}*/
/*
	public void Test()
	{
		Console.WriteLine("signal result: " + Syscall.signal(14, new Syscall.sighandler_t(SignalCatcher)));
		Syscall.alarm(3);
	}

	public storm_signal()
	{
		// catch the term-signal
		// and kill
		int result = Syscall.signal(9, new Syscall.sighandler_t(this.SignalCatcher));
		Console.WriteLine("storm_signal result: {0}", result);
		Syscall.alarm(3);
	}
}

*/
