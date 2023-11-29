using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Card cardPrefab;

    [SerializeField] private int _pairs = 10;
    [SerializeField] private ScrollRect _cardsContainer;

    //private List<Card> _cards;
    private Card _firstSelectCard;
    private Card _secondSelectCard;

    void Start()
    {
        SpawnCards();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnCards()
    {
        List<Card> cards = new List<Card>();

        for (int i = 0; i < _pairs; i++)
        {
            cards.Add(SpawnCard(i));
            cards.Add(SpawnCard(i));
        }

        int cardsCount = cards.Count;

        for (int i = 0; i < cardsCount; i++)
        {
            int randomIndex = Random.Range(0, cardsCount - i);
            cards[randomIndex].transform.SetAsFirstSibling();
            cards.RemoveAt(randomIndex);
        }
    }

    private Card SpawnCard(int index)
    {
        Card card = Instantiate(cardPrefab, _cardsContainer.content);
        card.Initialize(index);
        card.OnCardFlipped += OnCardFlipped;
        return card;
    }

    private void OnCardFlipped(Card card)
    {
        //transform.
    }
}
