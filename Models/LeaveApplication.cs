using System.ComponentModel.DataAnnotations;

public class LeaveApplication : IValidatableObject
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string LeaveType { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    [Required, StringLength(1500)]
    public string Reason { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";
    public DateTime AppliedOn { get; set; } = DateTime.UtcNow;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.Date < DateTime.UtcNow.Date)
            yield return new ValidationResult("Start date cannot be in the past.", new[] { nameof(StartDate) });

        if (EndDate.Date < StartDate.Date)
            yield return new ValidationResult("End date must be on or after the start date.", new[] { nameof(EndDate) });
    }
}
