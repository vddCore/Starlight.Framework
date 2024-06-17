namespace Starlight.Framework;

public class LedDisplayInfo
{
    public bool SleepPinActive { get; }
    public bool Dip1PinActive { get; }
    public bool IntbPinActive { get; }
    public LedDisplaySleepMode SleepMode { get; }
    public bool WakeOnCommandEnabled { get; }
    public byte IdRegister { get; }
    public byte ConfigurationRegister { get; }
    public byte GlobalBrightness { get; }
    public byte DisplayWidth { get; }
    public byte DisplayHeight { get; }
    public int TimeoutMilliseconds { get; }

    public LedDisplayInfo(byte[] rawData)
    {
        SleepPinActive = rawData[0] != 0;
        Dip1PinActive = rawData[1] != 0;
        IntbPinActive = rawData[2] != 0;
        SleepMode = (LedDisplaySleepMode)((rawData[3] & 0x7) >> 1);
        WakeOnCommandEnabled = (rawData[3] & 0x1) != 0;
        IdRegister = rawData[4];
        ConfigurationRegister = rawData[5];
        GlobalBrightness = rawData[6];
        DisplayWidth = rawData[7];
        DisplayHeight = rawData[8];

        TimeoutMilliseconds = (rawData[12] << 24) 
                            | (rawData[11] << 16)
                            | (rawData[10] << 8)
                            | rawData[9];
    }
}