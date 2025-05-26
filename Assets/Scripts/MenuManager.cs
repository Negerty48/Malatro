using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject exitPanel;

    public void Play()
    {
        gameObject.SetActive(false);
        canvas.SetActive(true);
    }

    public void Menu()
    {

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
