using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedButton : MonoBehaviour
{
    public Destination destination;
    private Animator anim;
    private bool inCooldown = false;
    public bool targetState = false;
    [SerializeField] private Button button;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    public void Click(int ID)
    {
        SortingGameController.Instance.DestinationSelect(destination, ID);
    }

    public void SetClickOutcome(bool isPositive, float coolDown = -1)
    {
        if (inCooldown)
        {
            return;
        }
        if (isPositive)
        {
            anim.SetTrigger("correct");
        }
        else
        {
            anim.SetTrigger("incorrect");
        }
        if(coolDown != -1)
        {
            StartCoroutine(Cooldown(coolDown));
        }
    }

    public void SetState(bool isActive, float delay = 0)
    {
        button.interactable = isActive;
        targetState = isActive;
        if (inCooldown)
            return;
        Invoke("SetAnim", delay);
    }

    private void SetAnim()
    {
        anim.SetBool("enabled", targetState);
    }

    private IEnumerator Cooldown(float cooldownTime)
    {
        inCooldown = true;
        yield return new WaitForSeconds(1);
        anim.SetBool("enabled", false);
        yield return new WaitForSeconds(cooldownTime);
        anim.SetBool("enabled", targetState);
        inCooldown = false;
    }
}
