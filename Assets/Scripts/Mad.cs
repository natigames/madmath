using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mad : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer theSR;

    public static Mad instance;

    private void Awake()
    {
        instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {

        anim = GetComponent<Animator>();
        theSR = GetComponent<SpriteRenderer>();
    }

    public void doAnim(string status)
    {
        anim.SetTrigger(status); // set Animator variable to animate jump!
    }


}
