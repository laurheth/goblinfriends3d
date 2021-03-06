﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Humanoid : Monster {

	// Use this for initialization
	protected override void Start () {
        monstertype = 0;
        bravery = Random.Range(0, maxhitpoints); // range of braveries
        base.Start();
	}

	protected override int Decisions(out bool invert)
	{
        // If pissed at player & within range, chase them!
        int usemapnum = 2;
        invert = false;

        if ((anger > 20 || fear > 20) && MapGen.mapinstance.DistGoal(transform.position, 0) < MapGen.mapinstance.PathDists[0])
        {
            usemapnum = 0;
            if (hitpoints < (maxhitpoints - bravery))
            {
                UseEmote(3);
                invert = true;
            }
            else
            {
                UseEmote(1);
            }
            //MapGen.mapinstance.RollDown(transform.position, out horizontal, out vertical, InverseDesires);
        }
        else if (MapGen.mapinstance.DistGoal(transform.position,4)<MapGen.mapinstance.PathDists[4])
        {
            usemapnum = 4;
            if (hitpoints < (maxhitpoints - bravery))
            {
                UseEmote(3);
                invert = true;
            }
        }
        // If hungry, find food
        else if (hunger > 20)
        {
            if (MapGen.mapinstance.DistGoal(transform.position, 5) < MapGen.mapinstance.PathDists[5])
            {
                usemapnum = 5;
            }
            else
            {
                usemapnum = 1;
            }
            //Debug.Log("Food finding");
        }
        // If tired, go home
        else if (tiredness > 20)
        {
            // Close-ish to a goblin house? Get more specific, A* to specific home
            if (MapGen.mapinstance.DistGoal(transform.position, 2) < 8)
            {
                if (tiredness > 20)
                {
                    tiredness = -10;
                    PathFound = MapGen.mapinstance.AStarPath(transform.position, HomeLocation);
                }
            }
            usemapnum = 2;
            //Debug.Log("Home finding");
        }
        else 
        {
            wandering = true;
        }
        //Debug.Log("mapnum:" + usemapnum);
        return usemapnum;
	}
}
