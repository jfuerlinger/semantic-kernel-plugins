using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    "gpt-35-turbo-16k",
    "https://aoaitest45.openai.azure.com/",
    "7e48f2135baf44f2aa05ae7694c21f26",
    "gpt-35-turbo-16k");
// var kernel = builder.Build();

// var result = await kernel.InvokePromptAsync(
//         "Give me a list of breakfast foods with eggs and cheese");
//     Console.WriteLine(result);

#pragma warning disable SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
builder.Plugins.AddFromType<ConversationSummaryPlugin>();
#pragma warning restore SKEXP0050 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
var kernel = builder.Build();

string input = @"I'm a vegan in search of new recipes. I love spicy food! 
Can you give me a list of breakfast recipes that are vegan friendly?";

var result = await kernel.InvokeAsync(
    "ConversationSummaryPlugin", 
    "GetConversationActionItems", 
    new() {{ "input", input }});

Console.WriteLine(result);