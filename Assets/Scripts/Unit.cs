using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    [HideInInspector] public Vector3 TargPos;
    [HideInInspector] public Quaternion TargDir;
    protected Rigidbody rb;
    protected Renderer rend;
    //[HideInInspector] public bool thisturn;
    //protected bool moving;
    public BoxCollider boxcollider;
    protected int horizontal;
    protected int vertical;
    private int glownum;
    protected int hunger;
    public int level;
    protected bool rotating;
    //protected bool falling;
    public bool thisturn;
    protected bool climbing;
    public bool moving;
    public bool stepping;
    public bool falling;
    protected int damagetodo;
    //    public GameObject spaceclaimerdefine;
    public GameObject spaceclaimer;
    public float movetime = 0.1f;
    protected float inversemovetime;
    protected GameObject righthand;
    protected GameObject lefthand;
    public GameObject EmoteBubbleObj;
    protected EmoteBubble emotebubble;
    protected bool ismonster;
    [HideInInspector] public int hitpoints;
    public int maxhitpoints;
    int emoteturncount;
    public Camera cam;
    protected CameraManager camscript;
    public float basedamagefrac;// = 0.1f;
    //public LayerMask steplayermask;
    [HideInInspector] public bool alive;
    protected int turnnum;

    // Use this for initialization
    protected virtual void Start()
    {
        turnnum = 0;
        emoteturncount = 0;
        //steplayermask=LayerMask.GetMask()
        stepping = false;
        hunger = 0;
        cam = FindObjectOfType<Camera>();
        camscript = cam.GetComponent<CameraManager>();
        climbing = false;
        glownum = 0;
        rend = GetComponent<MeshRenderer>();
        //maxhitpoints = hitpoints;
        hitpoints = maxhitpoints;
        falling = false;
        rotating = false;
        //ismonster = true;
        alive = true;
        moving = false;
        TargPos = transform.position;
        rb = GetComponent<Rigidbody>();
        //boxcollider = GetComponent<BoxCollider>();
        //GetComponent<MeshCollider>().enabled = false;
        inversemovetime = 1f / movetime;
        TargDir = transform.rotation;
        //thisturn = false;

        EmoteBubbleObj = Instantiate(EmoteBubbleObj, transform.position, Quaternion.identity);
        EmoteBubbleObj.transform.SetParent(transform,true);
        EmoteBubbleObj.transform.localPosition = Vector3.zero;//Vector3.up;//+Vector3.right;

        emotebubble = EmoteBubbleObj.GetComponent<EmoteBubble>();
        horizontal = 0;
        vertical = 0;
    }

    public virtual void UseEmote(int emotetype) {
        if (alive)
        {
            emoteturncount = 3;
            emotebubble.SetEmote(emotetype);
        }
    }

    protected virtual void Update()
    {
        if (falling || climbing)
        {
            if (transform.position.y - 1 > camscript.PlayerVert())
            {
                rend.enabled = false;
                gameObject.layer = 8;
                foreach (Renderer subrend in GetComponentsInChildren<Renderer>())
                {
                    subrend.enabled = false;
                }
            }
            else
            {
                rend.enabled = true;
                gameObject.layer = 0;
                foreach (Renderer subrend in GetComponentsInChildren<Renderer>())
                {
                    subrend.enabled = true;
                }
            }
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
        }*/
        if (falling == true)
        {
            RaycastHit hit;
            if (!CheckPosition(transform.position + Vector3.down, out hit))
            {
                if (rb.velocity.magnitude < 0.05)
                {
                    falling = false;
                    RagDollOff();
                }
            }
        }
    }

    public bool IsVisible() {
        return rend.isVisible;
    }

    protected void RagDollOn()
    {
        boxcollider.enabled = false;
        //GetComponent<MeshCollider>().enabled = true;
        rb.isKinematic = false;
    }

    protected void RagDollOff()
    {
        rb.isKinematic = true;
        boxcollider.enabled = true;
        //GetComponent<MeshCollider>().enabled = false;
        TargPos = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
        StartCoroutine(MoveSelf(TargPos));
        StartCoroutine(RotateSelf(TargDir));
        thisturn = false;
    }

    protected bool Step(int horiz, int verti, bool justgoup = false)
    {
        bool stepsuccess = false;
        if (horiz != 0 | verti != 0 | justgoup)
        {

            bool hitanything;
            RaycastHit hitinfo;
            int ymod = 0;
            if (justgoup)
            {
                ymod = 1;
            }
            hitanything = CheckPosition(new Vector3(horiz, 0.01f + ymod, verti) + transform.position, out hitinfo);
            if (hitanything)
            {
                //if ()
                TargPos = new Vector3(horiz, 0f, verti) + transform.position;
                TargPos[0] = Mathf.Round(TargPos[0]);
                TargPos[1] = Mathf.Round(TargPos[1]);
                TargPos[2] = Mathf.Round(TargPos[2]);
                StartCoroutine(MoveSelf(TargPos));
                TargDir = Quaternion.LookRotation(TargPos - transform.position);
                StartCoroutine(RotateSelf(TargDir));
                thisturn = false;
                stepsuccess = true;
            }
            else
            {
                GameObject hitentity = hitinfo.transform.gameObject;
                if (hitentity.tag == "Ladder")
                {
                    TargDir = hitentity.transform.rotation;
                    bool acceptableclimb = false;
                    if (hitentity.GetComponent<Ladder>().isbottom)
                    {
                        if (!CheckPosition(Vector3.up + transform.position, out hitinfo))
                        {
                            hitentity = hitinfo.transform.gameObject;
                            if (hitentity.tag == "Ladder")
                            {
                                if (justgoup)
                                {
                                    horiz = (int)(TargDir * Vector3.forward)[0];
                                    verti = (int)(TargDir * Vector3.forward)[2];
                                }
                                //TargDir = Quaternion.LookRotation(new Vector3(horiz, 0f, verti));
                                TargPos = new Vector3(horiz, GameManager.instance.yscale, verti) + transform.position;
                                acceptableclimb = true;
                            }
                        }
                    }
                    else
                    {
                        //TargDir = Quaternion.identity;
                        TargPos = new Vector3(horiz, -GameManager.instance.yscale, verti) + transform.position;
                        acceptableclimb = true;
                    }
                    if (acceptableclimb)
                    {
                        StartCoroutine(RotateSelf(TargDir));
                        StartCoroutine(Climb());
                        thisturn = false;
                        stepsuccess = true;
                    }
                }
                else
                {
                    //Debug.Log(hitentity.name);
                    if (hitentity.tag == "SpaceClaimer")
                    {
                        //Debug.Log("hit space claimer");
                        hitentity = hitentity.transform.parent.gameObject;
                    }
                    //int meleedamage = Mathf.CeilToInt(damagetodo * lefthand.GetComponent<Weapon>().meleefraction);
                    int meleedamage = Mathf.CeilToInt(damagetodo * WeaponMeleeFrac());
                    if (!ismonster)
                    {
                        if (hitentity.tag == "Plant")
                        {
                            Vector3 AttackDir = new Vector3(horiz, 0f, verti);
                            StartCoroutine(RotateSelf(Quaternion.LookRotation(AttackDir)));
                            Swing();
                            hitentity.GetComponent<Mushroom>().Harvest();
                            thisturn = false;
                            stepsuccess = true;
                        }
                        if (hitentity.tag == "Monster" && CheckHostility(hitentity))
                        {
                            Vector3 AttackDir = new Vector3(horiz, 0f, verti);
                            StartCoroutine(RotateSelf(Quaternion.LookRotation(AttackDir)));
                            Swing();
                            Monster enemytohit = hitentity.GetComponent<Monster>();
                            enemytohit.GetHit(AttackDir, meleedamage);
                            enemytohit.Anger(meleedamage * 10);
                            if (!enemytohit.alive) {
                                DidMurder(enemytohit);
                            }
                            thisturn = false;
                            stepsuccess = true;
                        }
                    }
                    else
                    {
                        if ((hitentity.tag == "Player" || hitentity.tag == "Monster") && CheckHostility(hitentity))
                        {
                            Vector3 AttackDir = new Vector3(horiz, 0f, verti);
                            StartCoroutine(RotateSelf(Quaternion.LookRotation(AttackDir)));
                            Swing();
                            //hitentity.GetComponent<Unit>().GetHit(AttackDir, meleedamage);
                            Unit enemytohit = hitentity.GetComponent<Unit>();
                            enemytohit.GetHit(AttackDir, meleedamage);
                            //enemytohit.Anger(meleedamage * 10);
                            if (!enemytohit.alive)
                            {
                                DidMurder(enemytohit);
                            }
                            thisturn = false;
                            stepsuccess = true;
                        }
                        if (hunger > 20 && hitentity.tag == "Plant")
                        {
                            Debug.Log("hitplant");
                            if (hitentity.GetComponent<Mushroom>().IsAlive())
                            {
                                Vector3 AttackDir = new Vector3(horiz, 0f, verti);
                                StartCoroutine(RotateSelf(Quaternion.LookRotation(AttackDir)));
                                Swing();
                                hitentity.GetComponent<Mushroom>().Harvest();
                                hunger -= hitentity.GetComponent<Mushroom>().Remove();
                                thisturn = false;
                                stepsuccess = true;
                            }
                            else if (!(hitentity.GetComponent<Mushroom>().IsGettingPickedUp()))
                            {
                                Debug.Log("nomplant");
                                Vector3 AttackDir = new Vector3(horiz, 0f, verti);
                                StartCoroutine(RotateSelf(Quaternion.LookRotation(AttackDir)));
                                hunger -= hitentity.GetComponent<Mushroom>().Remove(false);
                                thisturn = false;
                                stepsuccess = true;
                            }
                        }
                        if (hunger>20 && hitentity.tag == "Items" && hitentity.name.Contains("Mushroom"))
                        {
                            if (!(hitentity.GetComponent<Mushroom>().IsGettingPickedUp()))
                            {
                                Debug.Log("nomplant");
                                Vector3 AttackDir = new Vector3(horiz, 0f, verti);
                                StartCoroutine(RotateSelf(Quaternion.LookRotation(AttackDir)));
                                hunger -= hitentity.GetComponent<Mushroom>().Remove(false);
                                thisturn = false;
                                stepsuccess = true;
                            }
                        }
                    }
                }
            }
        }
        if (stepsuccess) {
            turnnum++;
            emoteturncount--;
            if (emoteturncount <= 0)
            {
                emotebubble.ClearEmote();
            }
        }
        return stepsuccess;
    }

    /*void RangedAttack(Vector3 TargetSpot)
    {
        //Vector3 attackvector=TargDir
        //bool hitanything;
        //RaycastHit hitinfo;
        //hitanything = CheckPosition(new Vector3(horiz, 0.01f, verti) + transform.position, out hitinfo);
    }*/


    public void GetHit(Vector3 AttackDir, int damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            AttackDir *= damage;
            RagDollOn();
            rb.AddForce(AttackDir, ForceMode.Impulse);
            emotebubble.ClearEmote();
            alive = false;
        }
    }

    public IEnumerator RotateSelf(Quaternion Target)
    {
        rotating = true;
        int breakout = 0;
        if (rend.isVisible)
        {
            float angleremaining = Quaternion.Angle(transform.rotation, Target);
            while (angleremaining > 1f && breakout < 1000 && alive)
            {
                breakout++;
                //Debug.Log(angleremaining);
                Quaternion newrotation = Quaternion.RotateTowards(transform.rotation, Target, 60f * inversemovetime * Time.deltaTime);
                rb.MoveRotation(newrotation);
                angleremaining = Quaternion.Angle(transform.rotation, Target);
                if (falling)
                {
                    rotating = false;
                    yield break;
                }
                yield return null;
            }
        }
        else
        {
            rb.MoveRotation(Target);
        }
        rotating = false;
    }

    protected virtual IEnumerator MoveSelf(Vector3 Target)
    {
        moving = true;
        stepping = true;
        //boxcollider.enabled = false;
        int breaker = 0;
        float sqrRemainingdistance = (transform.position - Target).sqrMagnitude;
        GameObject BlockSpace = Instantiate(spaceclaimer, Target, Quaternion.identity);
        BlockSpace.transform.parent = transform;
        if (rend.isVisible)
        {
            //rb.isKinematic = true;
            while (sqrRemainingdistance > 0.0001 & alive & breaker < 200)
            {
                /*if (rb.tag=="Player") {
                    Debug.Log(sqrRemainingdistance);
                }*/
                breaker++;
                Vector3 newPosition = Vector3.MoveTowards(rb.position, Target, inversemovetime * Time.deltaTime);
                rb.MovePosition(newPosition);
                sqrRemainingdistance = (transform.position - Target).sqrMagnitude;
                if (falling)
                {
                    moving = false;
                    Destroy(BlockSpace);
                    yield break;
                }
                else
                {
                    yield return null;
                }
            }
        }
        else
        {
            rb.MovePosition(Target);
        }
        //thisturn = false;
        moving = false;
        stepping = false;
        //rb.isKinematic = false;
        Destroy(BlockSpace);
        //RaycastHit hit;
        //if (CheckPosition(transform.position + Vector3.down, out hit))
        if (CheckFloor(transform.position))
        {
            falling = true;
            RagDollOn();
        }
        //boxcollider.enabled = true;
    }

    // Go up/down a ladder
    protected virtual IEnumerator Climb()
    {
        climbing = true;
        moving = true;
        stepping = true;
        bool firststep = true;
        int breaker = 0;
        Vector3 IntermediateTarget;
        float sqrRemainingdistance = (transform.position - TargPos).sqrMagnitude;
        float sqrIntermediatedistance;
        GameObject BlockSpace = Instantiate(spaceclaimer, TargPos, Quaternion.identity);
        BlockSpace.transform.parent = transform;
        StartCoroutine(Wiggle(GameManager.instance.yscale, 1));

        while (sqrRemainingdistance > 0.0001 & alive & breaker < 3000)
        {
            if (Mathf.RoundToInt(transform.position.y) < Mathf.RoundToInt(TargPos[1]))
            {
                IntermediateTarget = transform.position + Vector3.up;
            }
            else if (Mathf.RoundToInt(transform.position.y) > Mathf.RoundToInt(TargPos[1]))
            {
                //Debug.Log("fuck?");
                IntermediateTarget = transform.position;
                if (firststep)
                {
                    IntermediateTarget[0] = TargPos[0];
                    IntermediateTarget[2] = TargPos[2];
                    firststep = false;
                }
                else
                {
                    IntermediateTarget += Vector3.down;
                }
            }
            else
            {
                IntermediateTarget = TargPos;
            }
            sqrIntermediatedistance = (transform.position - IntermediateTarget).sqrMagnitude;
            while (sqrIntermediatedistance > 0.0001 & alive)
            {
                breaker++;
                Vector3 newPosition = Vector3.MoveTowards(rb.position, IntermediateTarget, inversemovetime * Time.deltaTime);
                rb.MovePosition(newPosition);
                sqrIntermediatedistance = (transform.position - IntermediateTarget).sqrMagnitude;
                yield return null;
            }
            sqrRemainingdistance = (transform.position - TargPos).sqrMagnitude;
            //Debug.Log(sqrRemainingdistance);
        }
        //thisturn = false;
        moving = false;
        climbing = false;
        stepping = false;
        //rb.isKinematic = false;
        Destroy(BlockSpace);
    }

    // Check if standing on a floor, returns true of no floor (so, fall down)
    protected bool CheckFloor(Vector3 position) {

        if (MapGen.mapinstance.TileType(position)=='.') {
            return false; // Don't need a raycast, we know there is floor here
        }
        RaycastHit hit;
        return CheckPosition(position+Vector3.down,out hit);
    }

    // returns true if able to move there
    protected bool CheckPosition(Vector3 Target, out RaycastHit hitinfo)
    {
        bool hit=false;

        boxcollider.enabled = false;
        hit = Physics.Linecast(transform.position, Target, out hitinfo);

        if (hit && hitinfo.transform.gameObject.GetComponent<Unit>())
        {
            if (hitinfo.transform.gameObject.GetComponent<Unit>().stepping)
            {
                //Debug.Log("Stepping,ignore");
                hit = false;
            }
        }

        boxcollider.enabled = true;
        return !hit;
    }

    // Swing weapon
    protected void Swing()//bool hand=true)
    {
        GameObject ToSwing=null;
        Weapon hasweapon = HasWeapon();
        // hand == true == left
        if (hasweapon != null)
        {
            ToSwing = hasweapon.gameObject;
        }
        else
        {
            if (lefthand != null)
            {
                ToSwing = lefthand;
            }
            else if (righthand != null)
            {
                ToSwing = righthand;
            }
        }

        if (ToSwing==null) {
            StartCoroutine(Wiggle(1, 0, -45f));
        }
        else {
            if (ToSwing == lefthand)
            {
                StartCoroutine(Wiggle(1, 1, -45f));
            }
            else {
                StartCoroutine(Wiggle(1, 1, 45f));
            }
            StartCoroutine(Swinging(ToSwing));
        }
    }

    IEnumerator Wiggle(int numwiggles=1,int wiggleaxis=1, float anglewanted=45f) {
        int stepstaken = 0;
        float stepsize = anglewanted * inversemovetime * Time.deltaTime;
        int steps = (int)(anglewanted / stepsize);
        Vector3 rotation = new Vector3(0, 0, 0);
        for (int k = 0; k < numwiggles;k++) {
            for (int i = -1; i < 2; i += 2)
            {
                rotation[wiggleaxis] = stepsize * i;
                for (int j = 0; j < steps; j++)
                {
                    if (!rotating)
                    {
                        transform.Rotate(rotation, Space.Self);
                        stepstaken += i;
                    }
                    yield return null;
                }
            }

        }
        if (stepstaken != 0)
        {
            rotation[wiggleaxis] = -stepsize * (stepstaken / Mathf.Abs(stepstaken));
            for (int j = 0; j < Mathf.Abs(stepstaken);j++) {
                if (!rotating)
                {
                    transform.Rotate(rotation, Space.Self);
                }
                yield return null;
            }
        }
    }

    IEnumerator Swinging(GameObject ToSwing)
    {
        moving = true;
        float anglewanted = 180f;
        float stepsize = anglewanted * inversemovetime*Time.deltaTime;
        int steps=(int)(anglewanted/stepsize);
        Vector3 rotation = new Vector3(0, 0, 0);
        for (int i = -1; i < 2; i += 2)
        {
            rotation[0] = -stepsize * i;
            for (int j = 0; j < steps; j++)
            {
                ToSwing.transform.Rotate(rotation, Space.Self);
                yield return null;
            }
        }
        moving = false;
    }

    public string Holding() {
        if (lefthand != null)
        {
            return lefthand.name.Split('(')[0];
        }
        else {
            return "Nothing";
        }
    }

    protected IEnumerator Jump(int horiz, int verti) {
        int breaker = 0;
        while (rotating && breaker<200) {
            breaker++;
            yield return null;
        }
        falling = true;
        RagDollOn();
        rb.velocity = 5*(new Vector3(horiz, 0.75f, verti));
    }

    void OnMouseOver() {
        Glow(true);
    }

    void OnMouseExit() {
        UnGlow();
    }

    public bool IsRotating() {
        return rotating;
    }

    /*void OnMouseOver() {
        //Debug.Log("moose!");
        Renderer render = GetComponent<Renderer>();
        Material mat = render.material;

        float emission = Mathf.PingPong(3*Time.time, 1.0f);
        Color baseColor = Color.gray; //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }*/

    void UnGlow() {
        //Debug.Log("Unmoose");
        //Renderer render = GetComponent<Renderer>();
        Material mat = rend.material;
        float emission = 0f;//Mathf.PingPong(Time.time, 1.0f);
        Color baseColor = Color.gray; //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }

    public void Glow(bool touched=false) {
        //Debug.Log("what");
        if (touched)
        {
            glownum = 1;
        }
        //Renderer render = GetComponent<Renderer>();
        Material mat = rend.material;

        float emission = Mathf.PingPong(3 * Time.time, 1.0f);
        Color baseColor = Color.gray; //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        //Debug.Log(finalColor);

        mat.SetColor("_EmissionColor", finalColor);
    }

    void OnCollisionEnter(Collision collision) {
        GameObject hitter = collision.gameObject;
        if (hitter.transform.parent != transform)
        {
            if (hitter.tag == "Projectile" && hitter.GetComponent<Rigidbody>().velocity.magnitude > 1f)
            {
                hitter.transform.parent = transform;
                hitter.GetComponent<Rigidbody>().isKinematic = true;
                hitter.GetComponent<BoxCollider>().enabled = false;
                GetHit(collision.impulse.normalized, Mathf.RoundToInt(collision.impulse.magnitude));
                GameManager.instance.waitforprojectile = false;
            }
        }
    }

    public float WeaponMeleeFrac() {
        Weapon weapontouse = HasWeapon();
        if (weapontouse==null) {
            return basedamagefrac;
        }
        return weapontouse.meleefraction;

    }

    public Weapon HasWeapon(bool ranged=false) {
        Weapon toreturn = null;
        if (lefthand != null) {
            toreturn = lefthand.GetComponent<Weapon>();
            if (toreturn != null) {
                if (!ranged || (ranged && toreturn.ranged))
                {
                    return toreturn;
                }
            }
        }
        if (righthand != null) {
            toreturn = righthand.GetComponent<Weapon>();
            if (toreturn != null)
            {
                if (!ranged || (ranged && toreturn.ranged))
                {
                    return toreturn;
                }
            }
        }
        return null;
    }

    // Virtual function, to be overridden with more detail
    public virtual bool CheckHostility(GameObject other) {
        return false;
    }

    /*
    public IEnumerator IgnoreTemporarily(float seconds=0.2f) {
        foreach (Collider ignorethis in GetComponents<Collider>())
        {
            Physics.IgnoreCollision(ignorethis, GetComponent<Collider>(), true);
        }
        float thistime = 0f;
        int breaker = 0;
        while (thistime<seconds && breaker<200) {
            breaker++;
            thistime += Time.deltaTime;
            yield return null;
        }

        foreach (Collider ignorethis in ignoreme.GetComponents<Collider>())
        {
            Physics.IgnoreCollision(ignoreme.GetComponent<Collider>(), GetComponent<Collider>(), false);
        }
    }*/

    protected virtual void DidMurder(Unit murdered) {
        return;
    }

}
