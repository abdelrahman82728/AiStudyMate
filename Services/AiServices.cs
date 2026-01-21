using System.Reflection.Metadata.Ecma335;
using StudyMate.API.Contracts;
using StudyMate.API.Contracts;
using StudyMate.API.Models.ModelsAi;
using static StudyMate.API.Models.ModelsAi.ClientConversationDTO;


namespace StudyMate.API.Services
{
    public class AiServices
    {

        private readonly IAiRepository _aiRepository;
        private readonly IExternalLLMClient _externalLLMClient;

        public AiServices(IAiRepository aiRepository , IExternalLLMClient externalLLMClient )
        {
            this._aiRepository = aiRepository;
            this._externalLLMClient = externalLLMClient;

        }

        public async Task<AiResponse<ClientMessageDTO>> HandleUserPromptAsync( int userId, UserPromptDTO clientPrompt)
        {
            // --- 1. VERIFICATION, CREATION, AND OWNERSHIP CHECK ---

            Conversation convoShell;

            if (clientPrompt.ConversationID == 0) // Signal to CREATE NEW CONVERSATION
            {
                // A. Create the minimum viable conversation shell.
                convoShell = new Conversation
                {
                    ConversationTitle = "temp title",
                    UserID = userId,
                    // Title and DateLastUpdated will be set/updated later.
                };

                // B. Save the new conversation to the DB to get the new, valid ID.
                convoShell = await _aiRepository.CreateNewConversationAsync(convoShell);

                // C. CRITICAL: Update the clientPrompt ID for all subsequent repository calls.
                clientPrompt.ConversationID = convoShell.ConversationID;

                // Log: New conversation created with ID {convoShell.ConversationID}
            }
            else // EXISTING CONVERSATION: Verify ID and Ownership
            {
                // Verify the ID is valid AND belongs to the user (single DB trip).
                convoShell = await _aiRepository.GetConversationShellAsync(clientPrompt.ConversationID, userId);

                if (convoShell == null)
                {
                    // Fail fast: Conversation doesn't exist OR user doesn't own it.
                    return new AiResponse<ClientMessageDTO>
                    {
                        Success = false,
                        Message = "Conversation not found or access is denied.",
                        Data = null
                    };
                }
            }

            // --- 2. PREPARE & SAVE USER MESSAGE (Memory Update 1) ---

            Message userMessage = new Message
            {
                // Now guaranteed to be a valid ID from step 1
                ConversationID = clientPrompt.ConversationID,
                UserID = userId,
                SenderType = "human",
                MessageContent = clientPrompt.MessageContent
            };

            // Save the user's message to the database.
            var savedUserMessage = await _aiRepository.AddNewMessageAsync(userMessage);


            // --- 3. RETRIEVE HISTORY & CALL LLM ---

            // Retrieve the context (last 10 messages including the new one).
            const int HistoryLimit = 11;

            List<AiPromptMessage> promptHistoryt = (await _aiRepository.GetLastMessagesAsync( clientPrompt.ConversationID, HistoryLimit))
                .Select(m => new AiPromptMessage
                {
                    // Only get the essential and not sensitive Data , for both perfromance AND security.
                    Sender = m.SenderType, 
                    Content = m.MessageContent
                }).ToList(); ;

            // Call the LLM (Large Language Model)
            string aiResponseContent = await _externalLLMClient.GenerateResponseAsync(promptHistoryt, clientPrompt.MessageContent);
            // 


            // --- 4. PREPARE & SAVE AI RESPONSE AND UPDATE CONVERSATION ---

            Message aiMessage = new Message
            {
                ConversationID = clientPrompt.ConversationID,
                UserID = userId,
                SenderType = "ai",
                MessageContent = aiResponseContent
            };

            // A. Save the AI's response to the database.
            var savedAiMessage = await _aiRepository.AddNewMessageAsync(aiMessage);

            // B. If this was a NEW chat, generate and save the title now.
            if (clientPrompt.ConversationID == convoShell.ConversationID && convoShell.ConversationTitle == "temp title")
            {
                // Simple title generation (could be an LLM call or simple substring)
                string newTitle = await _externalLLMClient.GenerateTitleFromPrompt(clientPrompt.MessageContent);

                // Update the conversation entity in the database
                await _aiRepository.UpdateConversationTitleAsync(convoShell.ConversationID, newTitle);
            }


            // --- 5. RETURN SUCCESS RESPONSE (Manual Mapping) ---

            // Using MessageResponseDTO for clarity, mapped from savedAiMessage.
            ClientMessageDTO responseDto = new ClientMessageDTO
            {
                MessageID = savedAiMessage.MessageID,
                ConversationID = savedAiMessage.ConversationID,
                SenderType = savedAiMessage.SenderType,
                MessageContent = savedAiMessage.MessageContent,
                DateCreated = savedAiMessage.DateCreated
            };


            return new AiResponse<ClientMessageDTO>
            {
                Success = true,
                Message = "AI response generated successfully.",
                Data = responseDto
            };
        }       


        public async Task<AiResponse<List<ClientConversationDTO>>> GetAllConversationByUserId(int userId)
        {
            // 1. Await the list of entities
            List<Conversation> entities = await _aiRepository.GetAllConversationShellsByUserIDAsync(userId);

            // 2. MANUALLY MAP the entire list using LINQ Select
            List<ClientConversationDTO> dtos = entities
                .Select(e => new ClientConversationDTO
                {
                    ConversationID = e.ConversationID,
                    ConversationTitle = e.ConversationTitle,
                    DateCreated = e.DateCreated,
                    LastMessageTimestamp = e.DateLastUpdated,                  

                }).ToList();

            // 3. Return the final, wrapped response
            return new AiResponse<List<ClientConversationDTO>>
            {
                Success = true,
                Message = $"Successfully retrieved {dtos.Count} conversations.",
                Data = dtos
            };
        }

        public async Task <AiResponse<List<ClientMessageDTO>>> GetAllMessagesofConvo(int ConvoId , int userId)
        {
            if(!await _aiRepository.ConversationExistsAndIsOwnedAsync(ConvoId , userId))
            {
                throw new UnauthorizedAccessException("Conversation not found or access is denied.");
            }

            List<Message> messageEntities = await _aiRepository.GetAllMessagesForConversationAsync( ConvoId , userId );



            List<ClientMessageDTO> dtos = messageEntities.Select(e => new ClientMessageDTO
            {
                MessageID = e.MessageID,
                 ConversationID = e.ConversationID,
                 DateCreated = e.DateCreated,
                MessageContent = e.MessageContent,
                SenderType = e.SenderType
            }).ToList();

            return new AiResponse<List<ClientMessageDTO>>
            {
                Success = true,
                Message = $"Successfully retrieved {dtos.Count} conversations.",
                Data = dtos
            };
        }

        

    }
}
 