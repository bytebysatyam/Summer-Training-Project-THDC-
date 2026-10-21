using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;
using WebPage.Models;

namespace WebPage.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                var verification = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
                if (verification != PasswordVerificationResult.Failed)
                {
                    if (verification == PasswordVerificationResult.SuccessRehashNeeded)
                    {
                        user.Password = _passwordHasher.HashPassword(user, model.Password);
                        await _context.SaveChangesAsync();
                    }

                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", user.Role);
                    HttpContext.Session.SetString("UserName", user.FullName);

                    TempData["ToastMessage"] = "Login successful!";
                    TempData["ToastType"] = "success";

                    return user.Role == "Admin"
                        ? RedirectToAction("Dashboard", "Admin")
                        : RedirectToAction(nameof(Dashboard));
                }
            }

            TempData["ToastMessage"] = "Invalid email or password.";
            TempData["ToastType"] = "danger";
            return View(model);
        }

        public IActionResult Dashboard()
        {
            var name = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrWhiteSpace(name))
                return RedirectToAction(nameof(Login));

            ViewBag.Name = name;
            return View();
        }

        public IActionResult Career() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["ToastMessage"] = "Logged out successfully.";
            TempData["ToastType"] = "info";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLowerInvariant();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null)
            {
                user.Password = _passwordHasher.HashPassword(user, model.NewPassword);
                await _context.SaveChangesAsync();
            }

            // Generic response avoids disclosing whether an account exists.
            TempData["ToastMessage"] = "If the account exists, the password has been reset.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var email = model.Email.Trim().ToLowerInvariant();
            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already registered.");
                return View(model);
            }

            var user = new User
            {
                FullName = model.FullName.Trim(),
                Email = email,
                Role = "User"
            };
            user.Password = _passwordHasher.HashPassword(user, model.Password);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["ToastMessage"] = "Registration successful. You can now log in.";
            TempData["ToastType"] = "success";
            return RedirectToAction(nameof(Login));
        }
    }
}
