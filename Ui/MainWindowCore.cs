namespace VARSEres.Ui {
    using System;
    using System.Windows.Forms;

	using Core;

    /// <summary>
    /// Main window's backbone.
    /// </summary>
    public class MainWindowCore {
        public MainWindowCore()
        {
            this.MainWindowView = new MainWindowView();

            // Events
            this.MainWindowView.TbBar.ButtonClick +=
                    (object sender, ToolBarButtonClickEventArgs e)
                        => this.OnToolbarButtonClicked( e.Button );

            this.MainWindowView.FormClosed += (sender, e) => this.OnQuit();
        }

        /// <summary>Manages the clicks in the toolbar</summary>
        void OnToolbarButtonClicked(ToolBarButton tbButton)
        {
            switch ( this.MainWindowView.TbBar.Buttons.IndexOf( tbButton ) ) {
                case 0:
                    this.OnOpen();
                    break;
                case 1:
                    this.OnSave();
                    break;
                case 2:
                    this.OnAbout();
                    break;
                default:
                    throw new ArgumentException( "toolbar button not found" );
            }

            return;
        }

        /// <summary>Opening a results file.</summary>
        void OnOpen()
        {
			using(OpenFileDialog openDlg = new OpenFileDialog())
            {
                openDlg.InitialDirectory = ".";
                openDlg.Filter = "res files (*.res)|*.res|All files (*.*)|*.*";
                openDlg.FilterIndex = 2;
                openDlg.RestoreDirectory = true;

                if ( openDlg.ShowDialog() == DialogResult.OK ) {
					try {
						this.Result = Persistence.Load(openDlg.FileName);                  
					} catch(ArgumentException exc) {
						MessageBox.Show( exc.Message, I18n.Get( I18n.Id.Loading ) );
					}
                }
            }

			this.UpdateResult();
			return;
        }

        void UpdateResult()
		{
			foreach(Core.Result.Event evt in this.Result.Events) {
				this.MainWindowView.TxtTags.Text = evt.ToString();
			}
		}

        /// <summary>Saving the results and text files.</summary>
        void OnSave()
        {
            
        }

        /// <summary>Shows the about panel.</summary>
        void OnAbout()
        {
            this.MainWindowView.PnlAbout.Show();
        }

        /// <summary>Quits de application.</summary>
        void OnQuit()
        {
            Application.Exit();
        }

        /// <summary>
        /// Gets the main window view.
        /// </summary>
        /// <value>The main <see cref="MainWindowView"/>.</value>
        public MainWindowView MainWindowView {
            get; private set;
        }

        /// <summary>
        /// Gets the result being managed.
        /// </summary>
		/// <value>The <see cref="Result"/> object.</value>
		public Result Result {
			get; private set;
		}
    }
}
