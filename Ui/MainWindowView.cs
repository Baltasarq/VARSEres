namespace VARSEres.Ui
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms;

    using Core;

    /// <summary>
    /// The main window view.
    /// </summary>
    public class MainWindowView: Form {
        enum Sheets { Summary, Tags, RR, All };

        /// <summary>
        /// Initializes a new instance of the <see cref="T:VARSEres.Ui.MainWindowView"/> class.
        /// </summary>
        public MainWindowView()
        {
            this.Build();
            this.Show();
        }

        void Build()
        {
            this.BaseFont = new Font(   SystemFonts.DefaultFont.FontFamily,
                                        12,
                                        FontStyle.Regular );

            this.StartPosition = FormStartPosition.CenterScreen;

            // Components
            this.BuildIcons();
            this.Notebook = this.BuildNotebook();
            this.PnlAbout = this.BuildAboutPanel();
            this.TbBar = this.BuildToolbar();

            // Layout
            this.Controls.Add( this.Notebook );
            this.Controls.Add( this.TbBar );
            this.Controls.Add( this.PnlAbout );

            this.Icon = Icon.FromHandle( this.AppIcon.GetHicon() );
            this.Text = AppInfo.CompleteName;
            this.MinimumSize = new Size( 600, 400 );
        }

        TabControl BuildNotebook()
        {
            var toret = new TabControl { Dock = DockStyle.Fill };
            var font = new Font( FontFamily.GenericMonospace, this.BaseFont.Size + 2 );

            toret.TabPages.Add( I18n.Get( I18n.Id.Summary ) );
            toret.TabPages.Add( I18n.Get( I18n.Id.Tag ) );
            toret.TabPages.Add( I18n.Get( I18n.Id.RR ) );
            toret.TabPages.Add( I18n.Get( I18n.Id.All ) );

            this.LbAll = new ListBox {
                Dock = DockStyle.Fill,
                Font = font
            };

            this.LbTags = new ListBox {
                Dock = DockStyle.Fill,
                Font = font
            };

            this.LbRR = new ListBox {
                Dock = DockStyle.Fill,
                Font = font
            };

            this.TbSummary = new TextBox {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                Font = font
            };

            toret.TabPages[ (int) Sheets.Summary ].Controls.Add( this.TbSummary );
            toret.TabPages[ (int) Sheets.Tags ].Controls.Add( this.LbTags );
            toret.TabPages[ (int) Sheets.RR ].Controls.Add( this.LbRR );
            toret.TabPages[ (int) Sheets.All ].Controls.Add( this.LbAll );

            return toret;
        }

        private Panel BuildAboutPanel()
        {
            // Sizes
            Graphics grf = this.CreateGraphics();
            SizeF fontSize = grf.MeasureString( "M", this.BaseFont );
            int charSize = (int) fontSize.Width + 5;

            // Panel for about info
            var toret = new Panel() {
                Dock = DockStyle.Bottom,
                BackColor = Color.LightYellow,
                ForeColor = Color.Black
            };

            toret.SuspendLayout();

            this.LblAbout = new Label {
                Text = AppInfo.CompleteName + ", " + AppInfo.Author,
                Dock = DockStyle.Left,
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Font = new Font( this.BaseFont, FontStyle.Bold )
            };

            var btCloseAboutPanel = new Button() {
                Text = "X",
                Dock = DockStyle.Right,
                Width = charSize * 5,
                FlatStyle = FlatStyle.Flat,
                Font = new Font( this.BaseFont, FontStyle.Bold )
            };

            btCloseAboutPanel.FlatAppearance.BorderSize = 0;
            btCloseAboutPanel.Click += (o, evt) => toret.Hide();
            toret.Controls.Add( LblAbout );
            toret.Controls.Add( btCloseAboutPanel );
            toret.Hide();
            toret.MinimumSize = new Size( this.Width, this.LblAbout.Height + 5 );
            toret.MaximumSize = new Size( Int32.MaxValue, this.LblAbout.Height + 5 );

            toret.ResumeLayout( false );

            return toret;
        }

        Bitmap CreateIcon(char c, int width, int height)
        {
            var msg = Convert.ToString( c );
            var toret = new Bitmap( width, height );
            var graphics = Graphics.FromImage( toret );
            var font = new Font( this.BaseFont.FontFamily, 8, FontStyle.Bold );
            var brush = new SolidBrush( Color.White );

            graphics.Clear( Color.Navy );
            graphics.DrawString( msg, font, brush, 0, 0 );

            return toret;
        }

        void BuildIcons()
        {
            try {
                this.AppIcon = new Bitmap(
                    System.Reflection.Assembly.GetEntryAssembly( ).
                        GetManifestResourceStream( "VARSEres.Res.app_icon.png" ) );

                this.OpenIcon = new Bitmap(
                    System.Reflection.Assembly.GetEntryAssembly( ).
                        GetManifestResourceStream( "VARSEres.Res.open_icon.png" ) );

                this.SaveIcon = new Bitmap(
                    System.Reflection.Assembly.GetEntryAssembly( ).
                        GetManifestResourceStream( "VARSEres.Res.save_icon.png" ) );

                this.AboutIcon = new Bitmap(
                    System.Reflection.Assembly.GetEntryAssembly( ).
                        GetManifestResourceStream( "VARSEres.Res.about_icon.png" ) );

            } catch(Exception e)
            {
                Debug.WriteLine( "ERROR loading icons: " + e.Message );

                if ( this.AppIcon == null ) {
                    this.AppIcon = this.CreateIcon( 'V', 32, 32 );
                    Debug.WriteLine( "App icon created instead of loaded" );
                }

                if ( this.OpenIcon == null ) {
                    this.OpenIcon = this.CreateIcon( 'O', 32, 32 );
                    Debug.WriteLine( "Open icon created instead of loaded" );
                }

                if ( this.SaveIcon == null ) {
                    this.SaveIcon = this.CreateIcon( 'S', 32, 32 );
                    Debug.WriteLine( "Save icon created instead of loaded" );
                }

                if ( this.AboutIcon == null ) {
                    this.AboutIcon = this.CreateIcon( '?', 32, 32 );
                    Debug.WriteLine( "About icon created instead of loaded" );
                }
            }

            return;
        }

        ToolBar BuildToolbar()
        {
            var toret = new ToolBar();

            // Create image list
            var imgList = new ImageList{ ImageSize = new Size( 24, 24 ) };
            imgList.Images.AddRange( new Image[] {
                this.OpenIcon,
                this.SaveIcon,
                this.AboutIcon,
            });

            // Buttons
            this.TbbOpen = new ToolBarButton        { ImageIndex = 0 };
            this.TbbSave = new ToolBarButton        { ImageIndex = 1 };
            this.TbbAbout = new ToolBarButton       { ImageIndex = 2 };

            // Polishing
            toret.ShowToolTips = true;
            toret.ImageList = imgList;
            toret.Dock = DockStyle.Top;
            toret.BorderStyle = BorderStyle.None;
            toret.Appearance = ToolBarAppearance.Flat;
            toret.Buttons.AddRange( new ToolBarButton[] {
                this.TbbOpen, this.TbbSave, this.TbbAbout
            });

            return toret;
        }

        /// <summary>
        /// Gets the notebook of the main window.
        /// </summary>
        /// <value>The notebook, as a <see cref="TabControl"/>.</value>
        public TabControl Notebook {
            get; private set;
        }

        public Panel PnlAbout {
            get; private set;
        }

        public Font BaseFont {
            get; private set;
        }

        public Label LblAbout {
            get; private set;
        }

        public Bitmap AppIcon {
            get; private set;
        }

        public Bitmap OpenIcon {
            get; private set;
        }

        public Bitmap SaveIcon {
            get; private set;
        }

        public Bitmap AboutIcon {
            get; private set;
        }

        public ToolBar TbBar {
            get; private set;
        }

        public ToolBarButton TbbOpen {
            get; private set;
        }

        public ToolBarButton TbbSave {
            get; private set;
        }

        public ToolBarButton TbbAbout {
            get; private set;
        }

        public ListBox LbRR {
            get; private set;
        }

        public ListBox LbTags {
            get; private set;
        }

        public TextBox TbSummary {
            get; private set;
        }
        
        public ListBox LbAll {
            get; private set;
        }
    }
}
