using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ImageManager : MonoBehaviour
{
    public List<Texture2D> ImageList => _images.Values.ToList();
    public Dictionary<string, Texture2D> Images => _images;
    //public Texture2D GetImage(string path) => _images.TryGetValue(path, out Texture2D texture) ? texture : null;
    public static ImageManager Instance { get; private set; }

    public event Action<string> OnImageAdded = (path) => { };
    public event Action<string> OnImageRemoved = (path) => { };
    public event Action OnImagesLoaded = () => { };

    private Dictionary<string, Texture2D> _images = new Dictionary<string, Texture2D>();
    //private List<Texture2D> _images;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("Initialized ImageManager");
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }       
    }

    public void LoadImages()
    {
        //_images = new List<Texture2D>();
        _images = new Dictionary<string, Texture2D>();

        foreach (string path in SaveManager.Instance.PlayerData.GetPhotoPaths())
        {
            if (TryGetImageTexture(path, out Texture2D texture))
            {
                Debug.Log($"Loaded texture {texture} of size {texture.width}:{texture.height} from path {path}");
                //_images.Add(texture);
                _images.TryAdd(path, texture);
            }
            else
            {
                Debug.Log($"Couldn't find texture on path {path}");
                RemoveImage(path);
            }
        }

        OnImagesLoaded();
    }

    public bool TryGetImageTexture(string path, out Texture2D texture)
    {
        texture = NativeGallery.LoadImageAtPath(path);        
        return path != null && texture != null;
    }

    public void AddImage()
    {
        if (NativeGallery.CanSelectMultipleFilesFromGallery())
        {
            PickMultipleImages();
        }
        else
        {
            PickSingleImage();
        }
    }

    private void PickSingleImage()
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);

            if (TryGetImageTexture(path, out Texture2D texture))
            {
                TryAddImage(path, texture);
            }
        });

        Debug.Log("Permission result: " + permission);
    }

    private void PickMultipleImages()
    {
        NativeGallery.Permission permission = NativeGallery.GetImagesFromGallery((paths) =>
        {
            foreach (string path in paths)
            {
                Debug.Log("Image path: " + path);

                if (TryGetImageTexture(path, out Texture2D texture))
                {
                    TryAddImage(path, texture);
                }
            }
        });

        Debug.Log("Permission result: " + permission);
    }

    public bool TryAddImage(string path, Texture2D texture2D)
    {
        if (SaveManager.Instance.PlayerData.AddPhotoPath(path))
        {
            SaveManager.Instance.SavePlayerData();
            _images.TryAdd(path, texture2D);
            OnImageAdded(path);
            return true;
        }

        return false;
    }

    public bool RemoveImage(string path)
    {
        if (SaveManager.Instance.PlayerData.RemovePhotoPath(path))
        {
            SaveManager.Instance.SavePlayerData();
            _images.Remove(path);
            OnImageRemoved(path);
            return true;
        }

        return false;
    }
}
