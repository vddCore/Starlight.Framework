using System;
using System.Threading;
using System.Threading.Tasks;
using Starlight.Framework;

var modules = LedModule.Enumerate();
var left = modules[1];
var right = modules[0];
var rendererL = new SingleModuleRenderer(left);
var rendererR = new SingleModuleRenderer(right);
var t = 0;

await right.SetAnimationState(false);
await left.SetAnimationState(false);

while (true)
{
    for (var y = 0; y < LedModule.Height; y++)
    for (var x = 0; x < LedModule.Width; x++)
    {
        var color = (byte)(
            63.0 + 63.0 * (Math.Sin(x + t / 8.0) + Math.Cos(y + t / 4.0)));
        await rendererL.SetXY(x, y, color);
        color = (byte)(
            63.0 + 63.0 * (Math.Sin(x + t / 8.0) + Math.Cos(y + t / 4.0)));
        await rendererR.SetXY(x, y, color);
    }

    await rendererL.Update();
    await rendererR.Update();
    t += 10;
}