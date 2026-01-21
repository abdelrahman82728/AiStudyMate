using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudyMate.API.Models.ModelsAuth;
using StudyMate.API.Services;
namespace StudyMate.API.Controllers
{
    [Route("api/Auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ProgressServices _progressServices;


        public AuthController(AuthService authService , ProgressServices progressServices)
        {
            // 3. Store the instance provided by the framework
            _authService = authService;
            _progressServices = progressServices;

        }

        [HttpPost("LoginUser", Name = "Loginuser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public ActionResult<LogInResponse> LogInUser([FromBody]  LogInRequest user)
        {

            try
            {
                LogInResponse Results = _authService.Authenticate(user);

                _progressServices.RecordLoginAsync(Results.UserID);

                return Ok(Results);

            }

            catch (AuthenticationException) {
                return Unauthorized(new
                {
                    Message = "Wrong username or password."
                });
            }
        }


        [HttpGet("{password}")]

        public ActionResult<string> gethashedpassword(string password) { 
        
        return Ok(BCrypt.Net.BCrypt.HashPassword(password));
        }
    }

    

  
}
