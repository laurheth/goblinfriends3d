using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveMap {

    /*
     * Important info to store:
     * Map[,,]
     * PathMap[,,]
     * public int xsize;
     * public int ysize;
     * public int zsize;
     * yscale
     * level
     * yslices
     * Then, the full list of objects, including decorations, ladders, monsters, items/mushrooms, etc
     * And their attributes
     * Monsters will need additional info, i.e. attitude, HP, etc
     * Chests will need whether or not they have been opened
     */
    public int[] Map;
    public char [] PathMap;
    public int xsize;
    public int ysize;
    public int zsize;
    public int yscale;
    public int level;
    public int yslices;
    public List<Thing> ThingList;
    public List<MonsterThing> MonsterThingList;

    [System.Serializable]
    public struct Thing {
        public string name; // Name of the thing
        public string category; // What type? i.e. map feature, plant, item, etc
        public Vector3 position; 
        public Quaternion rotation;
        public bool status; // Alive/dead, open/closed, etc, as appropriate for the entity in question
        public float field1; // colorind, etc
        public float field2; // logsize,etc
    }

    [System.Serializable]
    public struct MonsterThing {
        public string name; // Name of the monster
        public int category; // What type? 0=humanoid, 1=beast, 2=vegan
        public Vector3 position;
        public Vector3 homeposition;
        public Quaternion rotation;
        public bool status; // Alive/dead, open/closed, etc, as appropriate for the entity in question
        public int tiredness;
        public int bravery;
        //int tiredthresh;
        public int anger;
        public int fear;
        public int hunger;
    }



    public SaveMap() {
        ThingList = new List<Thing>();
        MonsterThingList = new List<MonsterThing>();
    }

    public void AddThing(GameObject thing) {
        Thing thisthing = new Thing();
        thisthing.name = thing.name;
        thisthing.category = thing.tag;
        thisthing.position = thing.transform.position;
        thisthing.rotation = thing.transform.rotation;
        thisthing.status = false;
        thisthing.field1 = 0f;
        thisthing.field2 = 0f;
        if (thing.tag=="Plant") {
            thisthing.status = thing.GetComponent<Mushroom>().IsAlive();
        }
        if (thing.tag=="Ladder") {
            thisthing.status = thing.GetComponent<Ladder>().isbottom;
        }
        if (thing.tag=="Items" || thing.tag=="Plant") {
            float[] vals = thing.GetComponent<Item>().GetAttributes();
            thisthing.field1 = vals[0];
            thisthing.field2 = vals[1];
        }
        ThingList.Add(thisthing);
    }

    public void AddMonsterThing(GameObject thing) {
        Monster monsterscript = thing.GetComponent<Monster>();
        MonsterThing thisthing = new MonsterThing();
        thisthing.name = thing.name;
        thisthing.category = thing.GetComponent<Monster>().monstertype;
        thisthing.position = thing.transform.position;
        thisthing.homeposition = monsterscript.HomeLocation;
        thisthing.rotation = thing.transform.rotation;
        thisthing.status = monsterscript.alive;
        thisthing.anger = monsterscript.anger;
        thisthing.tiredness = monsterscript.tiredness;
        thisthing.bravery = monsterscript.bravery;
        thisthing.fear = monsterscript.fear;
        thisthing.hunger = monsterscript.hunger;
        MonsterThingList.Add(thisthing);
    }

}
