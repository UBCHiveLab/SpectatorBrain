// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

﻿using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAccessor : Singleton<StateAccessor> {

    private const string MRICollection = "MRICollection";
    private const string BRAIN_PARTS = "Brain";
    private ResetState resetState;
    private GameObject brain;
    public enum Mode { Default, Isolated, MRI };

    private Mode currentMode;

    new private void Awake()
    {
        currentMode = Mode.Default;
        brain = GameObject.Find(BRAIN_PARTS);
        resetState = ResetState.Instance;
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool ChangeMode(Mode newMode)
    {
        if((currentMode == Mode.Default) || (newMode == Mode.Default))
        {
            if (!brain.GetComponent<IsolateStructures>().AtLeastOneStructureIsMovingOrResizing)
            {
                currentMode = newMode;
                return true;
            }
        }

        if( ((currentMode == Mode.MRI) && (newMode == Mode.Isolated)) || ((currentMode == Mode.Isolated) && (newMode == Mode.MRI)) )
        {
            if (!brain.GetComponent<IsolateStructures>().AtLeastOneStructureIsMovingOrResizing)
            {
                resetState.ResetEverything();
                currentMode = newMode;
                return true;
            }
        }

        return false;
    }

    public bool AbleToTakeAnInteraction()
    {
        return !(CurrentlyIsolatedOrIsolating() || CurrentlyInMRIMode());
    }

    public bool CurrentlyInMRIMode()
    {
        GameObject mri = GameObject.Find(MRICollection);
        if(mri == null)
        {
            return false;
        } else
        {
            return mri.GetComponent<MRIManager>().isCurrentlyInMRIMode();
        }
    }

    public bool CurrentlyIsolatedOrIsolating()
    {
        return brain.GetComponent<IsolateStructures>().CurrentlyInIsolationModeOrIsolating();
    }

    public bool CurrentlyInStudentMode()
    {
        return false;// GameObject.Find("StatusUI").GetComponent<StudentModeCommands>().CurrentlyInStudentMode();
    }

   public Mode GetCurrentMode()
    {
        return currentMode;
    }
}