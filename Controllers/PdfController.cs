using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;
using WebPage.Services;

public class PdfController : Controller
{
    private readonly ApplicationDbContext _context;

    public PdfController(ApplicationDbContext context) => _context = context;

    private bool IsAdmin => HttpContext.Session.GetString("UserRole") == "Admin";

    private IActionResult? RequireAdmin()
    {
        if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserEmail")))
            return RedirectToAction("Login", "Account");
        return IsAdmin ? null : Forbid();
    }

    public async Task<IActionResult> ExportComplaints()
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        var complaints = await _context.Complaints
            .OrderByDescending(c => c.DateCreated)
            .ToListAsync();

        var lines = complaints.Select(c =>
            $"#{c.Id} | {c.Subject} | {c.Status} | {c.DateCreated:dd-MMM-yyyy}");

        var pdf = SimplePdf.Create(lines, "Complaints Report");
        return File(pdf, "application/pdf", "ComplaintsReport.pdf");
    }

    public async Task<IActionResult> ExportLeaves()
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        var leaves = await _context.LeaveApplications
            .OrderByDescending(l => l.AppliedOn)
            .ToListAsync();

        var lines = leaves.Select(l =>
            $"#{l.Id} | {l.FullName} | {l.StartDate:dd-MMM-yyyy} to {l.EndDate:dd-MMM-yyyy} | {l.Status}");

        var pdf = SimplePdf.Create(lines, "Leave Applications Report");
        return File(pdf, "application/pdf", "LeavesReport.pdf");
    }
}
