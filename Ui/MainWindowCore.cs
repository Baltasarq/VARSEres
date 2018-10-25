// VARSEres (c) 2018 MIT License <baltasarq@gmail.com>

namespace VARSEres.Ui {
    using System;
    using System.IO;
    using System.Drawing;
    using System.Globalization;
    using System.Collections.Generic;
    using System.Windows.Forms;

	using Core;

    /// <summary>
    /// Main window's backbone.
    /// </summary>
    public class MainWindowCore {
        public MainWindowCore()
        {
            this.MainWindowView = new MainWindowView();
            I18n.Language = CultureInfo.CurrentCulture;

            // Events
            this.MainWindowView.TbBar.ButtonClick +=
                    (object sender, ToolBarButtonClickEventArgs e)
                        => this.OnToolbarButtonClicked( e.Button );

            this.MainWindowView.FormClosed += (sender, e) => this.OnQuit();
            this.MainWindowView.ResizeEnd += (sender, e) => this.OnResize();
        }
        
        void OnResize()
        {
            this.MainWindowView.Chart.Size = this.MainWindowView.ClientSize;
            this.MainWindowView.Chart.Draw();
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
			using(var openDlg = new OpenFileDialog())
            {
                openDlg.InitialDirectory = ".";
                openDlg.Filter = "res files (*.res)|*.res|All files (*.*)|*.*";
                openDlg.FilterIndex = 1;
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

        /// <summary>Updates the view of the result.</summary>
        void UpdateResult()
		{
            if ( this.Result != null ) {
                int numTags = 0;
                int numRR = 0;
                long maxRR = long.MinValue;
                long minRR = long.MaxValue;
                ListBox lbAll = this.MainWindowView.LbAll;
                ListBox lbTags = this.MainWindowView.LbTags;
                ListBox lbRR = this.MainWindowView.LbRR;
                TextBox tbSummary = this.MainWindowView.TbSummary;

                // Hide all
                lbAll.Hide();
                lbAll.Items.Clear();
                
                lbTags.Hide();
                lbTags.Items.Clear();
                
                lbRR.Hide();
                lbRR.Items.Clear();

                // Run over all events and clasify
                this.AccTime = 0;
                if ( this.Result != null ) {
                    foreach(Result.Event evt in this.Result.Events) {
                        lbAll.Items.Add( evt.ToString() );
                    }


                    foreach(Result.BeatEvent beatEvt in this.Result.Beats) {
                        lbRR.Items.Add( beatEvt.ToString() );
                        ++numRR;

                        this.AccTime += beatEvt.Value;
                        maxRR = Math.Max( maxRR, beatEvt.Value );
                        minRR = Math.Min( minRR, beatEvt.Value );
                    }

                    foreach(Result.TagEvent evt in this.Result.Tags) {                    
                        lbTags.Items.Add( evt.ToString() );
                        ++numTags;
                    }
                }
                
                // End
                tbSummary.Text = this.BuildReport( numTags, numRR, maxRR, minRR );
                this.DrawChart();

                // Restore all
                lbAll.Show();
                lbTags.Show();
                lbRR.Show();
            }

            return;
		}
        
        string BuildReport(int numTags, int numRR, long maxRR, long minRR)
        {
            var reportTemplate = @"
Id:                  {0:d7}
Id experiment:       {1:d7}
Id record:           {2:d7}      
            
Number of tags:      {3:d7}
Number of beats:     {4:d7}

Reported total time: {5:d7} ms
Accumulated RR time: {6:d7} ms
Max RR:              {7:d7} ms
Min RR:              {8:d7} ms
Avg RR:              {9:d7} ms";

            return string.Format( CultureInfo.CurrentCulture,
                                 reportTemplate,
                                 this.Result.Id.Value,
                                 this.Result.ExperimentId.Value,
                                 this.Result.UsrId.Value,
                                 numTags,
                                 numRR,
                                 this.Result.Time,
                                 this.AccTime,
                                 maxRR,
                                 minRR,
                                 (int) Math.Round( ( (double) maxRR + minRR ) / 2 )
                                );
        }
        
        void DrawChart()
        {
            Chart chart = this.MainWindowView.Chart;
            var hrs = new List<int>();
            var ts = new List<int>();
            long accTime = 0;
            
            // Calculate heart beats
            foreach(Result.Event evt in this.Result.Events) {
                if ( evt is Result.BeatEvent beatEvt ) {
                    long rr = beatEvt.Value;

                    hrs.Add( (int) ( (double) 60000 / rr ) );
                    ts.Add( (int) accTime );
                    accTime += rr;
                }
            }
            
            // Finish
            chart.LegendY = "Heart beats (bpm)";
            chart.LegendX = "Time (ms)";
            chart.ValuesY = hrs;
            chart.ValuesX = ts;
            chart.ShowLabels = false;
            chart.DataPen = new Pen( Color.Red ) { Width = 1 };
            chart.AxisPen = new Pen( Color.Black ) { Width = 2 };
            chart.BackColor = Color.White;
            chart.Draw();
        }
        
        /// <summary>Saving the results and text files.</summary>
        void OnSave()
        {
            if ( this.Result != null ) {
                using(var dirDlg = new FolderBrowserDialog())
                {
                    dirDlg.ShowNewFolderButton = true;

                    if ( dirDlg.ShowDialog() == DialogResult.OK ) {
                        try {
                            string dirPath = dirDlg.SelectedPath;
                            string baseFile = "result_" + this.Result.Id;
                            string tagsFile = Path.Combine( dirPath, baseFile + "_tags.txt" );
                            string beatsFile = Path.Combine( dirPath, baseFile + "_rr.txt" );

                            using (var tagsWriter = new StreamWriter( tagsFile )) {
                                using (var beatsWriter = new StreamWriter( beatsFile ))
                                {
                                    Persistence.ExportToStdTextFormat(
                                                                    this.Result,
                                                                    tagsWriter,
                                                                    beatsWriter );
                                }
                            }
                        } catch(ArgumentException exc) {
                            MessageBox.Show( exc.Message, I18n.Get( I18n.Id.Loading ) );
                        }
                    }
                }
            } else {
                MessageBox.Show( I18n.Get( I18n.Id.NoResult ) );
            }

            return;
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

        /// <summary>
        /// Gets the accumulated time (by calculating it).
        /// </summary>
        /// <value>The accumulated time, as a long.</value>
        public long AccTime {
            get; private set;
        }
    }
}
