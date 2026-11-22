using System;
using System.Collections.Generic;
using System.Text;

namespace FoodRecognitionApp.Shared.Dtos.Auth
{
    public class IdentityUserResult
    {
        public IdentityUserResult(Guid id, string email, string displayName, string userName)
        {
            Id = id;
            Email = email;
            DisplayName = displayName;
            UserName = userName;
        }

        public Guid Id { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string UserName { get; set; } = default!;
    }
}
