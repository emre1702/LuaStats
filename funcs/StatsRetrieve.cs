using System.Collections.Generic;
using System.IO;
using MTAResourceStats.enums;
using MTAResourceStats.gui;

namespace MTAResourceStats.funcs {
	static class StatsRetrieve {

		public static void Start ( string filepath, MainWindow window ) {
			if ( !Path.HasExtension ( filepath ) || Path.GetExtension ( filepath ).ToLower () != ".lua" )
				window.AddToData ( Stat.amountOtherFiles, 1 );
			else {
				window.AddToData ( Stat.amountLuaFiles, 1 );
				List<uint> newLinePositions = new List<uint> ();
				List<uint> multiLineCommentPositions = new List<uint> ();
				List<uint> singleLineCommentPositions = new List<uint> ();
				List<uint> stringPositions = new List<uint> ();
				HashSet<string> localvariables = new HashSet<string> ();
				string[] textlines = File.ReadAllLines ( filepath );
				string text = string.Join ( "\n", textlines );
				text = Text.GetTextWithoutUselessLines ( text );
				text = Text.GetTextWithoutUselessSpaces ( text ) + "\n";
				Position.LoadAllCommentStartPositions ( text, ref multiLineCommentPositions, ref singleLineCommentPositions );
				Position.LoadAllCommentsAndStrings ( text, ref multiLineCommentPositions, ref singleLineCommentPositions, ref stringPositions );

				string textwithoutcomment = Text.GetTextWithoutComment ( text, multiLineCommentPositions, singleLineCommentPositions );

				uint amountcommentchars = Counter.CountCommentData ( text, multiLineCommentPositions, singleLineCommentPositions, window );
				Counter.CountData ( text, textwithoutcomment, amountcommentchars, multiLineCommentPositions, singleLineCommentPositions, window );
				Counter.CountFunctions ( text, multiLineCommentPositions, singleLineCommentPositions, stringPositions, window );

				//this.LoadLocalVariables ( text, textwithoutcomment, ref localvariables );
			}
		}
	}
}
