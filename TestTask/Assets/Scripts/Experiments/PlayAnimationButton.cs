using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimationButton : MonoBehaviour
{
    public Animator myAnim;

    public void Next()
    {
        myAnim.SetBool("Next", true);

    }
    public void Back()
    {
        myAnim.SetBool("Back", true);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
