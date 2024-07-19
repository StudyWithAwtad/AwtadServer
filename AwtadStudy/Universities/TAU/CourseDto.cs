namespace AwtadStudy.Universities.TAU;

internal class CourseDto
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
    public required GroupDto?[] kvutzaData { get; set; }
    public string? matalot { get; set; }
}
