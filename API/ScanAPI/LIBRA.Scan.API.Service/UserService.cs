using AutoMapper;
using LIBRA.Scan.API.Common.Common;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly IConfiguration _config;

        public UserService(UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
        }

        public async Task<RequestResponse> Authencate([FromBody] AuthRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new RequestResponse(){
                    StatusCode = Code.NotFound,
                    Content = string.Empty,
                    Message = "Tài khoản không tồn tại!"
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!result.Succeeded)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.Unauthorized,
                    Content = string.Empty,
                    Message = "Sai mật khẩu!"
                };
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                user.Id.ToString(),
                expires: DateTime.Now.AddHours(3),
                signingCredentials: creds);

            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = new JwtSecurityTokenHandler().WriteToken(token),
                Message = "Đăng nhập thành công!"
            }; 
        }
    }
}
