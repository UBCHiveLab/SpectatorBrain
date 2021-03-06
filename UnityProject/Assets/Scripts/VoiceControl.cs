// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using System;
using HoloToolkit.Unity.InputModule;
using HolobrainConstants;
using UnityEngine.EventSystems;

public class VoiceControl : MonoBehaviour {
    private GameObject brain;
    private GameObject controlsUI;
    private HTGazeManager gazeManager;

    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> voiceRecognitionKeywords;
    private Dictionary<string, GameObject> buttonActionsToGameObjectName;
    private EventSystem eventSystem;
    
    private DataRecorder dataRecorder;

    // Use this for initialization
    void Start() {
        // Referencing game objects to access their scripts
        brain = GameObject.Find(Names.BRAIN_GAMEOBJECT_NAME);
        eventSystem = GameObject.Find(Names.EVENT_SYSTEM_NAME).GetComponent<EventSystem>();
        controlsUI = GameObject.Find(Names.CONTROLS_UI_GAMEOBJECT_NAME);
        gazeManager = GameObject.Find(Names.HOLOGRAM_COLLECTION_GAMEOBJECT_NAME).GetComponent<HTGazeManager>();

        voiceRecognitionKeywords = new Dictionary<string, Action>();

        foreach (var item in Names.GetStructureNames())
        {
            voiceRecognitionKeywords.Add("Add " + item, () => { HandleAddBrainPart(item); });
            voiceRecognitionKeywords.Add("Remove " + item, () => { HandleRemoveBrainPart(item); });
        }

        //map voice commands to the corresponding button name
        GameObject menu = GameObject.Find("menu");
        GameObject rooms = GameObject.Find("rooms");
        buttonActionsToGameObjectName = new Dictionary<string, GameObject>
        {
            { "Rotate", GameObject.Find("rotate-stop") },
            //{ "Rotate Walls", "rotate-walls-icon" },
            { "Stop", GameObject.Find("rotate-stop") },
            { "Expand", GameObject.Find("expand-collapse") },
            { "Collapse", GameObject.Find("expand-collapse") },
           // { "Reset", "reset-icon" },
            {"Scale Up", GameObject.Find("scale-up") },
            {"Scale Down", GameObject.Find("scale-down") },
            {"Isolate", menu },
            {"Hide Isolate", menu },
            {"Reposition", GameObject.Find("reposition-icon") },
            {"Add All", menu },
            {"Remove All", menu },
            { "Microglia", menu },
           { "Channel 1", menu },
            { "Channel 2", menu },
            { "Pin", GameObject.Find("pin-unpin") },
            {"Mute", GameObject.Find("Mute") },
            { "Play", menu },
            { "Pause", menu },
            { "Faster", menu },
            { "Slower", menu },
            { "Skip One", menu },
            { "Skip Ten", menu },
            { "Back One", menu },
           { "Back Ten", menu },
            { "Educational Room", rooms},
            { "MRI Room", rooms},
          { "fMRI Room", rooms},
            { "Brain Cell Room", rooms},
            { "DTI Room", rooms },
          { "End Tutorial", GameObject.Find("SkipButton")},
            { "Next", GameObject.Find("NextButton")},
            {"Locate", GameObject.Find("Locate") }
        };

        voiceRecognitionKeywords.Add("Rotate", HandleCommand(buttonActionsToGameObjectName["Rotate"], () =>
        {
            var rs = brain.GetComponent<RotateStructures>();
            return rs != null && rs.isRotating == false;
        }, typeof(RotateButtonAction)));
        voiceRecognitionKeywords.Add("Stop", HandleCommand(buttonActionsToGameObjectName["Stop"], () =>
        {
            var rs = brain.GetComponent<RotateStructures>();
            return rs != null && rs.isRotating == true;
        }, typeof(RotateButtonAction)));

        voiceRecognitionKeywords.Add("Scale Up", HandleCommand(buttonActionsToGameObjectName["Scale Up"]));
        voiceRecognitionKeywords.Add("Scale Down", HandleCommand(buttonActionsToGameObjectName["Scale Down"]));

        voiceRecognitionKeywords.Add("Expand", HandleCommand(buttonActionsToGameObjectName["Expand"], () =>
        {
            var sa = brain.GetComponent<StateAccessor>();
            return sa != null && sa.AbleToTakeAnInteraction();
        }, typeof(ExplodeButtonAction), "Expand"));
        voiceRecognitionKeywords.Add("Collapse", HandleCommand(buttonActionsToGameObjectName["Collapse"], () =>
        {
            var sa = brain.GetComponent<StateAccessor>();
            return sa != null && sa.AbleToTakeAnInteraction();
        }, typeof(ExplodeButtonAction), "Collapse"));

        voiceRecognitionKeywords.Add("Isolate", HandleCommand(buttonActionsToGameObjectName["Isolate"], () => {
            var rs = brain.GetComponent<RotateStructures>();
            return rs != null && !rs.isRotating;
        }, typeof(IsolateModeButtonAction)));
        voiceRecognitionKeywords.Add("Hide Isolate", HandleCommand(buttonActionsToGameObjectName["Hide Isolate"], () =>
        {
            var rs = brain.GetComponent<RotateStructures>();
            return rs != null && !rs.isRotating;
        }, typeof(IsolateExitButtonAction)));
        //voiceRecognitionKeywords.Add("Reset", HandleCommand(GameObject.Find(buttonActionsToGameObjectName["Reset"])));
        voiceRecognitionKeywords.Add("Reposition", HandleCommand(buttonActionsToGameObjectName["Reposition"]));
        voiceRecognitionKeywords.Add("Add All", HandleCommand(buttonActionsToGameObjectName["Add All"], typeof(IsolateButtonAction), "AddAll"));
        voiceRecognitionKeywords.Add("Remove All", HandleCommand(buttonActionsToGameObjectName["Remove All"], typeof(IsolateButtonAction), "RemoveAll"));
        // New Voice Commands

        voiceRecognitionKeywords.Add("Play", HandleCommand(buttonActionsToGameObjectName["Play"], typeof(PlayButtonAction), "play"));
        voiceRecognitionKeywords.Add("Pause", HandleCommand(buttonActionsToGameObjectName["Pause"], typeof(PlayButtonAction), "pause"));
        voiceRecognitionKeywords.Add("Faster", HandleCommand(buttonActionsToGameObjectName["Faster"], typeof(SpeedUpButtonAction)));
        voiceRecognitionKeywords.Add("Slower", HandleCommand(buttonActionsToGameObjectName["Slower"], typeof(SlowDownButtonAction)));
        voiceRecognitionKeywords.Add("Skip One", HandleCommand(buttonActionsToGameObjectName["Skip One"], typeof(SkipOneButtonAction)));
        voiceRecognitionKeywords.Add("Back One", HandleCommand(buttonActionsToGameObjectName["Skip One"], typeof(BackOneButtonAction)));
        voiceRecognitionKeywords.Add("Skip Ten", HandleCommand(buttonActionsToGameObjectName["Skip Ten"], typeof(SkipTenButtonAction)));
        voiceRecognitionKeywords.Add("Back Ten", HandleCommand(buttonActionsToGameObjectName["Back Ten"], typeof(BackTenButtonAction)));

        voiceRecognitionKeywords.Add("Educational Room", HandleCommand(buttonActionsToGameObjectName["Educational Room"], typeof(EduRoomCommand)));
        voiceRecognitionKeywords.Add("FMRI Room", HandleCommand(buttonActionsToGameObjectName["fMRI Room"], typeof(fMRIRoomCommand)));
        voiceRecognitionKeywords.Add("Functional MRI Room", HandleCommand(buttonActionsToGameObjectName["fMRI Room"], typeof(fMRIRoomCommand)));
        voiceRecognitionKeywords.Add("MRI Room", HandleCommand(buttonActionsToGameObjectName["MRI Room"], typeof(MRIRoomCommand)));
        voiceRecognitionKeywords.Add("MRI Scan Room", HandleCommand(buttonActionsToGameObjectName["MRI Room"], typeof(MRIRoomCommand)));
        voiceRecognitionKeywords.Add("Brain Cell Room", HandleCommand(buttonActionsToGameObjectName["Brain Cell Room"], typeof(CellRoomCommand)));

        voiceRecognitionKeywords.Add("Show Microglia", HandleCommand(buttonActionsToGameObjectName["Microglia"], typeof(SwapCellButtonAction), "microglia"));
        voiceRecognitionKeywords.Add("Show Channel One", HandleCommand(buttonActionsToGameObjectName["Channel 1"], typeof(SwapCellButtonAction), "channel1"));
        voiceRecognitionKeywords.Add("Show Channel Two", HandleCommand(buttonActionsToGameObjectName["Channel 2"], typeof(SwapCellButtonAction), "channel2"));

        //UNCOMMENT THIS FOR GAZE MARKER
        //voiceRecognitionKeywords.Add("Place Marker", HandlePlaceMarker);
        //voiceRecognitionKeywords.Add("Clear Marker", HandleClearMarker);
        voiceRecognitionKeywords.Add("Pin Menu", HandleCommand(buttonActionsToGameObjectName["Pin"], () =>
        {
            var cu = controlsUI.GetComponent<ControlsUIManager>();
            return cu != null && !cu.GetMenuPinState();
        }, typeof(PinButtonAction)));
        voiceRecognitionKeywords.Add("UnPin Menu", HandleCommand(buttonActionsToGameObjectName["Pin"], () => {
            var cu = controlsUI.GetComponent<ControlsUIManager>();
            return cu != null && cu.GetMenuPinState();
        }, typeof(PinButtonAction)));
        voiceRecognitionKeywords.Add("Mute", HandleCommand(buttonActionsToGameObjectName["Mute"], () => {
            return !buttonActionsToGameObjectName["Mute"].GetComponent<MuteButtonAction>().IsMuted();
            }, typeof(MuteButtonAction)));
        voiceRecognitionKeywords.Add("UnMute", HandleCommand(buttonActionsToGameObjectName["Mute"], () => {
            return buttonActionsToGameObjectName["Mute"].GetComponent<MuteButtonAction>().IsMuted();
        }, typeof(MuteButtonAction)));


        //voiceRecognitionKeywords.Add("End Tutorial", HandleEndTutorial);
        //voiceRecognitionKeywords.Add("Next", HandleNextChapter);

        // voiceRecognitionKeywords.Add("Locate", HandleCommand(GameObject.Find(buttonActionsToGameObjectName["Locate"])));

        keywordRecognizer = new KeywordRecognizer(voiceRecognitionKeywords.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();
	}

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Action keywordAction;
        if (voiceRecognitionKeywords.TryGetValue(args.text, out keywordAction))
        {
            Debug.Log("voice keyword: " + args.text);

            if (brain != null)
            {
                if (!brain.GetComponent<StateAccessor>().CurrentlyInStudentMode()) keywordAction.Invoke();
            }
        }               
    }

    private void executeClick(GameObject g)
    {
        g.GetComponent<ButtonCommands>().OnInputClicked(new InputClickedEventData(eventSystem));
    }

    //use when you just want to click surface, either found or not
    private Action HandleCommand(GameObject target)
    {
        return delegate
        {
            if (target != null)
            {
                executeClick(target);
            }
        };
    }

    //use when the click should only happen on precondition
    private Action HandleCommand(GameObject target, Func<bool> pred)
    {
        return delegate
        {
            if(pred() && target != null)
            {
                executeClick(target);
            }
        };
    }

    //use when click should happen on precondition, and to a child that has type component
    private Action HandleCommand(GameObject target, Func<bool> pred, System.Type t)
    {
        return delegate
        {
            if (pred() && target != null)
            {
                var btn = target.GetComponentsInChildren(t)[0];
                if (btn != null)
                {
                    executeClick(btn.gameObject);
                }
            }
        };
    }

    //overloaded to check gameobject name before clicking (cant be part of pred because it requires component t in target to be found already)
    private Action HandleCommand(GameObject target, Func<bool> pred, Type t, string name)
    {
        return delegate
        {
            if (pred() && target != null)
            {
                var btns = target.GetComponentsInChildren(t);
                foreach (Component c in target.GetComponentsInChildren(t))
                {
                    if (c.name == name)
                    {
                        executeClick(c.gameObject);
                    }
                }
            }
        };
    }

    //when button of interest isnt on the surface level, but we dont care about precondition
    private Action HandleCommand(GameObject target, Type t)
    {
        return HandleCommand(target, delegate { return true; }, t);
    }

    //when button of interest isnt on the surface level and we want to check name, but we dont care about precondition
    private Action HandleCommand(GameObject target, Type t, string name)
    {
        return HandleCommand(target, delegate { return true; }, t, name);
    }


    private void HandleAddBrainPart(string partName)
    {
        if(brain.GetComponent<IsolateStructures>().CurrentlyInIsolationModeOrIsolating())
        {
            foreach (ButtonAppearance button in controlsUI.transform.GetComponentsInChildren<ButtonAppearance>(true))
            {
                if(button.name == partName)
                {
                    executeClick(button.gameObject);
                }
            }
        }
    }

    private void HandleRemoveBrainPart(string partName)
    {
        if (brain.GetComponent<IsolateStructures>().CurrentlyInIsolationModeOrIsolating())
        {
            foreach (ButtonAppearance button in controlsUI.transform.GetComponentsInChildren<ButtonAppearance>(true))
            {
                if(button.name == partName)
                {
                    executeClick(button.gameObject);
                }
            }
        }
    }

    private void Update()
    {
    }

    //BELLOW HERE ARE UNUSED/UNLINKED COMMANDS

    //UNCOMMENT THIS FOR GAZE MARKER
    private void HandlePlaceMarker()
    {
        print("handleplacemarker called");
        if (gazeManager.Hit)
        {
            print("gazemanager hit");
            gazeManager.HitInfo.transform.GetComponent<GazeMarkerCommands>().OnSelect();
        }
    }

    //UNCOMMENT THIS FOR GAZE MARKER
    private void HandleClearMarker()
    {
        if (gazeManager.Hit)
        {
            gazeManager.HitInfo.transform.GetComponent<GazeMarkerCommands>().RemoveMarkerFromStructure();
        }
        GazeMarkerManager gmm = (GazeMarkerManager)FindObjectOfType(typeof(GazeMarkerManager));
        gmm.TryToRemoveGazeMarker();
    }

    private void HandleNextChapter()
    {
        //GameObject.Find(buttonActionsToGameObjectName["Next"]).GetComponent<ButtonCommands>().OnInputClicked(new InputClickedEventData(eventSystem));
    }

    private void HandleEndTutorial()
    {
        //GameObject.Find(buttonActionsToGameObjectName["End Tutorial"]).GetComponent<ButtonCommands>().OnInputClicked(new InputClickedEventData(eventSystem));
    }
}
