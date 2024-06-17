namespace Starlight.Framework.Communication;

using System;
using Hideous;

internal class Report
{
    private byte[] _data;

    private int _bufferPosition = 1;
    
    public Report(byte id, int length)
    {   
        if (length == 0)
        {
            throw new InvalidOperationException("Need at least 1 byte for report data.");
        }
        
        _data = new byte[length + 1];
        _data[0] = id;
    }

    public Report(ReportId id, int length)
        : this((byte)id, length)
    {
    }

    public Report Int32(int data)
    {
        return Bytes(new[]
        {
            (byte)(data & 0x000000FF >> 0),
            (byte)(data & 0x0000FF00 >> 8),
            (byte)(data & 0x00FF0000 >> 16),
            (byte)(data & 0xFF000000 >> 24)
        });
    }
    
    public Report Bytes(params byte[] data)
    {
        for (var i = 0; i < data.Length; i++)
        {
            if (_bufferPosition >= _data.Length)
            {
                throw new InvalidOperationException($"Too much data for the report buffer (max. {_data.Length - 1}).");
            }

            _data[_bufferPosition++] = data[i];
        }

        return this;
    }
    
    public Report Command(GlitterBasicCommand cmd)
        => Bytes(new[] { (byte)cmd });
    
    public Report GetFeature(HidDevice device)
    {
        device.GetFeatureReport(_data);
        return this;
    }

    public Report SetFeature(HidDevice device)
    {
        device.SetFeatureReport(_data);
        return this;
    }

    public byte[] Raw() => _data;
    public byte[] DataAsBytes() => _data[1..];
}