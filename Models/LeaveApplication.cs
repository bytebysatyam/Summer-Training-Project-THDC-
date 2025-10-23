using System;
using System.ComponentModel.DataAnnotations;

public class LeaveApplication
{
    public int Id { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required]
    public string LeaveType { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public string Reason { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime AppliedOn { get; set; } = DateTime.Now;
}
