using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject _imageItemPrefab;

    [Header("References")]
    [SerializeField] private ScrollRect _addedImagesRect;
    [SerializeField] private ScrollRect _imagesToRemoveRect;
    [SerializeField] private RemoveImageConfirm _removeImagePanel;

    private Dictionary<string, GameObject> _imageListToAddItems = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _imageListToRemoveItems = new Dictionary<string, GameObject>();

    private void Start()
    {
        ImageManager.Instance.OnImageAdded += OnImageAdded;
        ImageManager.Instance.OnImageRemoved += OnImageRemoved;

        StartImagePanels();
    }

    private void StartImagePanels()
    {
        foreach (var image in ImageManager.Instance.Images)
        {
            OnImageAdded(image.Key);
        }
    }

    private void OnImageRemoved(string path)
    {
        //throw new NotImplementedException();
        if (_imageListToAddItems.TryGetValue(path, out GameObject item))
        {
            _imageListToAddItems.Remove(path);
            Destroy(item);
        }
        
        if (_imageListToRemoveItems.TryGetValue(path, out item))
        {
            _imageListToRemoveItems.Remove(path);
            Destroy(item);
        }
    }

    private void OnImageAdded(string path)
    {
        InstantiateItem(_addedImagesRect, path, ImageManager.Instance.Images[path], false);
        InstantiateItem(_imagesToRemoveRect, path, ImageManager.Instance.Images[path], true);
    }

    private void InstantiateItem(ScrollRect list, string path, Texture2D texture, bool toRemove)
    {
        GameObject item = Instantiate(_imageItemPrefab, list.content);
        item.GetComponentInChildren<RawImage>().texture = texture;

        AspectRatioFitter aspectRatioFitter = item.GetComponentInChildren<AspectRatioFitter>();
        aspectRatioFitter.aspectRatio = ((float)texture.width) / ((float)texture.height);
        //Debug.Log($"{texture.width} / {texture.height} = {aspectRatioFitter.aspectRatio}");
        //aspectRatioFitter.enabled = true;

        Button button = item.GetComponent<Button>();
        if (toRemove)
        {
            _imageListToRemoveItems.TryAdd(path, item);
            button.onClick.AddListener(() =>
            {
                _removeImagePanel.SetPanel(path);
            });
        }
        else
        {
            _imageListToAddItems.TryAdd(path, item);
            Destroy(button);
        }
    }

    public void StartNewGame(int gameDifficulty)
    {
        if (gameDifficulty < GameManager.DifficultiesCount)
        {
            GameManager.CurDifficulty = (GameDifficulty) gameDifficulty;
            SceneManager.LoadScene("Game");
            //PlayerPrefs.SetInt("GameDifficulty", gameDifficulty);
        }
    }
}
