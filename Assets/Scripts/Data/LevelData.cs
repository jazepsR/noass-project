using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "levelData", menuName = "ScriptableObjects/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelName;
    public Sprite levelIcon;
    public Sprite levelScreenshot;
}
