using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Mvc;
using WebPage.Data;
using System.Text;

public class PdfController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _converter;

    public PdfController(ApplicationDbContext context, IConverter converter)
    {
        _context = context;
        _converter = converter;
    }

    public IActionResult ExportComplaints()
    {
        var complaints = _context.Complaints.ToList();

        var html = new StringBuilder();
        html.Append("<h2 style='text-align:center;'>Complaints Report</h2>");
        html.Append("<table border='1' width='100%' cellpadding='5'><tr><th>ID</th><th>Title</th><th>Status</th><th>Date</th></tr>");

        foreach (var c in complaints)
        {
            html.Append($"<tr><td>{c.Id}</td><td>{c.Subject}</td><td>{c.Status}</td><td>{c.DateCreated.ToShortDateString()}</td></tr>");
        }

        html.Append("</table>");

        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
            Objects = { new ObjectSettings { HtmlContent = html.ToString() } }
        };

        var pdf = _converter.Convert(doc);
        return File(pdf, "application/pdf", "ComplaintsReport.pdf");
    }

    public IActionResult ExportLeaves()
    {
        var leaves = _context.LeaveApplications.ToList();

        var html = new StringBuilder();
        html.Append("<h2 style='text-align:center;'>Leave Applications Report</h2>");
        html.Append("<table border='1' width='100%' cellpadding='5'><tr><th>ID</th><th>Full Name</th><th>From</th><th>To</th><th>Reason</th><th>Status</th></tr>");

        foreach (var l in leaves)
        {
            html.Append($"<tr><td>{l.Id}</td><td>{l.FullName}</td><td>{l.StartDate:dd-MMM-yyyy}</td><td>{l.EndDate:dd-MMM-yyyy}</td><td>{l.Reason}</td><td>{l.Status}</td></tr>");
        }

        html.Append("</table>");

        var doc = new HtmlToPdfDocument()
        {
            GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
            Objects = { new ObjectSettings { HtmlContent = html.ToString() } }
        };

        var pdf = _converter.Convert(doc);
        return File(pdf, "application/pdf", "LeavesReport.pdf");
    }
}
