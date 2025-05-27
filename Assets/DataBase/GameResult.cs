using SQLite;

public class GameResult
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int Round { get; set; }
    public int Score { get; set; }
    public string Result { get; set; } // "Win" o "Lose"
}
