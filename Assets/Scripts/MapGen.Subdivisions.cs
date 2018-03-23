using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All subdivisions for map generation go here
public partial class MapGen : MonoBehaviour {

    // Generate map subdivision
    // In principal, subdivision type can be swapped out for different generation methods
    // Starting basic though
    void GenerateSpace(int xorigin, int yorigin, int zorigin, int BuildMaterial, int choice)
    {
        switch (choice)
        {
            case 0:
                RoomVoid(xorigin, yorigin, zorigin, BuildMaterial);
                break;
            case 1:
                GazeboVoid(xorigin, yorigin, zorigin, BuildMaterial);
                break;
            case -1:
                GoblinTown(xorigin, yorigin, zorigin, BuildMaterial);
                break;
            default:
                RoomMaze(xorigin, yorigin, zorigin, BuildMaterial);
                break;

        }
    }

    // Cage-like Gazebo things with space in between; drops feet to lower levels
    void GazeboVoid(int xorigin, int yorigin, int zorigin, int BuildMaterial)
    {

        int placex;
        int placez;
        int gazebox;
        int gazeboz;

        for (int num = 0; num < 3; num++)
        {
            placex = Random.Range(maxroomsize, xsub - maxroomsize);
            placez = Random.Range(maxroomsize, zsub - maxroomsize);
            gazebox = Random.Range(minroomsize / 2, maxroomsize / 2);
            gazeboz = Random.Range(minroomsize / 2, maxroomsize / 2);
            for (int i = 0; i < 2 * gazebox; i++)
            {
                for (int j = 0; j < 2 * gazeboz; j++)
                {
                    if ((i == 0 && j == 0) || (i == 0 && j == (2 * gazeboz - 1)) || (j == 0 && i == (2 * gazebox - 1)) || (i == (2 * gazebox - 1) && j == (2 * gazeboz - 1)))
                    {
                        //PlaceWall(i+xorigin+placex-gazebox, yorigin, j+ zorigin + placez - gazeboz, BuildMaterial, RoomID);
                        DropPillar(i + xorigin + placex - gazebox, yorigin - yscale, j + zorigin + placez - gazeboz, yorigin, BuildMaterial);
                    }
                    else
                    {
                        PlaceWall(i + xorigin + placex - gazebox, yorigin, j + zorigin + placez - gazeboz, -BuildMaterial, RoomID);
                    }
                    if (i == 0 && j == gazeboz)
                    {
                        AddLadder(i + xorigin + placex - gazebox, j + zorigin + placez - gazeboz, yorigin - yscale, yorigin, Vector3.right);
                    }
                }
            }
        }

        RoomID++;
        for (int i = xorigin; i < (xorigin + xsub); i++)
        {
            for (int j = zorigin; j < (zorigin + zsub); j++)
            {
                if (i == xorigin || i == (xorigin + xsub - 1) || j == zorigin || j == (zorigin + zsub - 1))
                {
                    PlaceWall(i, yorigin, j, BuildMaterial, RoomID);
                }
            }
        }
    }

    // Large Open Space
    void RoomVoid(int xorigin, int yorigin, int zorigin, int BuildMaterial)
    {

        //PlaceWall(sx, yorigin, sz, -BuildMaterial, RoomID);
        // Interesting walls have been done, now to fill in edge & floors
        FillGaps(xorigin, yorigin, zorigin, BuildMaterial);
    }



    // Fill in remaining space with floors and wall
    void FillGaps(int xorigin,int yorigin, int zorigin, int BuildMaterial) {
        // Interesting walls have been done, now to fill in edge & floors
        RoomID++;
        for (int i = xorigin; i < (xorigin + xsub); i++)
        {
            for (int j = zorigin; j < (zorigin + zsub); j++)
            {
                if (i == xorigin || i == (xorigin + xsub - 1) || j == zorigin || j == (zorigin + zsub - 1))
                {
                    PlaceWall(i, yorigin, j, BuildMaterial, RoomID);
                }
                else
                {
                    PlaceWall(i, yorigin, j, -BuildMaterial, RoomID);
                }
            }
        }

    }

    // Goblin towne
    void GoblinTown(int xorigin, int yorigin, int zorigin, int BuildMaterial) {
        int numbuilt = 0;
        int breaker = 0;
        int sx, ex, sz, ez;
        while (numbuilt<numrooms && breaker<numrooms*50) {
            sx = Random.Range(xorigin + 2, xorigin + xsub - maxroomsize - 3);
            sz = Random.Range(zorigin + 2, zorigin + zsub - maxroomsize - 3);
            ex = Random.Range(minroomsize+1, maxroomsize);
            ez = Random.Range(minroomsize+1, maxroomsize);
            if (IsSpaceEmpty(sx-1,ex+2,sz-1,ez+2,yorigin,1)) {
                BuildRoom(sx, ex, sz, ez, yorigin, goblintownmaterial,true,2);
                numbuilt++;
            }
            breaker++;
        }
        // Interesting walls have been done, now to fill in edge & floors
        FillGaps(xorigin, yorigin, zorigin, BuildMaterial);
        for (int i = 0; i < NumGoblins;i++) {
            PlaceMonster(Random.Range(xorigin + 2, xorigin + xsub - 3), yorigin, Random.Range(zorigin + 2, zorigin + zsub - 3), 0);
        }
    }

    // Room maze (one I made first)
    void RoomMaze(int xorigin, int yorigin, int zorigin, int BuildMaterial)
    {
        //Debug.Log("xorigin=" + xorigin);
        //Debug.Log("yorigin=" + yorigin);
        //Debug.Log("zorigin=" + zorigin);
        int sx;
        int sz;
        int dx;
        int k;
        int dz = 0;
        int placex;
        int placez;
        int scratch = 0;
        dx = Random.Range(0, 2);
        if (dx > 1)
        {
            dx = 1;
        }
        if (dx == 0)
        {
            dz = 1;
        }
        int breaker = 0;
        for (int ii = 0; ii < numrooms; ii++)
        {
            // Find a place to start / where the door goes
            do
            {
                sx = Random.Range(xorigin + minroomsize, xorigin + xsub - minroomsize);
                sz = Random.Range(zorigin + minroomsize, zorigin + zsub - minroomsize);
                if ((sx + dx) % 2 == 0)
                {
                    sx++;
                }
                if ((sz + dz) % 2 == 0)
                {
                    sz++;
                }
                breaker++;
            } while (Map[sx, yorigin, sz] != -1 || breaker < 10000);

            RoomID++;
            PlaceWall(sx, yorigin, sz, -BuildMaterial, RoomID);


            // Switch from previous orientation
            scratch = dx;
            dx = dz;
            dz = scratch;

            for (int jj = -1; jj < 2; jj += 2)
            {
                k = 0;
                do
                {
                    k++;
                    placex = sx + dx * jj * k;
                    placez = sz + dz * jj * k;
                    //Debug.Log(xorigin+xsub);
                    //Debug.Log(zorigin+zsub);
                    //Debug.Log("placex=" + placex);
                    //Debug.Log("placez=" + placez);
                    //Debug.Log("yorigin=" + yorigin);
                } while (PlaceWall(placex, yorigin, placez, BuildMaterial, RoomID) && placex > xorigin && placez > zorigin && placex < (xorigin + xsub) && placez < (zorigin + zsub));
            }

        }


        // Interesting walls have been done, now to fill in edge & floors
        FillGaps(xorigin, yorigin, zorigin, BuildMaterial);
    }


}
