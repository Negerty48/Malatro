using System.IO;
using SQLite;
using UnityEngine;
using System.Collections.Generic;

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

    public List<GameResult> GetGames()
    {
        List<GameResult> games = new List<GameResult>();

        SQLiteConnection connection = new SQLiteConnection(dbPath);
        var results = connection.Table<GameResult>().ToList();
        foreach (var result in results)
        {
            games.Add(result);
        }
        connection.Close();

        return games;
    }
}