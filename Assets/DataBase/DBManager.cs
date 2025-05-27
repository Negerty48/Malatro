using System.IO;
using SQLite;
using UnityEngine;

public class DBManager : MonoBehaviour
{
    private string dbPath;

    private void Start()
    {
        dbPath = Path.Combine(Application.persistentDataPath, "game_results.db");        
        SQLiteConnection connection = new SQLiteConnection(dbPath);
        connection.CreateTable<GameResult>();
        connection.Close();        
    }

    public void SaveGameResult(int round, int score, string result)
    {
        SQLiteConnection connection = new SQLiteConnection(dbPath);
        var entry = new GameResult{
            Round = round,
            Score = score,
            Result = result
        };
        connection.Insert(entry);
        connection.Close();        
    }

    public void GetGames()
    {
        SQLiteConnection connection = new SQLiteConnection(dbPath);
        var results = connection.Table<GameResult>().ToList();
        foreach (var result in results)
        {
            Debug.Log($"ID: {result.Id}, Ronda: {result.Round}, Puntos: {result.Score}, Resultado: {result.Result}");
        }
        connection.Close();
    }
}
