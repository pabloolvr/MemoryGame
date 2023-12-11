using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public static string SavePath => Instance.savePath;
    public PlayerData PlayerData => _playerData;

    [SerializeField] private string _fileName = "jogodamemoria.save";
    [SerializeField] private PlayerData _playerData;

    public event Action OnDataLoaded = () => { };
    public event Action OnDataSaved = () => { };

    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            savePath = Application.persistentDataPath + "/" + _fileName;
            Initialize();

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // tries to read local save file, it can fail if it doesn't exist ot
        // the save file is corrupted, which happens if file is modified
        // manually or when loading a version with a save file structure
        // that does not match the locally stored save
        if (!LoadPlayerData())
        {
            SavePlayerData();
        }
    }

    /// <summary>
    /// Save the player data in memory in a binary save file.
    /// If this is called more than once at the same frame or in a very small 
    /// period of time, only the first call will work.
    /// </summary>
    public void SavePlayerData()
    {
        using (var stream = File.Open(savePath, FileMode.OpenOrCreate))
        {
            using (var writer = new BinaryWriter(stream))
            {
                string[] photoPaths = _playerData.GetPhotoPaths();
                writer.Write(photoPaths.Length);

                foreach (string path in photoPaths)
                {
                    writer.Write(path);
                }
            }
        }
#if UNITY_EDITOR
        Debug.Log("Saved Save at " + savePath);
#endif
        OnDataSaved();
        //formatter.Serialize(stream, _playerData);
    }

    public bool LoadPlayerData()
    {
        if (File.Exists(savePath))
        {
            try
            {
                using (var stream = File.Open(savePath, FileMode.Open))
                {
                    List<string> photoPaths = new List<string>();

                    using (var reader = new BinaryReader(stream))
                    {
                        int photoPathsCount = reader.ReadInt32();
                        for (int i = 0; i < photoPathsCount; i++)
                        {
                            string path = reader.ReadString();
#if UNITY_EDITOR
#endif
                            Debug.Log($"Loaded path {path}");
                            photoPaths.Add(path);
                        }
                    }

                    _playerData = new PlayerData(photoPaths);
                }
#if UNITY_EDITOR
#endif
                Debug.Log("Loaded Save from " + savePath);
                OnDataLoaded();
                return true;
            }
            catch
            {
#if UNITY_EDITOR
#endif
                Debug.LogError($"Error loading save file: Save is corrupt or incompatible with current version");
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
