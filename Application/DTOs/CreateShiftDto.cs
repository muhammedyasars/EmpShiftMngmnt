namespace Application.DTOs;

public class CreateShiftDto
{
    public long EmployeeId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }
}