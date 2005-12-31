# Copyright (C) 2005 Josef Schmeisser <blackdragon@users.berlios.de>
#
# This file is free software; as a special exception the author gives
# unlimited permission to copy and/or distribute it, with or without 
# modifications, as long as this notice is preserved.
#
# This program is distributed in the hope that it will be useful, but
# WITHOUT ANY WARRANTY, to the extent permitted by law; without even the
# implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

# Files to remove
CLEANFILES = storm-ircd *.o

# binarys
bin_PROGRAMS = storm-ircd

# sources
storm_ircd_SOURCES =	storm-ircd.cpp \
			Client.cpp \
			Client.h \
			ShutdownRequest.h \
			Starter.cpp \
			Starter.h

# libs
storm_ircd_LDADD =	$(BUILD_SKSTREAM_LIBS)

AM_CPPFLAGS = $(BUILD_SKSTREAM_CXXFLAGS)
