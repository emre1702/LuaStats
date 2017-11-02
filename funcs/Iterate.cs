using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTAResourceStats.enums;
using MTAResourceStats.gui;

namespace MTAResourceStats.funcs {

	static class Iterate {
		public static void MainIterate ( IterateType type, IEnumerable<string> source, Action<string> action ) {
			if ( type == IterateType.fastBlocking )
				Parallel.ForEach ( source, action );
			else if ( type == IterateType.slowNotBlocking )
				foreach ( string filepath in source )
					action ( filepath );
		}
	}
}
