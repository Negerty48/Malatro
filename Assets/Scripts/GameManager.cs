using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    private Dictionary<string, int> handChips = new Dictionary<string, int>();
    private string blind = "Small";
    void Start()
    {
        RectTransform panel = blinds.transform.Find(blind).gameObject.GetComponent<RectTransform>();
        panel.Find("Panel").gameObject.SetActive(false);
        Vector2 pos = panel.offsetMax;
        pos.y = -200;
        panel.offsetMax = pos;

        SetDictionary(handChips);
    }        

    public void Play()
    {
        TextMeshProUGUI scoreAtLeast = scoreBoard.gameObject.transform.Find("ScoreAtLeast/BlindInfo/ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI roundScore = scoreBoard.transform.Find("RoundScore/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI handPlayed = scoreBoard.transform.Find("HandScore/HandPlayed").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI chipsCount = scoreBoard.transform.Find("HandScore/Chips/Text").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI multiCount = scoreBoard.transform.Find("HandScore/Multi/Text").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI handCountText = scoreBoard.transform.Find("RoundInfo/Hands/Number").GetComponent<TextMeshProUGUI>();
        int.TryParse(handCountText.text, out int handCount);

        if (handCount > 1)
        {
            cardsManager.PlaySelectedCards();
            List<GameObject> playedCards = GetCardsPlayed(cardsManager.playedContainer);
            List<CardParser> cards = GetCardsParsed(playedCards);

            string hand = HandEvaluator(cards);
            handPlayed.text = hand.ToUpper();

            SetChipsAndMulti(hand, chipsCount, multiCount, cards);

            handCount--;
            handCountText.text = handCount.ToString();

            SetRoundScore(roundScore, chipsCount, multiCount);            
            foreach (GameObject card in playedCards)
            {
                Destroy(card);
            }
            
            //if (int.Parse(roundScore.text) >= int.Parse(scoreAtLeast.text))
            //{
            //    NextBlind();
            //}

            cardsManager.DrawCards(cards.Count);
        } else
        {
            cardsManager.PlaySelectedCards();
            List<GameObject> playedCards = GetCardsPlayed(cardsManager.playedContainer);
            List<CardParser> cards = GetCardsParsed(playedCards);

            string hand = HandEvaluator(cards);
            handPlayed.text = hand.ToUpper();

            SetChipsAndMulti(hand, chipsCount, multiCount, cards);

            handCount--;
            handCountText.text = handCount.ToString();

            SetRoundScore(roundScore, chipsCount, multiCount);
            foreach (GameObject card in playedCards)
            {
                Destroy(card);
            }

            //if (int.Parse(roundScore.text) >= int.Parse(scoreAtLeast.text))
            //{
            //    NextBlind();
            //} else
            //{
            //    GameOver();
            //}

            cardsManager.DrawCards(cards.Count);
        }
    }

    public void Discard()
    {
        List<GameObject> cards = cardsManager.SelectedCards;
        TextMeshProUGUI discardsCountText = scoreBoard.transform.Find("RoundInfo/Discards/Number").GetComponent<TextMeshProUGUI>();
        int.TryParse(discardsCountText.text, out int dicardsCount);

        if (dicardsCount > 0 && handContainer.transform.childCount < 9)
        {
            dicardsCount--;
            discardsCountText.text = dicardsCount.ToString();
            foreach (GameObject card in cards)
            {
                Destroy(card);
            }
            cardsManager.DrawCards(cards.Count);
        }
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

    public void SetRoundScore(TextMeshProUGUI roundScoreTxt, TextMeshProUGUI chipsCount, TextMeshProUGUI multiCount)
    {
        int.TryParse(chipsCount.text, out int chips);
        int.TryParse(multiCount.text, out int multi);
        int.TryParse(roundScoreTxt.text, out int roundScore);

        int handChips = chips * multi;
        roundScore += handChips;
        roundScoreTxt.text = roundScore.ToString();
    }

    public void SetChipsAndMulti(string hand, TextMeshProUGUI chipsCount, TextMeshProUGUI multiCount, List<CardParser> cards)
    {
        int.TryParse(chipsCount.text, out int chips);
        chipsCount.text = handChips[hand].ToString();        

        foreach (CardParser card in cards)
        {
            if (card.Value == 1)
            {
                chips += card.Value + 10;
            } else
            {
                chips = card.Value;
            }                
        }

        chips = chips + handChips[hand];
        chipsCount.text = chips.ToString();
        
        if (hand.Equals("Carta Alta")) {
            multiCount.text = "1";
        } else if (hand.Equals("Pareja") || hand.Equals("Doble Pareja")) {
            multiCount.text = "2";
        } else if (hand.Equals("Trio")) {
            multiCount.text = "3";
        } else if (hand.Equals("Escalera") || hand.Equals("Color") || hand.Equals("Full")) {
            multiCount.text = "4";
        } else if (hand.Equals("Poker")) {
            multiCount.text = "7";
        } else {
            multiCount.text = "8";
        }
    }    

    public List<GameObject> GetCardsPlayed(Transform playedCards)
    {
        List<GameObject> childs = new List<GameObject>();
        Transform parent = playedCards.transform;
        foreach (Transform child in parent)
        {
            childs.Add(child.gameObject);
        }
        return childs;
    }

    public string HandEvaluator(List<CardParser> cards)
    {
        bool color;
        bool escalera;

        color = CheckColor(cards);
        escalera = CheckEscalera(cards);

        if (color && escalera)
        {
            return "Escalera De Color";
        }
        else if (color)
        {
            return "Color";
        }
        else if (escalera) 
        {
            return "Escalera";
        } else
        {
            return CheckValues(cards);
        }
    }    

    public string CheckValues(List<CardParser> cards)
    {
        var valueCounts = cards
        .GroupBy(c => c.Value)
        .Select(g => g.Count())
        .OrderByDescending(c => c)
        .ToList();
        
        if (valueCounts.Contains(4)) return "Poker";
        if (valueCounts.Contains(3) && valueCounts.Contains(2)) return "Full";
        if (valueCounts.Contains(3)) return "Trio";
        if (valueCounts.Count(c => c == 2) == 2) return "Doble Pareja";
        if (valueCounts.Contains(2)) return "Pareja";

        return "Carta Alta";
    }

    public bool CheckEscalera(List<CardParser> cards)
    {
        cards.Sort();

        for (int i = 1; i < cards.Count; i++)
        {
            if (i == 1 && cards[i].Value == 10)
            {
                if (cards[i].Value != cards[i - 1].Value + 9)
                {
                    return false;
                } else
                {
                    if (cards[i].Value != cards[i - 1].Value + 1)
                    {
                        return false;
                    }
                }
            } else
            {
                if (cards[i].Value != cards[i - 1].Value + 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool CheckColor(List<CardParser> cards)
    {      
        string suit = "";
        if (cards.Count == 5) {
            for (int i = 0; i < cards.Count; i++)
            {
                if (i == 0) {
                    suit = cards[i].Suit;
                } else {
                    if (!cards[i].Suit.Equals(suit)) {
                        return false;
                    }
                }
            }
        } else return false;
        return true;
    }

    public List<CardParser> GetCardsParsed(List<GameObject> selectedCards)
    {
        List<CardParser> cards = new();
        int value;
        foreach (GameObject card in selectedCards)
        {
            string name = card.name.Replace("(Clone)", "").Trim();
            string[] parts = name.Split("_");
            if (char.IsLetter(parts[1][0])) 
            {
                value = 10;
            } else
            {
                value = int.Parse(parts[1]);
            }
            cards.Add(new CardParser(parts[0], value));
        }
        return cards;
    }

    public void SelectBlind()
    {        
        SetScoreAtLeast();
        blinds.gameObject.SetActive(false);
        handContainer.SetActive(true);
        ChangePositionBlindsPanel();
        cardsManager.DrawCards(9);
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