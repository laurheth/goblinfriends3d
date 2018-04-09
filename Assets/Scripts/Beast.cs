using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beast : Monster
{

    // Use this for initialization
    protected override void Start()
    {
        monstertype = 1;
        bravery = Mathf.RoundToInt(maxhitpoints * 0.9f); // always 90% brave
        base.Start();
    }

	protected override void DidMurder(Unit murdered)
	{
        hunger -= 10;
        return;
	}

    protected override int Decisions(out bool invert)
    {
        int usemapnum = 4;
        invert = false;

        /*if ((anger > 20 || fear > 20) && MapGen.mapinstance.DistGoal(transform.position, 0) < MapGen.mapinstance.PathDists[0])
        {
            usemapnum = 0;
            if (fear > 20) { invert = true; }
        }*/
        // If hungry, find food
        if (hunger > 30 || tiredness < 100)
        {
            if (MapGen.mapinstance.DistGoal(transform.position, 5) < MapGen.mapinstance.PathDists[5])
            {
                usemapnum = 5;
            }
            else
            {
                if (MapGen.mapinstance.DistGoal(transform.position, 3) < MapGen.mapinstance.DistGoal(transform.position, 0))
                {
                    usemapnum = 3;
                }
                else
                {
                    usemapnum = 0;
                }
            }
            if (hitpoints < (maxhitpoints - bravery))
            {
                UseEmote(3);
                invert = true;
            }
            else
            {
                UseEmote(2);
            }

        }
        // If tired, go home
        else //if (tiredness > 50)
        {
            tiredness = 0;
            recentemote = -1;
            PathFound = MapGen.mapinstance.AStarPath(transform.position, HomeLocation);
        }

        //Debug.Log(transform.position);
        //Debug.Log(usemapnum);
        //Debug.Log(invert);

        //if (invert) { usemapnum *= -1; }
        return usemapnum;
    }
}