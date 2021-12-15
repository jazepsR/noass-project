using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Games { mountain, sorting, gates, constructionRecycle, compost, powerGeneration }
public enum Destination { Mebeles, Kalns, Sadzives_Atkritumi, Bistams, Makulatura, Pet_Polietilens, Metals, Bio, Stilks, Neskirotie, Empty, Apgerbs,
Buvnieciba, Koks, Medikamenti, Partikas_Atkritumi, Skidrie_Atkritumi, Spuldzes, Zalie_Atkritumi, Citi, Gaze}

[CreateAssetMenu(fileName = "tileData", menuName = "ScriptableObjects/TileData", order = 1)]
public class TileScriptable : ScriptableObject
{
    public Sprite image;
    public Color[] possibleBgColors;
    public Destination[] possibleDestinations;
    public string DisplayName;
}
