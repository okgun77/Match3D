using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Audio
{
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "ScriptableObjects/SoundLibrary", order = 1)]
public class SoundLibrary : ScriptableObject
{
    public List<Audio> bgmAudios;
    public List<Audio> sfxAudios;

    public AudioClip GetClipByName(string _name, bool _isBGM)
    {
        List<Audio> audios = _isBGM ? bgmAudios : sfxAudios;
        foreach (Audio audio in audios)
        {
            if (audio.name == _name)
                return audio.clip;
        }
        return null;
    }

    
}