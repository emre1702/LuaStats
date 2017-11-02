using System.Collections.Generic;

namespace MTAResourceStats.funcs {
	static class LuaString {

		#region string
		public static bool IsIndexInString ( string text, int index, List<uint> stringPositions ) {
			for ( int i = 0; i < stringPositions.Count; i += 2 ) {
				if ( index < stringPositions[i] )
					return false;
				else if ( index > stringPositions[i] && index < stringPositions[i + 1] )
					return true;
			}
			return false;
		}
		#endregion
	}
}
