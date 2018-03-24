using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteBubble : MonoBehaviour
{

    public GameObject BubbleEmote;
    public GameObject BubbleBG;
    public Sprite[] EmoteSprites;
    Image EmoteImage;
    Camera cam;
    CameraManager camscript;
    Canvas canvas;
    RectTransform bubblesize;
    bool writing;
    Vector2 targsize;
    Vector2 minsize = new Vector2(32, 32);
    Vector2 maxsize = new Vector2(64, 70);

    // Use this for initialization
    void Awake()
    {
        writing = false;
        canvas = GetComponent<Canvas>();
        cam = FindObjectOfType<Camera>();
        camscript = cam.GetComponent<CameraManager>();
        transform.forward = -camscript.transform.forward;
        EmoteImage = BubbleEmote.GetComponent<Image>();
        //EmoteImage.sprite = heartsprite;
        EmoteImage.color = new Color(1f, 1f, 1f, 0f);
        bubblesize = BubbleBG.GetComponent<RectTransform>();
        targsize = minsize;//new Vector2(32, 32);
        bubblesize.sizeDelta = targsize;
        BubbleBG.SetActive(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = camscript.transform.forward;
    }

    public bool SetEmote(int spritesel)
    {
        if (spritesel >= EmoteSprites.Length) { spritesel = 0; }
        if (!writing)
        {
            StartCoroutine(Writing(EmoteSprites[spritesel]));
            return true;
        }
        return false;
    }

    public bool ClearEmote()
    {
        if (!writing)
        {
            StartCoroutine(Clearing());
            return true;
        }
        return false;
    }

    IEnumerator Writing(Sprite newemote)
    {
        BubbleBG.SetActive(true);
        EmoteImage.sprite = newemote;
        writing = true;

        for (int i = 0; i <= 10+Mathf.Max(Mathf.Abs(maxsize[0]-targsize[0]),
                                     Mathf.Abs(maxsize[1]-targsize[1]))/2; i++)
        {
            if (targsize[0]<maxsize[0]) {
                targsize[0]+=2;
            }
            if (targsize[1]<maxsize[1]) {
                targsize[1]+=2;
            }
            bubblesize.sizeDelta = targsize;
            yield return null;

        }

        targsize = maxsize;
        bubblesize.sizeDelta = targsize;
        //BubbleTextContent.text = towrite;
        EmoteImage.color = new Color(1f,1f,1f,1f);
        writing = false;
    }

    IEnumerator Clearing()
    {
        writing = true;
        EmoteImage.color = new Color(1f, 1f, 1f, 0f);

        for (int i = 0; i <= 10+Mathf.Max(Mathf.Abs(minsize[0] - targsize[0]),
                                   Mathf.Abs(minsize[1] - targsize[1]))/2; i++)
        {
            if (targsize[0] > minsize[0])
            {
                targsize[0]-=2;
            }
            if (targsize[1] > minsize[1])
            {
                targsize[1]-=2;
            }
            bubblesize.sizeDelta = targsize;
            yield return null;

        }

        targsize = minsize;
        bubblesize.sizeDelta = targsize;

        writing = false;
        BubbleBG.SetActive(false);
    }
}
