using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameFinishPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private TextMeshProUGUI _gameFinishText;
    [SerializeField] private TextMeshProUGUI _timerText;

    public void ShowGameFinishPanel(int hour, int minute, int second)
    {
        _gameFinishPanel.SetActive(true);
        _gameFinishText.text = $"Parabéns, você terminou esse jogo da memória em";

        if (hour > 0) _gameFinishText.text += $" {hour} hora{(hour > 1 ? "s" : "")},";
        if (minute > 0) _gameFinishText.text += $" {minute} minuto{(minute > 1 ? "s" : "")} e";
        if (second > 0) _gameFinishText.text += $" {second} segundo{(second > 1 ? "s" : "")}";

        _gameFinishText.text += " na dificuldade";

        switch (GameManager.CurDifficulty)
        {
            case GameDifficulty.VeryEasy:
                _gameFinishText.text += " Muito Fácil!";
                break;
            case GameDifficulty.Easy:
                _gameFinishText.text += " Fácil!";
                break;
            case GameDifficulty.Medium:
                _gameFinishText.text += " Média!";
                break;
            case GameDifficulty.Hard:
                _gameFinishText.text += " Difícil!";
                break;
            case GameDifficulty.VeryHard:
                _gameFinishText.text += " Muito Difícil!";
                break;
        }

        _gameFinishText.text += "\nSerá que você consegue fazer em menos tempo?";
    }

    public void UpdateTimerText(int hour, int minute, int second)
    {
        _timerText.text = $"{hour:00}:{minute:00}:{second:00}";
    }

    public void SetPausePanelActive(bool active)
    {
        _pausePanel.SetActive(active);
    }
}
