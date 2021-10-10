using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject objToSpawn;
    public Transform parent;
    // Start is called before the first frame update
    public GameObject SpawnObject()
    {
       return Instantiate(objToSpawn, parent);
    }
}
