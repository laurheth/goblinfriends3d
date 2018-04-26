using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public string Name;
    public float Mass;
    public string Icon;
    public Color IconColor;
    public Color color;
    protected MeshRenderer mesh;
    public bool pickedup;
    public float hardness;
	// Use this for initialization
	/*protected virtual void Start () {
        Name = "Item";
        Mass = 0f;
        Icon = "?";
        IconColor = Color.black;
	}*/
	
    protected virtual void Start() {
        color = Color.black;
        mesh = GetComponent<MeshRenderer>();
    }

    public string GetMass() {
        if (Mass>10) {
            return ((int)(Mass)).ToString("D");
        }
        else if (Mass>1) {
            return (Mass).ToString("F1");
        }
        else {
            return (Mass).ToString("F");
        }
    }

    protected virtual void OnMouseDown() {
        if (pickedup == false)
        {
            pickedup = true;
            UnGlow();
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().AddToInventory(gameObject);
        }
    }

    public virtual void SetCollide(bool holding) {
        foreach (Collider thisone in GetComponents<Collider>()) {
            thisone.enabled = holding;
        }
    }

    void OnMouseOver()
    {
        if (transform.parent == null)
        {
            Glow();
        }
    }

    void OnMouseExit()
    {
        UnGlow();
    }

    void UnGlow()
    {
        Material mat = mesh.material;

        mat.SetColor("_EmissionColor", color);
    }

    public void Glow()
    {
        //Renderer render = GetComponent<Renderer>();
        Material mat = mesh.material;

        float emission = Mathf.PingPong(3 * Time.time, 1.0f);
        Color baseColor = Color.gray; //Replace this with whatever you want for your base color at emission level '1'

        Color finalColor = baseColor * Mathf.LinearToGammaSpace(emission);

        mat.SetColor("_EmissionColor", finalColor);
    }

    public IEnumerator IgnoreTemporarily(GameObject ignoreme, float seconds=0.2f) {
        //Debug.Log("Ignoring " + ignoreme.name);
        //Debug.Log("via collider:" + GetComponent<Collider>());
        foreach (Collider ignorethis in ignoreme.GetComponentsInChildren<Collider>())
        {
            foreach (Collider ignorevia in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(ignorethis, ignorevia, true);
            }
        }
        float thistime = 0f;
        int breaker = 0;
        while (thistime<seconds && breaker<400) {
            breaker++;
            thistime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("Unignoring " + ignoreme.name);
        foreach (Collider ignorethis in ignoreme.GetComponents<Collider>())
        {
            foreach (Collider ignorevia in GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(ignorethis, ignorevia, false);
            }
        }
    }

    // Default "use" is putting into hand slot
    public void UseItem(Player playerscript) {
        if (playerscript.OnBody(0)==null) {
            playerscript.SetSlot(0, gameObject);
        }
        else if (playerscript.OnBody(1)==null) {
            playerscript.SetSlot(1, gameObject);
        }
        else {
            playerscript.AddToInventory(playerscript.OnBody(1));
            playerscript.SetSlot(1, gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        /*if (tag == "Items")
        {
            Debug.Log(collision.gameObject.name);
            Debug.Log(collision.collider);
        }*/
    }

    public virtual void SetAttributes(float first, float second, bool setheld=false) {
        return;
    }

    public virtual float[] GetAttributes() {
        float[] toreturn = { 0f,0f };
        return toreturn;
    }

}
