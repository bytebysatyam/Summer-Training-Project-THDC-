using Microsoft.AspNetCore.Mvc;
using WebPage.Data;


public class LeaveController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Leave/Apply
    public IActionResult Apply()
    {
        return View();
    }

    // POST: Leave/Apply
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Apply(LeaveApplication model)
    {
        if (ModelState.IsValid)
        {
            _context.LeaveApplications.Add(model);
            _context.SaveChanges();

            TempData["ToastMessage"] = "Leave Submitted!. You will be notified once it is reviewed by the admin.";

        }

        return View(model);
    }

    public IActionResult Confirmation()
    {
        return View();
    }

    public IActionResult MyLeaveStatus()
    {
        string? userEmail = HttpContext.Session.GetString("UserEmail");

        if (string.IsNullOrEmpty(userEmail))
        {
            TempData["ToastMessage"] = "You need to log in to view your leave status.";
            return RedirectToAction("Login", "Account");
        }

        var leaves = _context.LeaveApplications
            .Where(l => l.Email == userEmail)
            .OrderByDescending(l => l.AppliedOn)
            .ToList();

        return View(leaves);
    }
}

