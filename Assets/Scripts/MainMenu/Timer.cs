using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool IsRunning { get; private set; }
    public float ElapsedTime => _elapsedTime;
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
        set
        {
            _isPaused = value;
            IsRunning = !value;
        }
    }
    public int GetHour => Mathf.FloorToInt(_elapsedTime / 3600f);
    public int GetMinute => Mathf.FloorToInt((_elapsedTime - GetHour * 3600f) / 60f);
    public int GetSecond => Mathf.FloorToInt(_elapsedTime - GetHour * 3600f - GetMinute * 60f);

    private bool _isPaused;
    private float _elapsedTime;

    private void Awake()
    {
        IsRunning = false;
        _elapsedTime = 0f;
        _isPaused = false;
    }

    public void StartTimer()
    {
        IsRunning = true;
        _elapsedTime = 0f;
    }

    public void Stop()
    {
        IsRunning = false;
        _elapsedTime = 0f;
    }

    void Update()
    {
        if (IsRunning)
        {
            _elapsedTime += Time.deltaTime;
        }
    }
}
