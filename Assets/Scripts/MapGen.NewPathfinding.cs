using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
using UnityEngine;

// All pathfinding related code here - changing to A* for more flexible AI,
// BUT preserving original because it works very well for MapGen.
public partial class MapGen : MonoBehaviour
{
    // Class to store locations within A* pathfinding
    private class PathPos : IEquatable<PathPos>, IComparable<PathPos>
    {
        public int x;
        public int y;
        public int z;
        public PathPos PrevPos;
        public int cost;
        public int heuristic;
        // Constructor

        public PathPos(Vector3 StartPos,int newcost, Vector3 TargetPos, PathPos LastPos=null) {
            Initialize(Mathf.RoundToInt(StartPos.x), Mathf.RoundToInt(StartPos.y), Mathf.RoundToInt(StartPos.z), newcost, TargetPos,LastPos);
        }

        public PathPos(int Startx, int Starty, int Startz, int newcost, Vector3 TargetPos, PathPos LastPos = null)
        {
            Initialize(Startx, Starty, Startz, newcost, TargetPos,LastPos);
        }

        private void Initialize (int Startx, int Starty, int Startz, int newcost, Vector3 TargetPos, PathPos LastPos = null)
        {
            x = Startx;
            y = Starty;
            z = Startz;
            PrevPos = LastPos;
            cost = newcost;
            heuristic = Mathf.Abs(x - (int)TargetPos.x) + Mathf.Abs(y - (int)TargetPos.y) + Mathf.Abs(z - (int)TargetPos.z);
        }

        public int TotalCost() {
            return cost + heuristic;
        }

        // Equality is based on position, x y z
        public bool Equals(PathPos other)
        {
            if (other == null)
            {
                return false;
            }
            if (other.x == this.x && other.y == this.y && other.z == this.z)
            {
                return true;
            }
            return false;
        }

        // Comparison (for sorting) is based on cost
        public int CompareTo(PathPos other)
        {
            if (other == null)
            {
                return 1;
            }
            return this.TotalCost().CompareTo(other.TotalCost());
        }

        // Get Vector3
        public Vector3 GetVector3() {
            return new Vector3(x, y, z);
        }

    }

    // Expand open list, getting neighbours of NewPos
    private void AddToOpenList(List<PathPos> OpenList, List<PathPos> ClosedList, PathPos NewPos,Vector3 TargetPos) {
        int k=0;
        for (int i = -1; i < 2;i++) {
            for (int j = -1; j < 2;j++) {
                if (PathMap[i + NewPos.x, k + NewPos.y, j + NewPos.z] != '#')
                {
                    k = 0;
                    if (i==0 && j==0) {
                        if (PathMap[i + NewPos.x, k + NewPos.y, j + NewPos.z] == '>' || PathMap[i + NewPos.x, k + NewPos.y, j + NewPos.z] == ' ') {
                            k = -yscale;
                        }
                        if (PathMap[i + NewPos.x, k + NewPos.y, j + NewPos.z] == '<')
                        {
                            k = yscale;
                        }
                    }
                    PathPos AddPos = new PathPos(i+NewPos.x, k+NewPos.y, j+NewPos.z, NewPos.cost + 1, TargetPos, NewPos);
                    if (!ClosedList.Contains(AddPos))
                    {
                        if (!OpenList.Contains(AddPos))
                        {
                            OpenList.Add(AddPos);
                        }
                    }
                }
            }
        }
    }

    public List<Vector3> AStarPath(Vector3 CurrentPos,Vector3 TargetPos) {
        // List of positions along path
        List<Vector3> ReturnList=new List<Vector3>();
        // Initialize open and closed list
        List<PathPos> OpenList=new List<PathPos>();
        List<PathPos> ClosedList=new List<PathPos>();

        PathPos EndLocation = new PathPos(TargetPos, -1,TargetPos);

        // Add start position
        ClosedList.Add(new PathPos(CurrentPos, 0,TargetPos));
        AddToOpenList(OpenList, ClosedList, ClosedList[0], TargetPos);

        int loopbreaker = 0;
        bool success = false;
        // Loop while the list doesn't contain the thing, but not too many times
        while (!ClosedList.Contains(EndLocation) && loopbreaker<1000) {
            loopbreaker++;
            if (OpenList.Count > 0)
            {
                OpenList.Sort();

                ClosedList.Add(OpenList[0]);

                //Debug.Log(OpenList[0].PrevPos);

                AddToOpenList(OpenList, ClosedList, OpenList[0], TargetPos);
                if (OpenList[0] == EndLocation)
                {
                    EndLocation.PrevPos = OpenList[0].PrevPos;
                }
                else
                {
                    OpenList.RemoveAt(0);
                }
            }
            else { break; }
        }

        success = ClosedList.Contains(EndLocation);

        if (success) {
            PathPos thispos;
            thispos = OpenList[0];//EndLocation;
            int iii = 0;
            do
            {
                //Debug.Log(thispos.GetVector3());
                //Debug.Log(iii++);
                ReturnList.Add(thispos.GetVector3());
                //Instantiate(debugblock, thispos.GetVector3(), Quaternion.identity);
                thispos = thispos.PrevPos;
                if (iii>1000) {
                    break;
                }
            } while (thispos != null);
            //Debug.Log(ReturnList.Count);
            ReturnList.Reverse();
            /*for (int i = 0; i < ReturnList.Count; i++)
            {
                Debug.Log(ReturnList[i].ToString());
            }*/
            return ReturnList;
        }
        else {
            return null;
        }


    }

}
