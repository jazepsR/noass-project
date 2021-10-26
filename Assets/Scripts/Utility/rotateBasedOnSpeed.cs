using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateBasedOnSpeed : MonoBehaviour
{
    public float speed = 10;
    private Vector3 prevPosition = Vector3.zero;

    private void Start()
    {
        transform.rotation = Quaternion.identity;
    }
    // Update is called once per frame
    void Update()
    {
        if (prevPosition != Vector3.zero)
        {
            transform.Rotate(new Vector3(0, 0, (transform.position.magnitude - prevPosition.magnitude) * speed * Time.deltaTime));
        }
        prevPosition = transform.position;
    }
}
