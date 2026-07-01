using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Back_end_Innovation_Project.APP.DTOs;


public class ModelsDevRoot : Dictionary<string, ModelDevInfo> { }

public class ModelDevInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("pricing")]
    public ModelPricing? Pricing { get; set; }
}

public class ModelPricing
{
    [JsonPropertyName("prompt")]
    public double Prompt { get; set; }

    [JsonPropertyName("completion")]
    public double Completion { get; set; }
}