using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuiScript : MonoBehaviour {

    public GameObject player;
    public GameObject mapmanager;
    public GameObject playerinfotext;
    public GameObject inventorypanel;
    public GameObject EmoteMenu;
    EmoteSelector emoteselector;
    Inventory inventoryscript;
    private Player playerscript;
    private Text infotext;
    bool inventoryopen;

    // Startup
    void Start()
    {
        emoteselector = EmoteMenu.GetComponent<EmoteSelector>();
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
        if (Input.GetKeyDown(KeyCode.T)) {
            emoteselector.SetVisible();
        }
        infotext.text = playerscript.playername + "\n";
        infotext.text += "Level " + playerscript.level + " " + playerscript.playerclass + "\n";
        infotext.text += playerscript.hitpoints + " / " + playerscript.maxhitpoints + "\n";
        infotext.text += "Wielding : " + playerscript.Holding();
	}

    /*void WriteStats() {
        
    }*/
}
