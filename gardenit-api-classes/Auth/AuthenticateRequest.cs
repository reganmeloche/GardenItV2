using System;

namespace gardenit_api_classes.Auth
{
    public class AuthenticateRequest
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}