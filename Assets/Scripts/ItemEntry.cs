using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemEntry : MonoBehaviour,
//IPointerDownHandler,
IPointerClickHandler,
//IPointerUpHandler,
IPointerExitHandler,
IPointerEnterHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler
{
    GameObject ThisItem;
    //Text TextMass;
    //Text TextName;
    Text TextIcon;
    //Vector2 BaseIconPos;
    Image imagescript;
    Inventory inventoryscript;
    GameObject player;
    public GameObject InfoPanel;
    Text InfoText;
    public GameObject ParentCanvas;
    public LayerMask layermask;
    Vector3 InfoPanelSize;
    CanvasRenderer InfoPanelCanvas;
    int showinfo;
    int hideinventorycount;
    int hideval;
    bool isover;
    bool parentset;
    public int slot;

    // Use this for initialization
    void Awake()
    {
        hideval = 5;
        slot = -1;
        parentset = false;
        isover = false;
        showinfo = 0;
        ThisItem = null;
        Text[] textboxes = GetComponentsInChildren<Text>();
        for (int i = 0; i < textboxes.Length; i++)
        {
            if (textboxes[i].name == "InfoText")
            {
                InfoText = textboxes[i];
            }
            if (textboxes[i].name == "TextIcon")
            {
                TextIcon = textboxes[i];
            }
        }
        imagescript = GetComponent<Image>();
        InfoPanelCanvas = InfoPanel.GetComponent<CanvasRenderer>();
        InfoPanel.SetActive(false);
        InfoText.text = "Item\n0 kg";
        //TextMass.text = "0 kg";
        //TextName.text = "Item";
        TextIcon.text = "?";
        TextIcon.color = Color.black;
        //BaseIconPos = TextIcon.rectTransform.localPosition;
        //Debug.Log(BaseIconPos);
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void SetInventoryScript(Inventory script) {
        inventoryscript = script;
    }

    public void ItemInfo(GameObject item) {
        ThisItem = item;
        Item theitem = item.GetComponent<Item>();
        //TextName.text = theitem.Name;
        //TextMass.text = theitem.GetMass()+"kg";
        InfoText.text = theitem.Name + "\n" + theitem.GetMass() + "kg";
        TextIcon.text = theitem.Icon;
        TextIcon.color = theitem.IconColor;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Debug.Log("mooseui");
        imagescript.color = new Color(0.8f,0.8f,0.8f,1f);
        isover = true;
    }

    public void LateUpdate() {
        if (isover) {
            showinfo++;
            if (showinfo>20) {
                ShowInfo();
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imagescript.color = Color.white;
        isover = false;
        HideInfo();
    }

    void ShowInfo() {
        if (!parentset)
        {
            InfoPanel.SetActive(true);
            //InfoPanel.transform.SetParent(ParentCanvas.transform, true);
            InfoPanel.transform.SetParent(inventoryscript.gameObject.transform, true);
            parentset = true;
            //InfoPanelSize = Vector3.up * 35.5f;//new Vector3(InfoPanel.GetComponent<RectTransform>().rect.width,
                            //          InfoPanel.GetComponent<RectTransform>().rect.height,0)/2f;
            Debug.Log(InfoPanelSize);
            //InfoPanelCanvas.SetAlpha(0f);
        }
        //else {
            //InfoPanelCanvas.SetAlpha(1f);
        //}
        /*InfoPanelSize = new Vector3(InfoPanel.GetComponent<RectTransform>().rect.width,
                                      InfoPanel.GetComponent<RectTransform>().rect.height, 0) / 2f;*/
        InfoPanel.transform.position = Input.mousePosition;//+InfoPanelSize;
    }

    void HideInfo() {
        parentset = false;
        InfoPanel.transform.SetParent(transform, true);
        InfoPanel.SetActive(false);
        showinfo = 0;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        if (parentset) {
            HideInfo();
        }
        //TextIcon.transform.SetParent(ParentCanvas.transform,true);
        TextIcon.transform.SetParent(inventoryscript.gameObject.transform.parent, true);
    }

    public void OnDrag(PointerEventData eventData) {
        showinfo = 0;
        TextIcon.transform.SetAsLastSibling();
        TextIcon.transform.position = eventData.position;
        //Debug.Log(eventData.pointerCurrentRaycast.gameObject);
        if (eventData.pointerCurrentRaycast.gameObject==null) {
            hideinventorycount++;
            if (hideinventorycount > hideval)
            {
                //Debug.Log("hide screen!");
                inventoryscript.HideScreen();
            }
            RaycastHit Hit;
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, Mathf.Infinity, layermask)) {
                
                if (!player.GetComponent<Player>().IsRotating()) {
                    StartCoroutine(player.GetComponent<Player>().RotateSelf(Quaternion.LookRotation(Hit.transform.position - player.transform.position)));
                }
            }
            
        }
        else {
            if (hideinventorycount>hideval) {
                inventoryscript.RevealScreen();
            }
            hideinventorycount = 0;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        GameObject DropOnto = eventData.pointerCurrentRaycast.gameObject;
        if (DropOnto != null)
        {
            Debug.Log(DropOnto.name);

            if (DropOnto.name == "LeftHand")
            {
                player.GetComponent<Player>().SetSlot(0, ThisItem);
            }
            else if (DropOnto.name == "RightHand")
            {
                player.GetComponent<Player>().SetSlot(1, ThisItem);
            }
            else
            {
                TextIcon.rectTransform.SetParent(transform, true);
                TextIcon.rectTransform.localPosition = new Vector3(0, 0, -1);
            }
            GetComponentInParent<Inventory>().UpdateContents();
        }
        else {
            RaycastHit Hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, Mathf.Infinity, layermask))
            {
                TextIcon.rectTransform.SetParent(transform, true);
                TextIcon.rectTransform.localPosition = new Vector3(-140, 0, -1);
                Debug.Log(Hit.transform.position);
                ThisItem.SetActive(true);
                ThisItem.GetComponent<Collider>().enabled = true;
                ThisItem.GetComponent<Rigidbody>().isKinematic = false;
                ThisItem.transform.SetParent(null);
                ThisItem.transform.position = player.transform.position;
                ThisItem.GetComponent<Rigidbody>().velocity=(Vector3.up+Hit.transform.position - ThisItem.transform.position)*2f;
                ThisItem.GetComponent<Item>().pickedup = false;
                ThisItem.GetComponent<Item>().SetCollide(true);
                player.GetComponent<Player>().inventory.Remove(ThisItem);
                player.GetComponent<Player>().thisturn = false;
                if (slot>=0) {
                    player.GetComponent<Player>().SetSlot(slot);
                }


                StartCoroutine(ThisItem.GetComponent<Item>().IgnoreTemporarily(player,1f));

                GetComponentInParent<Inventory>().UpdateContents();
                //Object.Destroy(this.gameObject);
            }
        }

    }


    public void OnPointerClick(PointerEventData eventData) {
        //Debug.Log(eventData.button);
        if (parentset)
        {
            HideInfo();
        }
        if (eventData.button==PointerEventData.InputButton.Right) {
            if (slot>=0) {
                player.GetComponent<Player>().SetSlot(slot);
                player.GetComponent<Player>().AddToInventory(ThisItem);
            }
            else {
                ThisItem.GetComponent<Item>().UseItem(player.GetComponent<Player>());
                GetComponentInParent<Inventory>().UpdateContents();
            }
        }
    }

    public void OnDestroy() {
        Destroy(TextIcon);
        Destroy(InfoPanel);
    }
}
