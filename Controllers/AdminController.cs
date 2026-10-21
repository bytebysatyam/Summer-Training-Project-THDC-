using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context) => _context = context;

    private bool IsAdmin => HttpContext.Session.GetString("UserRole") == "Admin";

    private IActionResult? RequireAdmin()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserEmail")))
            return RedirectToAction("Login", "Account");
        return IsAdmin ? null : Forbid();
    }

    public IActionResult Dashboard()
    {
        var denied = RequireAdmin();
        return denied ?? View();
    }

    public async Task<IActionResult> Complaints()
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        return View(await _context.Complaints.OrderByDescending(c => c.DateCreated).ToListAsync());
    }

    [HttpGet]
    public async Task<IActionResult> Respond(int id)
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
        return complaint == null ? NotFound() : View(complaint);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(int id, string adminResponse, string status)
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        if (string.IsNullOrWhiteSpace(adminResponse) || (status != "Resolved" && status != "Rejected"))
        {
            TempData["ToastMessage"] = "Response and a valid status are required.";
            TempData["ToastType"] = "warning";
            return RedirectToAction(nameof(Respond), new { id });
        }

        var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
        if (complaint == null) return NotFound();

        complaint.AdminResponse = adminResponse.Trim();
        complaint.Status = status;
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Response submitted successfully!";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(Complaints));
    }

    public async Task<IActionResult> LeaveRequests()
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        return View(await _context.LeaveApplications.OrderByDescending(l => l.AppliedOn).ToListAsync());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        if (status != "Approved" && status != "Rejected") return BadRequest();

        var leave = await _context.LeaveApplications.FirstOrDefaultAsync(l => l.Id == id);
        if (leave == null) return NotFound();

        leave.Status = status;
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = $"Leave {status}!";
        TempData["ToastType"] = status == "Approved" ? "success" : "danger";
        return RedirectToAction(nameof(LeaveRequests));
    }
}
