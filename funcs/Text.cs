using System.Collections.Generic;
using System.Text.RegularExpressions;

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
			while ( newlineindex >= 0 ) {
				if ( newlineindex < text.Length - 1 && text[newlineindex + 1] == ' ' ) {
					text = text.Remove ( newlineindex + 1, 1 );
				}
				if ( newlineindex > 0 && text[newlineindex - 1] == ' ' ) {
					text = text.Remove ( newlineindex - 1, 1 );
					newlineindex--;
				}
				newlineindex = text.IndexOf ( '\n', newlineindex + 1 );
			}
			return text;
		}

		public static string GetTextWithoutComment ( string text, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions ) {
			for ( int multiI = multiLineCommentPositions.Count - 1, singleI = singleLineCommentPositions.Count - 1; multiI >= 0 || singleI >= 0; ) {
				if ( singleI < 0 || multiI >= 0 && multiLineCommentPositions[multiI - 1] > singleLineCommentPositions[singleI - 1] ) {
					text = text.Remove ( (int) multiLineCommentPositions[multiI - 1], (int) ( multiLineCommentPositions[multiI] - multiLineCommentPositions[multiI - 1] + 1 ) );
					multiI -= 2;
				} else {
					text = text.Remove ( (int) singleLineCommentPositions[singleI - 1], (int) ( singleLineCommentPositions[singleI] - singleLineCommentPositions[singleI - 1] + 1 ) );
					singleI -= 2;
				}
			}
			return GetTextWithoutUselessLines ( GetTextWithoutUselessSpaces ( text ) );
		}
	}
}