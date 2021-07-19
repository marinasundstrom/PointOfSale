namespace WebApi.Hal.Forms;

using System.Runtime.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using NJsonSchema.Converters;

public class Template
{
    [JsonProperty("title")]
    public string Title { get; set; } = null!;

    [JsonProperty("method")]
    public string? Method { get; set; }

    [JsonProperty("contentType", NullValueHandling = NullValueHandling.Ignore)]
    public string? ContentType { get; set; }

    [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
    public IList<Property>? Properties { get; set; }

    [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
    public string? Target { get; set; }
}

public class Property
{
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("prompt", NullValueHandling = NullValueHandling.Ignore)]
    public string? Prompt { get; set; }

    [JsonProperty("readOnly", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool ReadOnly { get; set; }

    [JsonProperty("regex", NullValueHandling = NullValueHandling.Ignore)]
    public string? Regex { get; set; }

    [JsonProperty("required", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Required { get; set; }

    [JsonProperty("templated", NullValueHandling = NullValueHandling.Ignore)]
    public string? Templated { get; set; }

    [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
    public string? Value { get; set; }

    // Additional properties

    [JsonProperty("cols", NullValueHandling = NullValueHandling.Ignore)]
    public int? Cols { get; set; }

    [JsonProperty("max", NullValueHandling = NullValueHandling.Ignore)]
    public int? Max { get; set; }

    [JsonProperty("maxLength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxLength { get; set; }

    [JsonProperty("min", NullValueHandling = NullValueHandling.Ignore)]
    public int? Min { get; set; }

    [JsonProperty("minLength", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinLength { get; set; }

    [JsonProperty("options", NullValueHandling = NullValueHandling.Ignore)]
    public Options? Options { get; set; }

    [JsonProperty("placeholder", NullValueHandling = NullValueHandling.Ignore)]
    public string? Placeholder { get; set; }

    [JsonProperty("rows", NullValueHandling = NullValueHandling.Ignore)]
    public int? Rows { get; set; }

    [JsonProperty("step", NullValueHandling = NullValueHandling.Ignore)]
    public int? Step { get; set; }

    [JsonProperty("type", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public PropertyType Type { get; set; } = PropertyType.Text;
}

public class Options
{
    [JsonProperty("selectedValues", NullValueHandling = NullValueHandling.Ignore)]
    public IList<object>? SelectedValues { get; set; } = null!;

    [JsonProperty("inline", NullValueHandling = NullValueHandling.Ignore)]
    public IList<object>? Inline { get; set; }

    [JsonProperty("link", NullValueHandling = NullValueHandling.Ignore)]
    public FormLink? Link { get; set; }

    [JsonProperty("maxItems", NullValueHandling = NullValueHandling.Ignore)]
    public int? MaxItems { get; set; }

    [JsonProperty("minItems", NullValueHandling = NullValueHandling.Ignore)]
    public int? MinItems { get; set; }

    [JsonProperty("promptField", NullValueHandling = NullValueHandling.Ignore)]
    public string? PromptField { get; set; }

    [JsonProperty("valueField", NullValueHandling = NullValueHandling.Ignore)]
    public string? ValueField { get; set; }
}

public class FormLink
{
    [JsonProperty("href")]
    public string Href { get; set; } = null!;

    [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
    public string? Type { get; set; }

    [JsonProperty("templated", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool Templated { get; set; }
}

[JsonConverter(typeof(StringEnumConverter))]
public enum PropertyType
{
    [EnumMember(Value = "text")]
    Text,
    [EnumMember(Value = "hidden")]
    Hidden,
    [EnumMember(Value = "textarea")]
    TextArea,
    [EnumMember(Value = "search")]
    Search,
    [EnumMember(Value = "tel")]
    Tel,
    [EnumMember(Value = "url")]
    Url,
    [EnumMember(Value = "email")]
    Email,
    [EnumMember(Value = "password")]
    Password,
    [EnumMember(Value = "date")]
    Date,
    [EnumMember(Value = "month")]
    Month,
    [EnumMember(Value = "week")]
    Week,
    [EnumMember(Value = "time")]
    Time,
    [EnumMember(Value = "datetime-local")]
    DatetimeLocal,
    [EnumMember(Value = "number")]
    Number,
    [EnumMember(Value = "range")]
    Range,
    [EnumMember(Value = "color")]
    Color
}