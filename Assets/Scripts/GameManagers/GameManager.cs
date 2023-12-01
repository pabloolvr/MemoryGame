using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Random = UnityEngine.Random;

public enum GameDifficulty
{
    VeryEasy,
    Easy,
    Medium,
    Hard,
    VeryHard
}

public class GameManager : MonoBehaviour
{
    //public int DiscoveredPairs { get; private set; }

    public static int DifficultiesCount => Enum.GetValues(typeof(GameDifficulty)).Length;
    public static GameDifficulty CurDifficulty { get; set; } = GameDifficulty.VeryEasy;

    [Header("References")]
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private ScrollRect _cardsContainer;
    [SerializeField] private Timer _timer;

    [Header("Prefabs")]
    [SerializeField] private Card cardPrefab;

    [Header("Settings")]
    [SerializeField] private int[] _pairsPerDifficulty;

    //private List<Card> _cards;
    //private int _gameDifficulty;
    private int _discoveredPairs;
    private Card _firstSelectCard;
    private Card _secondSelectCard;

    void Start()
    {
        //_gameDifficulty = PlayerPrefs.GetInt("GameDifficulty", 0);
        _discoveredPairs = 0;
        _timer.StartTimer();
        _firstSelectCard = null;
        _secondSelectCard = null;
        SpawnCards();
    }

    void Update()
    {
        _uiManager.UpdateTimerText(_timer.GetHour, _timer.GetMinute, _timer.GetSecond);
    }

    private void SpawnCards()
    {
        List<Card> cards = new List<Card>();
        List<Texture2D> images = ImageManager.Instance.ImageList;

        for (int i = 0; i < _pairsPerDifficulty[(int)CurDifficulty]; i++)
        {
            Texture2D texture;

            if (i < images.Count)
            {
                texture = images[i];
            }
            else
            {
                texture = null;
            }

            cards.Add(SpawnCard(i, texture));
            cards.Add(SpawnCard(i, texture));
        }

        int cardsCount = cards.Count;

        for (int i = 0; i < cardsCount; i++)
        {
            int randomIndex = Random.Range(0, cardsCount - i);
            cards[randomIndex].transform.SetAsFirstSibling();
            cards.RemoveAt(randomIndex);
        }
    }

    private Card SpawnCard(int index, Texture2D texture)
    {
        Card card = Instantiate(cardPrefab, _cardsContainer.content);
        card.Initialize(index, texture);
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
        else if (_firstSelectCard != card && _secondSelectCard == null)
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

        if (_discoveredPairs == _pairsPerDifficulty[(int)CurDifficulty])
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
        _timer.IsPaused = true;
        _uiManager.ShowGameFinishPanel(_timer.GetHour, _timer.GetMinute, _timer.GetSecond);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
