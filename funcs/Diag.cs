using System.Collections.Generic;
using System.Diagnostics;
using MTAResourceStats.enums;

namespace MTAResourceStats.funcs {
	class Diag {
		private Stopwatch watch;
		private string str = "";
		private Dictionary<string, long> timeperindex;
		private bool output;
		private DiagOption option;
	
		public Diag ( bool output, DiagOption option = DiagOption.startToEnd ) {
			this.output = output;
			if ( output ) {
				this.watch = new Stopwatch ();
				if ( option == DiagOption.startToEnd ) {
					this.watch.Start ();
				} else if ( option == DiagOption.indexValue ) {
					this.timeperindex = new Dictionary<string, long> ();
				}
			}
			this.option = option;
		}

		public void Start ( ) {
			this.watch.Start ();
		}
		
		public void Stop ( ) {
			this.watch.Reset ();
			this.watch.Stop ();
		}

		public void SaveTick ( string info ) {
			if ( this.output ) {
				this.watch.Stop ();
				if ( this.option == DiagOption.startToEnd ) {
					this.str += ( info + this.watch.ElapsedTicks ) + "\n";
				} else if ( this.option == DiagOption.indexValue ) {
					timeperindex[info] = ( timeperindex.ContainsKey ( info ) ? timeperindex[info] + this.watch.ElapsedTicks : this.watch.ElapsedTicks );
				}
				this.watch.Restart ();
			}
		}

		public void SaveSeconds ( string info ) {
			if ( this.output ) {
				this.watch.Stop ();
				if ( this.option == DiagOption.startToEnd ) {
					this.str += ( info + ( (double) this.watch.ElapsedMilliseconds / 1000 ) ) + "\n";
					this.watch.Restart ();
				} else if ( this.option == DiagOption.indexValue ) {
					timeperindex[info] = ( timeperindex.ContainsKey ( info ) ? timeperindex[info] + this.watch.ElapsedMilliseconds : this.watch.ElapsedMilliseconds );
					this.watch.Reset ();
				}
			}
		}

		public void End ( ) {
			if ( this.output ) {
				this.watch.Stop ();
				this.watch = null;
				if ( this.option == DiagOption.startToEnd ) {
					Message.SendInfoMessage ( this.str );
				} else if ( this.option == DiagOption.indexValue ) {
					string newstr = "";
					foreach ( KeyValuePair<string, long> entry in this.timeperindex ) {
						newstr += entry.Key + ": " + entry.Value + "\n";
					}
					Message.SendInfoMessage ( newstr );
				}
			}
		}
	}
}
