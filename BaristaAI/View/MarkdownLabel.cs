using Markdig;

namespace BaristaAI.View;

public partial class MarkdownView : WebView
{
    public static readonly BindableProperty MarkdownContentProperty = BindableProperty.Create(
        nameof(MarkdownContent), typeof(string), typeof(MarkdownView), propertyChanged: OnMarkdownContentChanged);

    public string MarkdownContent 
    { 
        get => (string)GetValue(MarkdownContentProperty); 
        set => SetValue(MarkdownContentProperty, value); 
    }

    private static void OnMarkdownContentChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (MarkdownView)bindable;
        control.Source = new HtmlWebViewSource 
        { 
            Html = GenerateStyledHtml((string)newValue)
        };
    }

    private static string GenerateStyledHtml(string markdown)
    {
        string body = Markdown.ToHtml(markdown);
        const string stylesheetFilePath = "Styles/css/MarkdownStyles.css";
        string htmlDocument = $@" 
<!DOCTYPE html> 
<html> 
    <head>
        <link rel='stylesheet' type='text/css' href='{stylesheetFilePath}'>
    </head> 
    <body> 
        {body} 
    </body> 
</html>";
        return htmlDocument;
    }
}