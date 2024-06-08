using AwtadStudy.Course;

namespace AwtadStudy.Database;

public sealed record CourseDTO(string CourseID, Semester Semester, IEnumerable<string> GroupIDs);
