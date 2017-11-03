using System.Collections.Generic;
using System.Linq;
using MTAResourceStats.enums;
using MTAResourceStats.gui;
using MTAResourceStats.structclass;

namespace MTAResourceStats.funcs {
	static class Counter {

		#region text
		public static void CountData ( LuaFile file ) {
			file.window.AddToData ( Stat.amountCharacters, (uint) ( file.content.Replace ( "\n", "" ).Length - file.amountCommentChars ) );
			file.window.AddToData ( Stat.amountLines, ( (uint) ( file.contentWithoutComment != "" ? 1 : 0 ) + (uint) file.contentWithoutComment.Count ( c => c == '\n' ) ) );
		}
		#endregion

		#region comment
		public static void CountCommentData ( LuaFile file ) {
			uint amountchars = 0;
			for ( int i = 0; i < file.comments.Count; i++ ) {
				string substr = file.content.Substring ( (int) file.comments[i].startindex, (int) ( file.comments[i].endindex - file.comments[i].startindex + 1 ) );
				uint amountnewlines = (uint) ( substr.Count ( c => c == '\n' ) );
				amountchars += (uint) substr.Length - amountnewlines;
				file.window.AddToData ( Stat.amountCommentLines, 1 + amountnewlines );
			}
			file.window.AddToData ( Stat.amountCommentCharacters, amountchars );
			file.amountCommentChars = amountchars;
		}
		#endregion

		#region function
		public static void CountFunctions ( LuaFile file ) {
			string text = file.content;
			uint amountfunctions = 0;
			int indexfunction = text.IndexOf ( "function" );
			while ( indexfunction != -1 ) {
				if ( !file.IsIndexInComment ( indexfunction ) ) {
					if ( !file.IsIndexInString ( indexfunction ) ) {
						if ( indexfunction == 0 || Util.CanStayBehind ( text[indexfunction - 1] ) ) {
							if ( text.Length > indexfunction + "function".Length ) {
								char nextchar = text[indexfunction + "function".Length];
								if ( Util.CanStayInFrontOfFunction ( nextchar ) ) {
									amountfunctions++;
								}
							}
						}
					}
				}
				indexfunction = text.IndexOf ( "function", indexfunction + 1 );
			}
			file.window.AddToData ( Stat.amountGlobalFunctions, amountfunctions );
		}
		#endregion
	}
}
