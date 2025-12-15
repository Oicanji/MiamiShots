using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCharacter : MonoBehaviour
{
    public string characterName;
    public string weaponName;

    public void Play()
    {
        PlayerPrefs.SetString("characterName", characterName);
        PlayerPrefs.SetString("weaponName", weaponName);
        PlayerPrefs.Save();
        FindFirstObjectByType<SceneController>().LoadScene("level_1");
    }
}
