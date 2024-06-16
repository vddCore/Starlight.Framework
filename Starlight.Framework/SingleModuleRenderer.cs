namespace Starlight.Framework;

using System.Threading.Tasks;

public class SingleModuleRenderer
{
    private LedModule _module;

    private byte[][] _pixelBuffer = new byte[LedModule.Width][];
    
    public SingleModuleRenderer(LedModule module)
    {
        _module = module;

        for (var i = 0; i < LedModule.Width; i++)
        {
            _pixelBuffer[i] = new byte[LedModule.Height];
        }

        _module.Connect();
    }

    public byte GetXY(int x, int y)
        => _pixelBuffer[x][y];

    public async Task SetXY(int x, int y, byte value, bool immediate = false)
    {
        _pixelBuffer[x][y] = value;

        if (immediate)
        {
            await Update();
        }
    }

    public async Task Update()
    {
        for (var x = 0; x < 9; x++)
        {
            await _module.SetColumn((byte)x, _pixelBuffer[x]);
        }

        await _module.FlushColumns();
    }
}