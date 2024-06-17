namespace Starlight.Framework;

using System;
using System.Linq;
using Communication;
using Hideous;

public class LedDisplay
{
    private static HidDeviceCollection? _currentDeviceCollection;
    private readonly HidDevice _hidDevice;

    public string SerialNumber => _hidDevice.Properties.SerialNumber;
    
    public byte Width { get; }
    public byte Height { get; }

    private LedDisplay(HidDevice hidDevice)
    {
        _hidDevice = hidDevice;
        _hidDevice.Connect();

        var status = GetCurrentStatus();
        
        Width = status.DisplayWidth;
        Height = status.DisplayHeight;
    }

    public LedDisplayInfo GetCurrentStatus()
        => new(new Report(ReportId.DeviceInfo, 32)
            .GetFeature(_hidDevice)
            .DataAsBytes()
        );

    public void Reboot(bool bootloader) 
        => new Report((byte)ReportId.BasicCommand, 32)
        .Command(GlitterBasicCommand.Reboot)
        .Bytes((byte)(bootloader ? 0x00 : 0x01))
        .SetFeature(_hidDevice);

    public void EnterSleepMode()
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.ToggleSleep)
            .Bytes(0x01)
            .SetFeature(_hidDevice);

    public void ExitSleepMode()
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.ToggleSleep)
            .Bytes(0)
            .SetFeature(_hidDevice);

    public void ToggleWakeOnCommand(bool enable)
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.ToggleWakeOnCommand)
            .Bytes((byte)(enable ? 1 : 0))
            .SetFeature(_hidDevice);

    public void SetSleepTimeout(TimeSpan timeSpan)
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.SetSleepTimeout)
            .Int32((int)timeSpan.TotalMilliseconds)
            .SetFeature(_hidDevice);

    public void DisableSleepTimeout()
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.SetSleepTimeout)
            .Int32(0)
            .SetFeature(_hidDevice);

    public void InhibitSleepRequests()
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.SetSleepTimeout)
            .Int32(-1)
            .SetFeature(_hidDevice);

    public void SetGlobalBrightness(byte brightness)
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.SetGlobalBrightness)
            .Bytes(brightness)
            .SetFeature(_hidDevice);

    public void DrawPixel(byte x, byte y, byte brightness)
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.DrawPixel)
            .Bytes(x, y, brightness)
            .SetFeature(_hidDevice);

    public void DrawLine(byte x1, byte y1, byte x2, byte y2, byte brightness)
        => new Report(ReportId.BasicCommand, 32)
            .Command(GlitterBasicCommand.DrawLine)
            .Bytes(x1, y1, x2, y2, brightness)
            .SetFeature(_hidDevice);

    public void DrawImage(byte[] pixels)
        => new Report(ReportId.GridPwmControl, 306)
            .Bytes(pixels)
            .SetFeature(_hidDevice);

    public void Clear() => DrawImage(new byte[306]);

    public void SetGlobalDcScale(byte value)
    {
        var matrix = new byte[306];
        Array.Fill(matrix, value);

        WriteDcScaleMatrix(matrix);
    }

    public void WriteDcScaleMatrix(byte[] matrix)
        => new Report(ReportId.GridDcScale, 306)
            .Bytes(matrix)
            .SetFeature(_hidDevice);

    public static LedDisplay[] Enumerate()
    {
        var modules = new LedDisplay[0];

        try
        {
            if (_currentDeviceCollection != null)
            {
                _currentDeviceCollection.Dispose();
            }

            _currentDeviceCollection = new HidDeviceCollection(0x32AC, 0x0020);
            modules = _currentDeviceCollection.Select(x => new LedDisplay(x)).ToArray();
        }
        catch
        {
            /* No processing necessary. */
        }

        return modules.ToArray();
    }
}