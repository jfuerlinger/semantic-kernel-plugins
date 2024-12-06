using Microsoft.SemanticKernel;

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
    "gpt-35-turbo-16k",
    "https://aoaitest45.openai.azure.com/",
    "7e48f2135baf44f2aa05ae7694c21f26",
    "gpt-35-turbo-16k");
var kernel = builder.Build();

var result = await kernel.InvokePromptAsync(
        "Give me a list of breakfast foods with eggs and cheese");
    Console.WriteLine(result);