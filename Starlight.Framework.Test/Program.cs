using System;
using System.Threading;
using Starlight.Framework;
using Starlight.Framework.Graphics;

var modules = LedDisplay.Enumerate();
var left = modules[0];
var right = modules[1];

// left.SetGlobalDcScale(80);
// right.SetGlobalDcScale(80);

left.SetGlobalBrightness(128);
right.SetGlobalBrightness(128);
left.Clear();
right.Clear();

var ddr = new DualDisplayRenderer();
ddr.Arrange(left, right);

var lsdr = new SingleDisplayRenderer(left);
var rsdr = new SingleDisplayRenderer(right);

int t = 0;
while (true)
{
    t++;
    
    for (byte y = 0; y < ddr.Height; y++)
    {
        for (byte x = 0; x < ddr.Width; x++)
        {
            var color = (byte)(32.0 * Math.Sin(Math.Sqrt(((x + 0.5) - ddr.Width / 2.0) * ((x + 0.5) - ddr.Width / 2.0) + (y - ddr.Height / 2.0) * (y - ddr.Height / 2.0)) + t / 4.0));
            ddr.DrawPixel(x, y, color);
            
            // var lcolor = (byte)(32.0 * Math.Sin(Math.Sqrt((x - lsdr.Width / 2.0) * (x - lsdr.Width / 2.0) + (y - lsdr.Height / 2.0) * (y - lsdr.Height / 2.0)) + t / 4.0));
            // var rcolor = (byte)(32.0 * Math.Sin(Math.Sqrt((x - lsdr.Width / 2.0) * (x - lsdr.Width / 2.0) + (y - lsdr.Height / 2.0) * (y - lsdr.Height / 2.0)) + t / 4.0));
            //
            // lsdr.DrawPixel(x, y, lcolor);
            // rsdr.DrawPixel(x, y, rcolor);
        }
    }

    ddr.PushFramebuffer();
    // lsdr.PushFramebuffer();
    // rsdr.PushFramebuffer();
}