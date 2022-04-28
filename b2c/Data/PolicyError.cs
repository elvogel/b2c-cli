using System;
using System.Text.Json.Serialization;

namespace b2c.Data;

// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
public class InnerError
{
    [JsonPropertyName("correlationId")]
    public string CorrelationId { get; set; }

    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    [JsonPropertyName("request-id")]
    public string RequestId { get; set; }

    [JsonPropertyName("client-request-id")]
    public string ClientRequestId { get; set; }
}

public class Error
{
    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }

    [JsonPropertyName("innerError")]
    public InnerError InnerError { get; set; }
}

public class PolicyError
{
    [JsonPropertyName("error")]
    public Error Error { get; set; }
}
