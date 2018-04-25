using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using JsonUtil;

[System.Serializable]
public class SavePlayer {

    /* Player details needed:
     * -Name
     * -Stats (NOT YET IMPLEMENTED)
     * -Position
     * -Inventory
     */
    public string PlayerName; // Player name
    public Vector3 Position; // Location
    public List<string> Inventory; // List of items in inventory
    public List<float> ItemVals; // Item values i.e. logsize for mushrooms
    public int lefthand; // Left hand object (index)
    public int righthand; // Right hand object (index)

    SavePlayer() {
        PlayerName = "Lauren";
        Position = new Vector3(10, 0, 10);
        Inventory = new List<string>();
        ItemVals = new List<float>();
        lefthand = -1;
        righthand = -1;
    }

}
