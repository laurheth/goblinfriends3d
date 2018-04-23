using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairCase : MonoBehaviour {

    //public float Height;
    //private float targypos;
    //private float adjustrate;
    // Use this for initialization
    private float thisx;
    private float thisz;

    private float[,] neighbourheight;

	void Start () {
        thisx = transform.position.x;
        thisz = transform.position.z;
        neighbourheight = new float[3, 3];
        for (int i = 0; i < 3;i++) {
            for (int j = 0; j < 3;j++) {
                neighbourheight[i, j] = 0f;
            }
        }
	}

	private void OnTriggerEnter(Collider other)
	{
        thisx = transform.position.x;
        thisz = transform.position.z;
        ReCalcNeighbours();
	}

	private void OnTriggerStay(Collider other)
	{
        Unit otherunit = other.gameObject.GetComponent<Unit>();
        if (otherunit!=null && other.gameObject.GetComponent<Rigidbody>().isKinematic) {
            Vector3 newposition = other.transform.position;
            newposition[1] = GetHeight(newposition.x,newposition.z);
            other.transform.position = newposition;
        }
	}

    protected void ReCalcNeighbours() {
        int x, y, z;
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        z = Mathf.RoundToInt(transform.position.z);



        for (int i = 0; i < 3;i++) {
            for (int j = 0; j < 3;j++) {
                Debug.Log(MapGen.mapinstance.TileType(x - 1+i, y, z - 1+j));
                if (MapGen.mapinstance.TileType(x-1+i,y,z-1+j)=='#') {
                    neighbourheight[i, j] = 1f;
                }
                else {
                    neighbourheight[i, j] = 0f;
                }
            }
        }
    }

    protected float GetHeight(float x, float z)
    {
        float Height = 0f;
        int xind = 1;
        int zind = 1;

        if (Mathf.Abs(thisx-x)<0.5f && Mathf.Abs(thisz-z)<0.5f) {
            Vector3 location = new Vector3(x-thisx, 0, z-thisz);
            Height = Vector3.Dot(transform.forward,location.normalized)/2f + 0.7f;
        }
        else {
            if (x<thisx-0.5) {
                xind = 0;
            }
            else if (x>thisx+0.5) {
                xind = 2;
            }
            else {
                xind = 1;
            }

            if (z < thisz - 0.5)
            {
                zind = 0;
            }
            else if (z > thisz + 0.5)
            {
                zind = 2;
            }
            else
            {
                zind = 1;
            }
            Height = neighbourheight[xind, zind];
        }
    

        return Height+transform.position.y;
    }
}
