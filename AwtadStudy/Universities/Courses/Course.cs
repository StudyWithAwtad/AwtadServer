namespace AwtadStudy.Universities.Courses;

public sealed record Course(string ID,
                            string Name,
                            double Credits,
                            string? Faculty,
                            int Year,
                            Semester Semester,
                            IEnumerable<CourseGroup> Groups);
