using System.Collections.Generic;

namespace MTAResourceStats.funcs {
	static class Variable {

		/*public static void LoadLocalVariables ( string text, string textwithoutcomment, ref HashSet<string> localvariables, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions, List<uint> stringPositions ) {
			int localindex = text.IndexOf ( "local" );
			while ( localindex != -1 ) {
				if ( !Comment.IsIndexInComment ( text, localindex, multiLineCommentPositions, singleLineCommentPositions ) ) {
					if ( !LuaString.IsIndexInString ( text, localindex, stringPositions ) ) {
						if ( localindex == 0 || Util.CanStayBehind ( text[localindex - 1] ) ) {
							int afterlocalindex = localindex + "local".Length;
							if ( afterlocalindex < text.Length && Util.CanStayInFrontOfLocal ( text[afterlocalindex] ) ) {
								int nextseperatorindex = Util.GetNextIndexOfSeperator ( text, afterlocalindex + 1, Util.variablenextseperator );
								while ( nextseperatorindex != -1 && Comment.IsIndexInComment ( text, nextseperatorindex, multiLineCommentPositions, singleLineCommentPositions ) )
									nextseperatorindex = Util.GetNextIndexOfSeperator ( text, nextseperatorindex + 1, Util.variablenextseperator );
								while ( nextseperatorindex != -1 && LuaString.IsIndexInString ( text, nextseperatorindex, stringPositions ) )
									nextseperatorindex = Util.GetNextIndexOfSeperator ( text, nextseperatorindex + 1, Util.variablenextseperator );
								if ( nextseperatorindex > 0 ) {
									string var = text.Substring ( afterlocalindex + 1, nextseperatorindex - afterlocalindex - 1 );
									if ( !localvariables.Contains ( var ) ) {
										localvariables.Add ( var );
									}
									Message.SendInfoMessage ( var + " - " + var.Length );
								}
							}
						}
					}
				}
				localindex = text.IndexOf ( "local", localindex + 1 );
			}
		}*/
	}
}
