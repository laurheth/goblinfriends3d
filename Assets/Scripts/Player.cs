﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    //private bool thisturn;
    public string playerclass;
    public string playername;
    public LayerMask layermask;
    public GameObject torch;
    public GameObject sword;
    public GameObject debugblock;
    public GameObject inventorymenu;
    Inventory inventoryscript;
    public List<GameObject> inventory;
    int dxmod;
    int dzmod;
    bool switchdir;
    bool emoting;
    private bool aiming;
    //public Camera cam;
    //int ragdollforawhile;
    // Use this for initialization

    public void Awake() {
        inventorymenu.GetComponent<Inventory>().playerscript = this;
        emoting = false;
    }

	protected override void Start () {
        inventoryscript = inventorymenu.GetComponent<Inventory>();
        inventory = new List<GameObject>();
        switchdir = false;
        dxmod = 1;
        dzmod = 1;
        aiming = false;
        level = 1;
        playername = "Lauren!";
        playerclass = "Goblin Show-er";
        hitpoints = 50;
        //ragdollforawhile = 0;
        ismonster = false;
        damagetodo = 5;
        alive = true;
        thisturn = true;
        righthand = Instantiate(torch,new Vector3(transform.position.x-0.41f,transform.position.y+0.2f,transform.position.z-0.3f),transform.rotation);
        righthand.transform.parent = transform;

        lefthand = Instantiate(sword, new Vector3(transform.position.x + 0.41f, transform.position.y + 0.2f, transform.position.z - 0.3f), transform.rotation);
        lefthand.transform.parent = transform;

        AddToInventory(righthand, false);
        AddToInventory(lefthand, false);

        base.Start();

	}

    public void ControlDirections(int angle) {
        int scratch=dxmod;
        if (angle>0) {
            dxmod = dzmod;
            dzmod = -scratch;
        }
        else {
            dxmod = -dzmod;
            dzmod = scratch;
        }
        switchdir = !switchdir;
    }

    public void AddToInventory(GameObject item, bool hideit = true,int slot=-1)
    {
        item.GetComponent<Item>().SetCollide(true);
        if (!inventory.Contains(item))
        {
            inventory.Add(item);
        }
        if (hideit)
        {
            item.transform.SetParent(transform);
            item.SetActive(false);
        }

        if (slot>-1) {
            SetSlot(slot, item);
        }

        inventoryscript.ClearContents();
        inventoryscript.UpdateContents();
    }

    protected override void Update()
    {
        base.Update();
        if (!emoting) {
            emoting=emotebubble.SetEmote(0);
        }
        /*else {
            emoting=!(emotebubble.ClearEmote());
        }*/
        if (falling == true) {
            thisturn = true;
            return;
        }
        if (alive == false)
        {
            thisturn = false;
        }
        if (moving == true)
        {
            return;
        }
        if (switchdir)
        {
            vertical = dzmod * (int)(Input.GetAxisRaw("Horizontal"));
            horizontal = dxmod * (int)(Input.GetAxisRaw("Vertical"));
        }
        else
        {
            horizontal = dxmod * (int)(Input.GetAxisRaw("Horizontal"));
            vertical = dzmod * (int)(Input.GetAxisRaw("Vertical"));
        }

        /*if (horiz != 0)
        {
            verti = 0;
        }*/

        if (thisturn == true)
        {
            if (Input.GetKey(KeyCode.Space) && (horizontal !=0 || vertical != 0) ) {
                StartCoroutine(RotateSelf(Quaternion.LookRotation(new Vector3(horizontal, 0f, vertical))));
                StartCoroutine(Jump(horizontal, vertical));
                thisturn = false;
                return;
            }
            if (Input.GetKeyDown(KeyCode.F) && HasWeapon(true)!=null) {
                if (aiming == true)
                {
                    aiming = false;
                }
                else
                {
                    aiming = true;
                }
            }
            if (Input.GetKeyDown(KeyCode.Period)) {
                thisturn = false;
                return;
            }
            if (aiming && HasWeapon(true) != null)
            {
                RaycastHit hit;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray.origin,ray.direction, out hit,Mathf.Infinity,layermask.value))
                {
                    Transform objectHit = hit.transform;
                    //GameObject hitentity = hit.transform.gameObject;
                    //Instantiate(debugblock, objectHit.position, Quaternion.identity);
                    if (objectHit.gameObject.tag == "Monster")
                    {
                        if (!rotating)
                        {
                            TargDir = Quaternion.LookRotation(objectHit.position - transform.position);
                            StartCoroutine(RotateSelf(TargDir));
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            //boxcollider = fals
                            HasWeapon(true).Shoot(objectHit.position, damagetodo);
                            // Add shooting animation
                            thisturn = false;
                            aiming = false;
                            TargPos = transform.position;
                        }
                    }
                }
            }
            else
            {
                Step(horizontal, vertical);
            }

            if (!thisturn)
            {
                if ((TargPos - transform.position).sqrMagnitude > 0.75)
                {
                    MapGen.mapinstance.RefreshDMap(0);
                    MapGen.mapinstance.AddMapGoal(0, TargPos);
                    MapGen.mapinstance.GenerateDMap(0);
                }
            }
        }
    }

    // Check body slots
    // 0==lefthand
    // 1==righthand
    public GameObject OnBody(int slot)
    {
        if (slot == 0)
        {
            return lefthand;
        }
        if (slot == 1)
        {
            return righthand;
        }
        return null;

    }

    public void SetSlot(int slot,GameObject addtoslot=null) {
        if (slot == 0)
        {
            lefthand=addtoslot;
            if (addtoslot != null)
            {
                lefthand.SetActive(true);
                lefthand.GetComponent<Item>().SetCollide(false);
                lefthand.GetComponent<Rigidbody>().isKinematic = true;
                lefthand.transform.position = transform.position + transform.rotation * new Vector3(-0.41f, 0.2f, 0.3f);
                lefthand.transform.rotation = transform.rotation;
            }

        }
        if (slot == 1)
        {
            righthand=addtoslot;
            if (addtoslot != null)
            {
                righthand.SetActive(true);
                righthand.GetComponent<Item>().SetCollide(false);
                righthand.GetComponent<Rigidbody>().isKinematic = true;
                righthand.transform.position = transform.position + transform.rotation * new Vector3(0.41f, 0.2f, 0.3f);
                righthand.transform.rotation = transform.rotation;
            }
        }
    }

}
