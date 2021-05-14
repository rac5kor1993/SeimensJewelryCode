using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewProject.BuisnessLogic;
using NewProject.Model;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NewProject.Interface;

namespace NewProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInfoesController : ControllerBase
    {

        private readonly IJeweleryBL _JewelryBL;
        private readonly UserManager<UserInfo> userinfomanager;
        private readonly RoleManager<IdentityRole> userrolemanager;
        private readonly IConfiguration _configuration;

        public UserInfoesController( UserManager<UserInfo> userinfomanager, RoleManager<IdentityRole> userrolemanager,
            IConfiguration configuration, IJeweleryBL jeweleryBL)
        {
            //_context = context;
            //_JewelryBL = _JewelryDBContext;
            this.userinfomanager = userinfomanager;
            this.userrolemanager = userrolemanager;
            _configuration = configuration;
            _JewelryBL = jeweleryBL;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await userinfomanager.FindByNameAsync(model.Username);
            if (user != null && await userinfomanager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await userinfomanager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return Unauthorized();
        }

        
        // GET: api/UserInfoes/5
        [HttpPost("{user}/Calculate")]
        public async Task<ActionResult<GoldPriceCalculation>> GetCalculatedInfo(string  user,[FromBody] GoldPriceCalculation goldPrice)
        {
            var _user = await userinfomanager.FindByNameAsync(user);
          //  var _userInfo =  _JewelryBL.GetUser(userInfo.Name);           

            if (user == null||_user==null)
            {
                return NotFound();
            }
            else
            {
                var userRoles = await userinfomanager.GetRolesAsync(_user);
                return _JewelryBL.GetTotalGoldPrice(goldPrice, userRoles[0].ToString());
            }
        }

        [HttpPost("{username}/PrintGoldInfo")]
        public  ActionResult DownloadToFile(string username, [FromBody] GoldPriceCalculation goldPrice)
        {
            DateTime dTime = DateTime.Now;
            string strPDFFileName = string.Format(username+" GoldPrice" + dTime.ToString("yyyyMMdd")+ ".pdf");
            var res =  _JewelryBL.CreatePdf(goldPrice);
            return File(res, "application/pdf", strPDFFileName);
        }

        [HttpPost("{username}/PrinterGoldInfo")]
        public ActionResult DownloadToPrinter(string username, [FromBody] GoldPriceCalculation goldPrice)
        {
            throw new NotImplementedException();
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var userExists = await userinfomanager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            UserInfo user = new UserInfo()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userinfomanager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] Register model)
        {
            var userExists = await userinfomanager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            UserInfo user = new UserInfo()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Username
            };
            var result = await userinfomanager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });

            if (!await userrolemanager.RoleExistsAsync(UserRoles.Owner))
                await userrolemanager.CreateAsync(new IdentityRole(UserRoles.Owner));
            if (!await userrolemanager.RoleExistsAsync(UserRoles.Priveleged))
                await userrolemanager.CreateAsync(new IdentityRole(UserRoles.Priveleged));

            if (await userrolemanager.RoleExistsAsync(UserRoles.Owner))
            {
                await userinfomanager.AddToRoleAsync(user, UserRoles.Owner);
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
        
    }
}
