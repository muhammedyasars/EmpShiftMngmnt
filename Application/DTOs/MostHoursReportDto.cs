using Domain;

namespace Application.DTOs;

public class MostHoursReportDto
{
    public EmployeeDto? Employee { get; set; }
    public decimal Hours { get; set; }
}