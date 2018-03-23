using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BlockProperties : MonoBehaviour {

    public int RoomID=0;

	// Use this for initialization
	void Start () {
        //RoomID = 0;
	}

    public void SetRoomID(int newroomid){
        //Debug.Log(newroomid);
        RoomID = newroomid;
    }
	
}
