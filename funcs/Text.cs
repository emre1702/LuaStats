using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using MTAResourceStats.structclass;

namespace MTAResourceStats.funcs {
	static class Text {

		public static string GetTextWithoutUselessLines ( string text ) {
			int firstpos = 0;
			int secondpos = text.IndexOf ( '\n' );
			while ( secondpos != -1 ) {
				if ( firstpos == secondpos ) {
					text = text.Remove ( firstpos, 1 );
					secondpos--;
				} else {
					string substring = text.Substring ( firstpos, secondpos - firstpos );
					if ( string.IsNullOrWhiteSpace ( substring ) ) {
						text = text.Remove ( firstpos, substring.Length );
						secondpos -= substring.Length;
					}
				}
				firstpos = secondpos + 1;
				secondpos = text.IndexOf ( '\n', firstpos );
			}
			while ( text != "" && text[text.Length - 1] == '\n' )
				text = text.Remove ( text.Length - 1 );

			return text;
		}

		public static string GetTextWithoutUselessSpaces ( string text ) {
			text = Regex.Replace ( text, @"[^\S\r\n]+", " " );
			int newlineindex = text.IndexOf ( '\n' );
			text = text.Replace ( " \n ", "\n" ).Replace ( " \n", "\n" ).Replace ( "\n ", "\n" );
			return text;
		}

		public static void LoadTextWithoutComment ( LuaFile file ) {
			string text = file.content;
			for ( int i = file.comments.Count - 1; i >= 0; i-- ) {
				text = text.Remove ( (int) file.comments[i].startindex, (int) ( file.comments[i].endindex - file.comments[i].startindex + 1 ) );
			}
			file.contentWithoutComment = GetTextWithoutUselessLines ( GetTextWithoutUselessSpaces ( text ) );
		}
	}
}