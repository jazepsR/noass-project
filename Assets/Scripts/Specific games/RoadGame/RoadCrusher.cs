using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadCrusher : MonoBehaviour
{
    public static RoadCrusher instance;
    private Animator anim;
    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartCrush()
    {
        anim.SetTrigger("crush");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
