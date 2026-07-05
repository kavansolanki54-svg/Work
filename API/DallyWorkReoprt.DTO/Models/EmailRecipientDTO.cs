namespace DallyWorkReoprt.DTO.Models;

public class EmailRecipientDTO
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string? Name { get; set; }
    public string RecipientType { get; set; } = null!;
    public bool ActiveStatus { get; set; }
    public DateTime CreateDate { get; set; }
    public int? EmployeeId { get; set; }
}
