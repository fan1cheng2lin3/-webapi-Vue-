using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWordStystemWebapi.Data;
using MyWordStystemWebapi.Models;
using MyWordStystemWebapi.Services.Email;
using MyWordStystemWebapi.Services.Interfaces;
using MyWordStystemWebapi.ViewModels;

namespace MyWordStystemWebapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly EmailService _emailService;

        public UsersController(IUserService userService, EmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(AuthenticateRequest model)
        {
            var user = _userService.GetUserByEmail(model.Email);
            if (user == null)
            {
                return BadRequest(new { message = "用户不存在" });
            }

            var newPassword = GenerateRandomPassword();
            var updateResult = _userService.ChangePassword(user.Userid, newPassword);
            if (!updateResult)
            {
                return BadRequest(new { message = "密码重置失败" });
            }

            // 发送邮件通知用户密码已更改
            _emailService.SendEmail(model.Email, "密码重置", $"您的新密码是：{newPassword}");
            return Ok(new { message = "密码重置成功，请查收您的邮箱" });
        }


        private string GenerateRandomPassword()
        {
            const string chars = "0123456789";
            var random = new Random();
            var password = new char[6];
            for (var i = 0; i < password.Length; i++)
            {
                password[i] = chars[random.Next(chars.Length)];
            }
            return new string(password);
        }



        [HttpPost("auth")]
        public ActionResult<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var response = _userService.Autnenticate(model);
            if (response == null)
            {
                return BadRequest(new { message = "用户名或者密码不正确！" });
            }
            return Ok(response);
        }




        [HttpPut("change-password")]
        public ActionResult<bool> ChangePassword(ChangePasswordRequest model)
        {
            var success = _userService.ChangePassword(model.UserId, model.CurrentPassword, model.NewPassword);
            if (!success)
            {
                return BadRequest(new { message = "修改密码失败，用户名不存在或当前密码不正确。" });
            }
            return Ok(true);
        }



        [HttpPost("register")]
        public ActionResult<bool> Register(User user, string password)
        {
            var success = _userService.RegisterUser(user, password);
            if (!success)
            {
                return BadRequest(new { message = "注册失败，用户已存在或输入无效。" });
            }
            return Ok(true);
        }


      




    }
}