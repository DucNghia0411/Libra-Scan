using LIBRA.Scan.API.Data.EFs;
using LIBRA.Scan.API.Data.Repositories.Constracts;
using LIBRA.Scan.API.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Data.Repositories
{
    public class UserRepo : GenericRepository<User>, IUserRepo
    {
        public UserRepo(ScanAppContext context) : base(context)
        {
        }
    }
}
