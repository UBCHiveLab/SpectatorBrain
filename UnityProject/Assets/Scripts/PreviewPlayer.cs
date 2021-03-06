﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPlayer : MonoBehaviour {

    private SpriteRenderer renderer;
    public double framesPerSecond = 10.0;
    private Sprite[] frames;
    private int index;
    private float timer;
	// Use this for initialization
	void Start () {
        renderer = this.GetComponent<SpriteRenderer>();
        if(renderer == null)
        {
            Debug.Log("Preview player needs a spriterenderer");
        }
        timer = 0;
        index = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if(frames == null || frames.Length == 0)
        {
            renderer.sprite = null;
            return;
        } else
        {
            timer += Time.deltaTime;
            if(timer >= 0.05)
            {
                timer = 0;
                index += 1;
            }
            index = index % frames.Length;
            renderer.sprite = frames[index];
            renderer.size = new Vector2(1024, 576);
        }
	}

    public void ChangePreview(Sprite[] newFrames)
    {
        frames = newFrames;
    }

    public void StopPreview()
    {
        frames = null;
    }
}
