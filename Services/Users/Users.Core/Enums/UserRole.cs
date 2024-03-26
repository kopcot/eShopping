using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Core.Enums
{
    public static class UserRole
    {
        public enum RoleType
        {
            None,
            Anonymous = None,
            User,
            Admin
        }
    }
}
