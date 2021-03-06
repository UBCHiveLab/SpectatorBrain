// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using HoloToolkit.Unity;

public class IsolateModeButtonAction : CommandToExecute {

    private AudioSource audio;
    private GameObject isolateMode;

    private const string BRAIN_PARTS_NAME = "Brain";
    private GameObject brain;

    public List<ButtonAppearance> buttonsToDisable;
    // Use this for initialization
    override public void Start () {
        brain = GameObject.Find(BRAIN_PARTS_NAME);
        base.Start();
    }
	
	override protected Action Command()
    {
        return delegate
        {
            if(buttonsToDisable != null)
            {
                foreach(ButtonAppearance ba in buttonsToDisable)
                {
                    ba.SetButtonDisabled();
                }
            }
            FindObjectOfType<MoveClippingPlane>().TurnOffClipping();
            brain.GetComponent<IsolateStructures>().InitiateIsolationMode();
        };
    }
}
