using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapGen : MonoBehaviour {
    /*
    public int[,,] Map;
    public char[,,] PathMap;
    public int xsize;
    public int ysize;
    public int zsize;
    public int yscale;
    public int level;
    public int yslices;
    public List<Thing> ThingList;
    public List<MonsterThing> MonsterThingList;
    */

    public SaveMap Save() {
        SaveMap saveMap = new SaveMap();

        saveMap.Map = new int[xsize*ysize*zsize];//Map;
        saveMap.PathMap = new char[xsize * ysize * zsize];//PathMap;

        for (int k = 0; k < ysize;k++) {
            for (int j = 0; j < zsize;j++) {
                for (int i = 0; i < xsize;i++) {
                    saveMap.Map[i + j * xsize + k * xsize * zsize] = Map[i, k, j];
                    saveMap.PathMap[i + j * xsize + k * xsize * zsize] = PathMap[i, k, j];
                }
            }
        }

        saveMap.xsize = xsize;
        saveMap.ysize = ysize;
        saveMap.zsize = zsize;
        saveMap.yscale = yscale;
        saveMap.level = level;
        saveMap.yslices = yslices;
        GameObject[] objlist;
        string[] tags = { "Items", "Decorations", "Plant", "Ladder", "Stairs", "Monster" };
        foreach (string thistag in tags) {
            objlist = GameObject.FindGameObjectsWithTag(thistag);
            if (objlist.Length>0) {
                foreach (GameObject thisobj in objlist) {
                    if (thisobj.transform.parent==null) {
                        if (thistag == "Monster")
                        {
                            saveMap.AddMonsterThing(thisobj);
                        }
                        else
                        {
                            saveMap.AddThing(thisobj);
                        }
                    }
                }
            }
        }
        return saveMap;
    }

    public void ClearMap() {
        GameObject[] objlist;
        mushrooms.Clear();
        string[] tags = { "Items", "Decorations", "Plant", "Ladder", "Stairs", "Monster","Blocks","Floor","Barrier"};
        foreach (string thistag in tags)
        {
            objlist = GameObject.FindGameObjectsWithTag(thistag);
            if (objlist.Length > 0)
            {
                foreach (GameObject thisobj in objlist)
                {
                    Destroy(thisobj);
                }
            }
        }
        GameManager.instance.ClearLists();
    }

    public void Load(SaveMap saveMap) {
        ClearMap();
        xsize = saveMap.xsize;
        ysize = saveMap.ysize;
        zsize = saveMap.zsize;
        yscale = saveMap.yscale;
        level = saveMap.level;
        yslices = saveMap.yslices;

        Map = new int[xsize, ysize, zsize];
        DMaps = new int[xsize, yslices, zsize, NumDMaps];
        PathMap = new char[xsize, ysize, zsize];
        RoomTags = new int[xsize, ysize, zsize];

        for (int k = 0; k < ysize; k++)
        {
            for (int j = 0; j < zsize; j++)
            {
                for (int i = 0; i < xsize; i++)
                {
                    Map[i, k, j] = saveMap.Map[i + j * xsize + k * xsize * zsize];
                    PathMap[i, k, j] = saveMap.PathMap[i + j * xsize + k * xsize * zsize];
                }
            }
        }

        MakeMap(true);

        foreach (SaveMap.Thing thisthing in saveMap.ThingList) {
            GameObject newthing = GameManager.instance.MakeGameObject(thisthing.name);


            if (newthing.tag == "Plant" || newthing.tag=="Items") {
                newthing.GetComponent<Item>().SetAttributes(thisthing.field1, thisthing.field2, !thisthing.status);    
            }
            if (newthing.tag == "Ladder") {
                newthing.GetComponent<Ladder>().isbottom = thisthing.status;
            }

            newthing.transform.position = thisthing.position;
            newthing.transform.rotation = thisthing.rotation;
        }

        foreach (SaveMap.MonsterThing thisthing in saveMap.MonsterThingList) {
            GameObject newthing = GameManager.instance.MakeGameObject(thisthing.name);
            Monster monscript = newthing.GetComponent<Monster>();
            monscript.InitUnit();
            newthing.transform.position = thisthing.position;
            newthing.transform.rotation = thisthing.rotation;
            monscript.HomeLocation = thisthing.homeposition;
            monscript.alive = thisthing.status;
            if (!monscript.alive) {
                monscript.RagDollOn();
            }
            monscript.tiredness = thisthing.tiredness;
            monscript.bravery = thisthing.bravery;
            monscript.anger = thisthing.anger;
            monscript.fear = thisthing.fear;
            monscript.hunger = thisthing.hunger;
        }

        FindObjectOfType<Camera>().GetComponent<CameraManager>().CamInit();
        RefreshEveryDMap();
        RenewMushroomMap();
        RenewGoblinHomeMap();
    }
}
