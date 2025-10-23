using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPage.Data;
using WebPage.Models;

public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult Dashboard()
    {
        return View();
    }

    public IActionResult Complaints()
    {
        var role = HttpContext.Session.GetString("UserRole");
        if (role != "Admin")
        {
            return Unauthorized(); // Prevent non-admin access
        }

        var complaints = _context.Complaints.ToList();
        return View(complaints);
    }
    // GET: Admin/Respond/5
    public IActionResult Respond(int id)
    {
        var complaint = _context.Complaints.FirstOrDefault(c => c.Id == id);
        if (complaint == null)
        {
            return NotFound();
        }
        return View(complaint);
    }

    // POST: Admin/Respond
    [HttpPost]
    public IActionResult Respond(Complaint updatedComplaint)
    {
        var complaint = _context.Complaints.FirstOrDefault(c => c.Id == updatedComplaint.Id);
        if (complaint == null)
        {
            TempData["ToastMessage"] = "Complaint not found.";
            TempData["ToastType"] = "danger";
            return NotFound();
        }

        complaint.AdminResponse = updatedComplaint.AdminResponse;
        complaint.Status = updatedComplaint.Status;
        _context.SaveChanges();

        // ✅ Toast for response submitted
        TempData["ToastMessage"] = "Response submitted successfully!";
        TempData["ToastType"] = "success";

        return RedirectToAction("Complaints");
    }
    public IActionResult LeaveRequests()
    {
        var leaves = _context.LeaveApplications
            .OrderByDescending(l => l.AppliedOn)
            .ToList();
        return View(leaves);
    }
    [HttpPost]
    public IActionResult UpdateStatus(int id, string status)
    {
        var leave = _context.LeaveApplications.FirstOrDefault(l => l.Id == id);
        if (leave != null && (status == "Approved" || status == "Rejected"))
        {
            leave.Status = status;
            _context.SaveChanges();
            // ✅ Toast for leave status update
            TempData["ToastMessage"] = $"Leave {status}!";
            TempData["ToastType"] = status == "Approved" ? "success" : "danger";
        }
        else
        {
            TempData["ToastMessage"] = "Unable to update leave status.";
            TempData["ToastType"] = "warning";
        }
        return RedirectToAction("LeaveRequests");
    }

}

