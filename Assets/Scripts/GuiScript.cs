using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiScript : MonoBehaviour {

    public GameObject player;
    public GameObject mapmanager;
    public GameObject playerinfotext;
    public GameObject inventorypanel;
    Inventory inventoryscript;
    private Player playerscript;
    private Text infotext;
    bool inventoryopen;

    // Startup
    void Start()
    {
        inventoryscript = inventorypanel.GetComponent<Inventory>();
        inventoryopen = false;
        infotext=playerinfotext.GetComponent<Text>();
        playerscript = player.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.I)) {
            inventoryopen = !inventoryopen;
            inventorypanel.SetActive(inventoryopen);
            if (inventoryopen) {
                inventoryscript.ClearContents();
                inventoryscript.UpdateContents();
                inventoryscript.RevealScreen();
            }
        }
        infotext.text = playerscript.playername + "\n";
        infotext.text += "Level " + playerscript.level + " " + playerscript.playerclass + "\n";
        infotext.text += playerscript.hitpoints + " / " + playerscript.maxhitpoints + "\n";
        infotext.text += "Wielding : " + playerscript.Holding();
	}

    /*void WriteStats() {
        
    }*/
}
