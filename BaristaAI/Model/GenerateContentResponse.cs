namespace BaristaAI.Model;

public abstract class GenerateContentResponse(string? error = null, bool isValid = true)
{
    public string? Error { get; set; } = error;

    public bool IsValid { get; set; } = isValid;
}