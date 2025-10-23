using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Reflection.Emit;
using WebPage.Data;
using WebPage.Models;

public class ComplaintController : Controller
{
    private readonly ApplicationDbContext _context;

    public ComplaintController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Complaint/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Complaint/Create
    [HttpPost]
    public IActionResult Create(Complaint model)
    {
        if (ModelState.IsValid)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            model.UserEmail = userEmail;
            model.DateCreated = DateTime.Now;

            _context.Complaints.Add(model);
            _context.SaveChanges();

            TempData["ToastMessage"] = "Complaint submitted successfully!";
            return RedirectToAction("MyComplaints");
        }

        return View(model); // Show same form again if model is invalid
    }

    public IActionResult MyComplaints()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login", "Account"); // user not logged in
        }

        var complaints = _context.Complaints
                                 .Where(c => c.UserEmail == email)
                                 .ToList();

        return View(complaints);
    }

    // GET: Complaint/Edit/5
    public IActionResult Edit(int id)
    {
        var complaint = _context.Complaints.FirstOrDefault(c => c.Id == id);
        if (complaint == null)
        {
            return NotFound();
        }

        if (!string.IsNullOrEmpty(complaint.AdminResponse))
        {
            return BadRequest("You cannot edit this complaint because it has already been responded to by the admin.");
        }

        var email = HttpContext.Session.GetString("UserEmail");
        if (complaint.UserEmail != email)
        {
            return Unauthorized();
        }

        return View(complaint);
    }

    // POST: Complaint/Edit/5
    [HttpPost]
    public IActionResult Edit(int id, Complaint updatedComplaint)
    {
        if (id != updatedComplaint.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var complaint = _context.Complaints.FirstOrDefault(c => c.Id == id);
            if (complaint == null)
            {
                return NotFound();
            }

            var email = HttpContext.Session.GetString("UserEmail");
            if (complaint.UserEmail != email)
            {
                return Unauthorized();
            }

            complaint.Subject = updatedComplaint.Subject;
            complaint.Description = updatedComplaint.Description;

            _context.SaveChanges();

            TempData["ToastMessage"] = "Complaint updated successfully!";
            return RedirectToAction("MyComplaints");
        }

        return View(updatedComplaint);
    }
}
