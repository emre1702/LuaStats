using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using System.Threading;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Linq;

namespace MTAResourceStats {

	enum Stats {
		amountLuaFiles, amountOtherFiles, amountLocalFunctions, amountGlobalFunctions, amountLines, amountCharacters, amountCommentLines, amountCommentCharacters
	}

	enum IterateTypes {
		fastBlocking, slowNotBlocking
	}


	public partial class MainWindow : Window {

		public MainWindow ( ) {
			InitializeComponent ();
		}

		#region button-click 
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
			string path = this.folderTextBox.Text;
			if ( Directory.Exists ( path ) ) {
				this.SetDefaultValues ();
				IterateTypes iteratetype = (ListBoxItem) ( this.iterateChoice.SelectedItem ) == fastIterateChoice ? IterateTypes.fastBlocking : IterateTypes.slowNotBlocking;
				await Task.Run ( ( ) => {
					this.Dispatcher.InvokeAsync ( ( ) => this.Cursor = Cursors.Wait );
					IEnumerable<string> files = Directory.EnumerateFiles ( path, "*", SearchOption.AllDirectories );
					// iterate files //
					this.MainIterate ( iteratetype, files, ( string filepath ) => {
						if ( !Path.HasExtension ( filepath ) || Path.GetExtension ( filepath ).ToLower () != ".lua" )
							this.AddToData ( Stats.amountOtherFiles, 1 );
						else {
							this.AddToData ( Stats.amountLuaFiles, 1 );
							List<uint> newLinePositions = new List<uint> ();
							List<uint> multiLineCommentPositions = new List<uint> ();
							List<uint> singleLineCommentPositions = new List<uint> ();
							List<uint> stringPositions = new List<uint> ();
							HashSet<string> localvariables = new HashSet<string> ();
							string[] textlines = File.ReadAllLines ( filepath );
							string text = string.Join ( "\n", textlines );
							text = this.GetTextWithoutUselessLines ( text );
							text = this.GetTextWithoutUselessSpaces ( text ) + "\n";
							this.LoadAllCommentStartPositions ( text, ref multiLineCommentPositions, ref singleLineCommentPositions );
							this.LoadAllCommentsAndStrings ( text, ref multiLineCommentPositions, ref singleLineCommentPositions, ref stringPositions );

							string textwithoutcomment = GetTextWithoutComment ( text, multiLineCommentPositions, singleLineCommentPositions );

							uint amountcommentchars = this.CountCommentData ( text, multiLineCommentPositions, singleLineCommentPositions );
							this.CountData ( text, textwithoutcomment, amountcommentchars, multiLineCommentPositions, singleLineCommentPositions );

							//this.LoadLocalVariables ( text, textwithoutcomment, ref localvariables );
						}
					} );
				} );
				this.Cursor = Cursors.Arrow;
				MessageBox.Show ( this, "finished", "Success", MessageBoxButton.OK, MessageBoxImage.Information );
			} else {
				SendErrorMessage ( "Folder doesn't exist!" );
			}
		}
		#endregion

		#region text 
		private string GetTextWithoutUselessLines ( string text ) {
			int firstpos = 0;
			int secondpos = text.IndexOf ( '\n' );
			while ( secondpos != -1 ) {
				if ( firstpos == secondpos ) {
					text = text.Remove ( firstpos, 1 );
					secondpos--;
				} else {
					string substring = text.Substring ( firstpos, secondpos - firstpos );
					if ( string.IsNullOrWhiteSpace ( substring ) ) {
						text = text.Remove ( firstpos, substring.Length );
						secondpos -= substring.Length;
					}
				}
				firstpos = secondpos + 1;
				secondpos = text.IndexOf ( '\n', firstpos );
			}
			while ( text != "" && text[text.Length - 1] == '\n' ) 
				text = text.Remove ( text.Length - 1 );

			return text;
		}

		private string GetTextWithoutUselessSpaces ( string text ) {
			text = Regex.Replace ( text, @"[^\S\r\n]+", " " );
			int newlineindex = text.IndexOf ( '\n' );
			while ( newlineindex >= 0 ) {
				if ( newlineindex < text.Length - 1 && text[newlineindex+1] == ' ' ) {
					text = text.Remove ( newlineindex + 1, 1 );
				}
				if ( newlineindex > 0 && text[newlineindex-1] == ' ' ) {
					text = text.Remove ( newlineindex - 1, 1 );
					newlineindex--;
				}
				newlineindex = text.IndexOf ( '\n', newlineindex + 1 );
			}
			return text;
		}

		private string GetTextWithoutComment ( string text, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions ) {
			for ( int multiI = multiLineCommentPositions.Count - 1, singleI = singleLineCommentPositions.Count - 1; multiI >= 0 || singleI >= 0; ) {
				if ( singleI < 0 || multiI >= 0 && multiLineCommentPositions[multiI - 1] > singleLineCommentPositions[singleI - 1] ) {
					text = text.Remove ( (int) multiLineCommentPositions[multiI - 1], (int) ( multiLineCommentPositions[multiI] - multiLineCommentPositions[multiI - 1] + 1 ) );
					multiI -= 2;
				} else {
					text = text.Remove ( (int) singleLineCommentPositions[singleI - 1], (int) ( singleLineCommentPositions[singleI] - singleLineCommentPositions[singleI - 1] + 1 ) );
					singleI -= 2;
				}
			}
			return GetTextWithoutUselessLines ( GetTextWithoutUselessSpaces ( text ) );
		}

		private void CountData ( string text, string textwithoutcomments, uint amountcommentchars, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions ) {
			AddToData ( Stats.amountCharacters, (uint) text.Replace ( "\n", "" ).Length - amountcommentchars );
			AddToData ( Stats.amountLines, ( (uint) ( textwithoutcomments != "" ? 1 : 0 ) + (uint) textwithoutcomments.Count ( c => c == '\n' ) ) );
		}
		#endregion

		#region comment
		private void LoadAllCommentStartPositions ( string text, ref List<uint> multiLineCommentPositions, ref List<uint> singleLineCommentPositions ) {
			int index = text.IndexOf ( "--" );
			while ( index != -1 ) {
				if ( text[index + 2] == '[' ) {
					string commentstr = "[";
					int addedint = index + 3;
					while ( text[addedint] == '=' ) {
						addedint++;
						commentstr += text[addedint];
					}
					if ( text[addedint] == '[' ) {
						//commentstr += '[';
						multiLineCommentPositions.Add ( (uint) index );
						index = text.IndexOf ( "--", index + 4 );
						continue;
					}
				}
				singleLineCommentPositions.Add ( (uint) index );
				index = text.IndexOf ( "--", index + 1 );
			}
		}

		private bool IsIndexInComment ( string text, int index, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions ) {
			for ( int i = 0; i < multiLineCommentPositions.Count; i += 2 ) {
				if ( index < multiLineCommentPositions[i] )
					return false;
				else if ( index > multiLineCommentPositions[i] && index < multiLineCommentPositions[i + 1] )
					return true;
			}
			for ( int i = 0; i < singleLineCommentPositions.Count; i += 2 ) {
				if ( index < singleLineCommentPositions[i] )
					return false;
				else if ( index > singleLineCommentPositions[i] && index < singleLineCommentPositions[i + 1] )
					return true;
			}
			return false;
		}

		private uint CountCommentData ( string text, List<uint> multiLineCommentPositions, List<uint> singleLineCommentPositions ) {
			uint amountchars = 0;
			for ( int i = 0; i < multiLineCommentPositions.Count; i += 2 ) {
				string substr = text.Substring ( (int) multiLineCommentPositions[i], (int) ( multiLineCommentPositions[i + 1] - multiLineCommentPositions[i] + 1 ) );
				uint amountnewlines = (uint) ( substr.Count ( c => c == '\n' ) );
				amountchars += (uint) substr.Length - amountnewlines;
				AddToData ( Stats.amountCommentLines, 1 + amountnewlines );
			}
			for ( int i = 0; i < singleLineCommentPositions.Count; i += 2 ) {
				amountchars += singleLineCommentPositions[i + 1] - singleLineCommentPositions[i] + 1;
				AddToData ( Stats.amountCommentLines, 1 );
			}
			AddToData ( Stats.amountCommentCharacters, amountchars );
			return amountchars;
		}
		#endregion

		#region string & comment
		private void LoadAllCommentsAndStrings ( string text, ref List<uint> multiLineCommentPositions, ref List<uint> singleLineCommentPositions, ref List<uint> stringPositions ) {
			int indexSingleStr = text.IndexOf ( '\'' );
			int indexMultiStr = text.IndexOf ( '"' );
			int indexSingleComm = singleLineCommentPositions.Count > 0 ? (int) singleLineCommentPositions[0] : -1;
			int indexMultiComm = multiLineCommentPositions.Count > 0 ? (int) multiLineCommentPositions[0] : -1;
			int posSingleComm = 0;
			int posMultiComm = 0;
			int currentpos = 0;
			char laststrchar = '|';

			while ( currentpos < text.Length && ( indexSingleStr != -1 || indexMultiStr != -1 || indexSingleComm != -1 || indexMultiComm != -1 ) ) {
				// ' is first //
				if ( ( indexSingleStr != -1
						&& laststrchar == '\'' )
						|| ( ( indexSingleStr < indexMultiStr || indexMultiStr == -1 ) 
						&& ( indexSingleStr < indexSingleComm || indexSingleComm == -1 ) 
						&& ( indexSingleStr < indexMultiComm || indexMultiComm == -1 ) ) ) {
					stringPositions.Add ( (uint) indexSingleStr );
					currentpos = indexSingleStr + 1;
					laststrchar = laststrchar == '\'' ? '|' : '\'';
				// " is first //
				} else if ( ( indexMultiStr != -1 
						&& laststrchar == '"' )
						|| ( ( indexMultiStr < indexSingleStr || indexSingleStr == -1 ) 
						&& ( indexMultiStr < indexSingleComm || indexSingleComm == -1 ) 
						&& ( indexMultiStr < indexMultiComm || indexMultiComm == -1 ) ) ) {
					stringPositions.Add ( (uint) indexMultiStr );
					currentpos = indexMultiStr + 1;
					laststrchar = laststrchar == '"' ? '|' : '"';
				// -- is first //
				} else if ( laststrchar == '|' 
						&& indexSingleComm != -1 
						&& ( indexSingleComm < indexSingleStr || indexSingleStr == -1 )
						&& ( indexSingleComm < indexMultiStr || indexMultiStr == -1 ) 
						&& ( indexSingleComm < indexMultiComm || indexMultiComm == -1 ) ) {
					currentpos = text.IndexOf ( "\n", indexSingleComm );
					singleLineCommentPositions.Insert ( posSingleComm + 1, (uint) currentpos - 1 );
					posSingleComm += 2;
				// --[[=...][ is first //
				} else if ( laststrchar == '|' 
						&& indexMultiComm != -1 
						&& ( indexMultiComm < indexSingleStr || indexSingleStr == -1 ) 
						&& ( indexMultiComm < indexMultiStr || indexMultiStr == -1 ) 
						&& ( indexMultiComm < indexSingleComm || indexSingleComm == -1 ) ) {
					int commStartEndPos = text.IndexOf ( '[', text.IndexOf ( '[', indexMultiComm ) + 1 );
					string commStart = text.Substring ( indexMultiComm, commStartEndPos - indexMultiComm + 1 );
					int endindex = text.IndexOf ( commStart.Substring ( 2 ).Replace ( '[', ']' ), indexMultiComm );
					if ( endindex != -1 ) {
						currentpos = endindex + ( commStart.Length - 2 );
						multiLineCommentPositions.Insert ( posMultiComm + 1, (uint) currentpos - 1 );
					} else {
						currentpos = text.Length;
						multiLineCommentPositions.Insert ( posMultiComm + 1, (uint) currentpos - 2 );
					}
					posMultiComm += 2;
				}

				indexSingleStr = text.IndexOf ( '\'', currentpos );
				indexMultiStr = text.IndexOf ( '"', currentpos );
				while ( singleLineCommentPositions.Count > posSingleComm && singleLineCommentPositions[posSingleComm] < currentpos ) {
					singleLineCommentPositions.RemoveAt ( posSingleComm );
				}
				indexSingleComm = singleLineCommentPositions.Count > posSingleComm ? (int) singleLineCommentPositions[posSingleComm] : -1;
				while ( multiLineCommentPositions.Count > posMultiComm && multiLineCommentPositions[posMultiComm] < currentpos ) {
					multiLineCommentPositions.RemoveAt ( posMultiComm );
				}
				indexMultiComm = multiLineCommentPositions.Count > posMultiComm ? (int) multiLineCommentPositions[posMultiComm] : -1;

				if ( laststrchar == '\'' && indexSingleStr == -1 || laststrchar == '"' && indexMultiStr == -1 ) {
					for ( int i = posSingleComm; i < singleLineCommentPositions.Count; i++ ) { 
						if ( singleLineCommentPositions[i] >= currentpos )
							singleLineCommentPositions.RemoveRange ( i, singleLineCommentPositions.Count - i );
					}
					for ( int i = posMultiComm; i < multiLineCommentPositions.Count; i++ ) {
						if ( multiLineCommentPositions[i] >= currentpos )
							multiLineCommentPositions.RemoveRange ( i, multiLineCommentPositions.Count - i );
					}
					return;
				}
			}
		}
		#endregion

		#region variables
		private void LoadLocalVariables ( string text, string textwithoutcomment, ref HashSet<string> localvariables ) {
			int localindex = text.IndexOf ( "local " );
			while ( localindex != -1 ) {
				if ( localindex == 0 || text[localindex-1] == '\n' || text[localindex-1] == ' ' ) {
					int varstartindex = localindex + "local ".Length;
					int nextspaceindex = text.IndexOf ( ' ', varstartindex );
					int nextnextlineindex = text.IndexOf ( '\n', varstartindex );
					string var = "";
					if ( nextspaceindex != -1 && ( nextnextlineindex == -1 || nextspaceindex < nextnextlineindex ) ) {
						var = text.Substring ( varstartindex, nextspaceindex - varstartindex );
					} else if ( nextnextlineindex != -1 && ( nextspaceindex == -1 || nextnextlineindex < nextspaceindex ) ) {
						var = text.Substring ( varstartindex, nextnextlineindex - varstartindex );
					}
					if ( var != "" ) {
						if ( !localvariables.Contains ( var ) ) {
							localvariables.Add ( var );
						}
					}
				}
				localindex = text.IndexOf ( "local ", localindex + 1 );
			}
		}
		#endregion

		#region function

		#endregion

		#region iterate
		private void MainIterate ( IterateTypes type, IEnumerable<string> source, Action<string> action ) {
			if ( type == IterateTypes.fastBlocking )
				Parallel.ForEach ( source, action );
			else if ( type == IterateTypes.slowNotBlocking )
				foreach ( string filepath in source )
					action ( filepath );
		}
		#endregion

		#region UI-data
		private void SetDefaultValues ( ) {
			this.Dispatcher.Invoke ( ( ) => {
				this.amountFiles.Content = "0";
				this.amountLuaFilesLabel.Content = "0";
				this.amountOtherFilesLabel.Content = "0";
				/*this.amountFunctionsLabel.Content = "0";
				this.amountGlobalFunctionsLabel.Content = "0";
				this.amountLocalFunctionsLabel.Content = "0";*/
				this.amountLinesLabel.Content = "0";
				this.amountCharactersLabel.Content = "0";
				this.amountCommentLinesLabel.Content = "0";
				this.amountCommentCharactersLabel.Content = "0";
			} );
		}

		private void AddToData ( Stats stat, uint valuetoadd ) {
			this.Dispatcher.Invoke ( () => {
				switch ( stat ) {
					case Stats.amountLuaFiles:
						this.amountFiles.Content = Convert.ToUInt32 ( this.amountFiles.Content ) + valuetoadd;
						this.amountLuaFilesLabel.Content = Convert.ToUInt32 ( this.amountLuaFilesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountOtherFiles:
						this.amountFiles.Content = Convert.ToUInt32 ( this.amountFiles.Content ) + valuetoadd;
						this.amountOtherFilesLabel.Content = Convert.ToUInt32 ( this.amountOtherFilesLabel.Content ) + valuetoadd;
						break;
					/*case Stats.amountLocalFunctions:
						this.amountFunctionsLabel.Content = Convert.ToUInt32 ( this.amountFunctionsLabel.Content ) + valuetoadd;
						break;
					case Stats.amountGlobalFunctions:
						this.amountFunctionsLabel.Content = Convert.ToUInt32 ( this.amountFunctionsLabel.Content ) + valuetoadd;
						break;*/
					case Stats.amountLines:
						this.amountLinesLabel.Content = Convert.ToUInt32 ( this.amountLinesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountCharacters:
						this.amountCharactersLabel.Content = Convert.ToUInt32 ( this.amountCharactersLabel.Content ) + valuetoadd;
						break;
					case Stats.amountCommentLines:
						this.amountCommentLinesLabel.Content = Convert.ToUInt32 ( this.amountCommentLinesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountCommentCharacters:
						this.amountCommentCharactersLabel.Content = Convert.ToUInt32 ( this.amountCommentCharactersLabel.Content ) + valuetoadd;
						break;
				}
			} );
		}
		#endregion

		#region message
		private void SendErrorMessage ( string message ) {
			MessageBox.Show ( this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
		}
		#endregion
	}
}
