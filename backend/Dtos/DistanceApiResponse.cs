using System.Collections.Generic;
using System.Text.Json.Serialization;

// This is a DTO to match the response from the Google Distance Matrix API
public class DistanceApiResponse
{
    [JsonPropertyName("rows")]
    public List<Row>? Rows { get; set; }
}

public class Row
{
    [JsonPropertyName("elements")]
    public List<Element>? Elements { get; set; }
}

public class Element
{
    [JsonPropertyName("distance")]
    public Distance? Distance { get; set; }
}

public class Distance
{
    [JsonPropertyName("value")]
    public double? Value { get; set; }
}
