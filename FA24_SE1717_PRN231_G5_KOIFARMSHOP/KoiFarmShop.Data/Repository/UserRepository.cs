using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;
using Microsoft.EntityFrameworkCore;
using KoiFarmShop.Data.Request;
using System.Threading.Tasks;

namespace KoiFarmShop.Data.Repository
{
    public class UserRepository : GenericRepository<User>
    {
        private readonly FA24_SE1717_PRN231_G5_KOIFARMSHOPContext _context;

        public UserRepository() { }

        public UserRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;


    }
}