using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// Setup and overarching mapgen script goes here
public partial class MapGen : MonoBehaviour
{

    public int xsize;
    public int ysize;
    public int zsize;
    public int numrooms;
    public int hallmaterial;
    public int goblintownmaterial;
    public int subdivisions;
    public int yscale;
    public int level;
    public int NumGoblins;
    public int NumVegans;
    public int NumBeasts;
    private int goblinsadded;
    private int vegansadded;
    private int beastsadded;
    private int numgenerators;
    private int xsub;
    private int yslices;
    private int zsub;
    public GameObject player;
    public GameObject debugblock;
    //public GameObject goblin;
    public GameObject[] decorations;
    //private GameObject[][] monsters;
    public GameObject[] humanoids;
    public GameObject[] beasts;
    public GameObject[] vegans;
    //public GameObject[] beasts;
    //Vector3[] roomcentres;
    //private Vector3[] connectfrom;
    public int maxroomsize;
    public int minroomsize;
    public GameObject[] blocks;
    public GameObject[] floors;
    public GameObject ladder;
    public GameObject mushroom;
    int[,,] Map; // contains materials & stuff
    char[,,] PathMap; // contains more basic details, .# M
    [HideInInspector] public int[,,,] DMaps; // 4d array consisting of several pathfinding maps,
                    // 0-> playerman, 1-> shroommap
    public int[] PathDists;
    private bool[] PathRefreshed;
    int NumDMaps;
    private int[,,] RoomTags;
    private int RoomID;

    public static MapGen mapinstance = null;

    // Use this for initialization
    void Start()
    {
        goblinsadded = 0;
        vegansadded = 0;
        beastsadded = 0;
        mushrooms = new List<Mushroom>();
        // 0-> player, 1-> mushrooms, 2-> goblintowne
        NumDMaps = PathDists.Length;
        //Check if instance already exists
        if (mapinstance == null)

            //if not, set instance to this
            mapinstance = this;

        //If instance already exists and it's not this:
        else if (mapinstance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        //Physics.queriesHitTriggers = true;

        numgenerators = 3;
        //level = 1;
        /*roomcentres = new Vector3[numrooms];
        connectfrom = new Vector3[numrooms];
        for (int i = 0; i < numrooms; i++)
        {
            roomcentres[i] = new Vector3(-1, -1, -1);
            connectfrom[i] = new Vector3(0, 0, 0);
        }*/
        xsub = xsize / subdivisions;
        zsub = zsize / subdivisions;
        yslices = ysize / yscale;
        GameManager.instance.yscale = yscale;
        RoomID = -1;
        Map = new int[xsize, ysize, zsize];
        DMaps = new int[xsize, yslices, zsize, NumDMaps];
        PathMap = new char[xsize, ysize, zsize];
        RoomTags = new int[xsize, ysize, zsize];
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < zsize; j++)
            {
                for (int k = 0; k < ysize; k++)
                {
                    Map[i, k, j] = -1;
                    RoomTags[i, k, j] = -1;
                    PathMap[i, k, j] = ' ';
                    if (k % yscale ==0) {
                        for (int h = 0; h < NumDMaps;h++) {
                            DMaps[i, k/yscale, j, h]=PathDists[h];
                        }
                    }

                }
            }
        }
        PathRefreshed = new bool[NumDMaps];
        for (int i = 0; i < NumDMaps;i++) {
            PathRefreshed[i] = true;
        }
        MakeMap();
    }

    // Generate a map
    void MakeMap_old()
    {
        RoomID = -1;

        Vector3 Origin = new Vector3(1, 0, 1);
        Vector3 RoomSize = new Vector3(1, 1, 1);
        //int nextsizex;
        //int nextsizez;
        Origin[0] = (int)(xsize / 2);//Random.Range(maxroomsize, xsize - 1 - maxroomsize);
        Origin[2] = (int)(zsize / 2);//Random.Range(maxroomsize, zsize - 1 - maxroomsize);
        RoomSize[0] = Random.Range(minroomsize, maxroomsize);
        RoomSize[2] = Random.Range(minroomsize, maxroomsize);

        int[,,] MapPlan = new int[subdivisions,yslices,subdivisions];
        int insertnext;
        int townx = Random.Range(0, subdivisions);
        int towny = Random.Range(0, yslices);
        int townz = Random.Range(0, subdivisions);
        if (townx == subdivisions) { townx--; }
        if (towny == yslices) { towny--; }
        if (townz == subdivisions) { townz--; }
        for (int k = 0; k < yslices; k++)
        {
            for (int i = 0; i < subdivisions; i++)
            {
                for (int j = 0; j < subdivisions; j++)
                {
                    insertnext = Random.Range(0, numgenerators + 1);
                    if (townx == i && towny == k && townz == j)
                    {
                        insertnext = -1;
                        if (k > 0)
                        {
                            if (MapPlan[i, k - 1, j] == 0)
                            {
                                MapPlan[i, k - 1, j] = 2;
                            }
                        }
                    }
                    else
                    {
                        if (k > 0)
                        {
                            if (MapPlan[i, k - 1, j] == 0)
                            {
                                insertnext = 1;
                            }
                            else if (insertnext == 0)
                            {
                                insertnext += 2;
                            }

                        }

                        if (k == 0 && insertnext == 1)
                        {
                            insertnext += 1;
                        }
                    }

                    MapPlan[i,k,j] = insertnext;
                }
            }
        }

        // Loop through subdivisions & slices until all sections made
        for (int k = 0; k < yslices; k++)
        {
            for (int i = 0; i < subdivisions; i++)
            {
                for (int j = 0; j < subdivisions; j++)
                {
                    GenerateSpace(i*xsub,k*yscale,j*zsub,hallmaterial,MapPlan[i,k,j]);
                }
            }
        }

        // Connect subdivisions
        SubdivisionConnect();

        for (int k = 0; k < ysize; k++)
        {
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < zsize; j++)
                {
                    if (Map[i, k, j] < -1) {
                        PathMap[i, k, j] = '.';
                    }
                    else if (Map[i, k, j] > 0)
                    {
                        PathMap[i, k, j] = '#';
                    }
                }
            }
        }

        // Add ladders
        int ladderx, ladderz;
        int breaker = 0;
        for (int num = 0; num < 6; num++)
        {

            for (int k = (yslices - 1); k > 0; k--)
            {
                breaker = 0;
                do
                {
                    ladderx = Random.Range(1, xsize - 2);
                    ladderz = Random.Range(1, zsize - 2);
                    breaker++;
                    //Debug.Log(ladderx);
                    //Debug.Log(ladderz);
                    //Debug.Log(k*yscale);
                } while (Map[ladderx, k * yscale, ladderz + 1] > -2 & breaker < 200 & NumNeighbours(ladderx, k * yscale, ladderz,true) < 8 );
                //Debug.Log(breaker);
                //Instantiate(debugblock, new Vector3(ladderx, k * yscale, ladderz + 1), Quaternion.identity);
                //Instantiate(debugblock, new Vector3(ladderx, (k - 1) * yscale, ladderz), Quaternion.identity);
                AddLadder(ladderx, ladderz, (k - 1) * yscale, k * yscale, Vector3.forward);
            }
        }

        // Find places to put decorations
        AddDecorations();

        // Fill in blocks
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < zsize; j++)
            {
                for (int k = 0; k < ysize; k++)
                {
                    //Debug.Log(k);
                    if (Map[i, k, j] > 0)
                    {
                        Instantiate(blocks[Map[i, k, j]], new Vector3(i, k, j), Quaternion.identity);
                    }
                    else if (Map[i, k, j] < -1)
                    {
                        Instantiate(floors[-(Map[i, k, j]+2)], new Vector3(i, k - 0.45f, j), Quaternion.identity);
                        if (k==0 || Map[i,k-1,j]<1) {
                            Instantiate(blocks[-Map[i, k, j]-2], new Vector3(i, k-1, j), Quaternion.identity);
                        }
                    }
                    else if (k>0) {
                        if (Map[i, k - 1, j] > 0)
                        {
                            Instantiate(blocks[Map[i, k - 1, j]], new Vector3(i, k, j), Quaternion.identity);
                            Map[i, k, j] = Map[i, k - 1, j];
                        }
                    }
                }
            }
        }

        for (int i = 0; i < 6; i++)
        {
            MushroomPatch();
        }
        GenerateDMap(1);
        GenerateDMap(2);

        player.transform.position = PlaceEntity();
        /*for (int i = 0; i < 16;i++) {
            PlaceMonster();
        }*/

    }

    // Add a ladder
    void AddLadder(int xpos, int zpos, int ystart, int yend, Vector3 forward) {
        //Debug.Log(ystart);
        GameObject newladder;
        for (int yy = ystart; yy < yend;yy++) {
            //PlaceWall(xpos, yy, zpos + 1, hallmaterial, -1, true);
            newladder=Instantiate(ladder, new Vector3(xpos, yy, zpos)+0.45f*forward, Quaternion.LookRotation(forward));
            if (yy == ystart)
            {
                newladder.GetComponent<Ladder>().isbottom = true;
                if (Map[xpos, yy, zpos] >= -1)
                {
                    PlaceWall(xpos, yy, zpos, -hallmaterial, -1, true);
                }
            }
            else
            {
                PlaceWall(xpos, yy, zpos, 0, -1, true);
            }
        }
        PathMap[xpos, ystart, zpos] = '<';
        // for pathfinding, add a constraint tile, either | or -, one tile behind the ladder
        int cx = Mathf.RoundToInt(forward[0]);
        int cz = Mathf.RoundToInt(forward[2]);
        if (cx==0 && cz==0) {
            Debug.Log("Something went wrong with cx and cz in ladder addition");
        }
        if (cx==0) {
            PathMap[xpos + cx, ystart, zpos + cz] = '-'; // disallow motion along z axis
        }
        else {
            PathMap[xpos + cx, ystart, zpos + cz] = '|'; // disallow motion along x axis
        }
        PathMap[xpos, yend, zpos] = '>';
        PlaceWall(xpos, yend, zpos, 0, -1, true);
    }

    // Connect subivisions
    void SubdivisionConnect(){
        int breakout = 0;
        int testspot;
        bool success = false;
        if (subdivisions==1) {
            // No subdivisions to connect
            return;
        }
        for (int k = 0; k < yslices; k++)
        {
            for (int i = 0; i < subdivisions; i++)
            {
                for (int j = 0; j < subdivisions; j++)
                {
                    
                    if (i > 0)
                    {
                        breakout = 0;
                        do
                        {
                            testspot = Random.Range(j * xsub + 2, (j + 1) * xsub + -2);
                            success = true;
                            for (int hh = -2; hh < 2; hh++)
                            {
                                success &= (Map[i * xsub + hh, k*yscale, testspot]<-1 | Map[i * xsub + hh, k * yscale, testspot] ==0);
                            }
                            breakout++;
                        } while (success == false && breakout < 100);
                        for (int hh = -2; hh < 2; hh++)
                        {
                            PlaceWall(i * xsub + hh, k*yscale, testspot, -hallmaterial, -1, true);
                        }
                    }
                    if (j > 0)
                    {
                        testspot = Random.Range(i*zsub+2, (i+1)*zsub - 2);
                        for (int hh = -1; hh < 1; hh++)
                        {
                            PlaceWall(testspot, k*yscale, j * zsub + hh, -hallmaterial, -1, true);
                        }
                    }
                }
            }
        }
    }

    // Place monster
    // montype reflects monstertype; 0=> goblin, 1=> beast, 2=> vegan
    void PlaceMonster(int x = -1,int y = -1, int z=-1, int montype=0, int forcemon=-1) {
        GameObject[] monsters;
        switch(montype) {
            default:
            case 0:
                monsters = humanoids;
                break;
            case 1:
                monsters = beasts;
                break;
            case 2:
                monsters = vegans;
                break;
        }
        int monindex = Random.Range(0, monsters.Length);
        if (forcemon >= 0)
        {
            monindex = forcemon;
        }
        if (monindex > monsters.Length)
            { monindex = monsters.Length; }
        GameObject monstertoplace = monsters[monindex];
        Instantiate(monstertoplace, PlaceEntity(x,y,z), Quaternion.identity * Quaternion.Euler(Vector3.up * 180));
    }

    // Mushroom patch
    void MushroomPatch() {
        Vector3 NewMushroomLoc = PlaceEntity();
        Vector3 NextMushroomLoc = NewMushroomLoc;
        Instantiate(mushroom, NewMushroomLoc, Quaternion.identity * Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
        //PathMap[(int)NewMushroomLoc[0], (int)NewMushroomLoc[1], (int)NewMushroomLoc[2]] = 'M';
        AddMapGoal(1, NewMushroomLoc);
        int dx=0;
        int dz=0;
        int breaker = 0;
        int placed = 1;
        do
        {
            dx += Random.Range(-1, 2);
            dz += Random.Range(-1, 2);
            NextMushroomLoc = NewMushroomLoc + Vector3.forward * dx + Vector3.right * dz;
            NextMushroomLoc = PlaceEntity((int)NextMushroomLoc[0], (int)NextMushroomLoc[1], (int)NextMushroomLoc[2]);
            if ((NextMushroomLoc - NewMushroomLoc).sqrMagnitude < 4)
            {
                dx = 0;
                dz = 0;
                NewMushroomLoc = NextMushroomLoc;
                Instantiate(mushroom, NewMushroomLoc, Quaternion.identity * Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
                //PathMap[(int)NewMushroomLoc[0], (int)NewMushroomLoc[1], (int)NewMushroomLoc[2]] = 'M';
                AddMapGoal(1, NewMushroomLoc);
                placed++;
            }
            breaker++;
        } while (breaker < 50 && placed < 7);
    }

    // Find spot for entity
    Vector3 PlaceEntity(int x=-1, int y=-1, int z=-1) {
        int breaker = 0;
        //int x, y, z;
        bool success = false;
        Vector3 testhere = new Vector3(-1,-1,-1);
        Vector3 testbox = new Vector3(0.2f, 0.2f, 0.2f);
        do
        {
            if (x < 0)
            {
                x = Random.Range(0, xsize);
            }
            if (y < 0)
            {
                y = yscale * Random.Range(0, yslices);
            }
            if (z < 0)
            {
                z = Random.Range(0, zsize);
            }
            testhere[0] = (int)x;
            testhere[1] = (int)y;
            testhere[2] = (int)z;

            if (x >= 0 && x <= (xsize - 1) && y >= 0 && y <= (ysize - 1) && z >= 0 && z <= (zsize - 1))
            {

                if ((Map[x, y, z] < -1) && !Physics.CheckBox(testhere, testbox))
                {
                    success = true;
                }
            }
            else { success = false; }
            x = -1;
            y = -1;
            z = -1;

            breaker++;
        } while (!success && breaker < 300);
        return testhere;
    }

    // Drop a pillar
    void DropPillar(int x, int y, int z, int yend, int BuildMaterial){
        for (int i = 0; i < (yend-y);i++) {
            PlaceWall(x, y+i, z, BuildMaterial, RoomID, true);
        }
    }


    // Place wall/floor
    // To place floor, set Buildfrom to be negative.
    bool PlaceWall(int x, int y, int z, int Buildfrom, int RoomTag, bool forceit = false)
    {
        bool success = false;
        if (x < 0 || x >= xsize || z < 0 || z >= zsize)
        {
            return success;
        }
        if (forceit || Map[x, y, z] == -1)
        {
            if (Buildfrom >= 0)
            {
                Map[x, y, z] = Buildfrom;
            }
            else
            {
                Map[x, y, z] = Buildfrom - 2;
            }
            RoomTags[x, y, z] = RoomTag;
            success = true;
        }
        return success;
    }

    // Number of neighbouring walls
    int NumNeighbours(int x, int y, int z,bool countfloors=false) {
        int toreturn = 0;

        for (int i = -1; i < 2;i++) {
            for (int j = -1; j < 2;j++) {
                if (countfloors)
                {
                    if (Map[x + i, y, z + j] < -1)
                    {
                        toreturn++;
                    }
                }
                else
                {
                    if (Map[x + i, y, z + j] > 0)
                    {
                        toreturn++;
                    }
                }
            }
        }

        return toreturn;
    }

    // Add decorations
    void AddDecorations() {
        for (int k = 0; k < yslices;k++) {
            for (int i = 1; i < xsize - 1;i++) {
                for (int j = 1; j < zsize - 1;j++) {
                    if (Map[i,k*yscale,j]<-1) {
                        if (NumNeighbours(i,k*yscale,j)==5) {
                            PlaceDecoration(i, k*yscale, j, 0);
                        }
                    }
                }
            }
        }
    }

    // Add a decoration
    void PlaceDecoration(int x, int y, int z, int ObjID)
    {
        if (Map[x, y, z] < -1)
        {
            Instantiate(decorations[ObjID], new Vector3(x, y, z), Quaternion.identity);
        }
    }

    public int RoomTag(Vector3 position) {
        int x, y, z;
        x = Mathf.RoundToInt(position[0]);
        y = Mathf.RoundToInt(position[1]);
        z = Mathf.RoundToInt(position[2]);
        return RoomTags[x, y, z];
    }
}
