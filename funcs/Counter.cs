using System.Collections.Generic;
using System.Linq;
using MTAResourceStats.enums;
using MTAResourceStats.gui;

namespace MTAResourceStats.funcs {
	static class Counter {

		#region text
		public static void CountData ( string text, string textwithoutcomments, uint amountcommentchars, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions, MainWindow window ) {
			window.AddToData ( Stat.amountCharacters, (uint) text.Replace ( "\n", "" ).Length - amountcommentchars );
			window.AddToData ( Stat.amountLines, ( (uint) ( textwithoutcomments != "" ? 1 : 0 ) + (uint) textwithoutcomments.Count ( c => c == '\n' ) ) );
		}
		#endregion

		#region comment
		public static uint CountCommentData ( string text, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions, MainWindow window ) {
			uint amountchars = 0;
			for ( int i = 0; i < multiLineCommentPositions.Count; i += 2 ) {
				string substr = text.Substring ( (int) multiLineCommentPositions[i], (int) ( multiLineCommentPositions[i + 1] - multiLineCommentPositions[i] + 1 ) );
				uint amountnewlines = (uint) ( substr.Count ( c => c == '\n' ) );
				amountchars += (uint) substr.Length - amountnewlines;
				window.AddToData ( Stat.amountCommentLines, 1 + amountnewlines );
			}
			for ( int i = 0; i < singleLineCommentPositions.Count; i += 2 ) {
				amountchars += singleLineCommentPositions[i + 1] - singleLineCommentPositions[i] + 1;
				window.AddToData ( Stat.amountCommentLines, 1 );
			}
			window.AddToData ( Stat.amountCommentCharacters, amountchars );
			return amountchars;
		}
		#endregion

		#region function
		public static void CountFunctions ( string text, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions, List<uint> stringPositions, MainWindow window ) {
			int indexfunction = text.IndexOf ( "function" );
			uint amountfunctions = 0;
			while ( indexfunction != -1 ) {
				if ( !Comment.IsIndexInComment ( text, indexfunction, multiLineCommentPositions, singleLineCommentPositions ) ) {
					if ( !LuaString.IsIndexInString ( text, indexfunction, stringPositions ) ) {
						if ( indexfunction == 0 || Util.CanStayBehind ( text[indexfunction - 1] ) ) {
							char nextchar = text[indexfunction + "function".Length];
							if ( nextchar == ' ' || nextchar == '(' || nextchar == '\n' ) {
								amountfunctions++;
							}
						}
					}
				}
				indexfunction = text.IndexOf ( "function", indexfunction + 1 );
			}
			window.AddToData ( Stat.amountGlobalFunctions, amountfunctions );
		}
		#endregion
	}
}
