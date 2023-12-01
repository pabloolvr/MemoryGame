using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PlayerData
{
    [SerializeField] private List<string> _photoPaths;

    public string[] GetPhotoPaths() => _photoPaths.ToArray();
    public bool AddPhotoPath(string path)
    {
        if (!_photoPaths.Contains(path))
        {
            _photoPaths.Add(path);
            return true;
        }

        return false;
    }
    public bool RemovePhotoPath(string path)
    {
        if (_photoPaths.Contains(path))
        {
            _photoPaths.Remove(path);
            return true;
        }

        return false;
    }

    public PlayerData(List<string> photoPaths)
    {
        _photoPaths = photoPaths;
    }
}
