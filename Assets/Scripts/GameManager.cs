using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas blinds;
    [SerializeField] private GameObject handContainer;
    [SerializeField] private GameObject scoreBoard;
    private string blind = "Small";
    void Start()
    {
        RectTransform panel = blinds.transform.Find(blind).gameObject.GetComponent<RectTransform>();
        panel.Find("Panel").gameObject.SetActive(false);
        Vector2 pos = panel.offsetMax;
        pos.y = -200;
        panel.offsetMax = pos;
    }        

    public void SelectBlind()
    {        
        SetScoreAtLeast();
        blinds.gameObject.SetActive(false);
        handContainer.SetActive(true);
        ChangePositionBlindsPanel();
    }    

    private void ChangePositionBlindsPanel()
    {
        RectTransform actual = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        actual.Find("Panel").gameObject.SetActive(true);
        Vector2 posActual = actual.offsetMax;
        posActual.y = -300;
        actual.offsetMax = posActual;
        ChangeBlind();
        RectTransform posterior = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        posterior.Find("Panel").gameObject.SetActive(false);
        Vector2 posPosterior = posterior.offsetMax;
        posPosterior.y = -200;
        posterior.offsetMax = posPosterior;
    }

    private void ChangeBlind()
    {
        if (blind.Equals("Small"))
        {
            blind = "Big";
        }
        else if (blind.Equals("Big"))
        {
            blind = "Boss";
        }
        else
        {
            blind = "Small";
        }
    }

    private void SetScoreAtLeast()
    {
        RectTransform scoreAtLeast = scoreBoard.gameObject.transform.Find("ScoreAtLeast").GetComponent<RectTransform>();
        TextMeshProUGUI blindText = scoreAtLeast.transform.Find("BlindText/Text").GetComponent<TextMeshProUGUI>();
        Debug.Log(blindText.text);
        Image blindImage = scoreAtLeast.gameObject.transform.Find("BlindInfo/Chip")
            .GetComponent<RectTransform>().gameObject.transform.GetComponent<Image>();
        TextMeshPro blindScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Score")
            .GetComponent<RectTransform>().gameObject.transform.GetComponent<TextMeshPro>();
        TextMeshPro blindReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Reward")
            .GetComponent<RectTransform>().gameObject.transform.GetComponent<TextMeshPro>();
        GameObject blindTextReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/TextReward")
            .GetComponent<GameObject>();

        RectTransform actual = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        Image actualImage = actual.gameObject.transform.Find("Chip").GetComponent<Image>();
        TextMeshPro actualScore = actual.gameObject.transform.Find("ScoreAtLeast/Score").GetComponent<TextMeshPro>();
        //IMPLEMENTAR RECOMPENSA

        /*if (blind.Equals("Small"))
        {
            blindText.text = "CIEGA PEQUEÑA";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            //ACTUALIZAR RECOMPENSA
        }
        else if (blind.Equals("Big"))
        {
            blindText.text = "CIEGA GRANDE";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            //ACTUALIZAR RECOMPENSA
        }
        else
        {
            blindText.text = "JEFE";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            //ACTUALIZAR RECOMPENSA
        }*/
    }
}