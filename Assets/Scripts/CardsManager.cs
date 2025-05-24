using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public Transform handContainer;
    public Transform playedContainer;
    public List<GameObject> cards;
    private List<GameObject> temp;
    public List<Card> SelectedCards = new();

    [Header("Test")]
    public int initialHandCount = 10;

    void Start()
    {
        temp = cards;
        for (int i = 0; i < initialHandCount; i++)
        {
            int cardIndex = Random.Range(0, temp.Count);
            GameObject newCard = Instantiate(temp[cardIndex], handContainer);
            temp.Remove(newCard);
        }
    }

    public void PlaySelectedCards()
    {
        if (SelectedCards.Count == 0)
            return;

        foreach (Card card in new List<Card>(SelectedCards))
        {
            card.Deselect();
            card.transform.SetParent(playedContainer, false);
            card.transform.SetSiblingIndex(playedContainer.childCount);
        }

        SelectedCards.Clear();
    }

    public List<GameObject> DrawCards(int carsToSpawn)
    {
        List<GameObject> cards = new();
        int cardIndex = Random.Range(0, temp.Count);
        GameObject newCard = Instantiate(temp[cardIndex], handContainer);
        temp.Remove(newCard);
        return cards;
    }
}