using System.Collections.Generic;

namespace MTAResourceStats.funcs {
	static class Position {

		#region comment
		public static void LoadAllCommentStartPositions ( string text, ref List<uint> multiLineCommentPositions, ref List<uint> singleLineCommentPositions ) {
			int index = text.IndexOf ( "--" );
			while ( index != -1 ) {
				if ( text[index + 2] == '[' ) {
					string commentstr = "[";
					int addedint = index + 3;
					while ( text[addedint] == '=' ) {
						addedint++;
						commentstr += text[addedint];
					}
					if ( text[addedint] == '[' ) {
						//commentstr += '[';
						multiLineCommentPositions.Add ( (uint) index );
						index = text.IndexOf ( "--", index + 4 );
						continue;
					}
				}
				singleLineCommentPositions.Add ( (uint) index );
				index = text.IndexOf ( "--", index + 1 );
			}
		}
		#endregion

		#region string & comment
		public static void LoadAllCommentsAndStrings ( string text, ref List<uint> multiLineCommentPositions, ref List<uint> singleLineCommentPositions, ref List<uint> stringPositions ) {
			int indexSingleStr = text.IndexOf ( '\'' );
			int indexMultiStr = text.IndexOf ( '"' );
			int indexSingleComm = singleLineCommentPositions.Count > 0 ? (int) singleLineCommentPositions[0] : -1;
			int indexMultiComm = multiLineCommentPositions.Count > 0 ? (int) multiLineCommentPositions[0] : -1;
			int posSingleComm = 0;
			int posMultiComm = 0;
			int currentpos = 0;
			char laststrchar = '|';

			while ( currentpos < text.Length && ( indexSingleStr != -1 || indexMultiStr != -1 || indexSingleComm != -1 || indexMultiComm != -1 ) ) {
				// ' is first //
				if ( indexSingleStr != -1
						&& ( laststrchar == '\''
						|| ( ( indexSingleStr < indexMultiStr || indexMultiStr == -1 )
						&& ( indexSingleStr < indexSingleComm || indexSingleComm == -1 )
						&& ( indexSingleStr < indexMultiComm || indexMultiComm == -1 ) ) ) ) {
					stringPositions.Add ( (uint) indexSingleStr );
					currentpos = indexSingleStr + 1;
					laststrchar = laststrchar == '\'' ? '|' : '\'';
					// " is first //
				} else if ( indexMultiStr != -1
						&& ( laststrchar == '"'
						|| ( ( indexMultiStr < indexSingleStr || indexSingleStr == -1 )
						&& ( indexMultiStr < indexSingleComm || indexSingleComm == -1 )
						&& ( indexMultiStr < indexMultiComm || indexMultiComm == -1 ) ) ) ) {
					stringPositions.Add ( (uint) indexMultiStr );
					currentpos = indexMultiStr + 1;
					laststrchar = laststrchar == '"' ? '|' : '"';
					// -- is first //
				} else if ( laststrchar == '|'
						&& indexSingleComm != -1
						&& ( indexSingleComm < indexSingleStr || indexSingleStr == -1 )
						&& ( indexSingleComm < indexMultiStr || indexMultiStr == -1 )
						&& ( indexSingleComm < indexMultiComm || indexMultiComm == -1 ) ) {
					currentpos = text.IndexOf ( "\n", indexSingleComm );
					singleLineCommentPositions.Insert ( posSingleComm + 1, (uint) currentpos - 1 );
					posSingleComm += 2;
					// --[[=...][ is first //
				} else if ( laststrchar == '|'
						&& indexMultiComm != -1
						&& ( indexMultiComm < indexSingleStr || indexSingleStr == -1 )
						&& ( indexMultiComm < indexMultiStr || indexMultiStr == -1 )
						&& ( indexMultiComm < indexSingleComm || indexSingleComm == -1 ) ) {
					int commStartEndPos = text.IndexOf ( '[', text.IndexOf ( '[', indexMultiComm ) + 1 );
					string commStart = text.Substring ( indexMultiComm, commStartEndPos - indexMultiComm + 1 );
					int endindex = text.IndexOf ( commStart.Substring ( 2 ).Replace ( '[', ']' ), indexMultiComm );
					if ( endindex != -1 ) {
						currentpos = endindex + ( commStart.Length - 2 );
						multiLineCommentPositions.Insert ( posMultiComm + 1, (uint) currentpos - 1 );
					} else {
						currentpos = text.Length;
						multiLineCommentPositions.Insert ( posMultiComm + 1, (uint) currentpos - 2 );
					}
					posMultiComm += 2;
				}

				indexSingleStr = text.IndexOf ( '\'', currentpos );
				indexMultiStr = text.IndexOf ( '"', currentpos );
				while ( singleLineCommentPositions.Count > posSingleComm && singleLineCommentPositions[posSingleComm] < currentpos ) {
					singleLineCommentPositions.RemoveAt ( posSingleComm );
				}
				indexSingleComm = singleLineCommentPositions.Count > posSingleComm ? (int) singleLineCommentPositions[posSingleComm] : -1;
				while ( multiLineCommentPositions.Count > posMultiComm && multiLineCommentPositions[posMultiComm] < currentpos ) {
					multiLineCommentPositions.RemoveAt ( posMultiComm );
				}
				indexMultiComm = multiLineCommentPositions.Count > posMultiComm ? (int) multiLineCommentPositions[posMultiComm] : -1;

				if ( laststrchar == '\'' && indexSingleStr == -1 || laststrchar == '"' && indexMultiStr == -1 ) {
					for ( int i = posSingleComm; i < singleLineCommentPositions.Count; i++ ) {
						if ( singleLineCommentPositions[i] >= currentpos )
							singleLineCommentPositions.RemoveRange ( i, singleLineCommentPositions.Count - i );
					}
					for ( int i = posMultiComm; i < multiLineCommentPositions.Count; i++ ) {
						if ( multiLineCommentPositions[i] >= currentpos )
							multiLineCommentPositions.RemoveRange ( i, multiLineCommentPositions.Count - i );
					}
					return;
				}
			}
		}
		#endregion
	}
}
