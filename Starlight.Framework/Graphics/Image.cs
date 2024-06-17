namespace Starlight.Framework.Graphics;

public class Image
{
    private readonly byte[] _data;
    
    public int Width { get; }
    public int Height { get; }

    public byte this[uint x, uint y]
    {
        get
        {
            if (x >= Width || y >= Height)
                return 0;
            
            return _data[y  * Width + x];
        }

        set
        {
            if (x >= Width || y >= Height)
                return;

            _data[y * Width + x] = value;
        }
    }
    
    public Image(int width, int height)
    {
        _data = new byte[width * height];
        
        Width = width;
        Height = height;
    }

    public byte[] GetData() => _data;
}