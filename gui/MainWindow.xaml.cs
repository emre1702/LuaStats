using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Linq;
using MTAResourceStats.funcs;
using MTAResourceStats.enums;

namespace MTAResourceStats.gui {

	partial class MainWindow : Window {
		public int amountfilealreadydone = 0;
		private string version = "v1.1.6";

		public MainWindow ( ) {
			InitializeComponent ();
			this.Title += " " + version;
		}
	}
}
