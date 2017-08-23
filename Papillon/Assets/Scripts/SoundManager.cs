﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {

    private AudioSource BGM;
    private AudioSource SE;

    public AudioClip[] BGMs;
    public AudioClip[] SEs;

    private Dictionary<string, int> BGMDic;
    private Dictionary<string, int> SEDic;

    public void Awake() {

        AudioSource[] sources = GetComponentsInChildren<AudioSource>();

        BGM = sources[0];
        SE = sources[1];

        setSounds();
        
    }

    // CAUTION : also add at the inspector!!
    private void setSounds() {
        BGMDic = new Dictionary<string, int>();

        BGMDic.Add("opening", 0);
        BGMDic.Add("base", 1);
        BGMDic.Add("map", 2);
        BGMDic.Add("rocket-launch", 3);

        SEDic = new Dictionary<string, int>();

        SEDic.Add("fail", 0);
        SEDic.Add("fail2", 1);
        SEDic.Add("flip", 2);
        SEDic.Add("move-item", 3);
        SEDic.Add("open", 4);
        SEDic.Add("step", 5);
        SEDic.Add("use-item", 6);
        SEDic.Add("snore", 7);
    }

    public void playBGM(string name) {
        

        if (BGM.clip != null && BGM.clip.Equals(BGMs[BGMDic[name]])) {
            return;
        }

        if (BGM.isPlaying) {
            BGM.Stop();
        }

        BGM.loop = true;
        BGM.clip = BGMs[BGMDic[name]];
        BGM.Play();
    }

    public void playSE(string name) {

        if (SE.isPlaying) {
            SE.Stop();
        }


        SE.loop = false;
        SE.clip = SEs[SEDic[name]];
        SE.Play();
    }
}