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
    int yslice;
    private int lastx;
    private int lastz;
    float scrollupdown;
    Transform playertrans;
    int lastroomid;
    int newroomid;
    public LayerMask layermask;
    public float camdistance;
    public Vector3 offset2;
    //int frames;
    bool itemlistmade;
    //int lastroomid;
    int xsizechunks, zsizechunks;

    List<GameObject[]> chunks;
    int xchunks, zchunks, chunksize;
    List<int> chunksgot;
    int currentchunk;

    //GameObject[] blocks;// = GameObject.FindGameObjectsWithTag("Blocks");
    //GameObject[] floors;// = GameObject.FindGameObjectsWithTag("Floor");
    GameObject[] items;// = GameObject.FindGameObjectsWithTag("Items");
    GameObject[] decos;// = GameObject.FindGameObjectsWithTag("Decorations");
    GameObject[] monsters;// = GameObject.FindGameObjectsWithTag("Monster");

    private int cameramode;

	// Use this for initialization
	void Start () {
        chunksize = 11;
        currentchunk = -1;
        chunksgot = new List<int>();
        itemlistmade = false;
        chunks = new List<GameObject[]>();
        rotating = false;
        playerscript = player.GetComponent<Player>();
        currentrotation = 0f;
        targetrotation = 0f;
        offset = baseoffset;
        scrollupdown = 0f;
        playertrans = player.transform;
        lastroomid = -1;
        //frames = 0;
        cam = GetComponent<Camera>();
        cameramode = 0;
        lasty = -10;
        yslice = -1;
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
        if (!itemlistmade) {
            itemlistmade = true;
            UpdateObjList();
        }
        if (cameramode == 0)
        {
            RenderSettings.fog = false;
            cam.farClipPlane = 1000;
            RenderSettings.fog = false;
            transform.position = playertrans.position + offset;// + Vector3.up*scrollupdown;

            /*if ((!player.GetComponent<Player>().falling && !player.GetComponent<Player>().climbing) &&
                ( Mathf.RoundToInt(playertrans.position.y) != lasty || (newroomid!=lastroomid)))*/
            if ((currentchunk!=Chunknum(playertrans.position.x,playertrans.position.z)) ||
                lasty > Mathf.RoundToInt(playertrans.position.y) ||
                (lasty < Mathf.RoundToInt(player.transform.position.y) && !player.GetComponent<Player>().falling))
            //if ((newroomid != lastroomid) || (lastx != Mathf.RoundToInt(playertrans.position.x)) || (lastz != Mathf.RoundToInt(playertrans.position.z)) || lasty > Mathf.RoundToInt(playertrans.position.y) || (lasty < Mathf.RoundToInt(player.transform.position.y) && !player.GetComponent<Player>().falling))
            {
                Debug.Log("camchange");
                if (!player.GetComponent<Player>().falling || lasty > Mathf.RoundToInt(playertrans.position.y)) {
                    lasty = Mathf.RoundToInt(playertrans.position.y) + (int)scrollupdown;
                }
                yslice = lasty / MapGen.mapinstance.yscale;
                lastx = Mathf.RoundToInt(playertrans.position.x);
                lastz = Mathf.RoundToInt(playertrans.position.z);
                lastroomid = newroomid;
                UpdateVisibility();
                player.GetComponent<MeshRenderer>().enabled = true;
                player.layer = 0;
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

    void VisibilityLoop(GameObject[] things, int defaultlayer = 0, int startind = 0, int maxinds=-1,bool forcehide=false) {
        Vector3 position;
        GameObject thing;
        Renderer rend;
        //foreach (GameObject thing in things)
        for (int i = startind; i < things.Length;i++)
        {
            if (i == maxinds) { break; }
            thing = things[i];
            //if (thing.tag=="Items" || thing.tag=="Decos")
            if (thing != null && thing.tag!="SpaceClaimer")
            {
                position = thing.transform.position;
                //if (position.y -1 > lasty)// && !MapGen.mapinstance.IsInBounds(position, lastroomid))
                //{
                //if (MapGen.mapinstance.RoomTag(position) != lastroomid)
                //if ((position.x < playertrans.position.x && position.z < playertrans.position.z) || MapGen.mapinstance.RoomTag(position) != lastroomid)
                //if (MapGen.mapinstance.RoomTag(position) != lastroomid)
                rend = thing.GetComponent<MeshRenderer>();
                if (rend==null) {
                    rend = thing.GetComponentInChildren<Renderer>();
                    if (rend==null) {
                        continue;
                    }
                }
                if ((position.y - 1 > lasty) || forcehide)// && !MapGen.mapinstance.IsInBounds(position,lastroomid,lasty))
                {
                    {
                        rend.enabled = false;
                        thing.layer = 8;
                    }
                    /*else {
                        thing.GetComponent<MeshRenderer>().enabled = true;
                        thing.layer = 0;
                    }*/
                }
                else
                {
                    rend.enabled = true;
                    thing.layer = defaultlayer;
                }
            }
        }
    }

    void UpdateVisibility() {
        /*GameObject[] blocks = GameObject.FindGameObjectsWithTag("Blocks");
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] items = GameObject.FindGameObjectsWithTag("Items");
        GameObject[] decos = GameObject.FindGameObjectsWithTag("Decorations");
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");*/

        /*Collider[] colliders = Physics.OverlapBox(playertrans.position, new Vector3(7f, 2f, 7f));

        GameObject[] foundobjs = new GameObject[colliders.Length];
        for (int i = 0; i < colliders.Length;i++) {
            foundobjs[i] = colliders[i].gameObject;
        }
        VisibilityLoop(foundobjs);*/

        int inthischunk = Chunknum(lastx, lastz);
        if (inthischunk != currentchunk)
        {
            currentchunk = inthischunk;
        }
        {
            int addchunk;
            List<int> newchunks = new List<int>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    addchunk = Chunknum(lastx + i * (chunksize), lastz + j * (chunksize));
                    if (addchunk > -1)
                    {
                        newchunks.Add(addchunk);
                    }
                    else {
                        addchunk = Chunknum(lastx + i * (chunksize - 1), lastz + j * (chunksize - 1));
                        if (addchunk > -1)
                        {
                            newchunks.Add(addchunk);
                        }
                    }
                }
            }
            for (int i = 0; i < newchunks.Count;i++) {
                //if (!chunksgot.Contains(newchunks[i])) {
                //    chunksgot.Add(newchunks[i]);
                //}
                VisibilityLoop(chunks[newchunks[i]]);
            }
            for (int i = 0; i < chunksgot.Count;i++) {
                if (!newchunks.Contains(chunksgot[i])) {
                    VisibilityLoop(chunks[chunksgot[i]], 0, 0, -1, true);
                }
            }
            chunksgot = newchunks;
        }

        VisibilityLoop(monsters);
        VisibilityLoop(items, 11);
        VisibilityLoop(decos,11);


        //VisibilityLoop(blocks);
        //VisibilityLoop(floors);
        //StartCoroutine(VisibilityCoroutine(blocks));
        //StartCoroutine(VisibilityCoroutine(floors));

        //GameObject[] things = FindObjectsOfType(typeof(GameObject)) as GameObject[];
        //VisibilityLoop(things);

    }

    IEnumerator VisibilityCoroutine(GameObject[] things, int defaultlayer = 0)
    {
        int stepsize = 40;
        int startind=0;
        int stopind=stepsize;

        while (startind<things.Length) {
            VisibilityLoop(things, defaultlayer, startind, stopind);
            startind += stepsize;
            stopind += stepsize;
            yield return null;
        }
    }

    public void UpdateObjList() {
        xsizechunks = MapGen.mapinstance.xsize + MapGen.mapinstance.xsize % chunksize;
        zsizechunks = MapGen.mapinstance.zsize + MapGen.mapinstance.zsize % chunksize;
        xchunks = (MapGen.mapinstance.xsize / chunksize)+1;
        zchunks = (MapGen.mapinstance.zsize / chunksize)+1;
        int[] numobjs = new int[xchunks*zchunks];
        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Blocks");
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject thisone in blocks) {
            numobjs[Chunknum(thisone.transform.position.x, thisone.transform.position.z)]++;
        }
        foreach (GameObject thisone in floors)
        {
            numobjs[Chunknum(thisone.transform.position.x, thisone.transform.position.z)]++;
        }

        for (int i = 0; i < xchunks * zchunks; i++)
        {
            chunks.Add(new GameObject[numobjs[i]]);
            numobjs[i] = 0;
            chunksgot.Add(i);
        }
        int numchunk;
        foreach (GameObject thisone in blocks)
        {
            numchunk = Chunknum(thisone.transform.position.x, thisone.transform.position.z);
            chunks[numchunk][numobjs[numchunk]] = thisone;
            numobjs[numchunk]++;
        }
        foreach (GameObject thisone in floors)
        {
            numchunk = Chunknum(thisone.transform.position.x, thisone.transform.position.z);
            chunks[numchunk][numobjs[numchunk]] = thisone;
            numobjs[numchunk]++;
        }

        decos = GameObject.FindGameObjectsWithTag("Decorations");
        items = GameObject.FindGameObjectsWithTag("Items");
        monsters = GameObject.FindGameObjectsWithTag("Monster");
    }

    int Chunknum(float x, float z) {
        return Chunknum(Mathf.RoundToInt(x), Mathf.RoundToInt(z));
    }
    int Chunknum(int x, int z) {
        if (x<0 || x>=xsizechunks || z<0 || z>zsizechunks) {
            return -1;
        }
        return xchunks * (z / chunksize) + x / chunksize;
    }

    public int PlayerVert() {
        return Mathf.RoundToInt(playertrans.position.y);
    }
}
