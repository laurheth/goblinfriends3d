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
    public string PlayerClass; // Player class
    public Vector3 Position; // Location
    public Quaternion Rotation;
    public List<string> Inventory; // List of items in inventory
    public List<float> ItemVal1; // Item values i.e. logsize for mushrooms
    public List<float> ItemVal2;
    public int lefthand; // Left hand object (index)
    public int righthand; // Right hand object (index)
    public int level;
    public int maxhp;
    public int hp;

    public SavePlayer(GameObject player) {
        Player playerscript = null;
        Inventory = new List<string>();
        ItemVal1 = new List<float>();
        ItemVal2 = new List<float>();
        if (player != null)
        {
            playerscript = player.GetComponent<Player>();
        }
        if (playerscript == null)
        {
            PlayerName = "Lauren";
            PlayerClass = "Goblin Shower";
            Position = new Vector3(-1, -1, -1);
            Rotation = Quaternion.identity;
            lefthand = -1;
            righthand = -1;
            maxhp = 50;
            hp = 50;
            level = 1;
        }
        else {
            PlayerName = playerscript.playername;
            PlayerClass = playerscript.playerclass;
            Position = player.transform.position;
            Rotation = player.transform.rotation;
            maxhp = playerscript.maxhitpoints;
            hp = playerscript.hitpoints;
            level = playerscript.level;
            lefthand = -1;
            righthand = -1;
            int itemind = -1;
            float[] vals = new float[2];
            //foreach (Item item in player.GetComponentsInChildren<Item>()) {
            foreach (GameObject item in playerscript.inventory) {
                itemind++;
                Inventory.Add(item.name);
                vals = item.GetComponent<Item>().GetAttributes();
                ItemVal1.Add(vals[0]);
                ItemVal2.Add(vals[1]);
                if (lefthand < 0 || righthand < 0)
                {
                    if (item == playerscript.OnBody(0))
                    {
                        lefthand = itemind;
                    }
                    else if (item == playerscript.OnBody(1))
                    {
                        righthand = itemind;
                    }
                }
            }

        }
    }

    public void LoadData(GameObject player) {
        Player playerscript = null;
        if (player != null)
        {
            playerscript = player.GetComponent<Player>();
        }
        if (player==null || playerscript==null) {
            return;
        }
        player.GetComponent<Rigidbody>().isKinematic = true;
        playerscript.boxcollider.enabled = true;
        playerscript.falling = false;
        player.transform.rotation = Quaternion.identity;
        playerscript.playername=PlayerName;
        playerscript.playerclass=PlayerClass;
        player.transform.position=Position;
        player.transform.rotation = Rotation;
        playerscript.maxhitpoints=maxhp;
        playerscript.hitpoints=hp;
        if (hp>0) {
            playerscript.alive = true;
        }
        else {
            playerscript.alive = false;
        }
        playerscript.level=level;
        //int itemind = -1;

        // Empty pre-existing inventory
        playerscript.ClearInventory();

        // Fill inventory from savefile
        for (int i = 0; i < Inventory.Count; i++)
        {
            GameObject newobject = GameManager.instance.MakeGameObject(Inventory[i]);
            if (newobject==null) {
                Debug.Log("Couldn't create " + Inventory[i]);
            }
            else {
                Debug.Log(newobject.name);
                Debug.Log(newobject.GetComponent<Item>().Name);
                Debug.Log(i);
                //Debug.Log(ItemVals.Count);
                newobject.GetComponent<Item>().SetAttributes(ItemVal1[i],ItemVal2[i],true);
                playerscript.AddToInventory(newobject);
                if (i==lefthand) {
                    playerscript.SetSlot(0, newobject);
                }
                else if (i==righthand) {
                    playerscript.SetSlot(1, newobject);
                }
            }
        }

        /*foreach (Item item in player.GetComponentsInChildren<Item>())
        {
            itemind++;
            Inventory.Add(item.gameObject.name);
            ItemVals.Add(item.GetAttributes());
            if (lefthand < 0 || righthand < 0)
            {
                if (item.gameObject == playerscript.OnBody(0))
                {
                    lefthand = itemind;
                }
                else if (item.gameObject == playerscript.OnBody(1))
                {
                    righthand = itemind;
                }
            }
        }*/

    }

}
