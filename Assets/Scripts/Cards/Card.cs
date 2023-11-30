using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int Id { get; private set; } = -1;
    public bool IsUpsideDown { get; private set; }
    public bool IsFlipping { get; private set; }

    public event Action<Card> OnCardClicked = (card) => { };
    public event Action<Card> OnCardFlipped = (card) => { };

    [Header("References")]
    [SerializeField] private GameObject _cardBack;
    [SerializeField] private TextMeshProUGUI _idField;
    [SerializeField] private GameObject _discoveredOverlay;

    [Header("Settings")]
    [SerializeField, Tooltip("Measured in angles per second.")] 
    private float _flipSpeed = 30f;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => 
        {
            //if (IsUpsideDown) Flip();
            OnCardClicked(this);
        });

        IsUpsideDown = true;
        IsFlipping = false;
    }

    public void Initialize(int id)
    {
        Id = id;
        _idField.text = Id.ToString();
    }

    public void Flip()
    {        
        StartCoroutine(FlipAnimation());
        Debug.Log($"Card {Id} flipped");
    }

    private IEnumerator FlipAnimation()
    {
        float targetY, newY, curY;

        if (IsUpsideDown)
        {
            targetY = 0;
        }
        else
        {
            targetY = 180;
        }
       
        do
        {
            curY = transform.rotation.eulerAngles.y;
            newY = Mathf.MoveTowards(curY, 90, _flipSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, newY, 0f);
            yield return null;
        } 
        //while (Mathf.Abs(newY - targetY) > 0.1f);
        while (newY != 90);

        _cardBack.SetActive(newY - targetY >= 0);

        do
        {
            curY = transform.rotation.eulerAngles.y;
            newY = Mathf.MoveTowards(curY, targetY, _flipSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, newY, 0f);
            yield return null;
        }
        //while (Mathf.Abs(newY - targetY) > 0.1f);
        while (newY != targetY);

        OnCardFlipped(this);
        IsUpsideDown = !IsUpsideDown;

        if (IsUpsideDown)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }

        yield return null;
    }

    public void SetDiscovered()
    {
        _button.interactable = false;
        _discoveredOverlay.SetActive(true);
    }
}
