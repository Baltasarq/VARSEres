namespace VARSEres.Ui {
    using System;
    using System.Windows.Forms;

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
    }
}
