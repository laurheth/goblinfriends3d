using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private List<Monster> monsters;
    List<Mushroom> mushrooms;
    public GameObject player;
    public int yscale;
    public bool waitforprojectile;
    public GameObject projectile;
    private Vector3 projectilelastspot;
    private int frameswaited;
    private int turns;
    private bool monsterturn;
    [HideInInspector] public bool playerturn;
    public static GameManager instance = null;

	// Use this for initialization
	void Awake () {
        frameswaited = 0;
        projectilelastspot = Vector3.zero;
        waitforprojectile = false;
        turns = 0;
        monsterturn = false;
        playerturn = true;

        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        monsters = new List<Monster>();
        mushrooms = new List<Mushroom>();
	}

	// Update is called once per frame
	void Update () {

        if (waitforprojectile) {
            if ((projectile.transform.position - projectilelastspot).magnitude < 0.005 || projectile.transform.position[1] < -10 || frameswaited > 50)
            {
                waitforprojectile = false;
                projectilelastspot = Vector3.zero;
            }
            else
            {
                frameswaited++;
                return;
            }
        }
        frameswaited = 0;
        if (monsterturn || player.GetComponent<Player>().thisturn) {
            return;
        }
        // set player position for first turn whether it would otherwise be set or not
        if (turns==0) {
            MapGen.mapinstance.RefreshDMap(0);
            MapGen.mapinstance.AddMapGoal(0, player.transform.position);
            MapGen.mapinstance.GenerateDMap(0);
        }
        turns++;
        //Debug.Log(turns);

        monsterturn = true;
        GrowMushrooms();

        if (turns % 10 ==0) {
            StartCoroutine(MapGen.mapinstance.RenewMushroomMap());
        }

        // Move enemies
        MoveMonsters();

        // Update DMaps for monster types
        /*for (int i=0; i < 3;i++) {
            MapGen.mapinstance.RenewMonsterMap(i, monsters);
        }*/
        MapGen.mapinstance.RenewMonsterMap(monsters);

	}

    void MoveMonsters() {

        monsterturn = true;

        //Debug.Log(monsters.Count);
        for (int i = 0; i < monsters.Count;i++) {
            monsters[i].EnemyTurn();
        }

        player.GetComponent<Player>().thisturn = true;
        playerturn = true;
        monsterturn = false;

    }

    void GrowMushrooms() {
        if (mushrooms.Count==0) {
            return;
        }
        for (int i = 0; i < mushrooms.Count;i++) {
            mushrooms[i].reprofreq = 6 * mushrooms.Count;
            mushrooms[i].Grow();
        }
    }

    public void AddMonsterToList(Monster script){
        monsters.Add(script);
    }

    public void AddMushroomToList(Mushroom script) {
        mushrooms.Add(script);
    }

    public void RemoveMushroomFromList(Mushroom script)
    {
        mushrooms.Remove(script);
    }

    public void TransmitEmote(int totransmit) {
        for (int i = 0; i < monsters.Count; i++)
        {
            if (monsters[i].IsVisible())
            {
                monsters[i].GetEmote(totransmit);
            }
        }
    }
}
