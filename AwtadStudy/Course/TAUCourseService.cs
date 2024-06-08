using System.Diagnostics;
using AwtadStudy.Database;

namespace AwtadStudy.Course;

public sealed class TAUCourseService(HttpClient http) : ICourseService
{
    public async Task<Course> GetCourse(string courseID, Semester semester)
    {
        var courseDTO = await QueryApi(courseID, semester);

        var groups = ParseGroups(courseDTO.kvutzaDatas);

        return new(courseDTO.cNum,
                   courseDTO.cName,
                   courseDTO.hoursNum,
                   courseDTO.depart,
                   int.Parse(courseDTO.cYear!),
                   semester,
                   groups);
    }

    public async IAsyncEnumerable<Course> GetCoursesForUser(string userID)
    {
        var courseDTOs = await GetUserCoursesFromDB(userID);

        var courseQueries = courseDTOs.Select(async c => (c, await QueryApi(c.CourseID, c.Semester)));
        var courseTuples = await Task.WhenAll(courseQueries);

        foreach (var (courseDTO, apiCourse) in courseTuples)
        {
            var apiGroups = apiCourse.kvutzaDatas.Where(g => g is not null && courseDTO.GroupIDs.Contains(g.gNum));

            if (!apiGroups.Any()) throw new UnreachableException($"User is not registered to any groups from course {courseDTO.CourseID}.");

            var courseGroups = ParseGroups(apiGroups);

            yield return new(apiCourse.cNum,
                             apiCourse.cName,
                             apiCourse.hoursNum,
                             apiCourse.depart,
                             int.Parse(apiCourse.cYear!),
                             courseDTO.Semester,
                             courseGroups);
        }

        Task<IEnumerable<CourseDTO>> GetUserCoursesFromDB(string userID) => throw new NotImplementedException();
    }

    public Task RegisterCourseForUser(string userID, string courseID, string groupID)
    {
        throw new NotImplementedException();
    }

    private async Task<Coursesinfo> QueryApi(string courseID, Semester semester)
    {
        string semesterParam = semester switch
        {
            Semester.A => "1",
            Semester.B => "2",
            _ => throw new NotImplementedException()
        };

        string url = $"https://bid-it.appspot.com/ajax/chosen-courses-info/?university=TAU&semester={semesterParam}&courses_list={courseID}";
        var rootDTO = await http.GetFromJsonAsync<RootDTO>(url);

        var courseDTO = rootDTO.coursesinfo[0]
            ?? throw new InvalidOperationException("No course found with the specified ID and semester.");

        if (courseDTO.cNum != courseID) throw new UnreachableException("courseID != returned course's ID.");
        if (courseDTO.semester != semesterParam) throw new UnreachableException("semester != returned course's semester.");

        if (courseDTO.kvutzaDatas is null) throw new UnreachableException("Course groups is null.");

        if (courseDTO.cName is null) throw new UnreachableException("Course name is null.");

        return courseDTO;
    }

    private static DayOfWeek DayFromHebrewDay(string? hebrewDay) => hebrewDay switch
    {
        "א" => DayOfWeek.Sunday,
        "ב" => DayOfWeek.Monday,
        "ג" => DayOfWeek.Tuesday,
        "ד" => DayOfWeek.Wednesday,
        "ה" => DayOfWeek.Thursday,
        "ו" => DayOfWeek.Friday,
        "ז" => DayOfWeek.Saturday,
        _ => throw new NotImplementedException()
    };

    private static IEnumerable<CourseGroup> ParseGroups(IEnumerable<Kvutzadata?> groupDTOs)
    {
        foreach (var groupDTO in groupDTOs)
        {
            if (groupDTO is null) throw new UnreachableException("Group is null.");

            string id = groupDTO.gNum ?? throw new UnreachableException("Group ID is null.");
            string lecturer = string.Join(" + ", groupDTO.lecturer?.Distinct() ?? []);
            IEnumerable<LectureInfo> lectures = ParseLectureInfo(groupDTO);
            IEnumerable<ExamInfo> exams = ParseExamInfo(groupDTO);

            yield return new(id, lecturer, lectures, exams);
        }
    }

    private static IEnumerable<LectureInfo> ParseLectureInfo(Kvutzadata groupDTO)
    {
        if (groupDTO.days is null or { Length: 0 }) throw new UnreachableException("Groups has no lectures.");

        for (int i = 0; i < groupDTO.days.Length; i++)
        {
            var day = DayFromHebrewDay(groupDTO.days[i]);
            var startTime = TimeOnly.ParseExact(groupDTO.beginHours![i].Replace(" ", ""), "H:mm");
            var endTime = TimeOnly.ParseExact(groupDTO.endHours![i].Replace(" ", ""), "H:mm");
            var place = groupDTO.place?[i] ?? "";

            yield return new(day, startTime, endTime, place);
        }
    }

    private static IEnumerable<ExamInfo> ParseExamInfo(Kvutzadata groupDTO)
    {
        if (groupDTO.dates is null or { Length: 0 }) yield break;

        for (int i = 0; i < groupDTO.dates.Length; i++)
        {
            var date = DateOnly.ParseExact(groupDTO.dates[i]!, "d/M/yyyy");
            var examType = groupDTO.moedType![i] switch
            {
                "בחינה סופית" => ExamType.Final,
                _ => throw new NotImplementedException()
            };
            var moed = groupDTO.moed![i]!;

            yield return new(date, moed, examType);
        }
    }

    private struct RootDTO
    {
        public Coursesinfo?[] coursesinfo { get; set; }
    }

    private class Coursesinfo
    {
        public string? zchutHours { get; set; }
        public string? depart { get; set; }
        public string? language { get; set; }
        public required string cNum { get; set; }
        public object?[]? students_ids { get; set; }
        public string? cYear { get; set; }
        public double hoursNum { get; set; }
        public string? havurotNum { get; set; }
        public string? semester { get; set; }
        public required string cName { get; set; }
        public bool video { get; set; }
        public bool isYearly { get; set; }
        public string? faculty { get; set; }
        public required Kvutzadata?[] kvutzaDatas { get; set; }
        public string? matalot { get; set; }
    }

    private class Kvutzadata
    {
        public string?[]? lecturer { get; set; }
        public string?[]? dates { get; set; }
        public string? gNum { get; set; }
        public string?[]? endMoedHours { get; set; }
        public string? ofenHoraa { get; set; }
        public string?[]? startMoedHours { get; set; }
        public string[]? days { get; set; }
        public string?[]? moedType { get; set; }
        public string?[]? excellence { get; set; }
        public string?[]? place { get; set; }
        public string[]? beginHours { get; set; }
        public string?[]? moed { get; set; }
        public string[]? endHours { get; set; }
        public double amountHours { get; set; }
        public string? havura { get; set; }
        public string? kind { get; set; }
        public string? matconet { get; set; }
    }
}
