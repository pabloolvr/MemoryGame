using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveImageConfirm : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    [SerializeField] private Button _yesButton;

    private string _curPath;

    private void Start()
    {
        _yesButton.onClick.AddListener(OnYesButtonClicked);
    }

    public void SetPanel(string path)
    {
        gameObject.SetActive(true);
        _curPath = path;
        _image.texture = ImageManager.Instance.Images[path];
    }

    private void OnYesButtonClicked()
    {
        ImageManager.Instance.RemoveImage(_curPath);
    }
}
