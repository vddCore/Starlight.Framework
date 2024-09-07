namespace Starlight.Framework.Graphics;

using System;
using System.Drawing;
using System.Numerics;

public class SingleDisplayRenderer : IRenderer
{
    private byte[] _framebuffer;
    private LedDisplay _display;

    public int Width => _display.Width;
    public int Height => _display.Height;

    public SingleDisplayRenderer(LedDisplay display)
    {
        _display = display;
        _framebuffer = new byte[Width * Height];
    }

    public void Invert(Rectangle area)
    {
        for (var y = area.Y; y < Height && y < area.Height; y++)
        for (var x = area.X; x < Width && x < area.Width; x++)
        {
            _framebuffer[y * Width + x] = (byte)(255 - _framebuffer[y * Width + x]);
        }
    }

    public void Invert()
        => Invert(new(0, 0, Width, Height));

    public void Clear()
        => Array.Clear(_framebuffer);

    public void DrawPixel(int x, int y, byte value)
    {
        if (x >= Width || y >= Height) return;
        if (x < 0 || y < 0) return;

        _framebuffer[y * Width + x] = value;
    }

    public void DrawImage(Vector2 position, Image image)
    {
        if (position.X >= Width || position.Y >= Height || position.X < 0 || position.Y < 0)
            return;

        for (var y = 0; y < image.Height; y++)
        {
            var ty = position.Y + y;
            
            if (ty >= Width)
                break;
            
            if (ty < 0) 
                continue;
            
            for (var x = 0; x < image.Width; x++)
            {
                var tx = position.X + x;

                if (tx >= Height)
                    break;
                
                if (ty < 0)
                    continue;
                    
                _framebuffer[(uint)(ty * Width + tx)] = image[(uint)x, (uint)y];
            }
        }
    }

    public void DrawImage(Image image)
        => DrawImage(Vector2.Zero, image);

    public void DrawLine(int x1, int y1, int x2, int y2, byte value)
    {
        var dx = Math.Abs(x2 - x1);
        var sx = x1 < x2 ? 1 : -1;
        var dy = -Math.Abs(y2 - y1);
        var sy = y1 < y2 ? 1 : -1;
        var err = dx + dy;
        var sqerr = 0;

        while (true)
        {
            DrawPixel(x1, y1, value);

            if (x1 >= x2 && y1 >= y2)
                break;

            sqerr = 2 * err;

            if (sqerr >= dy)
            {
                if (x1 >= x2)
                    break;
                
                err += dy;
                x1 += sx;
            }

            if (sqerr <= dx)
            {
                if (y1 >= y2)
                    break;

                err += dx;
                y1 += sy;
            }
        }
    }

    public void DrawLine(Vector2 a, Vector2 b, byte value)
    {
        DrawLine(
            (byte)a.X,
            (byte)a.Y,
            (byte)b.X,
            (byte)b.Y,
            value
        );
    }

    public void PushFramebuffer()
        => _display.DrawImage(_framebuffer);
}