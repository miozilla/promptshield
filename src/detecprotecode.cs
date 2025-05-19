using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContentSafetyProtectedMaterialForCodeSampleCode;

public class DetectionRequest
{
    public DetectionRequest(string code)
    {
        this.Code = code;
    }

    public string Code { get; set; }
}

public class CodeCitation
{
    public CodeCitation(string license, List<string> sourceUrls)
    {
        this.License = license;
        this.SourceUrls = sourceUrls;
    }

    public string License { get; set; }
    public List<string> SourceUrls { get; set; }
}

public class ProtectedMaterialAnalysis
{
    public ProtectedMaterialAnalysis(bool detected, List<CodeCitation> codeCitations)
    {
        this.Detected = detected;
        this.CodeCitations = codeCitations;
    }

    public bool Detected { get; set; }
    public List<CodeCitation> CodeCitations { get; set; }
}

public class DetectionResult
{
    public DetectionResult(ProtectedMaterialAnalysis protectedMaterialAnalysis)
    {
        this.ProtectedMaterialAnalysis = protectedMaterialAnalysis;
    }

    public ProtectedMaterialAnalysis ProtectedMaterialAnalysis { get; set; }
}

/// <summary>
/// Class representing a detection error response.
/// </summary>
public class DetectionErrorResponse
{
    /// <summary>
    /// The detection error.
    /// </summary>
    public DetectionError? error { get; set; }
}

/// <summary>
/// Class representing a detection error.
/// </summary>
public class DetectionError
{
    /// <summary>
    /// The error code.
    /// </summary>
    public string? code { get; set; }
    /// <summary>
    /// The error message.
    /// </summary>
    public string? message { get; set; }
    /// <summary>
    /// The error target.
    /// </summary>
    public string? target { get; set; }
    /// <summary>
    /// The error details.
    /// </summary>
    public string[]? details { get; set; }
    /// <summary>
    /// The inner error.
    /// </summary>
    public DetectionInnerError? innererror { get; set; }
}

/// <summary>
/// Class representing a detection inner error.
/// </summary>
public class DetectionInnerError
{
    /// <summary>
    /// The inner error code.
    /// </summary>
    public string? code { get; set; }
    /// <summary>
    /// The inner error message.
    /// </summary>
    public string? innererror { get; set; }
}


/// <summary>
/// Exception raised when there is an error in detecting the content.
/// </summary>
public class DetectionException : Exception
{
    public string Code { get; set; }

    /// <summary>
    /// Constructor for the DetectionException class.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public DetectionException(string code, string message) : base(message)
    {
        Code = code;
    }
}

public class ContentSafety
{
    /// <summary>
    ///     The version of the Content Safety API to use.
    /// </summary>
    private const string ApiVersion = "2024-09-15-preview";

    /// <summary>
    ///     The HTTP client.
    /// </summary>
    private static readonly HttpClient Client = new();

    /// <summary>
    ///     The JSON serializer options.
    /// </summary>
    private static readonly JsonSerializerOptions Options =
        new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, Converters = { new JsonStringEnumConverter() } };

    /// <summary>
    ///     Initializes a new instance of the <see cref="ContentSafety" /> class.
    /// </summary>
    /// <param name="endpoint">The endpoint URL for the Content Safety API.</param>
    /// <param name="subscriptionKey">The subscription key for the Content Safety API.</param>
    public ContentSafety(string endpoint, string subscriptionKey, string aadToken)
    {
        this.Endpoint = endpoint;
        this.SubscriptionKey = subscriptionKey;
        this.AADToken = aadToken;
    }

    private string Endpoint { get; set; }
    private string SubscriptionKey { get; set; }
    private string AADToken { get; set; }

    /// <summary>
    ///     Builds the URL for the Content Safety API based on the media type.
    /// </summary>
    /// <returns>The URL for the Content Safety API.</returns>
    public string BuildUrl()
    {
        return $"{this.Endpoint}/contentsafety/text:detectProtectedMaterialForCode?api-version={ApiVersion}";
    }

    /// <summary>
    ///     Builds the request body for the Content Safety API request.
    /// </summary>
    /// <param name="code">The content to analyze.</param>
    /// <returns>The request body for the Content Safety API request.</returns>
    public DetectionRequest BuildRequestBody(string code)
    {
        return new DetectionRequest(code);
    }

    /// <summary>
    ///     Deserializes the JSON string into a DetectionResult object based on the media type.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized DetectionResult object for the Content Safety API response.</returns>
    public DetectionResult? DeserializeDetectionResult(string json)
    {
        return JsonSerializer.Deserialize<DetectionResult>(json, Options);
    }

    /// <summary>
    ///     Detects unsafe content using the Content Safety API.
    /// </summary>
    /// <param name="code">The code to detect.</param>
    /// <returns>The response from the Content Safety API.</returns>
    public async Task<DetectionResult> Detect(string code)
    {
        var url = this.BuildUrl();
        DetectionRequest requestBody = this.BuildRequestBody(code);
        string payload = JsonSerializer.Serialize(requestBody, requestBody.GetType(), Options);

        var msg = new HttpRequestMessage(HttpMethod.Post, url);
        msg.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        msg.Headers.Add("Ocp-Apim-Subscription-Key", this.SubscriptionKey);
        msg.Headers.Add("Authorization", this.AADToken);

        var response = await Client.SendAsync(msg);
        var responseText = await response.Content.ReadAsStringAsync();

        Console.WriteLine((int)response.StatusCode);
        foreach (var header in response.Headers)
        {
            Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
        }

        Console.WriteLine(responseText);

        if (!response.IsSuccessStatusCode)
        {
            DetectionErrorResponse? error =
                JsonSerializer.Deserialize<DetectionErrorResponse>(responseText, Options);
            if (error == null || error.error == null || error.error.code == null || error.error.message == null)
            {
                throw new DetectionException(response.StatusCode.ToString(),
                    $"Error is null. Response text is {responseText}");
            }

            throw new DetectionException(error.error.code, error.error.message);
        }

        DetectionResult? result = this.DeserializeDetectionResult(responseText);
        if (result == null)
        {
            throw new DetectionException(response.StatusCode.ToString(),
                $"HttpResponse is null. Response text is {responseText}");
        }

        return result;
    }

    /// <summary>
    ///     Gets the accept result of the specified category from the given detection result.
    /// </summary>
    /// <param name="detectionResult">The detection result object to retrieve the accept result from.</param>
    public void PrintDetectionResult(DetectionResult detectionResult)
    {
        Console.WriteLine("Final decision: " + detectionResult.ProtectedMaterialAnalysis.Detected);
        foreach (var res in detectionResult.ProtectedMaterialAnalysis.CodeCitations)
        {
            Console.WriteLine($"License: {res.License}");
            Console.WriteLine("Source URLs:");
            foreach (var url in res.SourceUrls)
            {
                Console.WriteLine(url);
            }
        }
    }
}

public class Program
{
    static async Task Main(string[] args)
    {
        // Replace the placeholders with your own values
        // Choose to use key authorization or AAD token authorization
        string endpoint = "<endpoint>";
        string subscriptionKey = "<subscription_key>";
        string aadToken = "<AAD_token>";

        // Initialize the ContentSafety object
        ContentSafety contentSafety = new ContentSafety(endpoint, subscriptionKey, aadToken);

        // Set the content to be tested
        string content = @"<test_content>";

        // Detect content safety
        DetectionResult detectionResult = await contentSafety.Detect(content);

        // Make a decision
        contentSafety.PrintDetectionResult(detectionResult);
    }
}