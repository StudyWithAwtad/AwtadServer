using AwtadStudy.Universities.Courses;

namespace AwtadStudy.Universities;

public interface IUniversityService
{
    Task<Course> GetCourse(string courseID, Semester semester);

    IAsyncEnumerable<Course> GetCoursesForUser(string userID);

    Task RegisterCourseForUser(string userID, string courseID, string groupID);
}
