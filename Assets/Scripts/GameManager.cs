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
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject menuManager;
    TextMeshProUGUI scoreAtLeast;
    TextMeshProUGUI roundScore;
    TextMeshProUGUI handPlayed;
    TextMeshProUGUI chipsCount;
    TextMeshProUGUI multiCount;
    TextMeshProUGUI handCountText;
    TextMeshProUGUI discardCountText;
    TextMeshProUGUI roundText;
    private Dictionary<string, int> handChips = new Dictionary<string, int>();
    private string blind = "Small";
    private int round = 0;
    private int handCount;
    void Start()
    {
        RectTransform panel = blinds.transform.Find(blind).gameObject.GetComponent<RectTransform>();
        panel.Find("Panel").gameObject.SetActive(false);
        Vector2 pos = panel.offsetMax;
        pos.y = -200;
        panel.offsetMax = pos;

        SetDictionary(handChips);

        scoreAtLeast = scoreBoard.gameObject.transform.Find("ScoreAtLeast/BlindInfo/ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        roundScore = scoreBoard.transform.Find("RoundScore/Score").GetComponent<TextMeshProUGUI>();
        handPlayed = scoreBoard.transform.Find("HandScore/HandPlayed").GetComponent<TextMeshProUGUI>();
        chipsCount = scoreBoard.transform.Find("HandScore/Chips/Text").GetComponent<TextMeshProUGUI>();
        multiCount = scoreBoard.transform.Find("HandScore/Multi/Text").GetComponent<TextMeshProUGUI>();
        handCountText = scoreBoard.transform.Find("RoundInfo/Hands/Number").GetComponent<TextMeshProUGUI>();
        discardCountText = scoreBoard.transform.Find("RoundInfo/Discards/Number").GetComponent<TextMeshProUGUI>();
        handCount = int.Parse(handCountText.text);
        roundText = scoreBoard.transform.Find("RoundInfo/Round/Number").GetComponent<TextMeshProUGUI>();
    }        

    public void Play()
    {        
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

            if (int.Parse(roundScore.text) >= int.Parse(scoreAtLeast.text))
            {
                NextBlind();
                return;
            }

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

            if (int.Parse(roundScore.text) >= int.Parse(scoreAtLeast.text))
            {
                NextBlind();
            }
            else
            {
                GameOver();
            }            
        }
    }

    public void NextBlind()
    {        
        ResetForPlay();
        cardsManager.SetDeck();
        cardsManager.DeleteCards();
        handContainer.SetActive(false);        
        blinds.gameObject.SetActive(true);        
    }

    public void GameOver()
    {
        ResetForPlay();
        cardsManager.SetDeck();
        cardsManager.DeleteCards();
        handContainer.SetActive(false);
        blinds.gameObject.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        gameOverPanel.SetActive(false);
        blinds.gameObject.SetActive(true);
    }

    public void ExitToMenu()
    {
        gameOverPanel.SetActive(false);
        blinds.gameObject.SetActive(true);
        canvas.SetActive(false);
        menuManager.SetActive(true);
    }

    public void ResetForPlay()
    {
        roundScore.text = "";
        handPlayed.text = "";
        chipsCount.text = "";
        multiCount.text = "";
        handCountText.text = "4";
        discardCountText.text = "3";

        RectTransform scoreAtLeast = scoreBoard.gameObject.transform.Find("ScoreAtLeast").GetComponent<RectTransform>();
        TextMeshProUGUI blindText = scoreAtLeast.transform.Find("BlindText/Text").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image blindImage = scoreAtLeast.gameObject.transform.Find("BlindInfo/Chip").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI blindScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Reward").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindTextReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/TextReward").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image blindChipScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Chip").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI reward = scoreBoard.transform.Find("RoundInfo/Bank/Money").GetComponent<TextMeshProUGUI>();
        if (blindReward)
        {
            reward.text = blindReward.text;
        }
        blindText.text = "";
        blindImage.sprite = null;
        blindImage.gameObject.SetActive(false);
        blindScore.text = "";
        blindReward.text = "";
        blindTextReward.gameObject.SetActive(false);
        blindChipScore.gameObject.SetActive(false);

        round++;
        roundText.text = round.ToString();
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

        if (hand.Equals("Carta Alta"))
        {
            chips = cards.Max(c => c.Value) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "1";
        }
        else if (hand.Equals("Pareja"))
        {
            chips = SumByGroup(cards, 2) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "2";
        }
        else if (hand.Equals("Doble Pareja"))
        {
            chips = SumMultipleGroups(cards, 2, 2) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "2";
        }
        else if (hand.Equals("Trio"))
        {
            chips = SumByGroup(cards, 3) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "3";
        }
        else if (hand.Equals("Escalera") || hand.Equals("Color") || hand.Equals("Full"))
        {
            chips = cards.Sum(c => c.Value) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "4";
        }
        else if (hand.Equals("Poker"))
        {
            chips = SumByGroup(cards, 4) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "7";
        }
        else
        {
            chips = cards.Sum(c => c.Value) + handChips[hand];
            chipsCount.text = chips.ToString();
            multiCount.text = "8";
        }
    }

    private int SumByGroup(List<CardParser> cards, int groupSize)
    {
        var grouped = cards.GroupBy(c => c.Value).FirstOrDefault(g => g.Count() == groupSize);
        return grouped != null ? grouped.Sum(c => c.Value) : 0;
    }

    private static int SumMultipleGroups(List<CardParser> cards, int groupSize, int expectedGroups)
    {
        var pairs = cards
            .GroupBy(c => c.Value)
            .Where(g => g.Count() == groupSize)
            .Take(expectedGroups)
            .SelectMany(g => g);

        return pairs.Sum(c => c.Value);
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
        List<int> values = cards.Select(c => c.Value).OrderBy(v => v).ToList();

        bool isAceLowStraight = values.Contains(14) &&
                                values.Contains(2) &&
                                values.Contains(3) &&
                                values.Contains(4) &&
                                values.Contains(5);

        if (isAceLowStraight)
            return true;
        
        for (int i = 1; i < values.Count; i++)
        {
            if (values[i] != values[i - 1] + 1)
                return false;
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
                if (parts[1][0] == 'J') 
                {
                    value = 11;
                } else if (parts[1][0] == 'Q')
                {
                    value = 12;
                } else
                {
                    value = 13;
                }
            } else
            {
                if (int.Parse(parts[1]) == 1)
                {
                    value = 14;
                }
                else
                {
                    value = int.Parse(parts[1]);
                }
            }
            cards.Add(new CardParser(parts[0], value));
        }
        return cards;
    }

    public void SelectBlind()
    {        
        SetScoreAtLeast();
        ChangePositionBlindsPanel();
        blinds.gameObject.SetActive(false);
        handContainer.SetActive(true);        
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
        UnityEngine.UI.Image blindImage = scoreAtLeast.gameObject.transform.Find("BlindInfo/Chip").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI blindScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Reward").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindTextReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/TextReward").GetComponent<TextMeshProUGUI>();
        UnityEngine.UI.Image blindChipScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Chip").GetComponent<UnityEngine.UI.Image>();

        RectTransform actual = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        UnityEngine.UI.Image actualImage = actual.gameObject.transform.Find("Chip").GetComponent<UnityEngine.UI.Image>();
        TextMeshProUGUI actualScore = actual.gameObject.transform.Find("ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI actualReward = actual.gameObject.transform.Find("ScoreAtLeast/Reward").GetComponent<TextMeshProUGUI>();
        

        if (blind.Equals("Small"))
        {
            blindImage.gameObject.SetActive(true);
            blindChipScore.gameObject.SetActive(true);            
            blindText.text = "CIEGA PEQUEÑA";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            if (actualReward)
            {
                blindTextReward.gameObject.SetActive(true);
                blindReward.text = actualReward.text;
            }
        }
        else if (blind.Equals("Big"))
        {
            blindImage.gameObject.SetActive(true);
            blindChipScore.gameObject.SetActive(true);            
            blindText.text = "CIEGA GRANDE";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            if (actualReward)
            {
                blindTextReward.gameObject.SetActive(true);
                blindReward.text = actualReward.text;
            }
        }
        else
        {
            blindImage.gameObject.SetActive(true);
            blindChipScore.gameObject.SetActive(true);            
            blindText.text = "JEFE";
            blindImage.sprite = actualImage.sprite;
            blindScore.text = actualScore.text;
            if (actualReward)
            {
                blindTextReward.gameObject.SetActive(true);
                blindReward.text = actualReward.text;
            }
        }
    }
}