namespace AwtadStudy.Course;

public sealed record CourseGroup(string GroupID,
                                 string Lecturer,
                                 IEnumerable<LectureInfo> Lectures,
                                 IEnumerable<ExamInfo> Exams);

public sealed record LectureInfo(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime, string Place);

public sealed record ExamInfo(DateOnly Date, string Moed, ExamType Type);

public enum ExamType
{
    Final,
    Midterm,
}
