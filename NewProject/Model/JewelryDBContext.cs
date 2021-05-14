using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace NewProject.Model
{
    public class JewelryDBContext: IdentityDbContext<UserInfo>
    {
        
        public JewelryDBContext(DbContextOptions<JewelryDBContext> _dbcontextoptions ):base(_dbcontextoptions)
        {

            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
