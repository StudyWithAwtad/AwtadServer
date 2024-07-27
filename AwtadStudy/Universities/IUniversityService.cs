using AwtadStudy.Universities.Courses;

namespace AwtadStudy.Universities;

public interface IUniversityService
{
    Task<CourseInfo> GetCourse(string courseId, Semester semester);

    IAsyncEnumerable<CourseInfo> GetCoursesForUser(long userId);

    Task RegisterCourseForUser(long userId, Course course);
}
