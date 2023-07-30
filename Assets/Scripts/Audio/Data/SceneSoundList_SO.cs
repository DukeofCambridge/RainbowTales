using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Sound_SO/SceneSoundList")]
public class SceneSoundList_SO : ScriptableObject
{
    public List<SceneSoundItem> sceneSoundList;

    public SceneSoundItem GetSceneSoundItem(string name)
    {
        return sceneSoundList.Find(s => s.sceneName == name);
    }
}

[System.Serializable]
public class SceneSoundItem
{
    [SceneName] public string sceneName;
    public SoundName ambient;
    public SoundName bgm;
}