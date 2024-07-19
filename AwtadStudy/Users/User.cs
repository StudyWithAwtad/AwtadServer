using AwtadStudy.Universities;
using AwtadStudy.Universities.Courses;

namespace AwtadStudy.Users;

/// <summary>
/// User database entity.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Auto incremented ID (IDENTITY).
    /// </summary>
    public long Id { get; init; }
    public required string FirebaseUid { get; init; }
    public required University UniversityId { get; set; }
    public required List<Course> Courses { get; set; }
}
