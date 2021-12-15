using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortingGameGate : MonoBehaviour
{
    public static SortingGameGate instance;
    private Animator anim;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }




    public void Open()
    {
        anim.SetTrigger("open");
        Invoke("Close", 2.5f);
    }


    public void Close()
    {

        anim.SetTrigger("close");

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
