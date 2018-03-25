using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EmoteSelection : MonoBehaviour, IPointerClickHandler {

    public int EmoteType;
    Player playerscript;
    EmoteSelector emoteselector;
	// Use this for initialization
	void Start () {
        playerscript=GetComponentInParent<Player>();
        emoteselector=GetComponentInParent<EmoteSelector>();
	}

    public void OnPointerClick(PointerEventData eventData)
    {
        playerscript.UseEmote(EmoteType);
        playerscript.thisturn = false;
        emoteselector.SetVisible(false);
    }

}
