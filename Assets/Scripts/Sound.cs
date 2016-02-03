﻿using UnityEngine;
using System.Collections;

public class Sound : MonoBehaviour {

	public AudioClip attachSound;
	public AudioClip detachSound;
	public AudioClip headRoll;
	public AudioClip torsoWalk;
	public AudioClip footsteps;
 
	public AudioSource audioAttach;
	public AudioSource audioDetach;
	public AudioSource audioHeadRoll;
	public AudioSource audioTorso;
	public AudioSource audioFoot;
 
	public AudioSource AddAudio(AudioClip clip, bool loop, bool playAwake, float vol) { 
		AudioSource newAudio = gameObject.AddComponent<AudioSource>();
		newAudio.clip = clip; 
		newAudio.loop = loop;
		newAudio.playOnAwake = playAwake;
		newAudio.volume = vol; 
		return newAudio; 
	}
 
	public void Awake(){
		// add the necessary AudioSources:
		audioAttach = AddAudio(attachSound, false, true, 0.2f);
		audioDetach = AddAudio(detachSound, false, true, 0.2f);
		audioHeadRoll = AddAudio(headRoll, false, false, 0.2f); 
		audioTorso = AddAudio(torsoWalk, false, false, 0.2f); 
		audioFoot = AddAudio(footsteps, false, false, 0.2f);
    } 
}
