using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Games { mountain, sorting, gates, constructionRecycle, compost, powerGeneration }

[CreateAssetMenu(fileName = "tileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileScriptable : ScriptableObject
{
    public Sprite image;
    public Color[] possibleBgColors;
    public Games[] possibleGames;
    public string DisplayName;
    public bool isHazard;
}
