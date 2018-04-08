using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vegan : Monster
{

    // Use this for initialization
    protected override void Start()
    {
        monstertype = 2;
        bravery = Random.Range(0, maxhitpoints/2); // range of braveries
        base.Start();
    }

    protected override int Decisions(out bool invert)
    {
        // If pissed at player & within range, chase them!
        int usemapnum = 1;
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
        else if (MapGen.mapinstance.DistGoal(transform.position, 4) < MapGen.mapinstance.PathDists[4])
        {
            usemapnum = 4;
            UseEmote(3);
            invert = true;
        }
        else if ((hitpoints<maxhitpoints) && (MapGen.mapinstance.DistGoal(transform.position, 3) < MapGen.mapinstance.PathDists[3]))
        {
            usemapnum = 3;
            if (hitpoints < (maxhitpoints - bravery))
            {
                UseEmote(3);
                invert = true;
            }
        }
        // If hungry, find food
        else if (hunger > 20)
        {
            usemapnum = 1;
        }
        // If tired, go home
        else if (tiredness > 20)
        {
            PathFound = MapGen.mapinstance.AStarPath(transform.position, HomeLocation);
            usemapnum = 1;
        }
        // If nothing else but vaguely annoyed? Go after the player if they're around
        else if (anger > 0)
        {
            usemapnum = 0;
        }

        return usemapnum;
    }
}
