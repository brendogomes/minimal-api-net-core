using Microsoft.EntityFrameworkCore;
using MinimalApiCrud.Data;

namespace MinimalApiCrud.Students;

public static class StudentsEndpoints
{
    public static void AddStudentsEndpoints(this WebApplication app)
    {
        var studentsRoutes = app.MapGroup("students");

        studentsRoutes.MapPost("", async (AddStudentRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var studentNameExists = await context.Students.AnyAsync(student => student.Name == request.Name, ct);
            
            if (studentNameExists)
                return Results.Conflict("A student with this name already exists.");
            
            var newStudent = new Student(request.Name);
            
            await context.Students.AddAsync(newStudent, ct);
            await context.SaveChangesAsync(ct);

            var studentReturn = new StudentDto(newStudent.Id, newStudent.Name);

            return Results.Ok(studentReturn);
        });

        studentsRoutes.MapGet("", async (AppDbContext context, CancellationToken ct) =>
        {
            var students = await context.Students
                .Where(student => student.Active)
                .Select(student => new StudentDto(student.Id, student.Name))
                .ToListAsync(ct);
            
            return students;
        });

        studentsRoutes.MapPut("{id:guid}", async (Guid id, UpdateStudentRequest request, AppDbContext context, CancellationToken ct) =>
        {
            var student = await context.Students.FirstOrDefaultAsync(student => student.Id == id, ct);

            if (student == null)
                return Results.NotFound();

            student.UpdateStudentName(request.Name);

            await context.SaveChangesAsync(ct);

            return Results.Ok(new StudentDto(student.Id, student.Name));
        });

        studentsRoutes.MapDelete("{id:guid}", async (Guid id, AppDbContext context, CancellationToken ct) =>
        {
            var student = await context.Students.FirstOrDefaultAsync(student => student.Id == id, ct);

            if (student == null)
                return Results.NotFound();

            student.DeactivatedStudent();

            await context.SaveChangesAsync(ct);

            return Results.Ok();
        });
    }
}