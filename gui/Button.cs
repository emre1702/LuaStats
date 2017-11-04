using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MTAResourceStats.enums;
using MTAResourceStats.funcs;

namespace MTAResourceStats.gui {
	public partial class MainWindow : Window {

		private void OnFolderDialogButtonClick ( object sender, RoutedEventArgs e ) {
			using ( System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog () ) {
				dialog.Description = "Select the folder of your resource.";
				dialog.ShowNewFolderButton = false;
				System.Windows.Forms.DialogResult result = dialog.ShowDialog ();
				if ( result == System.Windows.Forms.DialogResult.OK ) {
					folderTextBox.Text = dialog.SelectedPath;
				}
			}
		}


		private async void OnStartButtonClick ( object sender, RoutedEventArgs e ) {
			Diag diag = new Diag ( false );
			string path = this.folderTextBox.Text;
			if ( Directory.Exists ( path ) ) {
				this.SetDefaultValues ();
				IterateType iteratetype = (ListBoxItem) ( this.iterateChoice.SelectedItem ) == fastIterateChoice ? IterateType.fastBlocking : IterateType.slowNotBlocking;
				this.Cursor = Cursors.Wait;
				Diag indexdiag = new Diag ( true, DiagOption.indexValue );
				await Task.Run ( ( ) => {
					MainRunner.Start ( path, iteratetype, this, indexdiag );
				} );
				indexdiag.End ();
				this.Cursor = Cursors.Arrow;
				MessageBox.Show ( this, "finished", "Success", MessageBoxButton.OK, MessageBoxImage.Information );
			} else {
				Message.SendErrorMessage ( "Folder doesn't exist!", this );
			}
			diag.SaveSeconds ( "end: " );
			diag.End ( );
		}
	}
}
