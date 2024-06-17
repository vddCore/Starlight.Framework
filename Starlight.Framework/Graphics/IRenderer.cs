namespace Starlight.Framework.Graphics;

using System.Numerics;

public interface IRenderer
{
    public int Width { get; }
    public int Height { get; }

    public void Clear();
    public void DrawPixel(int x, int y, byte value);
    
    public void DrawLine(int x1, int y1, int x2, int y2, byte value);
    public void DrawLine(Vector2 a, Vector2 b, byte value);

    public void DrawImage(Vector2 position, Image image);
    public void DrawImage(Image image);

    public void PushFramebuffer();
}