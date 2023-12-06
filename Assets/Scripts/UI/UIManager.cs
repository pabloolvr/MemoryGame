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
        _gameFinishText.text = $"Parab�ns, voc� terminou esse jogo da mem�ria em";

        if (hour > 0) _gameFinishText.text += $" {hour} hora{(hour > 1 ? "s" : "")},";
        if (minute > 0) _gameFinishText.text += $" {minute} minuto{(minute > 1 ? "s" : "")} e";
        if (second > 0) _gameFinishText.text += $" {second} segundo{(second > 1 ? "s" : "")}";

        _gameFinishText.text += " na dificuldade";

        switch (GameManager.CurDifficulty)
        {
            case GameDifficulty.VeryEasy:
                _gameFinishText.text += " Muito F�cil!";
                break;
            case GameDifficulty.Easy:
                _gameFinishText.text += " F�cil!";
                break;
            case GameDifficulty.Medium:
                _gameFinishText.text += " M�dia!";
                break;
            case GameDifficulty.Hard:
                _gameFinishText.text += " Dif�cil!";
                break;
            case GameDifficulty.VeryHard:
                _gameFinishText.text += " Muito Dif�cil!";
                break;
        }

        _gameFinishText.text += "\nSer� que voc� consegue fazer em menos tempo?";
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
