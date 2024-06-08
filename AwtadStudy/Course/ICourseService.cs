namespace AwtadStudy.Course;

public interface ICourseService
{
    Task<Course> GetCourse(string courseID, Semester semester);

    IAsyncEnumerable<Course> GetCoursesForUser(string userID);

    Task RegisterCourseForUser(string userID, string courseID, string groupID);
}
