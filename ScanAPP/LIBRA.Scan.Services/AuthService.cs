using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Services.Constracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services
{
    public class AuthService : IAuthService
    {
        private readonly ScanAppContext _context;

        public AuthService() 
        {
            _context = new ScanAppContext();
        }

        public async Task<long> Create(TokenCreateRequest request)
        {
            Token token = new Token()
            {
                token = request.token,
                created_date = request.created_date
            };

            try
            {
                await _context.tokens.AddAsync(token);
                await _context.SaveChangesAsync();
                return token.id;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public string GetLatestToken()
        {
            var token = _context.tokens.OrderByDescending(x => x.id).FirstOrDefault();

            if(token == null)
                return String.Empty;

            return token.token;
        }
    }
}
