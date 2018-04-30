using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// All pathfinding related code here
public partial class MapGen : MonoBehaviour {

    int minvalpath = 0;
    [HideInInspector] public List<Mushroom> mushrooms;

    // Add a map goal
    public void AddMapGoal(int mapnum, Vector3 maploc)
    {
        int x = Mathf.RoundToInt(maploc[0]);
        int y = Mathf.RoundToInt(maploc[1]);
        int z = Mathf.RoundToInt(maploc[2]);
        if (x > 0 && x < xsize - 1 && y >= 0 && y < ysize && z > 0 && z < zsize - 1)
        {
            DMaps[x, y / yscale, z, mapnum] = minvalpath;
        }
    }

    // Find most important priority at location
    // Most of this functions purpose is being moved to Monster.cs
    // TODO: Refactor this out
    /*
    float SummedVal(int x, int y, int z, float[] InverseDesires)
    {
        float returnval = InverseDesires[0] * DMaps[x, y, z, 0];
        for (int i = 1; i < InverseDesires.Length; i++)
        {
            if (InverseDesires[i] * DMaps[x, y, z, i] < returnval)
            {
                returnval = InverseDesires[i] * DMaps[x, y, z, i];
            }
        }
        //Debug.Log(returnval);
        return returnval;
    }*/

    public void RollDownMono(int x, int y, int z, out int horizontal, out int vertical,int mapnum,int ix=0,int iz=0) {
        horizontal = 0;
        vertical = 0;
        int yactual = y / yscale;
        int best=10000;// = DMaps[x, y, z, mapnum];
        for (int i = -1; i < 2;i++) {
            for (int j = -1; j < 2;j++) {
                
                if ((i!=0 && j!=0) || (i==0 && j==0)) {
                    continue;
                }
                if (i==ix && j==iz) {
                    continue;
                }
                //Debug.Log("Map val:" + DMaps[x + i, yactual, z + j, mapnum] + ", best val:" + best);
                /*if (yactual>0) {
                    Debug.Log("x:" + x + " y:" + y + " z:" + z);
                }*/
                if(DMaps[x + i, yactual, z + j, mapnum]<best) {
                    best = DMaps[x + i, yactual, z + j, mapnum];
                    horizontal = i;
                    vertical = j;
                }
            }
        }
    }

/*    public void RollDown(Vector3 startpos, out int horizontal, out int vertical, int mapnum,bool cardinal=false, int ix = 0, int iz = 0) {

        float[] MonoDesire = new float[PathDists.Length];
        for (int i = 0; i < PathDists.Length;i++) {
            if (i==mapnum) {
                MonoDesire[i] = 1;
            }
            else {
                MonoDesire[i] = 100000;
            }
        }

        RollDown(startpos, out horizontal, out vertical, MonoDesire, cardinal,ix,iz);
    }*/

    // Rolldown DMap

    public void RollDown(Vector3 startpos, out int horizontal, out int vertical, int mapnum, bool cardinal = false, int ix = 0, int iz = 0)
//    public void RollDown(Vector3 startpos, out int horizontal, out int vertical, float[] InverseDesires,bool cardinal=false,int ix=0,int iz=0)
    {
        int x = Mathf.RoundToInt(startpos[0]);
        int y = Mathf.RoundToInt(startpos[1]);
        int z = Mathf.RoundToInt(startpos[2]);

        horizontal = 0;
        vertical = 0;

        int hunwise = 0;
        int vunwise = 0;

        // nope out if location is invalid
        if (x < 0 || x >= xsize || y < 0 || y >= ysize || z < 0 || z >= zsize)
        {
            return;
        }

        int yactual = y / yscale;

        //float currentnum = SummedVal(x, yactual, z, InverseDesires);//DMaps[x, yactual, z, mapnum];
        float currentnum = DMaps[x, yactual, z, mapnum];
        //Debug.Log("Currentnum=" + currentnum);
        float best = currentnum;
        float lastresort = currentnum+1;
        // Prioritize checking for stairs

        float checkthisone;
        // check neighbouring squares
        for (int ii = -1; ii < 2; ii++)
        {
            for (int jj = -1; jj < 2; jj++)
            {
                if (cardinal && ii!=0 && jj != 0) {
                    continue;
                }
                if (ii==ix && jj==iz) {
                    continue;
                }
                if (x + ii >= 0 && x + ii < xsize && z + jj >= 0 && z + jj < zsize && y >= 0 && y < ysize)
                {
                    checkthisone = DMaps[x + ii, yactual, z + jj, mapnum];//SummedVal(x + ii, yactual, z + jj, InverseDesires);
                    if (checkthisone < best && checkthisone >= 0)
                    {
                        if (yactual > 0 && (PathMap[x + ii, y, z + jj] == '>' || PathMap[x + ii, y, z + jj] == ' '))
                        {
                            //if (SummedVal(x + ii, yactual - 1, z + jj, InverseDesires) > checkthisone)
                            if (DMaps[x + ii, yactual-1, z + jj, mapnum] > checkthisone)
                            {
                                continue;
                            }
                        }

                        if (PathMap[x + ii, y, z + jj] == ' ')
                        {
                            lastresort = checkthisone;
                            hunwise = ii;
                            vunwise = jj;
                        }
                        else
                        {
                            horizontal = ii;
                            vertical = jj;
                            best = checkthisone;
                        }
                    }
                }
            }
        }
        if (lastresort<(best-0.5)) {
            best = lastresort;
            horizontal = hunwise;
            vertical = vunwise;
        }
        if (yactual < yslices)
        {
            //Debug.Log(startpos); 
            if (PathMap[x, y, z] == '<')
            {
                //if (SummedVal(x, yactual + 1, z, InverseDesires) < best)
                if (DMaps[x, yactual + 1, z, mapnum] < best)
                {
                    horizontal = 0;
                    vertical = 0;
                    Debug.Log("At stairbase");
                    //return;
                }
            }
        }
        //Debug.Log("Best=" + best);
    }

    // Pathmap type?
    public char TileType(Vector3 position, bool replace = false,char replacewith=' ') {
        int x, y, z;
        x = Mathf.RoundToInt(position[0]);
        y = Mathf.RoundToInt(position[1]);
        z = Mathf.RoundToInt(position[2]);
        return TileType(x, y, z, replace, replacewith);
    }

    public char TileType(int x, int y, int z, bool replace=false,char replacewith=' ')
    {
        if (x<0 || x>=xsize || y<0 || y>=ysize || z<0 || z>=zsize) {
            return '#';
        }
        if (replace) {
            PathMap[x, y, z] = replacewith;
        }
        return PathMap[x, y, z];
    }

    // Distance val to particular goal?
    public int DistGoal(Vector3 position, int mapnum)
    {
        int x, y, z;
        x = Mathf.RoundToInt(position[0]);
        y = Mathf.RoundToInt(position[1]);
        z = Mathf.RoundToInt(position[2]);
        return DistGoal(x, y, z, mapnum);
    }

    public int DistGoal(int x, int y, int z,int mapnum) {
        //int x, y, z;
        int yactual = y / yscale;
        if (x < 0 || x >= xsize || yactual<0 || yactual>=yslices || z <0 || z>=zsize) {
            return 1000;
        }
        else {
            if (DMaps[x, yactual, z, mapnum] < PathDists[mapnum])
            {
                return DMaps[x, yactual, z, mapnum];
            }
            else {
                return 9000;
            }
        }
    }

    // Generate DMap
    public void GenerateDMap(int mapnum, bool forceit = false, bool ignorefeatures = false, bool localized = false, int sx = 1, int sz = 1, int startat=0,int maxiterations=9000)
    {//,bool thorough=false) {
        // Only generate this is it needs to be
        if (!forceit && !PathRefreshed[mapnum])
        {
            return;
        }
        int currentnum = -1+minvalpath+startat;
        int ii = 0;
        int jj = 0;
        bool iswall = false;
        int breaker = 0;
        bool changed = true;
        int[] bounds = { 1, xsize - 1, 1, zsize - 1 }; // Not localized? Do whole map.
        if (localized) {
            bounds[0] = Mathf.Max(sx - 1,1);
            bounds[1] = Mathf.Min(sx + 1,xsize-1);
            bounds[2] = Mathf.Max(sz - 1,1);
            bounds[3] = Mathf.Min(sz + 1,zsize-1);
        }
        //char currenttile;
        //bool numcheck = false;
        while (changed && breaker < 400)
        {
            /*if (ignorefeatures) {
                Debug.Log(breaker);
            }*/
            changed = false;
            breaker++;
            currentnum++;
            if (currentnum - startat > maxiterations+minvalpath) { return; }
            if (localized) {
                if (bounds[0] > 1) { bounds[0]--; }
                if (bounds[1] < xsize-1) { bounds[1]++; }
                if (bounds[2] > 1) { bounds[2]--; }
                if (bounds[3] < zsize - 1) { bounds[3]++; }
                //Debug.Log(bounds[0]+" "+bounds[1]+" "+bounds[2]+" "+bounds[3]);
            }
            for (int i = bounds[0]; i < bounds[1]; i++)
            {
                for (int j = bounds[2]; j < bounds[3]; j++)
                {
                    for (int k = 0; k < yslices; k++)
                    {
                        /*if (!thorough) {
                            numcheck = DMaps[i, k, j, mapnum] == currentnum;
                        }
                        else {
                            numcheck = true;
                            currentnum = DMaps[i, k, j, mapnum];
                        }*/

                        if (DMaps[i, k, j, mapnum] == currentnum)
                        {
                            if (PathMap[i, k * yscale, j] == ' ' && !ignorefeatures)
                            {
                                continue;
                            }
                            iswall = true;
                            for (int iii = 0; i < yscale;i++) {
                                if (k * yscale + iii >= ysize) { break; }
                                if (PathMap[i,k*yscale+iii,j] != '#') {
                                    iswall = false;
                                    break;
                                }
                            }
                            if (PathMap[i, k * yscale, j] == '#' && !ignorefeatures)
                            {
                                DMaps[i, k, j, mapnum] = -1;
                            }
                            else
                            {
                                if (!ignorefeatures)
                                {
                                    if (PathMap[i, k * yscale, j] == '>'|| ((k - 1) > 0 && PathMap[i, k * yscale, j] == ' '))
                                    {
                                        if (DMaps[i, k - 1, j, mapnum] > currentnum + 1)
                                        {
                                            DMaps[i, k - 1, j, mapnum] = currentnum + 1;
                                            changed = true;
                                        }
                                    }
                                    if (PathMap[i, k * yscale, j] == '<' || ((k + 1) < yslices && PathMap[i, (k + 1) * yscale, j] == ' '))
                                    {
                                        if (k < yslices - 1)
                                        {
                                            if (DMaps[i, k + 1, j, mapnum] > currentnum + 1)
                                            {
                                                DMaps[i, k + 1, j, mapnum] = currentnum + 1;
                                                changed = true;
                                            }
                                        }
                                    }
                                }
                                for (int kk = 0; kk < 4; kk++)
                                {
                                    {
                                        switch (kk)
                                        {
                                            case 0:
                                                ii = -1; jj = 0; break;
                                            case 1:
                                                ii = 1; jj = 0; break;
                                            case 2:
                                                ii = 0; jj = -1; break;
                                            case 3:
                                                ii = 0; jj = 1; break;
                                        }
                                        //for (int ii = -1; ii < 2;ii++) {
                                        if (PathMap[i, k * yscale, j] == '|' && ii != 0)
                                        { // No x motion if current tile is |
                                            continue;
                                        }
                                        //for (int jj = -1; jj < 2;jj++) {
                                        if (PathMap[i, k * yscale, j] == '-' && jj != 0) // No z motion if current tile is -
                                        {
                                            continue;
                                        }

                                        if (ii == 0 && jj == 0)
                                        {
                                            continue;
                                        }

                                        // No x motion into a | tile or z motion into a - tile
                                        if ((ii != 0 && PathMap[i + ii, k * yscale, j + jj] == '|') || (jj != 0 && PathMap[i + ii, k * yscale, j + jj] == '-'))
                                        {
                                            continue;
                                        }

                                        if (DMaps[i + ii, k, j + jj, mapnum] > currentnum + 1)//Mathf.Abs(ii) + Mathf.Abs(jj))
                                        {
                                            DMaps[i + ii, k, j + jj, mapnum] = currentnum + 1;//Mathf.Abs(ii) + Mathf.Abs(jj);
                                            changed = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        PathRefreshed[mapnum] = false;
    }

    // Refresh DMap
    public void RefreshDMap(int mapnum,bool force=false)
    {
        // Only refresh the map if it needs to be

        if (!PathRefreshed[mapnum] || force)
        {
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < zsize; j++)
                {
                    for (int k = 0; k < yslices; k++)
                    {
                        DMaps[i, k, j, mapnum] = PathDists[mapnum]+minvalpath;
                    }
                }
            }
        }
        PathRefreshed[mapnum] = true;
    }

    public void RefreshEveryDMap() {
        for (int i = 0; i < NumDMaps;i++) {
            RefreshDMap(i,true);
        }
    }

    // Renew Mushroom Map
    public IEnumerator RenewMushroomMap() {
        RefreshDMap(1);
        Debug.Log("Renewed mushroom Map");
        //Vector3 position;
        foreach (Mushroom shroom in mushrooms) {
            if (shroom != null && !shroom.pickedup)
            {
                AddMapGoal(1, shroom.transform.position);
            }
        }
        //AddMapGoal(2, TargPos);
        int itersize = 120*(3600)/(xsize*zsize);
        for (int i = 0; i < PathDists[1]; i += itersize)
        {
            GenerateDMap(1,false,false,false,1,1,i,itersize);
            yield return null;
        }
        Debug.Log("Finished mushroom Map");
    }

    // Renew Monster Maps
    // 0+2==goblins & other humanoids
    // 1+2==carnivores
    // 2+2==herbivores (not implemented yet)
    public void RenewMonsterMap(List<Monster> monsterscripts)
    //public void RenewMonsterMap(int monstertype, List<Monster> monsterscripts)
    {
        int mapoffset = 3;
        //int dmapnum = monstertype + 3;
        //Debug.Log(dmapnum);
        for (int i = 0; i < 3; i++)
        {
            RefreshDMap(i+mapoffset);
        }

        foreach (Monster thismonster in monsterscripts)
        {
            /*if (thismonster.monstertype == monstertype && thismonster.alive)
            {
                AddMapGoal(dmapnum, thismonster.transform.position);
            }*/
            if (thismonster.alive)
            {
                AddMapGoal(thismonster.monstertype + mapoffset, thismonster.transform.position);
            }
        }

        foreach (Monster thismonster in monsterscripts)
        {
            if (thismonster.alive)
            {
                GenerateDMap(thismonster.monstertype + mapoffset, true, false, true,
                             Mathf.RoundToInt(thismonster.transform.position.x),
                             Mathf.RoundToInt(thismonster.transform.position.z));
            }
        }

        //GenerateDMap(dmapnum);
    }

    public void RenewNookMap()
    {
        RefreshDMap(6);
        for (int k = 0; k < yslices; k++)
        {
            for (int i = 1; i < xsize - 1; i++)
            {
                for (int j = 1; j < zsize - 1; j++)
                {
                    if (Map[i, k * yscale, j] < -1)
                    {
                        if (NumNeighbours(i, k * yscale, j) > 5)
                        {
                            //PlaceDecoration(i, k * yscale, j, 0);
                            AddMapGoal(6, new Vector3(i, k * yscale, j));
                        }
                    }
                }
            }
        }
        GenerateDMap(6);
    }

    public void RenewGoblinHomeMap()
    {
        RefreshDMap(2);
        foreach (GameObject thismon in GameObject.FindGameObjectsWithTag("Monster")) {
            Humanoid monscript = thismon.GetComponent<Humanoid>();
            if (monscript!=null) {
                AddMapGoal(2, monscript.HomeLocation);
            }
        }
        GenerateDMap(2);
    }
}
