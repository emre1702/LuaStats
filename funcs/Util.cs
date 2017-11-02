using System.Collections.Generic;

namespace MTAResourceStats.funcs {
	class Util {
		private static HashSet<char> canstaybehindset = new HashSet<char> {
			',', ' ', '\n', ';', ')', '(', '"', '\'', '[', ']', '{', '}', '='
		};
		private static HashSet<char> canstayinfrontoffunction = new HashSet<char> {
			'(', ' ', '\n'
		};

		public static bool CanStayBehind ( char character ) {
			return canstaybehindset.Contains ( character );
		}

		public static bool CanStayInFrontOfFunction ( char character ) {
			return canstayinfrontoffunction.Contains ( character );
		}
	}
}
