using System.Collections.Generic;
using MTAResourceStats.gui;
using MTAResourceStats.enums;

namespace MTAResourceStats.structclass {
	class MTAResource {
		private MainWindow window;
		private List<string> otherfiles = new List<string> ();
		private List<LuaFile> luafiles = new List<LuaFile> ();

		public MTAResource ( MainWindow window ) {
			this.window = window;
		}

		public void AddLuaFile ( LuaFile file ) {
			this.luafiles.Add ( file );
			window.AddToData ( Stat.amountLuaFiles, 1 );
		}

		public void AddOtherFile ( string filepath ) {
			this.otherfiles.Add ( filepath );
			window.AddToData ( Stat.amountOtherFiles, 1 );
		}


	}
}
