using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "levelData", menuName = "ScriptableObjects/LevelData")]
public class LevelDataScriptable : ScriptableObject
{
    public string levelName;
    public string levelNameEn;
    public string levelScene;
    public Sprite levelIcon;
    public Sprite levelSelection;
    public Sprite[] tutorialSprites;
    public Sprite[] tutorialSpritesEn;
    public bool available = true;
    public TileGroup[] tileGroups;

    public bool isLastPage(int group, int page, int elementCount)
    {
        return (tileGroups[group].currentTiles.Count - 1) / elementCount == page;
    }
    public bool isLastGroup(int group)
    {
        // Debug.LogError("lenght: " + tileGroups.Length + " received: " + group + " result: " + (tileGroups.Length == group + 1));
        return tileGroups.Length == group + 1;
    }
    public int GetSheetCount(int group, int elementCount)
    {
        int c = (tileGroups[group].currentTiles.Count - 1) / elementCount;
        return c;
    }

    public void SetupElementList()
    {
        foreach (TileGroup group in tileGroups)
        {
            group.currentTiles = new List<TileScriptable>();
            foreach (TileScriptable tile in group.tiles)
            {
                if (tile.isHard)
                {
                    if (!Var.isEasy)
                    {
                        group.currentTiles.Add(tile);
                    }
                }
                else
                {
                    group.currentTiles.Add(tile);
                }
            }
        }
    }
}
