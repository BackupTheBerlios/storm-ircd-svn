# Copyright 2005 Josef Schmeisser
# Distributed under the terms of the GNU General Public License v2

DESCRIPTION="storm-ircd: "
HOMEPAGE="http://developer.berlios.de/projects/storm-ircd/"
SRC_URI="http://"

LICENSE="GPL-2"
SLOT="0"
KEYWORDS="~x86 ~amd64"

IUSE=""

DEPEND=">=dev-lang/mono-1.1.10
	">=dev-dotnet/nant-0.85_rc3"

pkg_preinst() {
	enewgroup ircd
}

src_unpack() {
	unpack ${A}
}

src_compile() {
	econf || die "./configure failed!"
	emake || die "make failed!"
}

src_install {
	dodoc README ChangeLog AUTHORS INSTALL NEWS
}
