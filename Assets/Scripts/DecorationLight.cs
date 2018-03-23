using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecorationLight : MonoBehaviour {

    MeshRenderer mesh;
    Light torchlight;
    ParticleSystem particles;

	// Use this for initialization
	void Start () {
        mesh = GetComponent<MeshRenderer>();
        torchlight = GetComponent<Light>();
        particles = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!mesh.enabled & torchlight.enabled) {
            torchlight.enabled = false;
            particles.Stop();
        }
        else if (mesh.enabled & !torchlight.enabled) {
            torchlight.enabled = true;
            particles.Play();   
        }
	}
}
