using System.Collections.Generic;
using MTAResourceStats.enums;
using MTAResourceStats.funcs;
using MTAResourceStats.gui;

namespace MTAResourceStats.structclass {

	class LuaFile {
		public MainWindow window;
		public string filepath;
		public string content;
		public string contentWithoutComment;
		public List<Function> functions;
		public List<LuaString> strings;
		public List<LuaComment> comments;
		public uint amountCommentChars;

		public void AddFunction ( Function function ) {
			this.functions.Add ( function );
			window.AddToData ( Stat.amountGlobalFunctions, 1 );
		}

		/*public void SortCommentsByStartIndex ( ) {
			this.comments.Sort ( ( x, y ) => x.startindex < y.startindex ? -1 : 1 );
		}*/

		public bool IsIndexInString ( int index ) {
			for ( int i = 0; i < this.strings.Count; i++ ) {
				if ( index < this.strings[i].startindex )
					return false;
				else if ( index > this.strings[i].startindex && index < this.strings[i].endindex )
					return true;
			}
			return false;
		}

		public bool IsIndexInComment ( int index ) {
			for ( int i = 0; i < this.comments.Count; i++ ) {
				if ( index < this.comments[i].startindex )
					return false;
				else if ( index > this.comments[i].startindex && index < this.comments[i].endindex )
					return true;
			}
			return false;
		}

		public void LoadContentWithoutComments ( ) {

		}
	}
}
