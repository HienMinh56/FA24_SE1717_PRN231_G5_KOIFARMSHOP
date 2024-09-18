using KoiFarmShop.Data.Base;
using KoiFarmShop.Data.Models;

namespace KoiFarmShop.Data.Repository
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository() { }

        public UserRepository(FA24_SE1717_PRN231_G5_KOIFARMSHOPContext context) => _context = context;
    }
}