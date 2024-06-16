namespace Starlight.Framework.Enumeration;

using System.Collections.Generic;

internal interface ISerialModuleEnumerator
{
    IEnumerable<string> Enumerate(ushort vid, ushort pid);
}