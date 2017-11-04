using System;
using System.Collections.Generic;
using MTAResourceStats.enums;
using MTAResourceStats.structclass;

namespace MTAResourceStats.funcs {
	static class Position {

		#region string & comment
		public static void LoadAllCommentsAndStrings ( LuaFile file ) {
			string text = file.content;
			string singlequote = "'";
			string doublequote = "\"";
			string doubleminus = "--";
			string nextline = "\n";
			int indexStr = Util.GetMinWithoutNegative ( text.IndexOf ( singlequote, StringComparison.Ordinal ), text.IndexOf ( doublequote, StringComparison.Ordinal ) );
			int indexComm = text.IndexOf ( doubleminus, StringComparison.Ordinal );
			int currentpos = 0;

			while ( currentpos < text.Length && ( indexStr != -1 || indexComm != -1 ) ) {
				bool strbeforecomment = ( indexComm == -1 || indexStr < indexComm && indexStr != -1 );
				
				// string came first //
				if ( strbeforecomment ) {
					int indexStrEnd = text.IndexOf ( text[indexStr]+"", indexStr + 1, StringComparison.Ordinal );
					LuaString str = new LuaString ();
					str.startindex = indexStr;
					bool isescaped = ( indexStrEnd > 1 && text[indexStrEnd - 1] == '\\' && text[indexStrEnd - 2] != '\\' );
					while ( isescaped ) {
						indexStrEnd = text.IndexOf ( text[indexStr]+"", indexStrEnd + 1, StringComparison.Ordinal );
						isescaped = ( indexStrEnd > 1 && text[indexStrEnd - 1] == '\\' && text[indexStrEnd - 2] != '\\' );
					}
					if ( indexStrEnd == -1 )
						indexStrEnd = text.Length - 1;
					str.endindex = indexStrEnd;
					text.Substring ( indexStr, indexStrEnd - indexStr + 1 );
					file.strings.Add ( str );
					currentpos = indexStrEnd + 1;
				// comment came first //
				} else if ( indexComm != -1 ) {
					LuaComment comment = new LuaComment ();
					comment.startindex = (uint) indexComm;
					// is it a multiple-line comment? //
					bool addedcomment = false;
					if ( text[indexComm + 2] == '[' ) {
						string commentstr = "[";
						int addedint = indexComm + 3;
						while ( text[addedint] == '=' ) {
							commentstr += text[addedint];
							addedint++;
						}
						// yes, it's a multiple-line comment //
						if ( text[addedint] == '[' ) {
							commentstr += '[';
							comment.type = CommentType.multiLine;
							comment.str = commentstr;
							string endstr = commentstr.Replace ( '[', ']' );
							int endcommindex = text.IndexOf ( endstr, addedint + 1, StringComparison.Ordinal );
							endcommindex = ( endcommindex == -1 ? text.IndexOf ( nextline, indexComm + 1, StringComparison.Ordinal ) : ( endcommindex + endstr.Length ) );
							comment.endindex = (uint) endcommindex;
							currentpos = endcommindex + 1;
							file.comments.Add ( comment );
							addedcomment = true;
						}
					}
					if ( !addedcomment ) {
						// no, it's a single-line comment //
						comment.type = CommentType.singleLine;
						comment.str = doubleminus;
						int endstrindex = text.IndexOf ( nextline, indexComm + 1, StringComparison.Ordinal );
						comment.endindex = (uint) endstrindex;
						file.comments.Add ( comment );
						currentpos = endstrindex + 1;
					}
				}
				// continue //
				indexStr = Util.GetMinWithoutNegative ( text.IndexOf ( singlequote, currentpos, StringComparison.Ordinal ), text.IndexOf ( doublequote, currentpos, StringComparison.Ordinal ) );
				indexComm = text.IndexOf ( doubleminus, currentpos, StringComparison.Ordinal );
			}
		}
		#endregion
	}
}
