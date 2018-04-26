using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Mushroom : Item {

    //MeshRenderer mesh;
    Light shroomlight;
    CapsuleCollider capcollide;
    BoxCollider boxcollide;

    Rigidbody rb;
    //public LayerMask layermask;
    private GameObject player;
    //private int glownum;
    private float maxshroombrightness;
    bool alive;
    bool gettingpickedup;
    public GameObject mushroom;
    float logsize;
    float size;
    public int reprofreq;
    //int hitpoints;
    //public Color color;
    Vector3 sizevect;
    Vector3 oldpos;
    //CameraManager camscript;
    private Color[] colors;
    private string[] colornames;
    private int colorind;
    private bool initialized;

    // Use this for initialization
    protected override void Start()
    {
        //base.Start();
        if (!initialized)
        {
            MushInit();
        }
    }
    private void MushInit() {
        mesh = GetComponent<MeshRenderer>();
        initialized = true;
        pickedup = false;
        //camscript = FindObjectOfType<Camera>().GetComponent<CameraManager>();
        maxshroombrightness = 0.6f;
        oldpos = transform.position;
        reprofreq = 100;
        gettingpickedup = false;
        player = GameObject.FindWithTag("Player");
        alive = true;
        //hitpoints = 1;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        //rb.isKinematic = true;
        colors = new Color[] { Color.red, Color.blue, Color.green, Color.cyan, Color.magenta, Color.yellow, Color.cyan };
        colornames = new string[] { "Red", "Blue", "Green", "Cyan", "Magenta", "Yellow", "Cyan" };
        //colors[0] = Color.red; colors[1] =
        int i = Random.Range(0, colors.Length);
        colorind = i;
        color = colors[i];
        IconColor = color;
        Name = colornames[i] + " Mushroom";
        Icon = "M";
        logsize = Mathf.Round(Random.Range(-50,1));
        //size = 0.1f;
        sizevect = new Vector3(0.1f, 0.1f, 0.1f);
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
        shroomlight = GetComponent<Light>();
        capcollide = GetComponent<CapsuleCollider>();
        capcollide.enabled = false;
        boxcollide = GetComponent<BoxCollider>();
        //boxcollide.enabled = false;
        Grow();
        mesh.enabled = false;
        GameManager.instance.AddMushroomToList(this);
        MapGen.mapinstance.mushrooms.Add(this);
        Material mat = mesh.material;
        SetColor(color);
    }

    public void Harvest(bool loadedthisway=false) {
        /*if (!alive) {
            return;
        }*/
        //gettingpickedup = true;
        rb.constraints = RigidbodyConstraints.None;
        //rb.isKinematic = false;
        boxcollide.enabled = false;
        capcollide.enabled = true;

        tag = "Items";
        gameObject.layer = 11;
        alive = false;
        if (!loadedthisway)
        {
            FindObjectOfType<Camera>().GetComponent<CameraManager>().UpdateObjList();
            rb.angularVelocity = new Vector3(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f));
            MapGen.mapinstance.TileType(transform.position, true, '.');
        }
        //MapGen.mapinstance.RenewMushroomMap();
    }

    public void SetColor(Color newcolor) {
        Material mat = mesh.material;

        mat.SetColor("_EmissionColor", newcolor);
        shroomlight.color = newcolor;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.transform.position.y+2 < transform.position.y) {
            mesh.enabled = false;
        }
        else {
            mesh.enabled = true;
        }
        /*
        if (glownum > 0)
        {
            glownum--;
            Glow();
        }
        else if (glownum == 0)
        {
            glownum--;
            UnGlow();
        }
        */
        if (!mesh.enabled & shroomlight.enabled)
        {
            shroomlight.enabled = false;
        }
        else if (mesh.enabled & !shroomlight.enabled)
        {
            shroomlight.enabled = true;
        }
        /*if (!alive) {
            PathMap[(int)NewMushroomLoc[0], (int)NewMushroomLoc[1], (int)NewMushroomLoc[2]] = 'M';
        }*/
    }

    public void Grow(bool forced=false) {
        if (alive || forced)
        {
            mesh.enabled = true;
            logsize++;
            if (logsize > 3)
            {
                //boxcollide.enabled = true;
                size = Mathf.Log10(logsize);
            }
            else { size = 0.2f; }

            if (logsize > 0 && logsize % reprofreq < 1)
            {
                // Reproduce
                int breaker = 0;
                int dx, dz;
                bool success = false;
                Vector3 NextMushroomLoc;
                while (!success && breaker < 12)
                {
                    breaker++;
                    dx = Random.Range(-1, 2);
                    dz = Random.Range(-1, 2);
                    NextMushroomLoc = transform.position + Vector3.forward * dx + Vector3.right * dz;
                    if (MapGen.mapinstance.TileType(NextMushroomLoc) == '.')
                    {
                        Instantiate(mushroom, NextMushroomLoc, Quaternion.identity * Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
                        //MapGen.mapinstance.TileType(NextMushroomLoc, true, 'M');
                        //MapGen.mapinstance.RenewMushroomMap();
                        success = true;
                        //PathMap[(int)NewMushroomLoc[0], (int)NewMushroomLoc[1], (int)NewMushroomLoc[2]] = 'M';
                    }
                }
            }

            sizevect[0] = size;
            sizevect[1] = size;
            sizevect[2] = size;
            rb.mass = 2f*size*size*size;
            Mass = rb.mass;
            transform.localScale = sizevect;
            shroomlight.intensity = Mathf.Min(size / 2f,maxshroombrightness);
        }
        else if (!pickedup && (transform.position - oldpos).sqrMagnitude>1.2f) {
            oldpos = transform.position;
            //MapGen.mapinstance.RenewMushroomMap();
        }
    }

    public int Remove(bool dopause=true) {
        if (pickedup == false)
        {
            gettingpickedup = true;
            StartCoroutine(ShrinkAway(dopause));
            GameManager.instance.RemoveMushroomFromList(this);
            MapGen.mapinstance.mushrooms.Remove(this);
            //MapGen.mapinstance.RenewMushroomMap();
            return Mathf.RoundToInt(rb.mass);
        }
        else {
            return 0;
        }
    }

    public bool IsAlive(){
        return alive;
    }

    public bool IsGettingPickedUp() {
        return gettingpickedup || pickedup;
    }

    protected IEnumerator ShrinkAway(bool dopause = true) {
        gettingpickedup = true;
        float time = 2f;
        if (dopause)
        {
            while (time > 0)
            {
                time -= Time.deltaTime;
                yield return null;
            }
        }
        for (int i = 2; i < 200;i++) {
            //Debug.Log(size);
            sizevect[0] = 2f*size/(float)i;
            sizevect[1] = 2f*size/(float)i;
            sizevect[2] = 2f*size/(float)i;
            transform.localScale = sizevect;
            shroomlight.intensity = Mathf.Min(2f * size / (2f * i), maxshroombrightness);
            yield return null;
        }
        Object.Destroy(this.gameObject);
    }

    protected override void OnMouseDown()
    {
        if (!alive && !gettingpickedup)
        {
            base.OnMouseDown();
        }
    }

    public override void SetCollide(bool holding)
    {
        boxcollide.enabled = false;
        capcollide.enabled = holding;
    }

	public override void SetAttributes(float first, float second, bool setheld=false)
	{
        MushInit();
        if (setheld) {
            Harvest(true);
        }
        int colorint = Mathf.RoundToInt(first);
        if (colorint < 0) { colorint = 0; }

        //colors = new Color[] { Color.red, Color.blue, Color.green, Color.cyan, Color.magenta, Color.yellow, Color.cyan };
        //colornames = new string[] { "Red", "Blue", "Green", "Cyan", "Magenta", "Yellow", "Cyan" };

        //capcollide = GetComponent<CapsuleCollider>();
        //boxcollide = GetComponent<BoxCollider>();

        if (colorint >= colors.Length) { colorint = colors.Length - 1; }
        colorind = colorint;
        color = colors[colorint];
        IconColor = color;
        SetColor(color);
        logsize = second - 1;
        Grow(true);
        Name = colornames[colorint] + " Mushroom";
	}

	public override float[] GetAttributes()
	{
        float[] toreturn = new float[2];
        //toreturn[0] = 
        toreturn[0] = colorind;
        toreturn[1] = logsize;
        return toreturn;
	}

}
