using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;
using WebPage.Models;

public class ComplaintController : Controller
{
    private readonly ApplicationDbContext _context;

    public ComplaintController(ApplicationDbContext context) => _context = context;

    private string? CurrentEmail => HttpContext.Session.GetString("UserEmail");
    private bool IsLoggedIn => !string.IsNullOrWhiteSpace(CurrentEmail);

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");
        return View(new Complaint());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Complaint model)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        ModelState.Remove(nameof(model.UserEmail));
        ModelState.Remove(nameof(model.Status));
        ModelState.Remove(nameof(model.AdminResponse));

        if (!ModelState.IsValid) return View(model);

        model.UserEmail = CurrentEmail;
        model.DateCreated = DateTime.UtcNow;
        model.Status = "Pending";
        model.AdminResponse = null;

        _context.Complaints.Add(model);
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Complaint submitted successfully!";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(MyComplaints));
    }

    public async Task<IActionResult> MyComplaints()
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var complaints = await _context.Complaints
            .Where(c => c.UserEmail == CurrentEmail)
            .OrderByDescending(c => c.DateCreated)
            .ToListAsync();

        return View(complaints);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");

        var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
        if (complaint == null) return NotFound();
        if (complaint.UserEmail != CurrentEmail) return Forbid();
        if (!string.IsNullOrEmpty(complaint.AdminResponse) || complaint.Status != "Pending")
            return BadRequest("You cannot edit a complaint after an admin has processed it.");

        return View(complaint);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Complaint updatedComplaint)
    {
        if (!IsLoggedIn) return RedirectToAction("Login", "Account");
        if (id != updatedComplaint.Id) return BadRequest();

        ModelState.Remove(nameof(updatedComplaint.UserEmail));
        ModelState.Remove(nameof(updatedComplaint.Status));
        ModelState.Remove(nameof(updatedComplaint.AdminResponse));

        if (!ModelState.IsValid) return View(updatedComplaint);

        var complaint = await _context.Complaints.FirstOrDefaultAsync(c => c.Id == id);
        if (complaint == null) return NotFound();
        if (complaint.UserEmail != CurrentEmail) return Forbid();
        if (!string.IsNullOrEmpty(complaint.AdminResponse) || complaint.Status != "Pending")
            return BadRequest("You cannot edit a complaint after an admin has processed it.");

        complaint.Subject = updatedComplaint.Subject.Trim();
        complaint.Description = updatedComplaint.Description.Trim();
        await _context.SaveChangesAsync();

        TempData["ToastMessage"] = "Complaint updated successfully!";
        TempData["ToastType"] = "success";
        return RedirectToAction(nameof(MyComplaints));
    }
}
