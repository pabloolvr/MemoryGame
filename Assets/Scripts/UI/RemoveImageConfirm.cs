using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveImageConfirm : MonoBehaviour
{
    [SerializeField] private RawImage _image;
    [SerializeField] private AspectRatioFitter _aspectRatioFitter;
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

        Texture2D texture = ImageManager.Instance.Images[path];

        _image.texture = texture;
        _aspectRatioFitter.aspectRatio = ((float)texture.width) / ((float)texture.height);
    }

    private void OnYesButtonClicked()
    {
        ImageManager.Instance.RemoveImage(_curPath);
    }
}
