## What is this
This is a high-level HID driver facilitating USB communication with Framework Laptop 16 running [Sparkle Firmware](https://github.com/vddCore/sparkle-fw16).

## How do I use it
1. Clone the project recursively. It uses [Hideous](https://github.com/vddCore/Hideous) as its HID backend.
2. Reference the library in your project.
3. Follow the examples.

## Examples

### Direct display control
> **NOTE**  
> Issuing a lot of direct draw requests directly might result in poor 
> performance. Consider using frame-buffering and pushing an entire 
> frame at once.

```csharp
using Starlight.Framework;

/* 
 * Will be empty if none found.
 */
var displays = LedDisplay.Enumerate();

foreach (var display in displays)
{
  display.SetGlobalBrightness(255);
  display.Clear();
  display.DrawLine(0, 0, display.Width - 1, display.Height - 1, 255);
}
```

### Single-display frame-buffered renderer
Single-display renderer proxies all draw calls and uses 
frame buffering to improve rendering performance.

```csharp
using System.Threading;

using Starlight.Framework;
using Starlight.Framework.Graphics;

var display = LedDisplay.Enumerate()[0];
var sdr = new SingleDisplayRenderer(display);

while (true)
{
  sdr.Clear();

  for (var y = 0; y < sdr.Height; y++)
  {
    sdr.Line(0, y, sdr.Width - 1, y, 255);
  }

  sdr.PushFramebuffer();
  Thread.Sleep(16); // roughly 60 FPS, you can lower this if you wanna be fancy
}
```

### Dual-display frame-buffered renderer
Dual-display renderer allows you to treat your two LED displays as if
they were a single, large LED matrix. It requires the user to manually
arrange the displays. Or not, for an artistic effect or something.

```csharp
using System.Threading;

using Starlight.Framework;
using Starlight.Framework.Graphics;

var displays = LedDisplay.Enumerate();
var ddr = new DualDisplayRenderer();

ddr.Arrange(
  left: displays[0],
  right: displays[1]
);

while (true)
{
  ddr.Clear();

  for (var y = 0; y < ddr.Height; y++)
  {
    ddr.Line(0, y, ddr.Width - 1, y, 255);
  }

  ddr.PushFramebuffer();
  Thread.Sleep(16);
}
```