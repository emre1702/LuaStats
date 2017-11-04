using System;
using System.Windows;
using MTAResourceStats.enums;

namespace MTAResourceStats.gui {
	partial class MainWindow : Window {

		private void SetDefaultValues ( ) {
			this.Dispatcher.InvokeAsync ( ( ) => {
				this.amountFiles.Content = "0";
				this.amountLuaFilesLabel.Content = "0";
				this.amountOtherFilesLabel.Content = "0";
				this.amountFunctionsLabel.Content = "0";
				//this.amountGlobalFunctionsLabel.Content = "0";
				//this.amountLocalFunctionsLabel.Content = "0";
				this.amountLinesLabel.Content = "0";
				this.amountCharactersLabel.Content = "0";
				this.amountCommentLinesLabel.Content = "0";
				this.amountCommentCharactersLabel.Content = "0";
			} );
		}

		public void AddToData ( Stat stat, uint valuetoadd ) {
			this.Dispatcher.InvokeAsync ( ( ) => {
				switch ( stat ) {
					case Stat.amountLuaFiles:
						this.amountFiles.Content = Convert.ToUInt32 ( this.amountFiles.Content ) + valuetoadd;
						this.amountLuaFilesLabel.Content = Convert.ToUInt32 ( this.amountLuaFilesLabel.Content ) + valuetoadd;
						break;
					case Stat.amountOtherFiles:
						this.amountFiles.Content = Convert.ToUInt32 ( this.amountFiles.Content ) + valuetoadd;
						this.amountOtherFilesLabel.Content = Convert.ToUInt32 ( this.amountOtherFilesLabel.Content ) + valuetoadd;
						break;
					/*case Stats.amountLocalFunctions:
						this.amountFunctionsLabel.Content = Convert.ToUInt32 ( this.amountFunctionsLabel.Content ) + valuetoadd;
						break;*/
					case Stat.amountGlobalFunctions:
						this.amountFunctionsLabel.Content = Convert.ToUInt32 ( this.amountFunctionsLabel.Content ) + valuetoadd;
						break;
					case Stat.amountLines:
						this.amountLinesLabel.Content = Convert.ToUInt32 ( this.amountLinesLabel.Content ) + valuetoadd;
						break;
					case Stat.amountCharacters:
						this.amountCharactersLabel.Content = Convert.ToUInt32 ( this.amountCharactersLabel.Content ) + valuetoadd;
						break;
					case Stat.amountCommentLines:
						this.amountCommentLinesLabel.Content = Convert.ToUInt32 ( this.amountCommentLinesLabel.Content ) + valuetoadd;
						break;
					case Stat.amountCommentCharacters:
						this.amountCommentCharactersLabel.Content = Convert.ToUInt32 ( this.amountCommentCharactersLabel.Content ) + valuetoadd;
						break;
				}
			} );
		}
	}
}
