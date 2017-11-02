using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MTAResourceStats.gui;

namespace MTAResourceStats.funcs {
	static class Message {

		public static void SendErrorMessage ( string message, MainWindow window ) {
			MessageBox.Show ( window, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
		}

		public static void SendInfoMessage ( string message, MainWindow window ) {
			MessageBox.Show ( window, message, "Info", MessageBoxButton.OK, MessageBoxImage.Information );
		}
	}
}
