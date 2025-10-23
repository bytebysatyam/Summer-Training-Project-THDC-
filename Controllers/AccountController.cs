using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Data;
using WebPage.Data;
using WebPage.Models;

namespace WebPage.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u =>
                u.Email == model.Email && u.Password == model.Password);

            if (user != null)
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetString("UserRole", user.Role ?? "User");
                HttpContext.Session.SetString("UserName", user.FullName);

                TempData["ToastMessage"] = "Login successful!";
                TempData["ToastType"] = "success";

                if (user.Role == "Admin")
                    return RedirectToAction("Dashboard", "Admin");
                else
                    return RedirectToAction("Dashboard", "Account");
            }

            TempData["ToastMessage"] = "Invalid email or password.";
            TempData["ToastType"] = "danger";
            return View(model);
        }

        public IActionResult Dashboard()
        {
            var name = HttpContext.Session.GetString("UserName");
            if (name == null)
                return RedirectToAction("Login");

            ViewBag.Name = name;
            return View();
        }

        public IActionResult Career()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["ToastMessage"] = "Logged out successfully.";
            TempData["ToastType"] = "info";
            return RedirectToAction("Login");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "ResetUserPassword";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var emailParam = command.CreateParameter();
                    emailParam.ParameterName = "userEmail";
                    emailParam.Value = model.Email;
                    command.Parameters.Add(emailParam);

                    var passwordParam = command.CreateParameter();
                    passwordParam.ParameterName = "newPassword";
                    passwordParam.Value = model.NewPassword;
                    command.Parameters.Add(passwordParam);

                    command.ExecuteNonQuery();
                }
            }

            TempData["ToastMessage"] = "Password reset successfully.";
            TempData["ToastType"] = "success";
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (existingUser != null)
            {
                TempData["ToastMessage"] = "Email already registered.";
                TempData["ToastType"] = "warning";
                return View(model);
            }

            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "RegisterUser";
                    command.CommandType = CommandType.StoredProcedure;

                    var param1 = command.CreateParameter();
                    param1.ParameterName = "fullName";
                    param1.Value = model.FullName;
                    command.Parameters.Add(param1);

                    var param2 = command.CreateParameter();
                    param2.ParameterName = "userEmail";
                    param2.Value = model.Email;
                    command.Parameters.Add(param2);

                    var param3 = command.CreateParameter();
                    param3.ParameterName = "userPassword";
                    param3.Value = model.Password;
                    command.Parameters.Add(param3);

                    command.ExecuteNonQuery();
                }
            }

            TempData["ToastMessage"] = "Registration successful. You can now log in.";
            TempData["ToastType"] = "success";
            return RedirectToAction("Login");
        }
    }
}

