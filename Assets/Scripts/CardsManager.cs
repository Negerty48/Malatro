using System.Collections.Generic;
using UnityEngine;

public class CardsManager : MonoBehaviour
{
    public Transform handContainer;
    public Transform playedContainer;
    public List<GameObject> cards;
    private List<GameObject> temp;
    public List<GameObject> SelectedCards = new();

    void Start()
    {
        temp = cards;        
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
            temp.Remove(newCard);
        }
        return cards;
    }
}