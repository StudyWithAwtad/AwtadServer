namespace AwtadStudy.Universities.Courses;

public sealed record CourseInfo(string Id,
                                string Name,
                                double Credits,
                                string? Faculty,
                                int Year,
                                Semester Semester,
                                IEnumerable<GroupInfo> Groups);
