using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MapGen : MonoBehaviour {
    public int fillpercent;
    public GameObject CeilingBlock;
    bool goblintownplaced;
    List<GameObject>[] objectsbytag;
    void MakeMap(bool skipgen=false) {
        RoomID = -1;

        //Vector3 Origin = new Vector3(1, 0, 1);
        //Vector3 RoomSize = new Vector3(1, 1, 1);
        //int nextsizex;
        //int nextsizez;
        //Origin[0] = (int)(xsize / 2);//Random.Range(maxroomsize, xsize - 1 - maxroomsize);
        //Origin[2] = (int)(zsize / 2);//Random.Range(maxroomsize, zsize - 1 - maxroomsize);
        //RoomSize[0] = Random.Range(minroomsize, maxroomsize);
        //RoomSize[2] = Random.Range(minroomsize, maxroomsize);
        int breaker;
        int breaker2;
        int maxbreaker;
        bool success;
        goblintownplaced = false;
        //bool gothome;
        int roomsizex, roomsizey, roomsizez;
        //=false;
        int x, y, z;
        int tx, tz;
        int dx, dz;
        if (!skipgen)
        {
            bool[] builtonlevelyet = new bool[yslices];
            for (int i = 0; i < yslices; i++) { builtonlevelyet[i] = false; }
            int numtobuild = (xsize * zsize * fillpercent * yslices) / 100;
            y = 0;

            // Use DMap(0) for hallway drawing
            int rememberpathdist = PathDists[0];
            PathDists[0] = 400;
            RefreshDMap(0, true);
            maxbreaker = -1;
            int roomtype = 0;
            // Add rooms
            //for (int i = 0; i < numrooms;i++) {
            while (maxbreaker < 100 && numtobuild > 0)
            {
                //RoomID++;
                maxbreaker++;
                breaker = 0;
                success = false;
                while (!success && breaker < 30)
                {
                    breaker++;
                    x = Random.Range(1, xsize - maxroomsize - 1);
                    y = Random.Range(0, yslices);
                    if (y == yslices) { y--; }
                    y *= yscale;
                    z = Random.Range(1, zsize - maxroomsize - 1);
                    //roomsizex = Random.Range(minroomsize, maxroomsize);
                    //roomsizez = Random.Range(minroomsize, maxroomsize);
                    roomtype = ChooseRoom(numtobuild, out roomsizex, out roomsizey, out roomsizez, y);
                    if ((x + roomsizex) > xsize) { x = xsize - 1 - roomsizex; }
                    if ((z + roomsizez) > zsize) { z = xsize - 1 - roomsizez; }
                    if (IsSpaceEmpty(x, roomsizex, z, roomsizez, y, 1))
                    {
                        for (int i = 0; i < roomsizey; i++)
                        {
                            if (builtonlevelyet[i + y / yscale])
                            {
                                tx = x + roomsizex / 2;
                                tz = z + roomsizez / 2;
                                dx = 0;
                                dz = 0;
                                int[] addbackladder = new int[4];
                                breaker2 = 0;
                                // Add hallway before drawing room
                                // generate pathfinding. Don't refresh yet
                                GenerateDMap(0, true, true);
                                RoomID++;
                                //gothome = false;
                                do
                                {
                                    DrawHallway(tx, y + i * yscale, tz, hallmaterial, 2, 0);
                                    AddMapGoal(0, new Vector3(tx, y + i * yscale, tz));
                                    //RollDown(new Vector3(tx, y, tz), out dx, out dz, 0, true,-dx,-dz);
                                    RollDownMono(tx, y + i * yscale, tz, out dx, out dz, 0, -dx, -dz);
                                    tx += dx;
                                    tz += dz;

                                    if (tx == x || tx == (x + roomsizex) || tz == z || tz == (z + roomsizez))
                                    {
                                        addbackladder[0] = tx;
                                        addbackladder[1] = tz;
                                        addbackladder[2] = -dx;
                                        addbackladder[3] = -dz;
                                    }

                                    //Debug.Log(dx);
                                    //Debug.Log(dz);
                                    breaker2++;
                                    numtobuild -= 3;
                                } while (DistGoal(new Vector3(tx, y + i * yscale, tz), 0) > minvalpath && breaker2 < 100);
                                DrawHallway(tx, y + i * yscale, tz, hallmaterial, 2, 0);
                                numtobuild -= 3;
                                AddMapGoal(0, new Vector3(tx, y + i * yscale, tz));
                                // Walk to ladderpositions
                                FindDropAddLadder(addbackladder[0], y + i * yscale, addbackladder[1], addbackladder[2], addbackladder[3]);
                                FindDropAddLadder(tx, y + i * yscale, tz, dx, dz);
                            }

                            builtonlevelyet[i + y / yscale] = true;
                        }
                        builtonlevelyet[y / yscale] = true;
                        BuildRoomType(x, roomsizex, z, roomsizez, y, hallmaterial, roomtype, false, 0, true);
                        /*RoomTagBounds.Add(new int[6]);
                        RoomTagBounds[RoomTagBounds.Count - 1][0] = x;
                        RoomTagBounds[RoomTagBounds.Count - 1][1] = roomsizex;
                        RoomTagBounds[RoomTagBounds.Count - 1][2] = y;
                        RoomTagBounds[RoomTagBounds.Count - 1][3] = roomsizey;
                        RoomTagBounds[RoomTagBounds.Count - 1][4] = z;
                        RoomTagBounds[RoomTagBounds.Count - 1][5] = roomsizez;*/
                        //BuildRoom(x, roomsizex, z, roomsizez, y, hallmaterial, false, 0,true);
                        numtobuild -= (roomsizex - 1) * (roomsizez - 1);
                        success = true;
                    }
                }
            }
            //Debug.Log(DMaps);
            PathDists[0] = rememberpathdist;

            // Find places to put decorations
            AddDecorations();

        }
        GameObject newblock;

        // Fill in blocks
        for (int i = 0; i < xsize; i++)
        {
            for (int j = 0; j < zsize; j++)
            {
                for (int k = 0; k < ysize; k++)
                {
                    //Debug.Log(k);
                    if (Map[i, k, j] > 0)
                    { // Build walls
                        newblock=Instantiate(blocks[Map[i, k, j]], new Vector3(i, k, j), Quaternion.identity);
                        newblock.GetComponent<BlockProperties>().SetRoomID(RoomTags[i, k, j]);
                    }
                    else if (Map[i, k, j] < -1)
                    { // Build floors and add a wall underneath each floor
                        newblock=Instantiate(floors[-(Map[i, k, j] + 2)], new Vector3(i, k - 0.45f, j), Quaternion.identity);
                        newblock.GetComponent<BlockProperties>().SetRoomID(RoomTags[i, k, j]);
                        if (k == 0 || Map[i, k - 1, j] < 1)
                        {
                            newblock=Instantiate(blocks[-Map[i, k, j] - 2], new Vector3(i, k - 1, j), Quaternion.identity);
                            newblock.GetComponent<BlockProperties>().SetRoomID(RoomTags[i, k, j]);
                        }
                        if (k > 0)
                        {
                            RoomTags[i, k - 1, j] = RoomTags[i, k, j];
                        }
                    }
                    else if (k > 0)
                    //else if(k % yscale != 0)
                    { // Expand walls up to fill a slice
                        if (Map[i, k - 1, j] > 0)
                        {
                            newblock=Instantiate(blocks[Map[i, k - 1, j]], new Vector3(i, k, j), Quaternion.identity);
                            RoomTags[i, k, j] = RoomTags[i, k - 1, j];
                            newblock.GetComponent<BlockProperties>().SetRoomID(RoomTags[i, k, j]);
                            Map[i, k, j] = Map[i, k - 1, j];
                        }
                    }
                    /*if (k>0 && RoomTags[i,k,j]<0) {
                        RoomTags[i, k, j] = RoomTags[i, k - 1, j];
                    }*/
                }
                Instantiate(CeilingBlock, new Vector3(i, ysize, j), Quaternion.identity);
            }
        }

        // Fill in path-map
        for (int k = 0; k < ysize; k++)
        {
            for (int i = 0; i < xsize; i++)
            {
                for (int j = 0; j < zsize; j++)
                {
                    if (Map[i, k, j] < -1)
                    {
                        PathMap[i, k, j] = '.';
                    }
                    else if (Map[i, k, j] > 0)
                    {
                        PathMap[i, k, j] = '#';
                    }
                }
            }
        }

        // shrooms
        if (!skipgen)
        {
            int numpatches = (xsize * zsize) / (600);
            for (int i = 0; i < numpatches; i++)
            {
                MushroomPatch();
            }
        }
        //GenerateDMap(1);
        // Generate goblinhome map
        GenerateDMap(2);

        // Generate map of nooks (vegans love nooks)
        RenewNookMap();

        player.transform.position = PlaceEntity();
        /*for (int i = 0; i < 16;i++) {
            PlaceMonster();
        }*/
        AttemptBatching();
        /*objectsbytag = new List<GameObject>[RoomID + 1];
        for (int i = 0; i <= RoomID;i++) {
            objectsbytag[i] = new List<GameObject>();
        }
        GameObject[] allobjects = GameObject.FindGameObjectsWithTag("Bl");*/
        RenewMushroomMap();

        if (!skipgen)
        {
            while (vegansadded < NumVegans)
            {
                vegansadded++;
                PlaceMonster(-1, -1, -1, 2);
            }
            while (beastsadded < NumBeasts)
            {
                beastsadded++;
                PlaceMonster(-1, -1, -1, 1);
            }
        }
        //SetRoomTagBounds();
    }

    void AttemptBatching() {
        GameObject[] blockbatch = GameObject.FindGameObjectsWithTag("Blocks");
        GameObject[] floorbatch = GameObject.FindGameObjectsWithTag("Floor");
        StaticBatchingUtility.Combine(blockbatch, gameObject);
        StaticBatchingUtility.Combine(floorbatch, gameObject);
    }

    void AddMapFeature(Vector3 position, int[] feature,bool forceit=false) {
        int x, y, z;
        x = Mathf.RoundToInt(position[0]);
        y = Mathf.RoundToInt(position[1]);
        z = Mathf.RoundToInt(position[2]);
        AddMapFeature(x,y,z,feature,forceit);
    }

    // Accept an array of material feature[] and built it from the ground up
    // use for doors or windows & shit
    void AddMapFeature(int x, int y, int z,int [] feature, bool forceit=false) {
        for (int i = 0; i < feature.Length;i++) {
            PlaceWall(x, y + i, z, feature[i], RoomID, forceit);
        }
    }

    // Walk in direction, add a ladder when a drop is encountered
    void FindDropAddLadder(int x, int y, int z, int dx, int dz,int maxsteps=10) {
        int cx=x; int cz = z;
        int scratch;
        //int yactual = y / yscale;
        if (y<yscale) {
            return;
        }
        for (int i = 0; i < maxsteps;i++) {
            if (i > 0)
            {
                cx += dx;
                cz += dz;
            }
            if (cx<1 || cx > xsize-2 || cz<1 || cz > zsize-2) {
                return;
            }
            if (Map[cx,y,cz]>0) {
                scratch = dx;
                dx = -dz;
                dz = dx;
            }
            if (PathMap[cx, y, cz] == '>') { return; }
            // Found empty space
            if (Map[cx,y,cz]==0) {
                // Floor below empty space?
                if (Map[cx, y - yscale, cz]<-1) {
                    // Add ladder
                    AddLadder(cx, cz, y - yscale, y, new Vector3(-dx, 0, -dz));
                    return;
                }
            }
        }
    }

    void DrawHallway(int x, int y, int z, int BuildMaterial,int thickness, int checkmapnum=-1) {
        bool drawfloor;
        for (int i = -1; i < 1+thickness;i++) {
            for (int j = -1; j < 1+thickness;j++) {
                if (i >= 0 && i < thickness && j >= 0 && j < thickness)
                {
                    drawfloor = true;
                    if (checkmapnum>-1) {
                        drawfloor &= !(DistGoal(i, y, j, checkmapnum) == minvalpath);
                    }
                    if (drawfloor)
                    {
                        PlaceWall(x + i, y, z + j, -BuildMaterial, RoomID, true);
                    }
                }
                else
                {
                    PlaceWall(x + i, y, z + j, BuildMaterial, RoomID);
                }
            }
        }
    }

    // Check that space is empty
    bool IsSpaceEmpty(int sx, int ex, int sz, int ez, int y, int ey,int emptyval=-1)
    {
        bool toreturn = true;
        /*if (emptyval != -1)
        {
            Debug.Log("sx :"+sx+","+ex+" sy :"+y+","+ey+" sz :"+sz+","+ez);
        }*/
        for (int i = sx; i < (sx + ex); i++)
        {
            for (int j = sz; j < (sz + ez); j++)
            {
                for (int k = y; k < (y + ey*yscale); k++)
                {
                    if (i<0 || i>=xsize || j<0 || j>=zsize) {
                        toreturn = false;
                        break;
                    }
                    /*if (emptyval != -1) {
                        Debug.Log("emptyval: " + emptyval + " mapval: " + Map[i, k, j]+" x,y,z="+i+" "+k+" "+j);
                    }*/
                    if (k == y)
                    {
                        toreturn &= (Map[i, k, j] == emptyval);
                    }
                    else{
                        toreturn &= (Map[i, k, j] == -1);
                    }
                }
            }
            // Don't bother with another loop if false
            if (!toreturn)
            {
                break;
            }
        }

        return toreturn;
    }

    // Select a room type and return sizing details
    int ChooseRoom(int tilesleft,out int ex, out int ey, out int ez, int yloc) {
        ex = 0;
        ey = 0;
        ez = 0;
        int toreturn;
        // 2 floor ladder room
        if (yloc==0 && Random.Range(0,10)>7) {
            ex = Random.Range(minroomsize+1, maxroomsize);
            ez = Random.Range(minroomsize+1, maxroomsize);
            ey = 2;
            toreturn = 1;
        }
        // big room (Goblin Towne)
        else if (!goblintownplaced && yloc==0 && tilesleft>(16*maxroomsize*maxroomsize) && Random.Range(0,10)>0) {
            ex = Random.Range(4 * maxroomsize, 6 * maxroomsize);
            ez = Random.Range(4 * maxroomsize, 6 * maxroomsize);
            ey = yslices;//Mathf.Max(2,yslices);
            toreturn = 3;
        }
        // regular cave
        else if (tilesleft>(8*maxroomsize*maxroomsize) && Random.Range(0,10)>5) {
            ex = Random.Range(minroomsize + 1, 2*maxroomsize);
            ez = Random.Range(minroomsize + 1, 2*maxroomsize);
            ey = 1;
            toreturn = 2;
        }
        // Basic rectangle room
        else {
            ex = Random.Range(minroomsize, maxroomsize);
            ez = Random.Range(minroomsize, maxroomsize);
            ey = 1;
            toreturn = 0;
        }
        return toreturn;
    }

    void BuildRoomType(int sx, int ex, int sz, int ez, int y, int BuildMaterial, int roomtype, bool includedoor = true, int mapgoal = -1, bool forceit = false)
    {
        RoomID++;
        switch(roomtype) {
            case 3:
                GoblinTowne(sx, ex, sz, ez, mapgoal);
                break;
            case 2:
                Cavern(sx, ex, sz, ez, y, mapgoal);
                break;
            case 1:
                BuildTwoFloor(sx, ex, sz, ez, y, BuildMaterial, mapgoal);
                break;
            case 0:
            default:
                BuildRoom(sx, ex, sz, ez, y, BuildMaterial, includedoor, mapgoal, forceit);
                break;
        }
    }

    // Build a 2 floor room with a ledge on the top section
    void BuildTwoFloor(int sx, int ex, int sz, int ez, int y, int BuildMaterial,int mapgoal=-1) {
        BuildRoom(sx+1,ex-2,sz+1,ez-2,y,BuildMaterial,false,mapgoal,true);
        BuildRoom(sx,ex,sz,ez,y+yscale,BuildMaterial,false,mapgoal,true,true);

        /*BuildRoom(sx, ex, sz, ez, y + yscale, BuildMaterial, false, mapgoal, true);
        RemoveFloors(sx+1, ex-2, sz+1, ez-2, y, 2);*/

        AddLadder(sx+2,sz+ez/2,y,y+yscale,Vector3.left);
        AddLadder(sx + ex-3, sz + ez / 2, y, y + yscale, Vector3.right);
        AddLadder(sx + ex/2, sz+2, y, y + yscale, Vector3.back);
        AddLadder(sx + ex/2, sz + ez-3, y, y + yscale, Vector3.forward);
    }

    // Build a basic rectangular room
    void BuildRoom(int sx, int ex, int sz, int ez, int y, int BuildMaterial, bool includedoor = true, int mapgoal = -1, bool forceit = false, bool hollow = false, bool roof = false,int roofmaterial=-1,bool addwindows=false,int roofid=-1)
    {
        //RoomID++;
        if (roofmaterial<0) {
            roofmaterial = BuildMaterial;
        }
        if (roofid==-1) {
            roofid = RoomID;
        }
        int doorcount = 0;
        int doorpos = Random.Range(0, 2*(ex+ez-4));
        bool placewall = false;
        bool placefloor = true;
        for (int i = sx; i < (sx + ex); i++)
        {
            for (int j = sz; j < (sz + ez); j++)
            {
                if (roof) {
                    PlaceWall(i, y + yscale, j, -roofmaterial, roofid,true);
                }
                placewall = false;
                placefloor = true;
                if (i == sx || j == sz || i == (sx + ex - 1) || j == (sz + ez - 1))
                {
                    placewall = true;
                    placefloor = false;
                    if (!((i == sx && j == sz) || (i == sx && j == (sz + ez - 1)) || (j == sz && i == (sx + ex - 1)) || (i == (sx + ex - 1) && j == (sz + ez - 1))))
                    {
                        if (includedoor)
                        {
                            if (doorcount == doorpos)
                            {
                                int[] door = { -BuildMaterial, 0, BuildMaterial };
                                placewall = false;
                                AddMapFeature(i,y,j,door,true);
                            }
                            doorcount++;
                        }
                    }
                    if (addwindows && placewall && ((i==sx+ex/2) || (j==sz+ez/2))) {
                        placewall = false;
                        int[] window = { BuildMaterial, -BuildMaterial, BuildMaterial };
                        Instantiate(decorations[1],new Vector3(i, y + 1, j),Quaternion.identity);
                        AddMapFeature(i, y, j, window, true);
                    }
                }
                if (placewall)
                {
                    PlaceWall(i, y, j, BuildMaterial, RoomID,includedoor && forceit);
                }
                else if (placefloor)
                {
                    if (mapgoal > -1)
                    {
                        AddMapGoal(mapgoal, new Vector3(i, y, j));
                    }
                    if (hollow && i > sx + 1 && j > sz + 1 && i < (sx + ex - 2) && j < (sz + ez - 2))
                    {
                        PlaceWall(i, y, j, 0, RoomID, forceit);
                        //continue;
                    }
                    else
                    {
                        PlaceWall(i, y, j, -BuildMaterial, RoomID, forceit);
                    }

                }
            }
        }
    }

    bool GoblinHouse(int sx, int sy, int sz, int BuildMaterial,int mainroomid,int goblinhomesize=4) {
        //int goblinhomesize = 5;
        bool success = IsSpaceEmpty(sx-1, 2+goblinhomesize, sz-1, 2+goblinhomesize, sy, 1, -5);

        if (success) {
            RoomID++;
            //Debug.Log("success?");
            BuildRoom(sx, goblinhomesize, sz, goblinhomesize, sy, BuildMaterial,true,2,true,false,true,2,true,mainroomid);
            for (int i = 1; i < (goblinhomesize - 1);i++) {
                for (int j = 1; j < (goblinhomesize - 1); j++)
                {
                    if (goblinsadded < NumGoblins)
                    {
                        goblinsadded++;
                        PlaceMonster(sx+i, sy, sz+j, 0);
                    }
                }
            }
        }

        return success;
    }

    // Goblin Towne
    void GoblinTowne(int sx, int ex, int sz, int ez,int mapgoal=-1) {
        Debug.Log("Goblin Towne");
        for (int k = 0; k < yslices;k++) {
            Cavern(sx, ex, sz, ez, k * yscale,mapgoal);
        }
        RemoveFloors(sx, ex, sz, ez, 0, yslices);
        int numhouses = 10;
        int breaker = 0;
        int x, y, z;
        int mainroomid = RoomID;
        int housesize = 6;
        while (breaker<100 && numhouses>0) {
            breaker++;
            x = sx+Random.Range(1, ex - 6);
            z = sz + Random.Range(1, ez - 6);
            y = Random.Range(0,yslices)*yscale;
            if (y == yslices) { y--; }
            if (GoblinHouse(x,y,z,1,mainroomid,housesize)) {
                numhouses--;
                if (housesize > 4) { housesize--; }
            }
        }
        while (goblinsadded < NumGoblins)
        {
            goblinsadded++;
            PlaceMonster(-1, -1, -1, 0);
        }
        goblintownplaced = true;
    }

    // Remove floors when a floor below exists
    void RemoveFloors(int sx, int ex, int sz, int ez, int sy, int slices) {
        for (int k = 1; k < slices;k++) {
            for (int i = sx; i < (sx + ex);i++) {
                for (int j = sz; j < (sz + ez);j++) {
                    // Floor exists on a layer below, insert void
                    if (Map[i,(k-1)*yscale,j]<-1) {
                        PlaceWall(i, k * yscale, j, 0, RoomID, true);
                    }
                }
            }
        }
    }

    // Cavern generated with a random walk method. Always rock.
    void Cavern(int sx, int ex, int sz, int ez, int y, int mapgoal = -1) {
        int midx = sx + ex / 2;
        int midz = sz + ez / 2;
        int x = midx;
        int z = midz;
        int fillfactor = 50;
        int dx=0, dz=0;
        int tilesleft = (ex * ez * fillfactor)/100;
        int breaker = 0;
        int maxsteps = ex * ez * 10;
        while (breaker<maxsteps && tilesleft>0) {
            breaker++;
            x += dx;
            z += dz;
            if (x<=sx+1 || x>=(sx+ex-1) || z<=sz+1 || z>=(sz+ez-1)) {
                x = midx;
                z = midz;
            }
            if (Map[x,y,z]!=-5) {
                DrawHallway(x, y, z, 3, 1);
                tilesleft--;
                if (mapgoal>=0) {
                    AddMapGoal(mapgoal, new Vector3(x, y, z));
                }
            }
            switch (Random.Range(0,4)) {
                case 0:
                    dx = 0;
                    dz = 1;
                    break;
                case 1:
                    dx = 0;
                    dz = -1;
                    break;
                case 2:
                    dx = 1;
                    dz = 0;
                    break;
                default:
                    dx = -1;
                    dz = 0;
                    break;
            }
        }
    }
}
