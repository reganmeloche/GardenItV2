using System;

namespace gardenit_api_classes.Auth
{
    public class AuthenticateResponse
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        // public AuthenticateResponse(User user, string token)
        // {
        //     UserId = user.Id;
        //     Email = user.Email;
        //     Token = token;
        // }
    }
}