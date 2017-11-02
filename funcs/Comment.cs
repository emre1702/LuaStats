using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTAResourceStats.enums;
using MTAResourceStats.gui;

namespace MTAResourceStats.funcs {
	static class Comment {

		public static bool IsIndexInComment ( string text, int index, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions ) {
			for ( int i = 0; i < multiLineCommentPositions.Count; i += 2 ) {
				if ( index < multiLineCommentPositions[i] )
					break;
				else if ( index > multiLineCommentPositions[i] && index < multiLineCommentPositions[i + 1] )
					return true;
			}
			for ( int i = 0; i < singleLineCommentPositions.Count; i += 2 ) {
				if ( index < singleLineCommentPositions[i] )
					return false;
				else if ( index > singleLineCommentPositions[i] && index < singleLineCommentPositions[i + 1] )
					return true;
			}
			return false;
		}

	}
}
