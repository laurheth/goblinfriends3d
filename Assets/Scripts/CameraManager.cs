using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    public GameObject player;
    Player playerscript;
    public Vector3 baseoffset;
    Vector3 offset;
    float currentrotation;
    bool rotating;
    float targetrotation;
    private Camera cam;
    private Quaternion angle;
    private int lasty;
    private int lastx;
    private int lastz;
    float scrollupdown;
    Transform playertrans;
    int lastroomid;
    int newroomid;
    public LayerMask layermask;
    public float camdistance;
    public Vector3 offset2;
    int frames;
    //int lastroomid;

    GameObject[] blocks;// = GameObject.FindGameObjectsWithTag("Blocks");
    GameObject[] floors;// = GameObject.FindGameObjectsWithTag("Floor");
    GameObject[] items;// = GameObject.FindGameObjectsWithTag("Items");
    GameObject[] decos;// = GameObject.FindGameObjectsWithTag("Decorations");
    GameObject[] monsters;// = GameObject.FindGameObjectsWithTag("Monster");

    private int cameramode;

	// Use this for initialization
	void Start () {
        rotating = false;
        playerscript = player.GetComponent<Player>();
        currentrotation = 0f;
        targetrotation = 0f;
        offset = baseoffset;
        scrollupdown = 0f;
        playertrans = player.transform;
        lastroomid = -1;
        frames = 0;
        cam = GetComponent<Camera>();
        cameramode = 0;
        lasty = -10;
        lastx = -10;
        lastz = -10;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = Color.black;
	}

    bool Rotate() {
        float anglerot = 10f;
        if (targetrotation>currentrotation+anglerot) {
            currentrotation += anglerot;
        }
        else if (targetrotation<currentrotation-anglerot) {
            anglerot *= -1f;
            currentrotation += anglerot;
        }
        else {
            anglerot = targetrotation - currentrotation;
            if (anglerot>1f || anglerot<-1f) {
                currentrotation += anglerot;
            }
            else {
                return false;
            }
        }
        offset = Quaternion.AngleAxis(anglerot, Vector3.up)*offset;
        transform.position=Quaternion.AngleAxis(anglerot, Vector3.up) * transform.position;
        transform.Rotate(new Vector3(0, anglerot, 0),Space.World);
        return true;
    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Q)) {
            targetrotation -= 90f;
            playerscript.ControlDirections(-90);
            rotating = true;
        }
        else if (Input.GetKeyDown(KeyCode.E)) {
            targetrotation += 90f;
            playerscript.ControlDirections(90);
            rotating = true;
        }
        if (rotating)
        {
            rotating=Rotate();
        }
        newroomid = MapGen.mapinstance.RoomTag(playertrans.position);
        //if (Input.GetAxis("Mouse ScrollWheel")!=0
        //scrollupdown += Input.GetAxis("Mouse ScrollWheel");
        //if (scrollupdown > 4) { scrollupdown = 4f; }
        //if (scrollupdown < -1) { scrollupdown = -1f; }
        //if (MapGen.mapinstance.RoomTag(player.transform.position)!=lastroomid)
        if (frames<10) {
            frames++;
            UpdateObjList();
        }
        if (cameramode == 0)
        {
            RenderSettings.fog = false;
            cam.farClipPlane = 1000;
            RenderSettings.fog = false;
            transform.position = playertrans.position + offset;// + Vector3.up*scrollupdown;

            if (lasty > Mathf.RoundToInt(playertrans.position.y+(int)scrollupdown) || (lasty < Mathf.RoundToInt(player.transform.position.y)+ (int)scrollupdown && !player.GetComponent<Player>().falling))
            //if ((newroomid != lastroomid) || (lastx != Mathf.RoundToInt(playertrans.position.x)) || (lastz != Mathf.RoundToInt(playertrans.position.z)) || lasty > Mathf.RoundToInt(playertrans.position.y) || (lasty < Mathf.RoundToInt(player.transform.position.y) && !player.GetComponent<Player>().falling))
            {
                lasty = Mathf.RoundToInt(playertrans.position.y)+(int)scrollupdown;
                lastx = Mathf.RoundToInt(playertrans.position.x);
                lastz = Mathf.RoundToInt(playertrans.position.z);
                lastroomid = newroomid;
                UpdateVisibility();
            }
            //Physics.CheckBox(transform.position, offset);
        }
        else if (cameramode==1) {
            //cam.nearClipPlane = 9;
            //cam.farClipPlane = 4;
            cam.orthographic = false;
            transform.position = playertrans.position - transform.rotation * Vector3.forward*camdistance+offset2;
            if (lasty > Mathf.RoundToInt(playertrans.position.y) || (lasty < Mathf.RoundToInt(player.transform.position.y) && !player.GetComponent<Player>().falling))
            {
                lasty = Mathf.RoundToInt(playertrans.position.y);
                UpdateVisibility();
            }
        }

        // Manual onmouseover
        /*
        RaycastHit Hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out Hit, Mathf.Infinity, layermask)) {
            GameObject objecthit = Hit.transform.gameObject;
            if (objecthit.tag == "Monster") {
                objecthit.GetComponent<Monster>().Glow(true);
            }
            if (objecthit.name.Contains("Mushroom"))
            {
                objecthit.GetComponent<Mushroom>().Glow(true);
                if (Input.GetMouseButtonDown(0) && !objecthit.GetComponent<Mushroom>().IsAlive() && !objecthit.GetComponent<Mushroom>().IsGettingPickedUp()) {
                    objecthit.GetComponent<Mushroom>().pickedup=true;
                    playerscript.AddToInventory(objecthit);
                }
            }
        }*/

	}

    /*void OnTriggerStay(Collider other) {
        other.gameObject.GetComponent<MeshRenderer>().enabled=false;
        other.gameObject.layer = 8;
    }

    void OnTriggerExit(Collider other) {
        other.gameObject.GetComponent<MeshRenderer>().enabled = true;
        other.gameObject.layer = 0;
    }*/

    void VisibilityLoop(GameObject[] things,int defaultlayer=0) {
        Vector3 position;
        foreach (GameObject thing in things)
        {
            if (thing != null)
            {
                position = thing.transform.position;
                if (position.y -1 > lasty)
                {
                    //if (MapGen.mapinstance.RoomTag(position) != lastroomid)
                    //if ((position.x < playertrans.position.x && position.z < playertrans.position.z) || MapGen.mapinstance.RoomTag(position) != lastroomid)
                    {
                        thing.GetComponent<MeshRenderer>().enabled = false;
                        thing.layer = 8;
                    }
                    /*else {
                        thing.GetComponent<MeshRenderer>().enabled = true;
                        thing.layer = 0;
                    }*/
                }
                else
                {
                    thing.GetComponent<MeshRenderer>().enabled = true;
                    thing.layer = defaultlayer;
                }
            }
        }
    }

    void RoomVisLoop() {
        
    }

    void UpdateVisibility() {
        /*GameObject[] blocks = GameObject.FindGameObjectsWithTag("Blocks");
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] items = GameObject.FindGameObjectsWithTag("Items");
        GameObject[] decos = GameObject.FindGameObjectsWithTag("Decorations");
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");*/
        VisibilityLoop(blocks);
        VisibilityLoop(floors);
        VisibilityLoop(items,11);
        VisibilityLoop(monsters);
        VisibilityLoop(decos,11);
        //GameObject[] things = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        //VisibilityLoop(things);

    }

    public void UpdateObjList() {
        blocks = GameObject.FindGameObjectsWithTag("Blocks");
        floors = GameObject.FindGameObjectsWithTag("Floor");
        items = GameObject.FindGameObjectsWithTag("Items");
        decos = GameObject.FindGameObjectsWithTag("Decorations");
        monsters = GameObject.FindGameObjectsWithTag("Monster");
    }

    public int PlayerVert() {
        return lasty;
    }
}
