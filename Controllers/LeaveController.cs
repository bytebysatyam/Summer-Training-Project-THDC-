using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;

public class LeaveController : Controller
{
    private readonly ApplicationDbContext _context;

    public LeaveController(ApplicationDbContext context) => _context = context;

    private string? CurrentEmail => HttpContext.Session.GetString("UserEmail");
    private string? CurrentName => HttpContext.Session.GetString("UserName");

    [HttpGet]
    public IActionResult Apply()
    {
        if (string.IsNullOrWhiteSpace(CurrentEmail))
            return RedirectToAction("Login", "Account");

        return View(new LeaveApplication
        {
            Email = CurrentEmail,
            FullName = CurrentName ?? string.Empty,
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(LeaveApplication model)
    {
        if (string.IsNullOrWhiteSpace(CurrentEmail))
            return RedirectToAction("Login", "Account");

        // Identity comes from the authenticated session, not user-submitted fields.
        model.Email = CurrentEmail;
        model.FullName = CurrentName ?? model.FullName;
        model.Status = "Pending";
        model.AppliedOn = DateTime.UtcNow;

        ModelState.Remove(nameof(model.Email));
        ModelState.Remove(nameof(model.FullName));
        ModelState.Remove(nameof(model.Status));

        if (!ModelState.IsValid) return View(model);

        _context.LeaveApplications.Add(model);
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Leave submitted successfully. You will see the admin decision in your leave status.";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(MyLeaveStatus));
    }

    public async Task<IActionResult> MyLeaveStatus()
    {
        if (string.IsNullOrWhiteSpace(CurrentEmail))
            return RedirectToAction("Login", "Account");

        var leaves = await _context.LeaveApplications
            .Where(l => l.Email == CurrentEmail)
            .OrderByDescending(l => l.AppliedOn)
            .ToListAsync();

        return View(leaves);
    }
}
