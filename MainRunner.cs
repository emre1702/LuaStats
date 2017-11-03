

using System.Collections.Generic;
using System.IO;
using MTAResourceStats.enums;
using MTAResourceStats.funcs;
using MTAResourceStats.gui;
using MTAResourceStats.structclass;

namespace MTAResourceStats {
	class MainRunner {

		public static void Start ( string path, IterateType iteratetype, MainWindow window ) {
			MTAResource resource = new MTAResource ( window );

			IEnumerable<string> files = Directory.EnumerateFiles ( path, "*", SearchOption.AllDirectories );
			// iterate files //
			Iterate.MainIterate ( iteratetype, files, ( filepath ) => StatsRetrieve ( filepath, window, resource ) );
		}

		private static void StatsRetrieve ( string filepath, MainWindow window, MTAResource resource ) {
			if ( !Path.HasExtension ( filepath ) || Path.GetExtension ( filepath ).ToLower () != ".lua" )
				resource.AddOtherFile ( filepath );
			else {
				LuaFile file = new LuaFile ();
				resource.AddLuaFile ( file );
				file.window = window;
				file.filepath = filepath;
				file.comments = new List<LuaComment> ();
				file.functions = new List<Function> ();
				file.strings = new List<LuaString> ();

				string[] textlines = File.ReadAllLines ( filepath );
				string text = string.Join ( "\n", textlines );
				text = Text.GetTextWithoutUselessLines ( text );
				text = Text.GetTextWithoutUselessSpaces ( text ) + "\n";
				file.content = text;
	
				Position.LoadAllCommentsAndStrings ( file );
				Text.LoadTextWithoutComment ( file );

				Counter.CountFunctions ( file );
				Counter.CountData ( file );
				Counter.CountCommentData ( file );
				Counter.CountFunctions ( file ); 

				//Variable.LoadLocalVariables ( text, textwithoutcomment, ref localvariables, multiLineCommentPositions, singleLineCommentPositions, stringPositions );
			}
		}
	}
}
