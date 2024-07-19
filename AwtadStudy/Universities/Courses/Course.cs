namespace AwtadStudy.Universities.Courses;

/// <summary>
/// Course database entity stored as jsonb.
/// </summary>
public sealed class Course
{
    // cant be named Id because of https://github.com/dotnet/efcore/issues/29380
    public required string Identifier { get; set; }
    public required string LectureGroupId { get; set; }
    public required string PracticeGroupId { get; set; }
    public required Semester Semester { get; set; }
}
