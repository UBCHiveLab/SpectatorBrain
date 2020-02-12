// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;

public class ButtonCommands : MonoBehaviour {

    public bool buttonIsEnabled { get; private set; }

    private SwitchRoomUICondition condition;
    private ControlsUIManager controlsUI;
    private AudioSource source;
    private ButtonAppearance appearance;

    public Sprite hoverState;
    public Sprite activeState;
    public string GAZE_FRAME_NAME = "white-border";
    Color FullOpacityColor;
    Color PartialOpacityColor;
    bool IsPressed;

    public delegate void MultiDelegate();
    public MultiDelegate Commands;

    private void Start()
    {
        controlsUI = transform.GetComponentInParent<ControlsUIManager>();
        source = transform.GetComponent<AudioSource>();
        appearance = transform.GetComponent<ButtonAppearance>();
        condition = transform.GetComponent<SwitchRoomUICondition>();
        
        //disable the white selection frame
        EnableOrDisableFrame(false);
    }

    private void Update()
    {
    }

    public void OnFocusEnter()
    {
        //let the UIManager know that it is being gazed at
        if(controlsUI != null) {
            controlsUI.OnGazeEnteredUI();
        }

        //visual change of the button on gaze over
        //EnableOrDisableFrame(true);
    }

    public void OnFocusExit()
    {
        //let the UIManager know that it is no longer being gazed at
        if(controlsUI != null)
        {
            controlsUI.OnGazeExitUI();
        }

        //visual change of the button on gaze over
        //EnableOrDisableFrame(false);
    }

    public void AddCommand(Action command)
    {
        Commands += delegate
        {
            command();
        };
    }

    public void OnInputClicked()
    {
        if(condition != null)
        {
            if(condition.SwitchCondition())
            {
                Commands?.Invoke();
                if (source != null)
                {
                    source.Play();
                }
                if (appearance != null)
                {
                    appearance.SetButtonActive();
                }
            }
        } else
        {
            Commands?.Invoke();
            if (source != null)
            {
                source.Play();
            }
            if (appearance != null)
            {
                appearance.SetButtonActive();
            }
        }
    }

    //enables or disables the white frame surrounding the button gaze
    private void EnableOrDisableFrame(bool frameState)
    {
        if (transform.Find(GAZE_FRAME_NAME) != null)
            transform.Find(GAZE_FRAME_NAME).GetComponent<SpriteRenderer>().enabled = frameState;
    }

}
