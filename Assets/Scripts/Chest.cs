using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    Animator animator;
    int openhash = Animator.StringToHash("OpenState");
    bool isopen;

	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        isopen = false;
	}
	
    public void OpenUp() {
        Debug.Log("Attempted open");
        if (!isopen)
        {
            animator.Play("Openning");
            isopen = true;
        }
    }
}
