using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

    public bool ranged;
    public float meleefraction;
    public float launchmultiplier;
    public GameObject projectile;

	// Use this for initialization
	//void Start () {
		
	//}
	
	// Update is called once per frame
	//void Update () {
		
	//}

    public void Shoot(Vector3 target, int basespeed) {
        GameObject ShotProjectile;
        ShotProjectile = Instantiate(projectile, transform.position, Quaternion.LookRotation(target-transform.position));
        ShotProjectile.GetComponent<Rigidbody>().velocity = (ShotProjectile.transform.rotation * Vector3.forward) * launchmultiplier * basespeed;
        ShotProjectile.transform.parent = transform.parent;
        GameManager.instance.projectile = ShotProjectile;
        GameManager.instance.waitforprojectile = true;
    }

}
