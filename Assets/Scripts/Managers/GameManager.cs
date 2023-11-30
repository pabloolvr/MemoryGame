using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //public int DiscoveredPairs { get; private set; }

    [Header("References")]
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private ScrollRect _cardsContainer;

    [Header("Prefabs")]
    [SerializeField] private Card cardPrefab;

    [Header("Settings")]
    [SerializeField] private int _pairs = 10;

    //private List<Card> _cards;
    private int _discoveredPairs;
    private Card _firstSelectCard;
    private Card _secondSelectCard;

    void Start()
    {
        _discoveredPairs = 0;
        SpawnCards();
    }

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
        card.OnCardClicked += OnCardClicked;
        return card;
    }

    private void OnCardClicked(Card card)
    {
        if (_firstSelectCard == null)
        {
            _firstSelectCard = card;
            _firstSelectCard.Flip();
        }
        else if (_secondSelectCard == null)
        {
            _secondSelectCard = card;
            _secondSelectCard.Flip();
            
            StartCoroutine(CheckSelectedCards());
        }
    }

    private IEnumerator CheckSelectedCards()
    {
        yield return new WaitForSeconds(1f);

        while (_firstSelectCard.IsFlipping || _secondSelectCard.IsFlipping)
        {
            yield return null;
        }

        if (_firstSelectCard.Id == _secondSelectCard.Id)
        {
            yield return new WaitForSeconds(.5f);
            SetSelectedCardsDiscovered();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            FlipSelectedCards();
        }

        _firstSelectCard = null;
        _secondSelectCard = null;
    }

    private void SetSelectedCardsDiscovered()
    {
        _firstSelectCard.SetDiscovered();
        _secondSelectCard.SetDiscovered();
        _discoveredPairs++;

        if (_discoveredPairs == _pairs)
        {
            FinishGame();
        }
    }

    private void FlipSelectedCards()
    {
        _firstSelectCard.Flip();
        _secondSelectCard.Flip();
    }

    private void FinishGame()
    {
        _uiManager.ShowGameFinishPanel();
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToMenu()
    {
        //SceneManager.LoadScene(0);
    }
}
