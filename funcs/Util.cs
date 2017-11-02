namespace MTAResourceStats.funcs {
	class Util {

		public static bool CanStayBehind ( char character ) {
			return ( character == ',' || character == ' ' || character == '\n' || character == ';' );
		}
	}
}
