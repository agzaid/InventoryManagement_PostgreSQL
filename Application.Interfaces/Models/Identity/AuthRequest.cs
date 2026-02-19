using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.Models.Identity
{
    public class AuthRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
