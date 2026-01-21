using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration; // Required for IConfiguration
using StudyMate.API.Models.ModelsAi;
using StudyMate.API.Contracts;





namespace StudyMate.API.Services
{
    // Inside Program.cs: You do NOT use AddHttpClient.
    // Instead, you register the client directly.
    // You would need to check the exact registration pattern for this SDK.

    // Inside GeminiClient.cs
    public class AiClient : IExternalLLMClient
    {
        private readonly Client _geminiClient;
        private readonly string _modelName; // Store model name as a field
        public AiClient(IConfiguration configuration)
        {
            string apiKey = configuration["Gemini:ApiKey"]
                ?? throw new InvalidOperationException("Gemini:ApiKey configuration is missing.");

            _modelName = configuration["Gemini:ModelName"] ?? "models/gemini-2.5-flash";

            // ✅ NEW: Initialize the Client with the API key
            // The Client class is the top-level entry point in the new SDK.
            _geminiClient = new Client(apiKey: apiKey);
        }
        // ... rest of your methods use _geminiModel ...

        public async Task<string> GenerateResponseAsync(List<AiPromptMessage> history, string newPrompt)
        {
            // Pass the role and a list (or array) of Part objects.
            var contents = history.Select(m => new Content
            {
                Role = (m.Sender.ToLowerInvariant() == "user") ? "user" : "model",
                Parts = new List<Part>
                {
                 new Part { Text = m.Content }
                }
            }).ToList();


            // And for the new prompt:
            contents.Add(new Content
            {
                Role = "user",
                Parts = new List<Part>
                {
                 new Part { Text = newPrompt }
                }
            });

            // Make the AI call using the models service on the Client object
            var response = await _geminiClient.Models.GenerateContentAsync(
                model: _modelName, // Model name is passed here on the call
                contents: contents
            );



            return response.Candidates[0].Content.Parts[0].Text ?? "Sorry, I was unable to generate a response.";
        }

        public async Task<string> GenerateTitleFromPrompt(string prompt)
        {
            // The instruction is now part of the user's prompt history
            string titleInstruction = "YOU ARE A TITLE GENERATOR. Review the following conversation topic. Return ONLY a concise 5-word title. Do NOT include quotes or explanation.";

            string combinedPrompt = $"{titleInstruction}\n\nTOPIC: {prompt}";

            // We now construct the contents object just like the working chat method
            var contents = new List<Content>
            {
                new Content
                {
                    Role = "user",
                    Parts = new List<Part> { new Part { Text = combinedPrompt } }
                }
            };
          
            // Use the working method signature from your GenerateResponseAsync
            var response = await _geminiClient.Models.GenerateContentAsync(
                model: _modelName,
                contents: contents
            );

            // The response navigation path is the same regardless of the prompt type
            if (response.Candidates == null || response.Candidates.Count == 0)
            {
                return "New Conversation";
            }

            // Extract and trim the text
            return response.Candidates[0].Content.Parts[0].Text?.Trim() ?? "New Conversation";
        }
    }

}

