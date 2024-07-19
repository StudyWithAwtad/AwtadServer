namespace AwtadStudy.Universities.Courses;

public sealed record GroupInfo(string GroupId,
                               string Lecturer,
                               IEnumerable<LectureInfo> Lectures,
                               IEnumerable<ExamInfo> Exams);
