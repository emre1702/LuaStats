using System.Windows;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Windows.Input;
using System.Threading;
using System.Windows.Controls;

namespace MTAResourceStats {

	enum Stats {
		amountLuaFiles, amountOtherFiles, amountFunctions, amountLines, amountCharacters, amountCommentLines, amountCommentCharacters
	}

	enum IterateTypes {
		fastBlocking, slowNotBlocking
	}


	public partial class MainWindow : Window {

		public MainWindow ( ) {
			InitializeComponent ();
		}

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
			string path = folderTextBox.Text;
			if ( Directory.Exists ( path ) ) {
				SetDefaultValues ();
				IterateTypes iteratetype = (ListBoxItem) ( iterateChoice.SelectedItem ) == fastIterateChoice ? IterateTypes.fastBlocking : IterateTypes.slowNotBlocking;
				await Task.Run ( ( ) => {
					this.Dispatcher.InvokeAsync ( ( ) => this.Cursor = Cursors.Wait );
					IEnumerable<string> files = Directory.EnumerateFiles ( path, "*", SearchOption.AllDirectories );
					// iterate files //
					MainIterate ( iteratetype, files, ( string filepath ) => {
						if ( !Path.HasExtension ( filepath ) || Path.GetExtension ( filepath ).ToLower () != ".lua" )
							AddToData ( Stats.amountOtherFiles, 1 );
						else {
							AddToData ( Stats.amountLuaFiles, 1 );
							string status = "normal";
							char laststrchar = '"';
							bool multiplelinescommentused = false;
							string multiplelinescommentstart = "";
							string multiplelinescommentendsofar = "";
							string[] lines = File.ReadAllLines ( filepath );
							// iterate lines //
							foreach ( string line in lines ) {
								if ( !string.IsNullOrWhiteSpace ( line ) ) {
									char charbefore = ' ';
									string functioncheckstr = "";
									bool alreadycountedline = false;
									bool alreadycountedcommentline = false;
									//MessageBox.Show ( status );
									if ( status == "comment" && !multiplelinescommentused )
										status = "normal";
									// iterate chars //
									foreach ( char character in line ) {
										switch ( status ) {
											case "comment":
												if ( !alreadycountedcommentline ) {
													AddToData ( Stats.amountCommentLines, 1 );
													alreadycountedcommentline = true;
												}
												CheckForCharacterAdd ( character, true );
												if ( multiplelinescommentused ) {
													this.CheckMultipleLinesCommentEnd ( character, ref multiplelinescommentstart, ref multiplelinescommentendsofar, ref status );
												}
												break;
											case "string":
												CheckForCharacterAdd ( character );
												this.CheckStringEnd ( character, ref laststrchar, ref status );
												break;
											default:
												this.CheckCommentStart ( character, ref multiplelinescommentstart, ref multiplelinescommentused, ref status, ref alreadycountedcommentline, ref alreadycountedline );
												this.CheckStringStart ( character, ref laststrchar, ref status );
												this.CheckFunctionString ( character, ref functioncheckstr, charbefore );
												if ( status != "comment" && multiplelinescommentstart == "" ) {
													CheckForCharacterAdd ( character );
													if ( !alreadycountedline ) {
														AddToData ( Stats.amountLines, 1 );
														alreadycountedline = true;
													}
												}
												break;
										}
										charbefore = character;


									}
								}
							}
						}
					} );
				} );
				this.Cursor = Cursors.Arrow;
				MessageBox.Show ( this, "finished", "Success", MessageBoxButton.OK, MessageBoxImage.Information );
			} else {
				SendErrorMessage ( "Folder doesn't exist!" );
			}			
		}

		private void MainIterate ( IterateTypes type, IEnumerable<string> source, Action<string> action ) {
			if ( type == IterateTypes.fastBlocking )
				Parallel.ForEach ( source, action );
			else if ( type == IterateTypes.slowNotBlocking )
				foreach ( string filepath in source )
					action ( filepath );
		}

		private void CheckForCharacterAdd ( char character, bool iscomment = false ) {
			if ( !char.IsWhiteSpace ( character ) ) {
				if ( !iscomment )
					AddToData ( Stats.amountCharacters, 1 );
				else
					AddToData ( Stats.amountCommentCharacters, 1 );
			}
 		}

		private void SetDefaultValues ( ) {
			this.Dispatcher.Invoke ( ( ) => {
				this.amountLuaFilesLabel.Content = "0";
				this.amountOtherFilesLabel.Content = "0";
				this.amountFunctionsLabel.Content = "0";
				this.amountLinesLabel.Content = "0";
				this.amountCharactersLabel.Content = "0";
				this.amountCommentLinesLabel.Content = "0";
				this.amountCommentCharactersLabel.Content = "0";
			} );
		}

		private void AddToData ( Stats stat, ulong valuetoadd ) {
			this.Dispatcher.Invoke ( () => {
				switch ( stat ) {
					case Stats.amountLuaFiles:
						this.amountFiles.Content = Convert.ToUInt64 ( this.amountFiles.Content ) + valuetoadd;
						this.amountLuaFilesLabel.Content = Convert.ToUInt64 ( this.amountLuaFilesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountOtherFiles:
						this.amountFiles.Content = Convert.ToUInt64 ( this.amountFiles.Content ) + valuetoadd;
						this.amountOtherFilesLabel.Content = Convert.ToUInt64 ( this.amountOtherFilesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountFunctions:
						this.amountFunctionsLabel.Content = Convert.ToUInt64 ( this.amountFunctionsLabel.Content ) + valuetoadd;
						break;
					case Stats.amountLines:
						this.amountLinesLabel.Content = Convert.ToUInt64 ( this.amountLinesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountCharacters:
						this.amountCharactersLabel.Content = Convert.ToUInt64 ( this.amountCharactersLabel.Content ) + valuetoadd;
						break;
					case Stats.amountCommentLines:
						this.amountCommentLinesLabel.Content = Convert.ToUInt64 ( this.amountCommentLinesLabel.Content ) + valuetoadd;
						break;
					case Stats.amountCommentCharacters:
						this.amountCommentCharactersLabel.Content = Convert.ToUInt64 ( this.amountCommentCharactersLabel.Content ) + valuetoadd;
						break;
				}
			} );
		}
			 
		private void CheckCommentStart ( char character, ref string multiplelinescommentstart, ref bool multiplelinescommentused, ref string status, ref bool alreadycountedcommentline, ref bool alreadycountedline ) {
			if ( multiplelinescommentstart == "" ) {
				if ( character == '-' ) {
					multiplelinescommentstart = "-";
				}
			} else {
				switch ( character ) {
					case '-':
						if ( multiplelinescommentstart == "-" ) {
							multiplelinescommentstart = "--";
						} else if ( multiplelinescommentstart == "--" ) { 
							SetOneLineComment ( ref multiplelinescommentused, ref status, ref alreadycountedcommentline, ref multiplelinescommentstart );
							return;
						}
						break;
					case '[':
						if ( multiplelinescommentstart == "--" ) {
							multiplelinescommentstart = "[";
						} else if ( multiplelinescommentstart == "-" ) {
							multiplelinescommentstart = "";
						} else if ( multiplelinescommentstart.Length > 0 ) {
							if ( multiplelinescommentstart[0] == '[' ) {
								multiplelinescommentstart += character;
								SetMultipleLineComment ( ref multiplelinescommentstart, ref multiplelinescommentused, ref status, ref alreadycountedcommentline );
							}
						}
						break;
					case '=':
						if ( multiplelinescommentstart.Length > 0 ) {
							if ( multiplelinescommentstart[0] == '[' ) {
								multiplelinescommentstart += character;
							} else if ( multiplelinescommentstart == "--" ) {
								SetOneLineComment ( ref multiplelinescommentused, ref status, ref alreadycountedcommentline, ref multiplelinescommentstart );
								return;
							} else if ( multiplelinescommentstart == "-" )
								multiplelinescommentstart = "";
						}
						break;
					default:
						if ( multiplelinescommentstart == "--" ) {
							SetOneLineComment ( ref multiplelinescommentused, ref status, ref alreadycountedcommentline, ref multiplelinescommentstart );
							return;
						} else if ( multiplelinescommentstart == "[" ) {
							AddToData ( Stats.amountCommentCharacters, 1 );
							SetOneLineComment ( ref multiplelinescommentused, ref status, ref alreadycountedcommentline, ref multiplelinescommentstart );
							return;
						} else if ( multiplelinescommentstart == "-" )
							multiplelinescommentstart = "";
						break;
				}
				// because the first "-" didn't get counted
				if ( multiplelinescommentstart == "" ) {
					AddToData ( Stats.amountCharacters, 1 );
					if ( !alreadycountedline ) {
						AddToData ( Stats.amountLines, 1 );
						alreadycountedline = true;
					}
				}
			}
		}

		private void SetOneLineComment ( ref bool multiplelinescommentused, ref string status, ref bool alreadycountedcommentline, ref string multiplelinescommentstart ) {
			status = "comment";
			AddToData ( Stats.amountCommentLines, 1 );
			AddToData ( Stats.amountCommentCharacters, 2 );
			alreadycountedcommentline = true;
			multiplelinescommentused = false;
			multiplelinescommentstart = "";
		}

		private void SetMultipleLineComment ( ref string multiplelinescommentstart, ref bool multiplelinescommentused, ref string status, ref bool alreadycountedcommentline ) {
			status = "comment";
			AddToData ( Stats.amountCommentLines, 1 );
			AddToData ( Stats.amountCommentCharacters, (ulong) ( multiplelinescommentstart.Length + 2 ) );
			alreadycountedcommentline = true;
			multiplelinescommentused = true;
			
		}

		private void CheckMultipleLinesCommentEnd ( char character, ref string multiplelinescommentstart, ref string multiplelinescommentendsofar, ref string status ) {
			if ( multiplelinescommentendsofar == "" ) {
				if ( character == ']' )
					multiplelinescommentendsofar = "]";
			} else {
				int commentpos = multiplelinescommentendsofar.Length;
				string checkstr = multiplelinescommentstart.Replace ( '[', ']' );
				if ( checkstr[commentpos] == character ) {
					multiplelinescommentendsofar += character;
					// comment closed //
					if ( checkstr == multiplelinescommentendsofar ) {
						status = "normal";
						multiplelinescommentstart = "";
						multiplelinescommentendsofar = "";
					}
				}
			}
		}

		private void CheckStringStart ( char character, ref char laststrchar, ref string status ) {
			if ( status == "normal" ) {
				if ( character == '"' || character == '\'' ) {
					status = "string";
					laststrchar = character;
				}
			}
		}

		private void CheckStringEnd ( char character, ref char laststrchar, ref string status ) {
			if ( character == '"' || character == '\'' ) {
				if ( character == laststrchar ) {
					status = "normal";
				}
			}
		}

		private void CheckFunctionString ( char character, ref string functioncheckstr, char charbefore ) {
			// function ( | function( | function NAME //
			if ( functioncheckstr == "function" && ( char.IsWhiteSpace ( character ) || character == '(' ) ) {
				AddToData ( Stats.amountFunctions, 1 );
				functioncheckstr = "";
			// function | _function //
			} else if ( character == 'f' && charbefore == ' ' ) {
				functioncheckstr = "f";
			// function | local function | _function //
			} else if ( charbefore != ' ' && "function".StartsWith ( functioncheckstr + character ) ) {
				functioncheckstr += character;
			} else
				functioncheckstr = "";
		}

		private void SendErrorMessage ( string message ) {
			MessageBox.Show ( this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
		}
	}
}
