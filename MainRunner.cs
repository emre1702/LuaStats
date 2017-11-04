using System.Collections.Generic;
using System.IO;
using System.Text;
using MTAResourceStats.enums;
using MTAResourceStats.funcs;
using MTAResourceStats.gui;
using MTAResourceStats.structclass;

namespace MTAResourceStats {
	class MainRunner {

		public static void Start ( string path, IterateType iteratetype, MainWindow window, Diag indexdiag ) {
			MTAResource resource = new MTAResource ( window );

			IEnumerable<string> files = Directory.EnumerateFiles ( path, "*", SearchOption.AllDirectories );
			// iterate files //
			Iterate.MainIterate ( iteratetype, files, ( filepath ) => StatsRetrieve ( filepath, window, resource, indexdiag ) );
		}

		private static void StatsRetrieve ( string filepath, MainWindow window, MTAResource resource, Diag indexdiag ) {
			if ( !Path.HasExtension ( filepath ) || Path.GetExtension ( filepath ).ToLower () != ".lua" )
				resource.AddOtherFile ( filepath );
			else {
				Diag diag = new Diag ( false );
				LuaFile file = new LuaFile ();
				resource.AddLuaFile ( file );
				file.window = window;
				file.filepath = filepath;
				diag.SaveTick ( filepath+" :" );
				file.comments = new List<LuaComment> ();
				file.functions = new List<Function> ();
				file.strings = new List<LuaString> ();
				diag.SaveTick ( "1: " );
				string[] textlines = File.ReadAllLines ( filepath );
				diag.SaveTick ( "2: " );
				StringBuilder builder = new StringBuilder ();
				Text.GetTextWithoutUselessLines ( ref textlines, ref builder );

				diag.SaveTick ( "3: " );
				Text.GetTextWithoutUselessSpaces ( ref builder );
				file.content = builder.ToString();

				diag.SaveTick ( "4: " );
				Position.LoadAllCommentsAndStrings ( file );
				diag.SaveTick ( "5: " );
				Text.LoadTextWithoutCommentIntoBuilder ( file, ref builder );
				string textwithoutcomment = builder.ToString ();
				diag.SaveTick ( "6: " );

				
				Counter.CountFunctions ( file, ref textwithoutcomment );
				diag.SaveTick ( "7: " );
				Counter.CountData ( file, ref textwithoutcomment );
				diag.SaveTick ( "8: " );
				Counter.CountCommentData ( file, ref textwithoutcomment );
				diag.SaveTick ( "9: " );
				diag.End ( );

				//Variable.LoadLocalVariables ( text, textwithoutcomment, ref localvariables, multiLineCommentPositions, singleLineCommentPositions, stringPositions );
			}
		}
	}
}
