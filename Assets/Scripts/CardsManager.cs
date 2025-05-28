using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public Transform handContainer;
    public Transform playedContainer;
    public List<GameObject> deck;
    public List<GameObject> temp;
    public List<GameObject> SelectedCards = new();

    void Start()
    {
        SetDeck();      
    }

    public void SetDeck()
    {
        temp = new List<GameObject>(deck);
    }

    public void PlaySelectedCards()
    {
        if (SelectedCards.Count == 0)
            return;

        foreach (GameObject card in new List<GameObject>(SelectedCards))
        {
            card.GetComponent<Card>().Deselect();
            card.transform.SetParent(playedContainer, false);
            card.transform.SetSiblingIndex(playedContainer.childCount);
        }

        SelectedCards.Clear();
    }

    public List<GameObject> DrawCards(int carsToSpawn)
    {
        List<GameObject> cards = new();
        for (int i = 0; i < carsToSpawn; i++) 
        {
            int cardIndex = Random.Range(0, temp.Count);
            GameObject newCard = Instantiate(temp[cardIndex], handContainer);
            temp.Remove(temp[cardIndex]);
        }
        return cards;
    }

    public void DeleteCards()
    {
        foreach (Transform child in handContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void DicardCards()
    {
        if (SelectedCards.Count == 0)
            return;

        foreach (GameObject card in new List<GameObject>(SelectedCards))
        {
            card.GetComponent<Card>().Deselect();
            Destroy(card);
        }                
    }
}