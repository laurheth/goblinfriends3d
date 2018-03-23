using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeechBubble : MonoBehaviour {

    public GameObject BubbleText;
    Text BubbleTextContent;
    public GameObject BubbleBG;
    Camera cam;
    CameraManager camscript;
    Canvas canvas;
    bool writing;

	// Use this for initialization
	void Awake () {
        writing = false;
        canvas = GetComponent<Canvas>();
        cam = FindObjectOfType<Camera>();
        camscript=cam.GetComponent<CameraManager>();
        transform.forward = -camscript.transform.forward;
        BubbleTextContent = BubbleText.GetComponent<Text>();
        BubbleTextContent.text = "";
        BubbleBG.SetActive(false);
	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.forward = camscript.transform.forward;
        /*if (!writing) {
            if (BubbleTextContent.text == "")
            {
                WriteText("HELLO I AM A GOBLIN!!!\nPLEASE BE MY FRIEND!!!");
            }
            else {
                ClearText();
            }
        }*/
	}

    public void WriteText(string towrite)
    {
        if (!writing)
        {
            StartCoroutine(Writing(towrite));
        }
    }

    public void ClearText() {
        if (!writing)
        {
            StartCoroutine(Clearing());
        }
    }

    IEnumerator Writing(string towrite)
    {
        BubbleBG.SetActive(true);
        BubbleTextContent.text = "";
        writing = true;
        for (int i = 0; i < towrite.Length; i++)
        {
            BubbleTextContent.text += towrite[i];
            yield return null;

        }
        BubbleTextContent.text = towrite;
        writing = false;
    }

    IEnumerator Clearing()
    {
        writing = true;

        string currentstring = BubbleTextContent.text;

        for (int i = 0; i < currentstring.Length; i++)
        {
            BubbleTextContent.text = currentstring.Substring(i);

            yield return null;

        }

        BubbleTextContent.text = "";
        writing = false;
        BubbleBG.SetActive(false);
    }
}
