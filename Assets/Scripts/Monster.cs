using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : Unit
{
    public GameObject[] helditems;
    public GameObject player;
    public float[] desires;
    public float[] InverseDesires;
    private int tiredness;
    //int tiredthresh;
    private int anger;
    private int fear;
    private List<Vector3> PathFound;
    Vector3 HomeLocation;

    public bool active;
    //private bool thisturn;
    // Enemy turn
    protected override void Start() {
        PathFound = null;
        anger = 0;
        fear = 0;
        InverseDesires = new float[desires.Length];
        //for (int i = 0; i < InverseDesires.Length;i++) {
        //    InverseDesires[i] = 1f / desires[i];
        //}
        thisturn = false;
        ismonster = true;
        active = false;
        damagetodo = 2;
        //hitpoints = 10;
        GameManager.instance.AddMonsterToList(this);
        lefthand = Instantiate(helditems[0], new Vector3(transform.position.x + 0.41f, transform.position.y + 0.2f, transform.position.z - 0.3f), transform.rotation);
        lefthand.GetComponent<Item>().pickedup = true;
        lefthand.transform.parent = transform;

        player = GameObject.FindGameObjectWithTag("Player");
        tiredness = Random.Range(0, 40);
        base.Start();
        hunger += Random.Range(-40, 10);
        for (int i = 0; i < InverseDesires.Length;i++) {
            UpdateDesire(i);
        }
        HomeLocation = transform.position;
    }

    void UpdateDesire(int choose) {
        switch(choose) {
            case 0:
                desires[0] = Mathf.Max(5*anger,0);
                break;
            case 1:
                desires[1] = Mathf.Max((hunger-20)/6,0);
                break;
            case 2:
                desires[2] = Mathf.Max(Mathf.Min(10, (tiredness-20) / 4),0);
                break;
        }
        if (desires[choose]>0) {
            InverseDesires[choose] = 1f / desires[choose];
        }
        else {
            InverseDesires[choose] = 10000f;
            if (choose==0) {
                InverseDesires[choose] *= 10f;
            }
        }
    }

    public void EnemyTurn()
    {
        tiredness++;
        hunger++;

        if (MapGen.mapinstance.DistGoal(transform.position, 2) < 8)
        {
            if (tiredness > 20)
            {
                tiredness = -10;
                PathFound = MapGen.mapinstance.AStarPath(transform.position, HomeLocation);
            }
            UpdateDesire(2);
        }

        if (tiredness % 4 ==0 && tiredness>20) {// || hunger % 3 ==0) {
            UpdateDesire(2);
        }
        if (hunger % 6 ==0) {
            UpdateDesire(1);
        }

        if (alive==false || falling == true){
            if (alive==false && !rend.isVisible) {
                rb.isKinematic = true;
            }
            else {
                rb.isKinematic = false;
            }
            return;
        }
        if (moving || climbing) {
            return;
        }
        horizontal = 0;
        vertical = 0;

        //MapGen.mapinstance.RollDown(transform.position, out horizontal, out vertical, InverseDesires);
        //if (rend.isVisible && (PathFound==null || (turnnum % 5)==0)) {
        //PathFound=MapGen.mapinstance.AStarPath(gameObject.transform.position, player.transform.position);
        /*for (int i = 0; i < ReturnList.Count; i++)
        {
            Debug.Log(ReturnList[i].ToString());
        }*/
        //}
        if (PathFound != null && PathFound.Count > 0)
        {
            if ((PathFound[0]-transform.position).sqrMagnitude < 0.5f) {
                PathFound.RemoveAt(0);
            }
            if (PathFound.Count > 0)
            {
                if (PathFound[0].x > gameObject.transform.position.x)
                {
                    horizontal = 1;
                }
                else if (PathFound[0].x < gameObject.transform.position.x)
                {
                    horizontal = -1;
                }

                if (PathFound[0].z > gameObject.transform.position.z)
                {
                    vertical = 1;
                }
                else if (PathFound[0].z < gameObject.transform.position.z)
                {
                    vertical = -1;
                }
            }
            else
            {
                PathFound = null;
            }
        }
        else {
            MapGen.mapinstance.RollDown(transform.position, out horizontal, out vertical, InverseDesires);
        }

        if (horizontal==0 && vertical==0 && MapGen.mapinstance.TileType(transform.position)=='<') {
            if (Step(0,0,true)) {
                thisturn = false;
            }
        }
        else {
        // Try to step in direction. If failed, try each axis individually, then give up and step randomly
        //if (!thisturn)
        //{
            if (!Step(horizontal, vertical))
            {
                if (!Step(horizontal, 0))
                {
                    if (!Step(0, vertical))
                    {
                        horizontal = Random.Range(-1, 2);//(int)(Input.GetAxisRaw("Horizontal"));
                        vertical = Random.Range(-1, 2);//(int)(Input.GetAxisRaw("Vertical"))
                        if (!Step(horizontal, vertical))
                        {
                            thisturn = false;
                        }
                    }
                }
            }
            else
            {
                thisturn = false;
            }
        }
        /*if (climbing==true || falling==true) {
            rend.enabled = TargPos[1] - 1 < player.transform.position[1];
        }*/
    }

    public void Anger(int addval = 10) {
        anger += addval;
        UpdateDesire(0);
        UseEmote(1);
    }

    public void GetEmote(int emotetoget)
    {
        switch (emotetoget)
        {
            case 0:
                UseEmote(0);
                anger = 0;
                UpdateDesire(0);
                break;
            case 1:
                anger += 10;
                UpdateDesire(0);
                UseEmote(1);
                break;
            case 2:
                fear += 10;
                if (fear > anger)
                {
                    anger = 0;
                    tiredness += 50;
                    UpdateDesire(0);
                    UpdateDesire(2);
                    UseEmote(3);
                }
                break;
            case 3:
                UseEmote(0);
                anger = 0;
                UpdateDesire(0);
                break;
            default:
                //UseEmote(0);
                break;
        }
    }
}
