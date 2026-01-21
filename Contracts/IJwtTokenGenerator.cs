using StudyMate.API.Models.ModelsAuth;

namespace StudyMate.API.Contracts
{
    public interface IJwtTokenGenerator
    {
            string GenerateToken(User user);
        
    }
}
