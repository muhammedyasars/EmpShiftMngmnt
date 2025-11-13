using Application.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Application.UseCases.Employees;
using Application.UseCases.Shifts;
using Application.UseCases.Reports;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IShiftRepository, ShiftRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<CreateEmployeeCommand>();
builder.Services.AddScoped<GetEmployeeQuery>();
builder.Services.AddScoped<UpdateEmployeeCommand>();
builder.Services.AddScoped<CreateShiftCommand>();
builder.Services.AddScoped<GetShiftQuery>();
builder.Services.AddScoped<UpdateShiftCommand>();
builder.Services.AddScoped<ReportQueries>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<EmpShifMngmnt.Middleware.GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();