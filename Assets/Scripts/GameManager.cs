using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DBManager dbManager;
    [SerializeField] private Canvas blinds;
    [SerializeField] private GameObject handContainer;
    [SerializeField] public GameObject scoreBoard;
    [SerializeField] private GameObject playedConatainer;
    [SerializeField] private CardsManager cardsManager;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject menuManager;
    TextMeshProUGUI scoreAtLeast;
    [HideInInspector] public TextMeshProUGUI roundScore;
    [HideInInspector] public TextMeshProUGUI handPlayed;
    [HideInInspector] public TextMeshProUGUI chipsCount;
    [HideInInspector] public TextMeshProUGUI multiCount;
    TextMeshProUGUI handCountText;
    TextMeshProUGUI discardCountText;
    TextMeshProUGUI roundText;
    private Dictionary<string, int> handChips = new Dictionary<string, int>();
    [HideInInspector]public string blind = "Small";
    private int round = 1;
    private int handCount;
    private int maxScore = int.MinValue;

    void Start()
    {
        SetFirstBlind();
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
        if (cardsManager.SelectedCards.Count > 0)
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

                int score = SetRoundScore(roundScore, chipsCount, multiCount);
                foreach (GameObject card in playedCards)
                {
                    Destroy(card);
                }

                if (int.Parse(roundScore.text) >= int.Parse(scoreAtLeast.text))
                {
                    if (blind.Equals("Boss"))
                    {
                        if (score >= maxScore)
                        {
                            maxScore = score;
                        }
                        Win();
                    }
                    else
                    {
                        if (score >= maxScore)
                        {
                            maxScore = score;
                        }
                        NextBlind();
                    }
                }
                else
                {
                    cardsManager.DrawCards(cards.Count);
                }
            }
            else
            {
                cardsManager.PlaySelectedCards();
                List<GameObject> playedCards = GetCardsPlayed(cardsManager.playedContainer);
                List<CardParser> cards = GetCardsParsed(playedCards);

                string hand = HandEvaluator(cards);
                handPlayed.text = hand.ToUpper();

                SetChipsAndMulti(hand, chipsCount, multiCount, cards);

                handCount--;
                handCountText.text = handCount.ToString();

                int score = SetRoundScore(roundScore, chipsCount, multiCount);
                foreach (GameObject card in playedCards)
                {
                    Destroy(card);
                }

                if (int.Parse(roundScore.text) >= int.Parse(scoreAtLeast.text))
                {
                    if (blind.Equals("Boss"))
                    {
                        if (score >= maxScore)
                        {
                            maxScore = score;
                        }
                        Win();
                    }
                    else
                    {
                        if (score >= maxScore)
                        {
                            maxScore = score;
                        }
                        NextBlind();
                    }
                }
                else
                {
                    if (score >= maxScore)
                    {
                        maxScore = score;
                    }
                    GameOver();
                }
            }
        }
    }

    private void NextBlind()
    {        
        ResetForPlay();
        cardsManager.SetDeck();
        cardsManager.DeleteCards();
        handContainer.SetActive(false);
        handCountText = scoreBoard.transform.Find("RoundInfo/Hands/Number").GetComponent<TextMeshProUGUI>();
        handCount = int.Parse(handCountText.text);
        ChangePositionBlindsPanel();
        blinds.gameObject.SetActive(true);        
    }

    private void Win()
    {
        ResetForPlay();
        cardsManager.SetDeck();
        cardsManager.DeleteCards();
        handContainer.SetActive(false);
        blinds.gameObject.SetActive(false);

        TextMeshProUGUI roundPanel = winPanel.transform.Find("Round/Number").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI score = winPanel.transform.Find("Score/Number").GetComponent<TextMeshProUGUI>();

        roundPanel.text = round.ToString();
        score.text = maxScore.ToString();
        dbManager.SaveGameResult(round, maxScore, "Victoria");
        winPanel.SetActive(true);
    }

    private void GameOver()
    {
        ResetForPlay();
        cardsManager.SetDeck();
        cardsManager.DeleteCards();
        handContainer.SetActive(false);
        blinds.gameObject.SetActive(false);
        TextMeshProUGUI roundPanel = gameOverPanel.transform.Find("Round/Number").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI score = gameOverPanel.transform.Find("Score/Number").GetComponent<TextMeshProUGUI>();

        roundPanel.text = round.ToString();
        score.text = maxScore.ToString();
        dbManager.SaveGameResult(round, maxScore, "Derrota");
        gameOverPanel.SetActive(true);
    }

    public void PlayAgain(GameObject panel)
    {
        panel.SetActive(false);
        ResetPositionBlindsPanel();
        blind = "Small";      
        SetFirstBlind();
        SetRoundScoreAt0();
        roundText.text = "";
        round = 1;
        handCountText = scoreBoard.transform.Find("RoundInfo/Hands/Number").GetComponent<TextMeshProUGUI>();
        handCount = int.Parse(handCountText.text);
        TextMeshProUGUI moneyText = scoreBoard.transform.Find("RoundInfo/Bank/Money").GetComponent<TextMeshProUGUI>();
        moneyText.text = "4";
        maxScore = int.MinValue;
        blinds.gameObject.SetActive(true);
    }

    public void ExitToMenu(GameObject panel)
    {
        panel.SetActive(false);
        blinds.gameObject.SetActive(true);
        canvas.SetActive(false);
        menuManager.SetActive(true);
    }

    private void ResetForPlay()
    {
        handCountText.text = "4";
        discardCountText.text = "3";

        RectTransform scoreAtLeast = scoreBoard.gameObject.transform.Find("ScoreAtLeast").GetComponent<RectTransform>();
        TextMeshProUGUI blindText = scoreAtLeast.transform.Find("BlindText/Text").GetComponent<TextMeshProUGUI>();
        Image blindImage = scoreAtLeast.gameObject.transform.Find("BlindInfo/Chip").GetComponent<Image>();
        TextMeshProUGUI blindScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Score").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Reward").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI blindTextReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/TextReward").GetComponent<TextMeshProUGUI>();
        Image blindChipScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Chip").GetComponent<Image>();
        TextMeshProUGUI moneyText = scoreBoard.transform.Find("RoundInfo/Bank/Money").GetComponent<TextMeshProUGUI>();
        int money = int.Parse(moneyText.text);
        if (!blindReward.Equals(""))
        {
            int.TryParse(blindReward.text.Replace("$", ""), out int reward);
            money += reward;
            moneyText.text = money.ToString();
        }
        blindText.text = "";
        blindImage.sprite = null;
        blindImage.gameObject.SetActive(false);
        blindScore.text = "";
        blindReward.text = "";
        blindTextReward.gameObject.SetActive(false);
        blindChipScore.gameObject.SetActive(false);

        if (!blind.Equals("Boss"))
        {
            round++;
            roundText.text = round.ToString();
        }
    }

    public void Discard()
    {
        List<GameObject> cards = cardsManager.SelectedCards;
        TextMeshProUGUI discardsCountText = scoreBoard.transform.Find("RoundInfo/Discards/Number").GetComponent<TextMeshProUGUI>();
        int discardsCount = int.Parse(discardsCountText.text);

        if (discardsCount > 0 && cards.Count > 0)
        {
            discardsCount--;
            discardsCountText.text = discardsCount.ToString();
            cardsManager.DrawCards(cards.Count);
            cardsManager.DicardCards();                        
            cardsManager.SelectedCards.Clear();
        }
    }

    private void SetDictionary(Dictionary<string, int> handChips)
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

    private int SetRoundScore(TextMeshProUGUI roundScoreTxt, TextMeshProUGUI chipsCount, TextMeshProUGUI multiCount)
    {
        int.TryParse(chipsCount.text, out int chips);
        int.TryParse(multiCount.text, out int multi);
        int.TryParse(roundScoreTxt.text, out int roundScore);

        int handChips = chips * multi;
        roundScore += handChips;
        roundScoreTxt.text = roundScore.ToString();
        return roundScore;
    }

    private void SetChipsAndMulti(string hand, TextMeshProUGUI chipsCount, TextMeshProUGUI multiCount, List<CardParser> cards)
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

    private List<GameObject> GetCardsPlayed(Transform playedCards)
    {
        List<GameObject> childs = new List<GameObject>();
        Transform parent = playedCards.transform;
        foreach (Transform child in parent)
        {
            childs.Add(child.gameObject);
        }
        return childs;
    }

    private string HandEvaluator(List<CardParser> cards)
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

    private string CheckValues(List<CardParser> cards)
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

    private bool CheckEscalera(List<CardParser> cards)
    {
        List<int> values = cards.Select(c => c.Value).OrderBy(v => v).ToList();
        
        if (values.Count == 5)
        {
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
        } else return false;

        return true;
    }

    private bool CheckColor(List<CardParser> cards)
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

    private List<CardParser> GetCardsParsed(List<GameObject> selectedCards)
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
        SetRoundScoreAt0();
        TextMeshProUGUI roundText = scoreBoard.transform.Find("RoundInfo/Round/Number").GetComponent<TextMeshProUGUI>();
        roundText.text = round.ToString();
        SetScoreAtLeast();
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
        ChangeBlind();
        RectTransform posterior = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        posterior.Find("Panel").gameObject.SetActive(false);
        Vector2 posPosterior = posterior.offsetMax;
        posPosterior.y = -200;
        posterior.offsetMax = posPosterior;        
    }

    private void SetFirstBlind()
    {
        RectTransform panel = blinds.transform.Find(blind).gameObject.GetComponent<RectTransform>();
        panel.Find("Panel").gameObject.SetActive(false);
        Vector2 pos = panel.offsetMax;
        pos.y = -200;
        panel.offsetMax = pos;
    }

    public void ResetPositionBlindsPanel()
    {
        List<string> blindsName = new List<string>{"Small", "Big", "Boss"};
        foreach (string blind in blindsName) 
        {
            RectTransform actual = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
            actual.Find("Panel").gameObject.SetActive(true);
            Vector2 posActual = actual.offsetMax;
            posActual.y = -300;
            actual.offsetMax = posActual;
        }
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
        TextMeshProUGUI blindTextReward = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/TextReward").GetComponent<TextMeshProUGUI>();
        Image blindChipScore = scoreAtLeast.gameObject.transform.Find("BlindInfo/ScoreAtLeast/Chip").GetComponent<Image>();

        RectTransform actual = blinds.gameObject.transform.Find(blind).GetComponent<RectTransform>();
        Image actualImage = actual.gameObject.transform.Find("Chip").GetComponent<Image>();
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

    public void SetRoundScoreAt0()
    {
        round = 1;
        SetFirstBlind();
        roundScore.text = "";
        handPlayed.text = "";
        chipsCount.text = "";
        multiCount.text = "";
        roundText.text = "";
    }
}