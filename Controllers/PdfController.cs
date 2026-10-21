using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using WebPage.Data;

public class PdfController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _converter;

    public PdfController(ApplicationDbContext context, IConverter converter)
    {
        _context = context;
        _converter = converter;
    }

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

        var complaints = await _context.Complaints.OrderByDescending(c => c.DateCreated).ToListAsync();
        var html = new StringBuilder("<h2 style='text-align:center;'>Complaints Report</h2>");
        html.Append("<table border='1' width='100%' cellpadding='5'><tr><th>ID</th><th>Subject</th><th>Status</th><th>Date</th></tr>");

        foreach (var c in complaints)
            html.Append($"<tr><td>{c.Id}</td><td>{WebUtility.HtmlEncode(c.Subject)}</td><td>{WebUtility.HtmlEncode(c.Status)}</td><td>{c.DateCreated:dd-MMM-yyyy}</td></tr>");

        html.Append("</table>");
        return BuildPdf(html.ToString(), "ComplaintsReport.pdf");
    }

    public async Task<IActionResult> ExportLeaves()
    {
        var denied = RequireAdmin();
        if (denied != null) return denied;

        var leaves = await _context.LeaveApplications.OrderByDescending(l => l.AppliedOn).ToListAsync();
        var html = new StringBuilder("<h2 style='text-align:center;'>Leave Applications Report</h2>");
        html.Append("<table border='1' width='100%' cellpadding='5'><tr><th>ID</th><th>Full Name</th><th>From</th><th>To</th><th>Reason</th><th>Status</th></tr>");

        foreach (var l in leaves)
            html.Append($"<tr><td>{l.Id}</td><td>{WebUtility.HtmlEncode(l.FullName)}</td><td>{l.StartDate:dd-MMM-yyyy}</td><td>{l.EndDate:dd-MMM-yyyy}</td><td>{WebUtility.HtmlEncode(l.Reason)}</td><td>{WebUtility.HtmlEncode(l.Status)}</td></tr>");

        html.Append("</table>");
        return BuildPdf(html.ToString(), "LeavesReport.pdf");
    }

    private FileContentResult BuildPdf(string html, string filename)
    {
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
            Objects = { new ObjectSettings { HtmlContent = html } }
        };

        var pdf = _converter.Convert(doc);
        return File(pdf, "application/pdf", filename);
    }
}
