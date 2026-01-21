using System.Security.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using StudyMate.API.Contracts;
using StudyMate.API.DataAccess;
using StudyMate.API.Models.ModelsAuth;
using StudyMate.API.Models;

namespace StudyMate.API.Services
{
    public class AuthService 
    {

        private readonly IUsersRepository _usersRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;

       public AuthService(IUsersRepository usersRepository, IJwtTokenGenerator tokenGenerator)
        {
            _usersRepository = usersRepository;
            _tokenGenerator = tokenGenerator;
        }

        public  LogInResponse Authenticate(LogInRequest LoginDTO)
        {
            var user = _usersRepository.GetByUsername(LoginDTO.UserName);

            if (user == null) {


                throw new AuthenticationException("Invalid credentials");

            }
            //using hash function 
            //we get the password from the client and then put it with the hash from the Database
            //Database doesnt store raw password , it stores hashes
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(LoginDTO.Password, user.Password);

            if (!isPasswordValid)
            {

                throw new AuthenticationException("Invalid credentials");
            }

            string tokenString = _tokenGenerator.GenerateToken(user);

            return new LogInResponse
            {
                UserID = user.UserID,
                UserName = user.UserName,
                Token = tokenString
                //  UserName = user.UserName
                // DO NOT map the PasswordHash
            };
        }
        
    }
}
