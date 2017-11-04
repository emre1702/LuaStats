using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MTAResourceStats.enums;
using MTAResourceStats.gui;
using MTAResourceStats.structclass;

namespace MTAResourceStats.funcs {
	static class Counter {

		#region text
		public static void CountData ( LuaFile file, ref string textwithoutcomment ) {
			file.window.AddToData ( Stat.amountCharacters, (uint) ( file.content.Replace ( "\n", "" ).Length - file.amountCommentChars ) );
			file.window.AddToData ( Stat.amountLines, ( (uint) ( textwithoutcomment != "" ? 1 : 0 ) + (uint) textwithoutcomment.Count ( c => c == '\n' ) ) );
		}
		#endregion

		#region comment
		public static void CountCommentData ( LuaFile file, ref string textwithoutcomment ) {
			uint amountchars = (uint) textwithoutcomment.Length;

			file.window.AddToData ( Stat.amountCommentCharacters, (uint) textwithoutcomment.Length );
			file.window.AddToData ( Stat.amountCommentLines, (uint) ( Text.GetCountOfStringInText ( ref textwithoutcomment, "\n" ) ) );

			file.amountCommentChars = amountchars;
		}
		#endregion

		#region function
		public static void CountFunctions ( LuaFile file, ref string text ) {
			uint amountfunctions = 0;
			int indexfunction = text.IndexOf ( "function", StringComparison.Ordinal );
			while ( indexfunction != -1 ) {
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
				indexfunction = text.IndexOf ( "function", indexfunction + 1, StringComparison.Ordinal );
			}
			file.window.AddToData ( Stat.amountGlobalFunctions, amountfunctions );
		}
		#endregion
	}
}
