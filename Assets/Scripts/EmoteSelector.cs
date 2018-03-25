using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteSelector : MonoBehaviour
{
    Camera cam;
    CameraManager camscript;
    Canvas canvas;
    public GameObject emotepanel;
    bool isvisible;

    // Use this for initialization
    void Awake()
    {
        canvas = GetComponent<Canvas>();
        cam = FindObjectOfType<Camera>();
        camscript = cam.GetComponent<CameraManager>();
        transform.forward = -camscript.transform.forward;
        SetVisible(false);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = camscript.transform.forward;
    }

    public void SetVisible() {
        SetVisible(!isvisible);
    }

    public void SetVisible(bool setvis) {
        emotepanel.SetActive(setvis);
        isvisible = setvis;
    }

}
