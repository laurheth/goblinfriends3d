using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour {

    Renderer rend;
    ParticleSystem party;
    bool smashed;
    CameraManager camscript;

	// Use this for initialization
	void Start () {
        smashed = false;
        rend = gameObject.GetComponent<Renderer>();
        party = gameObject.GetComponent<ParticleSystem>();
        camscript = FindObjectOfType<Camera>().GetComponent<CameraManager>();
	}
	
    void OnTriggerEnter() {
        if (!smashed)
        {
            smashed = true;
            rend.enabled = false;
            party.Play();
            tag = "Untagged";
            camscript.UpdateObjList();
        }
    }
}
