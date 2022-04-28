using System.Text.Json.Serialization;

namespace b2c.Data;

public class Token
{
    /// <summary>
    /// The access token value to be used with the Bearer Authentication scheme for subsequent calls to the API.
    /// </summary>
    [JsonPropertyName("access_token")]
    public string accessToken { get; set; }

    /// <summary>
    /// The number of seconds until the access token is no longer valid.
    /// </summary>
    [JsonPropertyName("expires_in")]
    public int expiresIn { get; set; }

    /// <summary>
    /// The number of seconds until the refresh token is no longer valid.
    /// </summary>
    [JsonPropertyName("refresh_expires_in")]
    public int refreshExpiresIn { get; set; }

    /// <summary>
    /// The refresh token used to request a new access token.
    /// </summary>
    [JsonPropertyName("refresh_token")]
    public string refreshToken { get; set; }

    /// <summary>
    /// The type of token provided. A successful request returns a bearer token.
    /// </summary>
    [JsonPropertyName("token_type")]
    public string bearer { get; set; }

    /// <summary>
    /// When applicable, restricts authentication prior to the timestamp value.
    /// </summary>
    [JsonPropertyName("not-before-policy")]
    public long notBeforePolicy { get; set; }

    /// <summary>
    /// The session state string.
    /// </summary>
    [JsonPropertyName("session_state")]
    public string sessionState { get; set; }
}
