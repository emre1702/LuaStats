using System.Diagnostics;

namespace MTAResourceStats.funcs {
	class Diag {
		private Stopwatch watch;
		private string str = "";
		private bool output;
	
		public Diag ( bool output ) {
			this.output = output;
			if ( output ) {
				this.watch = new Stopwatch ();
				this.watch.Start ();
			}
		}

		public void SaveTick ( string info ) {
			if ( this.output ) {
				watch.Stop ();
				this.str += ( info + watch.ElapsedTicks ) + "\n";
				watch.Restart ();
			}
		}

		public void SaveSeconds ( string info ) {
			if ( this.output ) {
				watch.Stop ();
				this.str += ( info + ( (double) watch.ElapsedMilliseconds / 1000 ) ) + "\n";
				watch.Restart ();
			}
		}

		public void End ( ) {
			if ( this.output ) {
				watch.Stop ();
				watch = null;
				Message.SendInfoMessage ( this.str );
			}
		}
	}
}
