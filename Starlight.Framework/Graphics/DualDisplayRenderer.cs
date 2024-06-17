namespace Starlight.Framework.Graphics;

using System;
using System.Numerics;

public class DualDisplayRenderer : IRenderer
{
    private byte[]? _leftFramebuffer;
    private byte[]? _rightFramebuffer;
    
    private LedDisplay? _leftDisplay;
    private LedDisplay? _rightDisplay;

    public int Width
    {
        get
        {
            EnsureDisplaysArranged();
            
            return _leftDisplay!.Width + _rightDisplay!.Width;
        }
    }

    public int Height
    {
        get
        {
            EnsureDisplaysArranged();
            return _leftDisplay!.Height;
        }
    }

    public void Arrange(LedDisplay left, LedDisplay right)
    {
        _leftDisplay = left;
        _rightDisplay = right;

        _leftFramebuffer = new byte[left.Width * left.Height];
        _rightFramebuffer = new byte[right.Width * right.Height];
    }

    public void Clear()
    {
        EnsureDisplaysArranged();
        Array.Clear(_leftFramebuffer!);
        Array.Clear(_rightFramebuffer!);
    }

    public void DrawPixel(int x, int y, byte value)
    {
        EnsureDisplaysArranged();

        var buf = x < _leftDisplay!.Width ? _leftFramebuffer : _rightFramebuffer;
        var actual_x = (byte)(x % 9);

        buf![y * _leftDisplay.Width + actual_x] = value;
    }

    public void DrawLine(int x1, int y1, int x2, int y2, byte value)
    {
        EnsureDisplaysArranged();

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

    public void DrawImage(Vector2 position, Image image)
    {
        EnsureDisplaysArranged();
        
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
                    
                DrawPixel((int)tx, (int)ty, image[(uint)x, (uint)y]);
            }
        }
    }

    public void DrawImage(Image image)
    {
        DrawImage(Vector2.Zero, image);
    }

    public void PushFramebuffer()
    {
        EnsureDisplaysArranged();

        _leftDisplay!.DrawImage(_leftFramebuffer!);
        _rightDisplay!.DrawImage(_rightFramebuffer!);
    }

    private void EnsureDisplaysArranged()
    {
        if (_leftDisplay == null || _rightDisplay == null)
            throw new InvalidOperationException("Both displays are required to be initialized for this operation.");
    }
}