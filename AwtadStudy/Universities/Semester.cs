namespace AwtadStudy.Universities;

/// <summary>
/// Semester identifiers.
/// </summary>
// Note: this is only used by Course which is stored as jsonb in the database, which is why it doesn't have a corresponding postgres enum.
// (in json enums are serialized as ints)
public enum Semester
{
    /// <summary>
    /// Semester A.
    /// </summary>
    Winter = 1,
    /// <summary>
    /// Semester B.
    /// </summary>
    Spring,
    Summer,
}
