using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DifficultyConfig : MonoBehaviour
{
    [SerializeField] private int _minPairQty = 30;
    [SerializeField] private int _maxPairQty = 50;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private Button _startButton;

    private List<string> _options;

    void Start()
    {
        _startButton.onClick.AddListener(StartGame);

        _options = new List<string>();
        for (int i = _minPairQty; i <= _maxPairQty; i++)
        {
            _options.Add(i.ToString());
        }

        _dropdown.AddOptions(_options);
    }

    private void StartGame()
    {
        GameManager.MaxDifficPairsQty = int.Parse(_options[_dropdown.value]);
        Debug.Log($"MaxDifficPairsQty is {GameManager.MaxDifficPairsQty}");
        SceneManager.LoadScene("Game");
    }
}
