using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public bool followX = false;
    public bool followY = false;
    public bool followZ = false;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
       if (target == null)
            return;
        Vector3 pos = transform.position;
        if (followX)
            pos.x = target.position.x;
        if (followY)
            pos.y = target.position.y;
        if (followZ)
            pos.z = target.position.z;
        transform.position = pos;
    }
}
