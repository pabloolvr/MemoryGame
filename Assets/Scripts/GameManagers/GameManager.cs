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

[Serializable]
public struct DifficultySettings
{
    public GameDifficulty Difficulty => _difficulty;
    public int PairsQty
    {
        get
        {
            return _pairsQty;
        }
        set
        {
            _pairsQty = value;
        }
    }
    public int GridSize => _gridSize;

    [SerializeField] private GameDifficulty _difficulty;
    [SerializeField] private int _pairsQty;
    [SerializeField] private int _gridSize;
}

public class GameManager : MonoBehaviour
{
    //public int DiscoveredPairs { get; private set; }

    public static int DifficultiesCount => Enum.GetValues(typeof(GameDifficulty)).Length;
    public static GameDifficulty CurDifficulty { get; set; } = GameDifficulty.VeryEasy;
    public static int MaxDifficPairsQty { get; set; } = 30;

    [Header("References")]
    [SerializeField] private UIManager _uiManager;
    [SerializeField] private ScrollRect _cardsContainer;
    [SerializeField] private Timer _timer;

    [Header("Prefabs")]
    [SerializeField] private Card cardPrefab;

    [Header("Settings")]
    [SerializeField] private DifficultySettings[] _difficulties;

    //private List<Card> _cards;
    //private int _gameDifficulty;
    private GridLayoutGroup _cardGridLayoutGroup;
    private float _cardGridWidth;
    private float _cardGridHeight;
    private int _discoveredPairs;
    private Card _firstSelectCard;
    private Card _secondSelectCard;

    private void Awake()
    {
        _discoveredPairs = 0;
        _firstSelectCard = null;
        _secondSelectCard = null;
        
        _cardGridLayoutGroup = _cardsContainer.content.GetComponent<GridLayoutGroup>();
    }

    void Start()
    {
        //_gameDifficulty = (GameDifficulty) PlayerPrefs.GetInt("GameDifficulty", 0);
        _timer.StartTimer();
        SpawnCards();
        UpdateCardGridCellSize();
    }

    void Update()
    {
        _uiManager.UpdateTimerText(_timer.GetHour, _timer.GetMinute, _timer.GetSecond);

        if (_cardGridWidth != _cardsContainer.viewport.rect.width || _cardGridHeight != _cardsContainer.viewport.rect.height)
        {
            UpdateCardGridCellSize();
        }
    }

    private void UpdateCardGridCellSize()
    {
        int gridSize = _difficulties[(int)CurDifficulty].GridSize;
        float gridWidth = _cardsContainer.viewport.rect.width;
        float gridHeight = _cardsContainer.viewport.rect.height;
        float xSpacing = _cardGridLayoutGroup.spacing.x;
        float ySpacing = _cardGridLayoutGroup.spacing.y;
#if UNITY_EDITOR
        Debug.Log($"Updating Grid Cell Size with gridWidth = {gridWidth}, gridHeight = {gridHeight}, xSpacing = {xSpacing}, ySpacing = {ySpacing}");
#endif
        float cellSizeX = (gridWidth - (xSpacing * (gridSize + 1))) / gridSize;
        float cellSizeY = (gridHeight - (ySpacing * (gridSize + 1))) / gridSize;
        Vector2 cellSize = new Vector2(cellSizeX, cellSizeY);

        _cardGridWidth = gridWidth;
        _cardGridHeight = gridHeight;
        _cardGridLayoutGroup.cellSize = cellSize;
    }

    private void SpawnCards()
    {
        List<Card> cards = new List<Card>();
        List<Texture2D> images = ImageManager.Instance.ImageList;

        if (CurDifficulty == GameDifficulty.VeryHard)
        {
            _difficulties[(int)CurDifficulty].PairsQty = MaxDifficPairsQty;
        }

        for (int i = 0; i < _difficulties[(int)CurDifficulty].PairsQty; i++)
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

        Shuffle(cards);

        int cardsCount = cards.Count;

        for (int i = 0; i < cardsCount; i++)
        {
            int randomIndex = Random.Range(0, cardsCount - i);
            cards[randomIndex].transform.SetAsFirstSibling();
            cards.RemoveAt(randomIndex);
        }
    }

    private void Shuffle(List<Card> cards)
    {
        int n = cards.Count;

        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            Card val = cards[k];
            cards[k] = cards[n];
            cards[n] = val;
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

        if (_discoveredPairs == _difficulties[(int)CurDifficulty].PairsQty)
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

    public void PauseGame(bool active)
    {
        _uiManager.SetPausePanelActive(active);
        _timer.IsPaused = active;
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
