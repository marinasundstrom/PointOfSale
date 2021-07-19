namespace WebApi.Shared;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using WebApi.Hal;


public class CustomField : Resource
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public object Value { get; set; } = null!;
}

public class CreateOrUpdateCustomField
{
    public string Name { get; set; } = null!;

    public object Value { get; set; } = null!;
}

public static partial class Mapper
{
    public static object ConvertValue(string v)
    {
        if (bool.TryParse(v, out var boolValue))
        {
            return boolValue;
        }

        if (double.TryParse(v, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var doubleValue))
        {
            return doubleValue;
        }

        if (int.TryParse(v, out var intValue))
        {
            return intValue;
        }

        return v;
    }
}