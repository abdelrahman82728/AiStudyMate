using StudyMate.API.Models.ModelsAi;

namespace StudyMate.API.Contracts
{
    public interface IExternalLLMClient
    {
       
            // Generates the response content based on history and the new prompt.
            // It takes the clean, lightweight DTOs (AIPromptMessage) for context.
            Task<string> GenerateResponseAsync(List<AiPromptMessage> history, string newPrompt);

        // Generates a title from the entire conversation history.
           Task<string> GenerateTitleFromPrompt(string prompt);


    }
}
 