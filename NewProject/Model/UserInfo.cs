using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace NewProject.Model
{
    public class UserInfo: IdentityUser
    {
        [Key]
        public new Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsPriveleged { get; set; }
    }
}
