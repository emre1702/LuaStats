using System.Collections.Generic;
using System.Linq;

namespace MTAResourceStats.funcs {
	class Util {
		private static HashSet<char> canstaybehindset = new HashSet<char> {
			',', ' ', '\n', ';', ')', '(', '"', '\'', '[', ']', '{', '}', '='
		};
		private static HashSet<char> canstayinfrontoffunction = new HashSet<char> {
			'(', ' ', '\n'
		};
		private static HashSet<char> canstayinfrontoflocal = new HashSet<char> {
			' ', '\n'
		};
		public static List<char> variablenextseperator = new List<char> {
			'=', ' ', '\n'
		};

		public static bool CanStayBehind ( char character ) {
			return canstaybehindset.Contains ( character );
		}

		public static bool CanStayInFrontOfFunction ( char character ) {
			return canstayinfrontoffunction.Contains ( character );
		}

		public static bool CanStayInFrontOfLocal ( char character ) {
			return canstayinfrontoflocal.Contains ( character );
		}

		public static int GetNextIndexOfSeperator ( string text, int startindex, List<char> seperator ) {
			int[] indexlist = new int[seperator.Count];
			for ( int i = 0; i < seperator.Count; i++ ) {
				indexlist[i] = text.IndexOf ( seperator[i], startindex );
			}
			IEnumerable<int> indexlistwithoutnegatives = indexlist.Where ( i => i >= 0 );
			return indexlistwithoutnegatives.Count () == 0 ? -1 : indexlistwithoutnegatives.Min ();
		}

		public static int GetMinWithoutNegative ( int num1, int num2 ) {
			if ( num1 == -1 )
				return num2;
			else if ( num2 == -1 )
				return num1;
			else
				return num1 < num2 ? num1 : num2;
		}
	}
}
