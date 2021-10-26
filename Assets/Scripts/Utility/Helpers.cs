using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers 
{

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
