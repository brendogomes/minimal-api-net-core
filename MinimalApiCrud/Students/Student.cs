namespace MinimalApiCrud.Students;

public class Student
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public bool Active { get; private set; }

    public Student(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
        Active = true;
    }

    public void UpdateStudentName(string name)
    {
        Name = name;
    }
    
    public void DeactivatedStudent()
    {
        Active = false;
    }
}

