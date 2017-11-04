using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using MTAResourceStats.structclass;

namespace MTAResourceStats.funcs {
	static class Text {

		private static Regex multiSpaceRegex = new Regex ( @"[^\S\r\n]+" );

		public static void GetTextWithoutUselessLines ( ref string[] textlines, ref StringBuilder builder ) {
			for ( int i = 0; i < textlines.Length; i++ ) {
				if ( !string.IsNullOrWhiteSpace ( textlines[i] ) ) {
					builder.Append ( textlines[i].Trim() + "\n" );
				}
			}
		}

		public static void GetTextWithoutUselessSpaces ( ref StringBuilder builder ) {
			string newstr = ( multiSpaceRegex.Replace ( builder.ToString(), " " ) );
			builder.Clear().Append ( newstr ).Replace ( " \n ", "\n" ).Replace ( " \n", "\n" ).Replace ( "\n ", "\n" ).Append ( "\n" );
		}

		public static void LoadTextWithoutCommentIntoBuilder ( LuaFile file, ref StringBuilder builder ) {
			for ( int i = file.comments.Count - 1; i >= 0; i-- ) {
				builder.Remove ( (int) file.comments[i].startindex, (int) ( file.comments[i].endindex - file.comments[i].startindex + 1 ) );
			}
		}
	}
}