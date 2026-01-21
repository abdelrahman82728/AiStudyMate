using StudyMate.API.Models.ModelsAi;

namespace StudyMate.API.Contracts
{
    public interface IAiRepository
    {
        // --- Conversation Retrieval/Management ---

        // Efficiently checks if a conversation exists AND belongs to the user.
        Task<bool> ConversationExistsAndIsOwnedAsync(int convoId, int userId);

        // Gets the Conversation entity without loading messages (the "shell").
        Task<Conversation> GetConversationShellAsync(int convoId, int userId);

        // Gets the list of conversations for the user (for the chat sidebar).
        // Does NOT load messages to keep it fast.
        Task<List<Conversation>> GetAllConversationShellsByUserIDAsync(int userId);

        // Creates a new conversation shell.
        Task<Conversation> CreateNewConversationAsync(Conversation conversation);


        // --- Message/Memory Management ---

        // CRITICAL for AI memory: Gets only the last N messages.
        Task<List<Message>> GetLastMessagesAsync(int convoId, int count);

        // Adds a new message (user prompt or AI response) to the database.
        Task<Message> AddNewMessageAsync(Message message);

        Task <string> UpdateConversationTitleAsync(int ConversationID, string newTitle);

        Task<List<Message>> GetAllMessagesForConversationAsync(int convoId, int userId);
    }
}
