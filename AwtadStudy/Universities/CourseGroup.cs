namespace AwtadStudy.Universities;

public sealed record CourseGroup(string GroupID,
                                 string Lecturer,
                                 IEnumerable<LectureInfo> Lectures,
                                 IEnumerable<ExamInfo> Exams);

public sealed record LectureInfo(DayOfWeek Day, TimeOnly StartTime, TimeOnly EndTime, string Place);

public sealed record ExamInfo(DateOnly Date, Moed Moed, ExamType Type);

public enum ExamType
{
    Final,
    Midterm,
}

public enum Moed
{
    A,
    B,
    Special,
}
