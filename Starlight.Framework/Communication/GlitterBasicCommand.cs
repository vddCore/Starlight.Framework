namespace Starlight.Framework;

internal enum GlitterBasicCommand : byte
{
    Reboot,
    ToggleSleep,
    ToggleWakeOnCommand,
    SetSleepTimeout,
    SetGlobalBrightness,
    DrawPixel,
    DrawLine
}