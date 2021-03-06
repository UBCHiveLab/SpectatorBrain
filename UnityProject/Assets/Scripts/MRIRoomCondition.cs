﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HolobrainConstants;

public class MRIRoomCondition : SwitchRoomUICondition {

	// Use this for initialization
    public override bool SwitchCondition()
    {
        GameObject Brain = GameObject.Find(Names.BRAIN_GAMEOBJECT_NAME);
        return !Brain.GetComponent<IsolateStructures>().CurrentlyInIsolationModeOrIsolating();
    }
}
