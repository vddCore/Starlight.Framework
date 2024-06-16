namespace Starlight.Framework.Enumeration;

using System;
using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
internal class WmiSerialModuleEnumerator : ISerialModuleEnumerator
{
    public IEnumerable<string> Enumerate(ushort vid, ushort pid)
    {
        using var searcher = new ManagementObjectSearcher($@"SELECT * FROM Win32_PnPEntity where DeviceID Like ""USB\\VID_{vid:X4}&PID_{pid:X4}%""");
        using var collection = searcher.Get();

        var portNames = new List<string>();
        
        foreach (var device in collection)
        {
            var caption = device.GetPropertyValue("Caption") as string;

            var index = -1;
            if (caption != null && (index = caption.IndexOf("(COM", StringComparison.InvariantCulture)) > 0)
            {
                portNames.Add(caption.Substring(index + 1, caption.Length - index - 2));
            }
        }

        return portNames.ToArray();
    }
}