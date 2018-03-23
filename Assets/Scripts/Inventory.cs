using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour, IPointerEnterHandler {
    public Player playerscript;
    public GameObject player;
    public GameObject ItemEntry;
    public GameObject contentholder;
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject ParentCanvas;
    RectTransform recttransform;
    float opensize;
    float closedsize;
    float sizerate;
    bool changingsize;
    //public static Inventory
	// Use this for initialization
	void Start () {
        sizerate = 1200f;
        changingsize = false;
        opensize = 608f;
        closedsize = 64f;
        recttransform = GetComponent<RectTransform>();
        //currentysize = recttransform.rect.height;
        Debug.Log(player);
        //Debug.Log(player.GetComponent<Player>());
        //playerscript = player.GetComponent<Player>();
        gameObject.SetActive(false);
	}

    public void ClearContents() {
        GameObject[] CycleThroughThese={contentholder,LeftHand,RightHand};

        foreach (GameObject thisobject in CycleThroughThese)
        {
            foreach (Transform child in thisobject.transform)
            {
                if (child != null)
                {
                    //Debug.Log(child.gameObject.tag);
                    if (child.gameObject.tag == "GUIItemEntry")
                    {
                        Destroy(child.gameObject);
                    }
                }
            }
        }
    }

    /*void Update() {
        ClearContents();
        UpdateContents();
    }*/

    public void UpdateContents() {
        ClearContents();
        GameObject nextentry;
        int size = 0;
        /*if (player==null) {
            playerscript = player.GetComponent<Player>();
        }*/
        Debug.Log(player);
        Debug.Log(playerscript);
        foreach (GameObject nextitem in playerscript.inventory) {
            //Debug.Log(nextitem.name);
            //Debug.Log(nextitem.GetComponent<Item>().Name);
            nextentry=Instantiate(ItemEntry,Vector3.forward,Quaternion.identity);
            
            if (nextitem == playerscript.OnBody(0))
            {
                nextentry.transform.SetParent(LeftHand.transform, false);
                nextentry.GetComponent<ItemEntry>().slot = 0;
            }
            else if (nextitem == playerscript.OnBody(1))
            {
                nextentry.transform.SetParent(RightHand.transform, false);
                nextentry.GetComponent<ItemEntry>().slot = 1;
            }
            else
            {
                nextentry.transform.SetParent(contentholder.transform, false);
            }
            nextentry.GetComponent<ItemEntry>().ItemInfo(nextitem);
            nextentry.GetComponent<ItemEntry>().ParentCanvas=ParentCanvas;
            nextentry.GetComponent<ItemEntry>().SetInventoryScript(this);
            size++;
        }

        //contentholder.GetComponent<RectTransform>().sizeDelta = new Vector2(0,size*80);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!changingsize)
        {
            RevealScreen();
        }
    }

    public void HideScreen() {
        Debug.Log(!changingsize);
        if (!changingsize)
        {
            Debug.Log("shrinknow");
            StartCoroutine(Shrinking());
        }
    }

    IEnumerator Shrinking() {
        changingsize = true;
        int breaker = 0;
        float x = recttransform.rect.x;
        float y = recttransform.rect.y;
        float w = recttransform.rect.width;
        float h = recttransform.rect.height;
        //Debug.Log(h);
        while (recttransform.rect.height>closedsize && breaker<500) {
            
            breaker++;
            h -= sizerate*Time.deltaTime;
            if (h < closedsize) { h = closedsize-0.5f; }
            recttransform.sizeDelta = new Vector2(w, h);
            Debug.Log(recttransform.sizeDelta);
            yield return null;
        }
        changingsize = false;
    }

    IEnumerator Growing()
    {
        changingsize = true;
        int breaker = 0;
        float x = recttransform.rect.x;
        float y = recttransform.rect.y;
        float w = recttransform.rect.width;
        float h = recttransform.rect.height;
        //Debug.Log(h);
        while (recttransform.rect.height < opensize && breaker < 500)
        {

            breaker++;
            h += sizerate* Time.deltaTime;
            if (h > opensize) { h = opensize + 0.5f; }
            recttransform.sizeDelta = new Vector2(w, h);
            Debug.Log(recttransform.sizeDelta);
            yield return null;
        }
        changingsize = false;
    }

    public void RevealScreen() {
        StartCoroutine(Growing());
    }

}
