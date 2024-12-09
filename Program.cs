using GettingStarted.Model;
using GettingStarted.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;

#region Load Configuration

ConfigurationBuilder configurationBuilder = new();
IConfiguration configuration = configurationBuilder.AddUserSecrets<Program>().Build();

var openAiConfiguration = configuration.GetSection("OpenAi") ?? throw new ConfigurationException("Unable to find 'OpenAi' configuration!");
string? openAiDeploymentName = openAiConfiguration["DeploymentName"] ?? throw new ConfigurationException("Unable to resolve 'DeploymentName'!");
string? openAiEndpoint = openAiConfiguration["Endpoint"] ?? throw new ConfigurationException("Unable to resolve 'Endpoint'!");
string? openAiApiKey = openAiConfiguration["ApiKey"] ?? throw new ConfigurationException("Unable to resolve 'ApiKey'!");
string? openAiServiceId = openAiConfiguration["ServiceID"] ?? throw new ConfigurationException("Unable to resolve 'ServiceID'!");

#endregion

var systemprompt = """
          Du bist ein AI-Assistent für die Anwendung elVIS und antwortest immer höflich. Deine Aufgabe es es die Benutzer der elVIS zu verwalten (Benutzer auslesen
          und neue Benutzer hinzuzufügen).
          Wenn du etwas nicht weißt, dann antwortest du bitte mit 'Dies kann ich nicht beantworten. Bitte wenden Sie sich an den Support unter support@bfi-ooe.at'.
          Wenn du dir nicht sicher bist, dann antworte nur mit den gesicherten bzw. korrekten Informationen!
        """;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    openAiDeploymentName,
    openAiEndpoint,
    openAiApiKey,
    openAiServiceId);

builder.Plugins.AddFromType<MathPlugin>();
builder.Plugins.AddFromType<D365Plugin>();

var kernel = builder.Build();

// Get chat completion service
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Start the conversation
ConsoleColor prevColor = Console.ForegroundColor;
Console.ForegroundColor = ConsoleColor.Yellow;
Console.Write("User > ");
Console.ForegroundColor = prevColor;
string? userInput;
ChatHistory history = [];

OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    ChatSystemPrompt = systemprompt
};

while ((userInput = Console.ReadLine()) != null)
{
    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

    history.AddUserMessage(userInput);

    // Get the response from the AI
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        history,
                        executionSettings: openAIPromptExecutionSettings,
                        kernel: kernel);

    // Stream the results
    StringBuilder fullMessage = new();
    var first = true;
    await foreach (var content in result)
    {
        if (content.Role.HasValue && first)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("Assistant > ");
            Console.ForegroundColor = prevColor;
            first = false;
        }
        Console.Write(content.Content);
        fullMessage.Append(content.Content);
    }
    Console.WriteLine();

    // Add the message from the agent to the chat history
    history.AddAssistantMessage(fullMessage.ToString());

    // Get user input again
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("User > ");
    Console.ForegroundColor = prevColor;
}