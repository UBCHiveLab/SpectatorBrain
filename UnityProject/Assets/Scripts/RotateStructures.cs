// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

﻿using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotateStructures : MonoBehaviour {
    private const string BRAIN_PARTS_GAMEOBJECT_NAME = "Brain";
    private const string GAZE_MARKER_GAMEOBJECT_NAME = "GazeMarker";
    private const string MRI_COLLECITON_GAMEOBJECT_NAME = "MRICollection";
    private const string FMRI_BRAINS_GAMEOBJECT_NAME = "fMRIBrains";
    private const int ROTATION_SPEED = 20;

    private List<Transform> isolatedStructures;
    private Transform gazeMarker;
    private GameObject brain;
    public GameObject MRICollection;
    public GameObject fMRIBrains;
    private Quaternion brainOriginalRotation;
    private Quaternion MRIOriginalRotation;
    private Quaternion fMRIOriginalRotation;
    public bool isRotating { get; private set; }

    private CustomMessages customMessages;
    private AudioSource soundFX;

    void Start(){
        customMessages = CustomMessages.Instance;
        // Assign the ToggleRotateMessageReceived() function to be a message handler for ToggleRotate messages
        // MessageHandlers is a dictionary with TestMessageIDs as keys and MessageCallbacks as values
        if (customMessages != null)
        {
            customMessages.MessageHandlers[CustomMessages.TestMessageID.ToggleRotate] = this.MessageReceived;
        }

        //UNCOMMENT THIS FOR GAZE MARKER
        gazeMarker = GameObject.Find(GAZE_MARKER_GAMEOBJECT_NAME).transform;
        brain = GameObject.Find(BRAIN_PARTS_GAMEOBJECT_NAME);

        brainOriginalRotation = brain.transform.localRotation;
        MRIOriginalRotation = MRICollection.transform.localRotation;
        fMRIOriginalRotation = fMRIBrains.transform.localRotation;
        isolatedStructures = null;
        soundFX = gameObject.GetComponent<AudioSource>();

        ResetRotation();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            RotateForOneFrame();
        }
    }

    public void OnSelect()
    {
        //SendRotateMessage();
        //ToggleRotate();
    }

    public void StartRotate()
    {
        if (!isRotating)
        {
            SendRotateMessage();
            ToggleRotate();
        }
    }

    public void StopRotate()
    {
        if (isRotating)
        {
            SendRotateMessage();
            ToggleRotate();
        }
    }

    public void MessageReceived(NetworkInMessage msg)
    {
        // This reads the user ID which we do not need
        msg.ReadInt64();

        // The message contains a byte (1 or 0) to pass the boolean value through
        isRotating = msg.ReadByte() == 1;
        ToggleRotate();
    }

    public void ToggleRotate()
    {
       soundFX.Play();
       isRotating = !isRotating;
       Debug.Log(isRotating);
    }

    private void RotateForOneFrame()
    {
       // if (isolatedStructures == null)
       // {
            brain.transform.Rotate(new Vector3(0, Time.deltaTime * ROTATION_SPEED, 0));
       // }
       /*
        else
        {
            Vector3 rotation = new Vector3(0, Time.deltaTime * ROTATION_SPEED, 0);
            foreach (Transform structure in isolatedStructures) {
                structure.Rotate(rotation);
            }
            //UNCOMMENT THIS FOR GAZE MARKER
            //gazeMarker.RotateAround(isolatedStructures[0].position, Vector3.up, Time.deltaTime * ROTATION_SPEED);
        }*/
    }

    public void SetIsolatedStructures(List<Transform> newIsolatedStructures)
    {
        if (newIsolatedStructures.Any())
        {
            isolatedStructures = newIsolatedStructures;
        }
        else
        {
            isolatedStructures = null;
            isRotating = false;
        }
    }

    public void ResetRotation()
    {
        brain.transform.localRotation = brainOriginalRotation;
        MRICollection.transform.localRotation = MRIOriginalRotation;
        if (fMRIBrains != null)
        {
            fMRIBrains.transform.localRotation = fMRIOriginalRotation;
        }
        isolatedStructures = null;
        isRotating = false;
    }

    private void SendRotateMessage()
    {
        if (customMessages != null)
        {
            customMessages.SendToggleRotateMessage(isRotating);
        }
    }
}
