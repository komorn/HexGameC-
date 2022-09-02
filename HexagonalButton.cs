using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace HexGame
{
    class HexagonalButton : Button
    {
        protected override void OnPaint(PaintEventArgs pevent)
         {
            // Create an array of points.
            Point[] myArray =
                     {
                new Point(Convert.ToInt32((33.3/Convert.ToDouble(100)*Convert.ToDouble(ClientSize.Width))),  0),
                new Point(0,ClientSize.Height/2),
                new Point( Convert.ToInt32((33.3/Convert.ToDouble(100)*Convert.ToDouble(ClientSize.Width))),ClientSize.Height),
                new Point(Convert.ToInt32((66.6/Convert.ToDouble(100)*Convert.ToDouble(ClientSize.Width))), ClientSize.Height),  // Half
                new Point(ClientSize.Width, ClientSize.Height/2),
                new Point(Convert.ToInt32((66.6/Convert.ToDouble(100)*Convert.ToDouble(ClientSize.Width))),0),
                new Point(Convert.ToInt32((33.3/Convert.ToDouble(100)*Convert.ToDouble(ClientSize.Width))), 0)
             };

            // Create a GraphicsPath object and add a polygon.
            GraphicsPath myPath = new GraphicsPath();
            myPath.AddPolygon(myArray);

            this.Region = new System.Drawing.Region(myPath);
            base.OnPaint(pevent);
        }
    }
}
