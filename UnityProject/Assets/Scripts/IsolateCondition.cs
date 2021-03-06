﻿using UnityEngine;
using HolobrainConstants;

public class IsolateCondition : SwitchRoomUICondition {

    // Use this for initialization
    public override bool SwitchCondition()
    {
        GameObject Brain = GameObject.Find(Names.BRAIN_GAMEOBJECT_NAME);
        return (!Brain.GetComponent<ExplodingCommands>().Exploded()) && !(Brain.GetComponent<RotateStructures>().isRotating);
    }
}
