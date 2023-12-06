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

    private Dictionary<string, Texture2D> _images = new Dictionary<string, Texture2D>();
    //private List<Texture2D> _images;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadImages();        
    }

    private void LoadImages()
    {
        //_images = new List<Texture2D>();
        _images = new Dictionary<string, Texture2D>();

        foreach (string path in SaveManager.Instance.PlayerData.GetPhotoPaths())
        {
            if (TryGetImageTexture(path, out Texture2D texture))
            {
                //_images.Add(texture);
                _images.TryAdd(path, texture);
            }
            else
            {
                RemoveImage(path);
            }
        }
    }

    public bool TryGetImageTexture(string path, out Texture2D texture)
    {
        texture = NativeGallery.LoadImageAtPath(path);

        return path != null && texture != null;
    }

    public void PickImage(int maxSize)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            
            if (TryGetImageTexture(path, out Texture2D texture))
            {
                //// Assign texture to a temporary quad and destroy it after 5 seconds
                //GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //quad.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 2.5f;
                //quad.transform.forward = Camera.main.transform.forward;
                //quad.transform.localScale = new Vector3(1f, texture.height / (float)texture.width, 1f);

                //Material material = quad.GetComponent<Renderer>().material;
                //if (!material.shader.isSupported) // happens when Standard shader is not included in the build
                //    material.shader = Shader.Find("Legacy Shaders/Diffuse");

                //material.mainTexture = texture;

                //Destroy(quad, 5f);

                TryAddImage(path, texture);
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
