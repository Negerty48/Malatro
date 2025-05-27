using System;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private DBManager dbManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject exitPanel;

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
        dbManager.GetGames(); 
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
}
