using NewProject.Interface;
using NewProject.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewProject.Repository
{
    public class JeweleryBr: IJeweleryBr
    {
        private readonly JewelryDBContext _JewelryDBContext;

        public JeweleryBr( JewelryDBContext JewelryDBContext)
        {
            
            _JewelryDBContext = JewelryDBContext;
        }
        public UserInfo GetUser(string username)
        {
          return  _JewelryDBContext.Users.Where(q => q.Name == username).FirstOrDefault();
            
        }

        public UserInfo GetUserInfo(Guid id)
        {
            return _JewelryDBContext.Users.Where(q => q.Id == id).FirstOrDefault();

        }
    }
}
