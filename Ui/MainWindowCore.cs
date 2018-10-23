// VARSEres (c) 2018 MIT License <baltasarq@gmail.com>

namespace VARSEres.Ui {
    using System;
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
			using(OpenFileDialog openDlg = new OpenFileDialog())
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
            int numTags = 0;
            int numRR = 0;
            long accTime = 0;
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
            if ( this.Result != null ) {
                foreach(Result.Event evt in this.Result.Events) {
                    lbAll.Items.Add( evt.ToString() );
                    
                    if ( evt.Type == Result.Event.EventType.Tag ) {
                        lbTags.Items.Add( evt.ToString() );
                        ++numTags;
                    }
                    else
                    if ( evt.Type == Result.Event.EventType.Beat ) {
                        var beatEvt = (Result.BeatEvent) evt;
                    
                        lbRR.Items.Add( beatEvt.ToString() );
                        ++numRR;

                        accTime += beatEvt.Value;
                        maxRR = Math.Max( maxRR, beatEvt.Value );
                        minRR = Math.Min( minRR, beatEvt.Value );
                    } else {
                        MessageBox.Show( "Event type is not supported" );
                    }
                }
            }
            
            // End
            tbSummary.Text = this.BuildReport( numTags, numRR, accTime, maxRR, minRR );
            this.DrawChart();

            // Restore all
            lbAll.Show();
            lbTags.Show();
            lbRR.Show();
		}
        
        string BuildReport(int numTags, int numRR, long accTime, long maxRR, long minRR)
        {
            var report = @"
Id:                  ${id}
Id experiment:       ${experimentid}
Id record:           ${usrid}      
            
Number of tags:      ${numtags}
Number of beats:     ${numbeats}

Reported total time: ${time} ms
Accumulated RR time: ${acctime} ms
Max RR:              ${maxrr} ms
Min RR:              ${minrr} ms
Avg RR:              ${avgrr} ms";

            var strId = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                this.Result.Id.Value );

            var strExperimentId = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                this.Result.ExperimentId.Value );

            var strUsrId = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                this.Result.UsrId.Value );

            var strAccTime = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                accTime );
                                
            var strTime = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                this.Result.Time );
                                
            var strNumTags = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                numTags );
                                
            var strNumRR = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                numRR );
                                
            var strMinRR = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                minRR );
                                
            var strMaxRR = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                maxRR );
                                
            var strAvgRR = string.Format(
                                CultureInfo.CurrentCulture,
                                "{0:d7}",
                                (int) Math.Round( ( (double) maxRR + minRR ) / 2 ) );
                                
            return report.Replace( "${id}", strId )
                                .Replace( "${experimentid}", strExperimentId )
                                .Replace( "${usrid}", strUsrId )
                                .Replace( "${numtags}", strNumTags )
                                .Replace( "${numbeats}", strNumRR )
                                .Replace( "${time}", strTime )
                                .Replace( "${acctime}", strAccTime )
                                .Replace( "${maxrr}", strMaxRR )
                                .Replace( "${minrr}", strMinRR )
                                .Replace( "${avgrr}", strAvgRR );
        }
        
        void DrawChart()
        {
            Chart chart = this.MainWindowView.Chart;
            var hrs = new List<int>();
            
            // Calculate heart beats
            foreach(Result.Event evt in this.Result.Events) {
                if ( evt is Result.BeatEvent beatEvt ) {
                    hrs.Add( (int) ( ( (double) 60 / beatEvt.Value ) * 1000 ) );
                }
            }
            
            // Finish
            chart.LegendY = "Heart beats (bpm)";
            chart.LegendX = "Time (seconds)";
            chart.Values = hrs;
            chart.ShowLabels = false;
            chart.DataPen = new Pen( Color.Red ) { Width = 1 };
            chart.AxisPen = new Pen( Color.Black ) { Width = 2 };
            chart.BackColor = Color.White;
            chart.Draw();
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
