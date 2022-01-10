using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public static class Helpers 
{

    public static bool IsTileAcceptable(TileScript tile, List<Destination> possibleDestinations)
    {
        foreach (Destination dest in tile.data.possibleDestinations)
        {
            //Debug.LogError("dest: " + dest + " possible: " + string.Join(",", possibleDestinations));
            if (possibleDestinations.Contains(dest))
            {
                return true;
            }
        }
        return false;
    }

    public static bool isLatvian()
    {
        return LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[0];
    }
    public static bool ListContainsTile(TileScript ts, List<Destination> destinationList)
    {
        bool correctType = false;
        foreach (Destination dest in ts.data.possibleDestinations)
        {
            if (destinationList.Contains(dest))
            {
                correctType = true;
                break;
            }
        }
        return correctType;
    }
    public static List<T> ShuffleList<T>(this List<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        return list;
    }

    public static List<T> CopyList<T>(this List<T> list)
    {
        List<T> newList = new List<T>();
       foreach(T obj in list)
        {
            newList.Add(obj);
        }
        return newList;
    }

}
