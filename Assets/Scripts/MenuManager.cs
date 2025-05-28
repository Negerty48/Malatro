using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform contentViewport;
    [SerializeField] private GameObject rowPrefab;
    [SerializeField] private DBManager dbManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject menuPanel;

    public void Play()
    {
        gameObject.SetActive(false);
        canvas.SetActive(true);
        gameManager.ResetPositionBlindsPanel();
        try
        {
            gameManager.blind = "Small";
            gameManager.SetRoundScoreAt0();
            gameManager.scoreBoard.transform.Find("RoundInfo/Bank/Money").GetComponent<TextMeshProUGUI>().text = "4";
        } catch (NullReferenceException){}
    }

    public void Menu()
    {
        List<GameResult> games = dbManager.GetGames();

        menuPanel.SetActive(true);
        foreach (GameResult game in games)
        {
            GameObject newRow = Instantiate(rowPrefab, contentViewport);
            TextMeshProUGUI[] textos = newRow.GetComponentsInChildren<TextMeshProUGUI>();
            textos[0].text = game.Id.ToString();
            textos[1].text = game.Round.ToString();
            textos[2].text = game.Score.ToString();
            textos[3].text = game.Result;
        }
    }

    public void ShowExitPanel()
    {
        exitPanel.SetActive(true);
    }

    public void YesExit()
    {
        //Application.Quit();

        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void NoExit()
    {
        exitPanel.SetActive(false);
    }

    public void ExitMenuPanel()
    {
        foreach (Transform child in contentViewport)
        {
            Destroy(child.gameObject);
        }
        menuPanel.SetActive(false);
    }
}