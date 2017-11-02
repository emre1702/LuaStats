using System.Collections.Generic;

namespace MTAResourceStats.funcs {
	static class Variable {

		public static void LoadLocalVariables ( string text, string textwithoutcomment, ref HashSet<string> localvariables ) {
			int localindex = text.IndexOf ( "local " );
			while ( localindex != -1 ) {
				if ( localindex == 0 || text[localindex - 1] == '\n' || text[localindex - 1] == ' ' ) {
					int varstartindex = localindex + "local ".Length;
					int nextspaceindex = text.IndexOf ( ' ', varstartindex );
					int nextnextlineindex = text.IndexOf ( '\n', varstartindex );
					string var = "";
					if ( nextspaceindex != -1 && ( nextnextlineindex == -1 || nextspaceindex < nextnextlineindex ) ) {
						var = text.Substring ( varstartindex, nextspaceindex - varstartindex );
					} else if ( nextnextlineindex != -1 && ( nextspaceindex == -1 || nextnextlineindex < nextspaceindex ) ) {
						var = text.Substring ( varstartindex, nextnextlineindex - varstartindex );
					}
					if ( var != "" ) {
						if ( !localvariables.Contains ( var ) ) {
							localvariables.Add ( var );
						}
					}
				}
				localindex = text.IndexOf ( "local ", localindex + 1 );
			}
		}
	}
}
