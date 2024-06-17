namespace AwtadStudy.Universities.Courses;

public sealed record CourseGroup(string GroupID,
                                 string Lecturer,
                                 IEnumerable<LectureInfo> Lectures,
                                 IEnumerable<ExamInfo> Exams);
