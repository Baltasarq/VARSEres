﻿// VARSEres (c) 2018 MIT License <baltasarq@gmail.com>

namespace VARSEres.Ui {
    using System.Linq;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Collections.Generic;

    /// <summary>
    /// Draws a simple chart with the RR.
    /// </summary>
    public class Chart: PictureBox {
        public enum ChartType { Lines, Bars };
        
        public Chart(int width, int height)
        {
            this.values = new List<int>();
            this.Width = width;
            this.Height = height;
            this.FrameWidth = 50;
            this.Type = ChartType.Lines;
            this.AxisPen = new Pen( Color.Black ) { Width = 10 };
            this.DataPen = new Pen( Color.Red ) { Width = 4 };
            this.DataFont = new Font( FontFamily.GenericMonospace, 12 );
            this.LegendFont = new Font( FontFamily.GenericSansSerif, 12 );
            this.LegendPen = new Pen( Color.Navy );
        
            this.Build();
        }
        
        /// <summary>
        /// Redraws the chart
        /// </summary>
        public void Draw()
        {
            this.grf.Clear( this.BackColor );
            this.DrawAxis();
            this.DrawData();
            this.DrawLegends();
        }
        
        void DrawLegends()
        {
            StringFormat verticalDrawFmt = new StringFormat {
                FormatFlags = StringFormatFlags.DirectionVertical
            };
            int legendXWidth = (int) this.grf.MeasureString(
                                                        this.LegendX,
                                                        this.LegendFont ).Width;
            int legendYHeight = (int) this.grf.MeasureString(
                                                        this.LegendY,
                                                        this.LegendFont,
                                                        new Size( this.Width,
                                                                  this.Height ),
                                                        verticalDrawFmt ).Height;

            this.grf.DrawString(
                    this.LegendX,
                    this.LegendFont,
                    this.LegendPen.Brush,
                    new Point( 
                        ( this.Width - legendXWidth ) / 2,
                        this.FramedEndPosition.Y + 5 ) );

            this.grf.DrawString(
                    this.LegendY,
                    this.LegendFont,
                    this.LegendPen.Brush,
                    new Point( 
                        this.FramedOrgPosition.X - ( this.FrameWidth / 2 ),
                        ( this.Height - legendYHeight ) / 2 ),
                    verticalDrawFmt );
        } 
        
        void DrawData()
        {
            int numValues = this.values.Count;
            var p = this.DataOrgPosition;
            int xGap = this.GraphWidth / ( numValues + 1 );
            int baseLine = this.DataOrgPosition.Y;

            if ( numValues > 0 ) {
	            this.NormalizeData();
                
                // First point
                p = new Point( p.X, baseLine - this.normalizedData[ 0 ] );

                // Plot graph
	            for(int i = 1; i < numValues; ++i) {	                
	                var nextPoint = new Point(
	                    p.X + xGap, baseLine - this.normalizedData[ i ]
	                );
	                
	                if ( this.Type == ChartType.Bars ) {
	                    p = new Point( nextPoint.X, baseLine );
	                }
	                
	                this.grf.DrawLine( this.DataPen, p, nextPoint );
	                
	                if ( this.ShowLabels ) {
                        string label = this.values[ i ].ToString();
                        int tagWidth = (int) this.grf.MeasureString(
                                                            label,
                                                            this.DataFont ).Width;
		                this.grf.DrawString( label,
		                                     this.DataFont,
		                                     this.DataPen.Brush,
		                                     new Point( nextPoint.X - tagWidth,
		                                                nextPoint.Y ) );
	                }
	                
	                p = nextPoint;
	            }
            }
            
            return;
        }
        
        void DrawAxis()
        {
            // Y axis
            this.grf.DrawLine( this.AxisPen,
                               this.FramedOrgPosition,
                               new Point(
                                        this.FramedOrgPosition.X,
                                        this.FramedEndPosition.Y ) );
                                        
            // X axis
            this.grf.DrawLine( this.AxisPen,
                               new Point(
                                        this.FramedOrgPosition.X,
                                        this.FramedEndPosition.Y ),
                               this.FramedEndPosition );
        }

        void Build()
        {
            Bitmap bmp = new Bitmap( this.Width, this.Height );
            this.Image = bmp;
            this.grf = Graphics.FromImage( bmp );
        }
        
        void NormalizeData()
        {
            int maxHeight = this.DataOrgPosition.Y - this.FrameWidth;
            int maxValue = this.values.Max();

            this.normalizedData = this.values.ToArray();

            for(int i = 0; i < this.normalizedData.Length; ++i) {
                this.normalizedData[ i ] =
                                    ( this.values[ i ] * maxHeight ) / maxValue;
            }
            
            return;
        }
        
        /// <summary>
        /// Gets or sets the values used as data.
        /// </summary>
        /// <value>The values.</value>
        public IEnumerable<int> Values {
            get {
                return this.values.ToArray();
            }
            set {
                this.values.Clear();
                this.values.AddRange( value );
            }
        }
        
        /// <summary>
        /// Gets the framed origin.
        /// </summary>
        /// <value>The origin <see cref="Point"/>.</value>
        public Point DataOrgPosition {
            get {
                int margin = (int) ( this.AxisPen.Width * 2 );
                
                return new Point(
                    this.FramedOrgPosition.X + margin,
                    this.FramedEndPosition.Y - margin );
            }
        }
        
        /// <summary>
        /// Gets or sets the width of the frame around the chart.
        /// </summary>
        /// <value>The width of the frame.</value>
        public int FrameWidth {
            get; set;
        }
        
        /// <summary>
        /// Gets the framed origin.
        /// </summary>
        /// <value>The origin <see cref="Point"/>.</value>
        public Point FramedOrgPosition {
            get {
                return new Point( this.FrameWidth, this.FrameWidth );
            }
        }
        
        /// <summary>
        /// Gets the framed end.
        /// </summary>
        /// <value>The end <see cref="Point"/>.</value>
        public Point FramedEndPosition {
            get {
                return new Point( this.Width - this.FrameWidth,
                                  this.Height - this.FrameWidth );
            }
        }
        
        /// <summary>
        /// Gets the width of the graph.
        /// </summary>
        /// <value>The width of the graph.</value>
        public int GraphWidth {
            get {
                return this.Width - ( this.FrameWidth * 2 );
            }
        }
        
        /// <summary>
        /// Gets or sets the pen used to draw the axis.
        /// </summary>
        /// <value>The axis <see cref="Pen"/>.</value>
        public Pen AxisPen {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the pen used to draw the data.
        /// </summary>
        /// <value>The data <see cref="Pen"/>.</value>
        public Pen DataPen {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the data font.
        /// </summary>
        /// <value>The data <see cref="Font"/>.</value>
        public Font DataFont {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the legend for the x axis.
        /// </summary>
        /// <value>The legend for axis x.</value>
        public string LegendX {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the legend for the y axis.
        /// </summary>
        /// <value>The legend for axis y.</value>
        public string LegendY {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the font for legends.
        /// </summary>
        /// <value>The <see cref="Font"/> for legends.</value>
        public Font LegendFont {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the pen for legends.
        /// </summary>
        /// <value>The <see cref="Pen"/> for legends.</value>
        public Pen LegendPen {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets the type of the chart.
        /// </summary>
        /// <value>The <see cref="ChartType"/>.</value>
        public ChartType Type {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:VARSEres.Ui.Chart"/> shows labels.
        /// </summary>
        /// <value><c>true</c> if it should show labels; otherwise, <c>false</c>.</value>
        public bool ShowLabels {
            get; set;
        }

        Graphics grf;
        List<int> values;
        int[] normalizedData;
    }
}
