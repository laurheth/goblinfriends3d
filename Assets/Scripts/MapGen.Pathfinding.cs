using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// All pathfinding related code here
public partial class MapGen : MonoBehaviour {

    int minvalpath = 1;
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
    }

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

    public void RollDown(Vector3 startpos, out int horizontal, out int vertical, int mapnum,bool cardinal=false, int ix = 0, int iz = 0) {
        //horizontal = 0;
        //vertical = 0;
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
    }

    // Rolldown DMap
    // This needs to be heavily refactored
    public void RollDown(Vector3 startpos, out int horizontal, out int vertical, float[] InverseDesires,bool cardinal=false,int ix=0,int iz=0)
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

        float currentnum = SummedVal(x, yactual, z, InverseDesires);//DMaps[x, yactual, z, mapnum];
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
                    checkthisone = SummedVal(x + ii, yactual, z + jj, InverseDesires);
                    if (checkthisone < best && checkthisone >= 0)
                    {
                        if (yactual > 0 && (PathMap[x + ii, y, z + jj] == '>' || PathMap[x + ii, y, z + jj] == ' '))
                        {
                            if (SummedVal(x + ii, yactual - 1, z + jj, InverseDesires) > checkthisone)
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
                if (SummedVal(x, yactual + 1, z, InverseDesires) < best)
                //if (DMaps[x, yactual + 1, z, mapnum] < currentnum)
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
    public char TileType(Vector3 position, bool replace = false,char replacewith=' ')
    {
        int x, y, z;
        x = Mathf.RoundToInt(position[0]);
        y = Mathf.RoundToInt(position[1]);
        z = Mathf.RoundToInt(position[2]);
        if (x<0 || x>=xsize || y<0 || y>=ysize || z<0 || z>=zsize) {
            return ' ';
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
            return DMaps[x, yactual, z, mapnum];
        }
    }

    // Generate DMap
    public void GenerateDMap(int mapnum,bool forceit=false,bool ignorefeatures=false)
    {//,bool thorough=false) {
        // Only generate this is it needs to be
        if (!forceit && !PathRefreshed[mapnum])
        {
            return;
        }
        int currentnum = -1+minvalpath;
        int ii = 0;
        int jj = 0;
        int breaker = 0;
        bool changed = true;
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
            for (int i = 1; i < xsize-1; i++)
            {
                for (int j = 1; j < zsize-1; j++)
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
                            if (PathMap[i, k * yscale, j] == '#' && !ignorefeatures)
                            {
                                DMaps[i, k, j, mapnum] = -1;
                            }
                            else
                            {
                                if (!ignorefeatures)
                                {
                                    if (PathMap[i, k * yscale, j] == '>')
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

    // Renew Mushroom Map
    public void RenewMushroomMap() {
        RefreshDMap(1);
        /*
        for (int i = 0; i < xsize;i++) {
            for (int j = 0; j < zsize;j++) {
                for (int k = 0; k < ysize;k++) {
                    if (TileType(new Vector3(i,k,j))=='M') {
                        AddMapGoal(1, new Vector3(i, k, j));
                    }
                }
            }
        }*/
        //Vector3 position;
        foreach (Mushroom shroom in mushrooms) {
            if (shroom != null && !shroom.pickedup)
            {
                AddMapGoal(1, shroom.gameObject.transform.position);
            }
        }
        //AddMapGoal(2, TargPos);

        GenerateDMap(1);
    }

}
