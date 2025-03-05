namespace BaristaAI.Model;

public class GenerateTextResponse(string? text, string? error = null, bool isValid = true) 
    : GenerateContentResponse(error, isValid)
{
    public string? Text { get; set; } = text;
}