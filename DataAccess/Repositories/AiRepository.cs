using Microsoft.EntityFrameworkCore;
using StudyMate.API.Contracts;
using StudyMate.API.DataAccess.Context;
using StudyMate.API.Models.ModelsAi;
using StudyMate.API.Models.ModelsAuth;

namespace StudyMate.API.DataAccess.Repositories
{
    public class AiRepository : IAiRepository
    {
        private readonly ApplicationDbContext _context;

        public AiRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Conversation Retrieval/Management ---

        // Refactored from your DoesConverstionExist: checks existence
        public async Task<bool> ConversationExistsAndIsOwnedAsync(int convoId, int userId)
        {
            // Executes a single query that returns only TRUE/FALSE
            return await _context.Conversations
                 .AnyAsync(c => c.ConversationID == convoId && c.UserID == userId);
        }

        // Refactored from your GetConversationByUserIDAndConversationID: gets the shell
        public async Task<Conversation> GetConversationShellAsync(int convoId, int userId)
        {
            // Executes one query: SELECT * FROM Conversations WHERE...
            // Does NOT include messages, keeping it fast for existence/title checks.
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.ConversationID == convoId && c.UserID == userId);
        }

        // Refactored from your GetAllConversationsByUserID: gets list of shells
        public async Task<List<Conversation>> GetAllConversationShellsByUserIDAsync(int userId)
        {
            // Executes one query. CRITICAL: Removed .Include(c => c.Messages) 
            // to prevent loading ALL messages for ALL conversations in the sidebar list.
            return await _context.Conversations
                .Where(n => n.UserID == userId)
                .OrderByDescending(c => c.DateLastUpdated) // Order by last message time
                .ToListAsync();
        }

        // New: Creates a new conversation
        public async Task<Conversation> CreateNewConversationAsync(Conversation conversation)
        {
            _context.Conversations.Add(conversation);
            // CRITICAL: Must use SaveChangesAsync()
            await _context.SaveChangesAsync();
            return conversation;
        }


        // --- Message/Memory Management ---

        // NEW: The essential method for AI memory context (solves over-fetching)
        public async Task<List<Message>> GetLastMessagesAsync(int convoId, int count)
        {
            // Executes one query: SELECT * FROM Messages WHERE... ORDER BY... LIMIT @count
            return await _context.Messages
                .Where(m => m.ConversationID == convoId)
                .OrderByDescending(m => m.DateCreated)
                .Take(count)
                .ToListAsync();
        }

        // Refactored from your AddNewMessage: made async
        public async Task<Message> AddNewMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            // CRITICAL: Must use SaveChangesAsync()
            await _context.SaveChangesAsync();

            return message;
        }

        public async Task<string> UpdateConversationTitleAsync(int conversationId, string newTitle)
        {
            // 1. Retrieve the existing conversation entity
            var convo = await _context.Conversations
                .FirstOrDefaultAsync(c => c.ConversationID == conversationId);

            if (convo == null)
            {
                // Handle case where ID might be invalid, though the Service should prevent this
                throw new InvalidOperationException($"Conversation ID {conversationId} not found for title update.");
            }

            // 2. Update the properties
            convo.ConversationTitle = newTitle;
            convo.DateLastUpdated = DateTime.UtcNow; // Ensure the modification time is updated

            // 3. Save the changes to the database
            // EF Core tracks the changes and only generates an UPDATE statement for Title and DateLastUpdated.
            await _context.SaveChangesAsync();

            // 4. Return the new title string for confirmation in the service layer
            return newTitle;
        }

        public Task<List<Message>> GetAllMessagesForConversationAsync(int convoId, int userId)
        {
            return _context.Messages.Where(m => m.ConversationID == convoId && m.UserID == userId).ToListAsync();
        }
    }
}
