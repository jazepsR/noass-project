using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOneObject : MonoBehaviour
{
    public GameObject[] objects;
    // Start is called before the first frame update
    void Start()
    {
        EnableOneObj();
    }

    private void EnableOneObj()
    {
        int selectedObj = Random.Range(0, objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].SetActive(selectedObj == i);
        }
    }
}