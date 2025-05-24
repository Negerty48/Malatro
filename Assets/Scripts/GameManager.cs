using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Canvas blinds;
    [SerializeField] private GameObject handContainer;
    [SerializeField] private GameObject scoreBoard;
    [SerializeField] private GameObject playedConatainer;
    [SerializeField] private CardsManager cardsManager;
    private string blind = "Small";
    void Start()
    {
        RectTransform panel = blinds.transform.Find(blind).gameObject.GetComponent<RectTransform>();
        panel.Find("Panel").gameObject.SetActive(false);
        Vector2 pos = panel.offsetMax;
        pos.y = -200;
        panel.offsetMax = pos;
    }        

    public void Play()
    {
        Dictionary<string, int> handChips = new Dictionary<string, int>();
        List<Card> cards = cardsManager.SelectedCards;
        TextMeshProUGUI roundScore = scoreBoard.gameObject.transform.Find("RoundScore/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI handPlayed = scoreBoard.gameObject.transform.Find("HandScore/HandPlayed").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI chipsCount = scoreBoard.gameObject.transform.Find("HandScore/Chips/Text").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI handCountText = scoreBoard.gameObject.transform.Find("RoundInfo/Hands/Number").GetComponent<TextMeshProUGUI>();
        int.TryParse(handCountText.text, out int handCount);

        for (int i = 0; i < cards.Count; i++)
        {
            Debug.Log(cards[i].gameObject.name);
        }
        SetDictionary(handChips);
        cardsManager.PlaySelectedCards();
        string hand = HandEvaluator(cards);
        handPlayed.text = hand.ToUpper();
        chipsCount.text = handChips[hand].ToString();
        handCount--;
        handCountText.text = handCount.ToString();
    }    

    public void SetDictionary(Dictionary<string, int> handChips)
    {
        handChips.Add("Carta Alta", 5);        
        handChips.Add("Pareja", 10);        
        handChips.Add("Doble Pareja", 20);        
        handChips.Add("Trio", 30);        
        handChips.Add("Poker", 60);        
        handChips.Add("Full", 40);        
        handChips.Add("Color", 35);        
        handChips.Add("Escalera", 30);        
        handChips.Add("Escalera De Color", 100);        
    }

    public string HandEvaluator(List<Card> cards)
    {
        var parsed = ParseSelectedCards(cards);

        if (parsed.Count == 0)
            return "No cards selected";

        // Tratar el As (1) como 14 para la mayoría de los casos
        List<int> values = parsed.Select(c => c.Value == 1 ? 14 : c.Value).OrderBy(v => v).ToList();
        var suits = parsed.Select(c => c.Suit).ToList();

        bool isFlush = suits.Distinct().Count() == 1;
        bool isStraight = values.Distinct().Count() == parsed.Count &&
                          values.Max() - values.Min() == parsed.Count - 1;

        // Comprobar escalera baja A-2-3-4-5 (As como 1)
        if (parsed.Any(c => c.Value == 1))
        {
            var alt = parsed.Select(c => c.Value).ToList(); // usar As como 1
            alt = alt.OrderBy(v => v).ToList();
            if (alt.Distinct().Count() == parsed.Count &&
                alt.Max() - alt.Min() == parsed.Count - 1)
            {
                isStraight = true;
                values = alt; // usar esta para comparar grupos
            }
        }

        var groups = values.GroupBy(v => v).Select(g => g.Count()).OrderByDescending(c => c).ToList();

        if (isFlush && isStraight) return "Escalera De Color";
        if (groups.Contains(4)) return "Poker";
        if (groups.Contains(3) && groups.Contains(2)) return "Full";
        if (isFlush) return "Color";
        if (isStraight) return "Escalera";
        if (groups.Contains(3)) return "Trio";
        if (groups.Count(c => c == 2) == 2) return "Doble Pareja";
        if (groups.Contains(2)) return "Pareja";

        return "Carta Alta";
    }

    public static List<CardParser> ParseSelectedCards(List<Card> selected)
    {
        var parsed = new List<CardParser>();

        foreach (var card in selected)
        {
            Debug.Log(card.gameObject.name);
            string cleanName = card.gameObject.name.Replace("(Clone)", "").Trim();
            Debug.Log(cleanName);
            string[] parts = cleanName.Split('_');

            if (parts.Length == 2 && int.TryParse(parts[1], out int val))
            {
                parsed.Add(new CardParser
                {
                    Suit = parts[0],
                    Value = val
                });
            }
            else
            {
                Debug.LogWarning("Formato de carta no válido: " + card.name);
            }
        }

        return parsed;
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
        RectTransform posterior = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        posterior.Find("Panel").gameObject.SetActive(false);
        Vector2 posPosterior = posterior.offsetMax;
        posPosterior.y = -200;
        posterior.offsetMax = posPosterior;
        ChangeBlind();
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
        Image blindImage = scoreAtLeast.gameObject.transform.Find("BlindInfo/Chip").GetComponent<Image>();
        TextMeshProUGUI blindScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Reward").GetComponent<TextMeshProUGUI>();
        Image blindTextReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/TextReward").GetComponent<Image>();
        Image blindChipScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Chip").GetComponent<Image>();

        RectTransform actual = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        Image actualImage = actual.gameObject.transform.Find("Chip").GetComponent<Image>();
        TextMeshProUGUI actualScore = actual.gameObject.transform.Find("ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI actualReward = actual.gameObject.transform.Find("ScoreAtLeast/Reward").GetComponent<TextMeshProUGUI>();
        

        if (blind.Equals("Small"))
        {
            blindImage.gameObject.SetActive(true);
            blindChipScore.gameObject.SetActive(true);
            blindScore.gameObject.SetActive(true);
            blindText.text = "CIEGA PEQUEÑA";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            if (actualReward)
            {
                blindReward.text = actualReward.text;
            }
        }
        else if (blind.Equals("Big"))
        {
            blindImage.gameObject.SetActive(true);
            blindChipScore.gameObject.SetActive(true);
            blindScore.gameObject.SetActive(true);
            blindText.text = "CIEGA GRANDE";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            if (actualReward)
            {
                blindReward.text = actualReward.text;
            }
        }
        else
        {
            blindImage.gameObject.SetActive(true);
            blindChipScore.gameObject.SetActive(true);
            blindScore.gameObject.SetActive(true);
            blindText.text = "JEFE";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            if (actualReward)
            {
                blindReward.text = actualReward.text;
            }
        }
    }
}