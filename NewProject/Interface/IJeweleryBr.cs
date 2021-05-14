using NewProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewProject.Interface
{
    public interface IJeweleryBr
    {
        UserInfo GetUser(string nam);
        UserInfo GetUserInfo(Guid id);
    }
}
