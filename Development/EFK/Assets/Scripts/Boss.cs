using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    private Animator animator;
    private ExitTrigger exitTrigger;
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActivateAnimator(ExitTrigger trigger)
    {
        animator.enabled = true;
        exitTrigger = trigger;
        FindObjectOfType<AudioManager>().Play("BossTheme");
    }

    public void ActivateError()
    {
        animator.SetTrigger("Error");
    }
    
    public void ActivateExplosion()
    {
        animator.SetTrigger("Explosion");
        FindObjectOfType<AudioManager>().Play("Explosion");
        FindObjectOfType<AudioManager>().Stop("BossTheme");
    }

    public void DeactivateBoss()
    {
        exitTrigger.SetBossExploded();
        
        gameObject.SetActive(false);
    }


}
