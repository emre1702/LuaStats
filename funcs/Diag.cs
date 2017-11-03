using System.Diagnostics;

namespace MTAResourceStats.funcs {
	class Diag {
		private Stopwatch watch;
		private string str = "";
	
		public Diag ( ) {
			this.watch = new Stopwatch ();
			this.watch.Start ();
		}

		public void SaveTick ( string info ) {
			watch.Stop ();
			this.str += ( info + watch.ElapsedTicks ) + "\n";
			watch.Restart ();
		}

		public void SaveSeconds ( string info ) {
			watch.Stop ();
			this.str += ( info + ( (double) watch.ElapsedMilliseconds / 1000 ) ) + "\n";
			watch.Restart ();
		}

		public void End ( bool output = false ) {
			watch.Stop ();
			watch = null;
			if ( output )
				Message.SendInfoMessage ( this.str );
		}
	}
}
