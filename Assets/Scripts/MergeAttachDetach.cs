﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MergeAttachDetach : MonoBehaviour
{

	[SerializeField]
	public GameObject leg, arm, torso, twoLegs, twoArms;
	[SerializeField]
	public Sprite[] bodyStates;//{head, headTorso, headTorsoArm, headTorsoTwoArms,headTorsoTwoArmsLeg, headTorsoTwoArmsTwoLegs, headTorsoLeg, headTorsoTwoLegs, headTorsoArmLeg, headTorsoArmTwoLegs};

	List<GameObject> armsList;
	List<GameObject> torsoList;
	List<GameObject> legsList;

	public AudioClip attachSound;


	string objectTag;
	private Sprite currentBodyState; //stores the current state of the body, gets from the array
	private Animator myAnimator; //Animator for the different states
	private GameObject[] nearbyLimbsofType;
	public bool hasTorso, hasArm, hasSecondArm, hasLeg, hasSecondLeg;
	public bool hasBoot, hasTorch, hasShovel, hasPickaxe;
	private Player player;
	private float minimumDistance = 2.5f;
	private Vector3 pos;
	private Sound sounds;
	private bool playSound;
	private MergeAttachDetach checkLimbs;

	// Use this for initialization
	void Start()
	{
		objectTag = "";
		armsList = new List<GameObject> ();
		legsList = new List<GameObject> ();
		torsoList = new List<GameObject> ();
		myAnimator = GetComponent<Animator>();
		player = GetComponent<Player>();
		bodyStates = new Sprite[10];
		torso = GameObject.Find("Torso");
		arm = GameObject.Find("Arm");
		leg = GameObject.Find("leg");
		twoArms = GameObject.Find("Arm");
		twoLegs = GameObject.Find("leg");
		sounds = player.GetComponent<Sound>();
		playSound = false;
		assignState();

	}

	void Update()
	{

		multipleLimbs ();
		//Debug.Log(arm);
		if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Xbox_BButton"))
		{

			//Debug.Log("X Pressed");
			whichLimb();
		}
		else if(Input.GetButtonDown("Xbox_LeftButton")|| Input.GetButtonDown("Xbox_RightButton") || Input.GetButtonDown("Xbox_YButton") ||
				Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Alpha3))
		{
			
			detach();
		}

		if (!hasArm && !hasSecondArm)
			hasPickaxe = false;
		handleSounds();

	}

	/*
	 * 
	 * This is changing the players body states to whatever 
	 * the player attaches itself to. 
	 * 
	 * 
	 * */

	/*
	* 0 -  just the head
	* 1 - head and torso
	* 2 - head torso and one arm
	* 3 - head torso and both arms
	* 4 - head torso, both arms and one leg
	* 5 - full body
	* 6 - head, torso and one leg
	* 7 - head, torso and both legs
	* 8 - head, torso, leg and one arm
	* 9 - head, torso, arm and two legs
	*/
	private void assignState() //assigns the state of the sprite
	{
		if (hasTorso && hasLeg && hasSecondLeg && hasArm && hasSecondArm)
		{
			myAnimator.SetInteger("state", 5);
			currentBodyState = bodyStates[5];
		}
		else if (hasTorso && hasLeg && !hasSecondLeg && hasArm && !hasSecondArm){
			myAnimator.SetInteger("state", 8);
			currentBodyState = bodyStates[8];
		}
		else if (hasTorso && hasLeg && hasSecondLeg && !hasArm && !hasSecondArm)
		{
			myAnimator.SetInteger("state", 7);
			currentBodyState = bodyStates[7];
		}
		else if (hasTorso && hasLeg && !hasSecondLeg && !hasArm && !hasSecondArm)
		{
			myAnimator.SetInteger("state", 6);
			currentBodyState = bodyStates[6];
		}
		else if (hasTorso && hasLeg && !hasSecondLeg && hasArm && hasSecondArm)
		{
			myAnimator.SetInteger("state", 4);
			currentBodyState = bodyStates[4];
		}
		else if (hasTorso && !hasLeg && !hasSecondLeg && hasArm && !hasSecondArm)
		{
			myAnimator.SetInteger("state", 2);
			currentBodyState = bodyStates[2];
		}
		else if (hasTorso && !hasLeg && !hasSecondLeg && hasArm && hasSecondArm)
		{
			myAnimator.SetInteger("state", 3);
			currentBodyState = bodyStates[3];
		}
		else if (!hasTorso && !hasLeg && !hasSecondLeg && !hasArm && !hasSecondArm)
		{
			myAnimator.SetInteger("state", 0);
			currentBodyState = bodyStates[0];
		}
		else if (hasTorso && !hasLeg && !hasSecondLeg && !hasArm && !hasSecondArm)
		{
			myAnimator.SetInteger("state", 1);
			currentBodyState = bodyStates[1];
		}
		else if (hasTorso && hasLeg && hasSecondLeg && hasArm && !hasSecondArm)
		{
			myAnimator.SetInteger("state", 9);
			currentBodyState = bodyStates[9];
		}
		else if (!hasTorso)
			myAnimator.SetInteger("state", 0);
		currentBodyState = bodyStates[0];
	}

	/*
	 * 
	 * Which limbs the player are nearby
	 * 
	 * 
	 * */
	private void whichLimb()
	{
		if (nearbyLimbOfType("arm") )//&& canAttach(arm))
		{
			sounds.audioAttach.Play();
			attachLimb(armsList);	
		}
		else if  ((nearbyLimbOfType("pickaxe")))
		{
			sounds.audioAttach.Play();
			attachLimb(armsList);
		}
		else if (nearbyLimbOfType("leg") )//&& canAttach(leg))
		{
			sounds.audioAttach.Play();
			attachLimb(legsList);
		}
		else if (nearbyLimbOfType("torso") )//&& canAttach(torso))
		{
			sounds.audioAttach.Play();
			attachLimb(torsoList);
		}

	}

	/*
	 * 
	 * This is to test if players have necessary limbs to attach itself
	 * 
	 * 
	private bool canAttach(GameObject limb)
	{
		if ((!hasSecondArm) && (hasTorso))
		{
			return true;
		}
		else if ( !hasSecondLeg && hasTorso)
		{
			return true;
		}
		else if ( !hasTorso)
		{
			return true;
		}
		else
			return false;
	}*/

	/*
	* 
	* This returns true if player is near a certain limb object
		* 
		* 
		* */
		bool nearbyLimbOfType(string tag)
	{
		List<GameObject> whichList = new List<GameObject>();


		if (tag == "torso") {
			whichList = torsoList;

		} else if (tag == "arm" || tag == "pickaxe") {

			whichList = armsList;

		} else if (tag == "leg") {

			whichList = legsList;

		}
		for (int i = 0; i < whichList.Count; ++i)
		{

			if (Vector3.Distance(transform.position, whichList[i].transform.position) <= minimumDistance)
			{
				if (whichList[i].tag == "torso" && !hasTorso && (whichList[i].gameObject.activeSelf == true))
				{
					torso = whichList[i].gameObject;
					objectTag = "torso";
					return true;
				}
				else if ((whichList[i].tag == "arm" || whichList[i].tag == "pickaxe"|| whichList[i].tag == "shovel" || whichList[i].tag == "torch" ) 
					&& !hasArm && !hasSecondArm && hasTorso && (whichList[i].gameObject.activeSelf == true))
				{
					arm = whichList[i].gameObject;
					objectTag = "arm";
					return true;
				}

				else if((whichList[i].tag == "arm" || whichList[i].tag == "pickaxe" || whichList[i].tag == "shovel" || whichList[i].tag == "torch" )  
					&& hasArm && !hasSecondArm && hasTorso && (whichList[i].gameObject.activeSelf == true)){
					twoArms = whichList[i].gameObject;
					objectTag = "arm";

					return true;
				}
				else if ((whichList[i].tag == "leg" || whichList[i].tag == "boot") 
					&& !hasLeg && !hasSecondLeg && hasTorso && (whichList[i].gameObject.activeSelf == true))
				{
					leg = whichList[i].gameObject;
					objectTag = "leg";
					return true;
				}
				else if((whichList[i].tag == "leg" || whichList[i].tag == "boot") 
					&& hasLeg && !hasSecondLeg && hasTorso && (whichList[i].gameObject.activeSelf == true)){
					twoLegs = whichList[i].gameObject;
					objectTag = "leg";
					return true;
				}
			}
		}
		return false;
	}

	/*
	 * 
	 * Multiple Game Objects
	 * 
	 * 
	 * */

	void multipleLimbs(){
		Transform[] hinges = GameObject.FindObjectsOfType(typeof(Transform)) as Transform[];
		//GameObject[] armsWithAxe = GameObject.FindGameObjectsWithTag("pickaxe");
		//foreach(object go in allObjects)
		foreach (Transform go in hinges) {
			//Destroy (child.gameObject);
			if(go.tag == "arm"){
				armsList.Add(go.gameObject);
			}
			else if(go.tag == "torso"){
				torsoList.Add (go.gameObject);
			}
			else if(go.tag == "leg"){
				legsList.Add (go.gameObject);
			}
		}

	}


	/*
	 * 
	 * This is attaching the limb.
	 * It will disable the object in the game if player decides to attach.
	 * Then, it will change the player's state.
	 * 
	 * */
	public void attachLimb(List<GameObject> limb)
	{

		if (!hasTorso && objectTag == "torso")
		{
			hasTorso = true;
			assignState();
			torso.SetActive(false);

		}
		else if (hasTorso && objectTag == "arm")
		{
			
			if (!hasArm)
			{
				arm.SetActive (false);
				//Debug.Log (arm + "1");
				hasArm = true;

			}
			else if (hasArm && !hasSecondArm)
			{
				//Debug.Log (twoArms + "2" );
				hasSecondArm = true;
				twoArms.SetActive(false);
			}
			hasPickaxe = true;
			assignState();
		}
		else if (objectTag == "leg" && hasTorso)
		{
			if (!hasLeg)
			{
				hasLeg = true;
				leg.SetActive(false);
			}
			else if (hasLeg && !hasSecondLeg)
			{
				hasSecondLeg = true;
				twoLegs.SetActive(false);
			}

			assignState();


		}
		//checkForWeapon ();
	}

	/*
	 * 
	 * This is the detach part.
	 * Checks if player has the necessary parts to detach
	 * Then, the limb will respawn where players are at. 
	 * 
	 * 
	 * */
	public void detach()
	{
		
		if ((Input.GetKeyDown(KeyCode.Alpha1) || Input.GetButtonDown("Xbox_YButton")) && hasTorso)
		{
			torso.SetActive(true); 
			checkForDifferentLimbs();
			instantiateBodyParts(torso);
			hasTorso = false;
			assignState();
			sounds.audioDetach.Play();
			//detachWeaponLimbs();
		}
		if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetButtonDown("Xbox_LeftButton"))   && hasArm && !hasSecondArm)
		{

				arm.SetActive(true);
				instantiateBodyParts(arm);
				hasArm = false;
				assignState();
				sounds.audioDetach.Play();
			
		}
		if ((Input.GetKeyDown(KeyCode.Alpha2) || Input.GetButtonDown("Xbox_LeftButton")) && hasArm && hasSecondArm)
		{
			
				twoArms.SetActive(true);
				instantiateBodyParts(twoArms);
				hasSecondArm = false;
				assignState();
				sounds.audioDetach.Play();
			
			
		}
		else if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetButtonDown("Xbox_RightButton")) && hasLeg && !hasSecondLeg)
		{
			leg.SetActive(true);
			hasLeg = false;
			instantiateBodyParts(leg);
			assignState();
			sounds.audioDetach.Play();
			//detachWeaponLimbs();
		}
		else if ((Input.GetKeyDown(KeyCode.Alpha3) || Input.GetButtonDown("Xbox_RightButton")) && hasSecondLeg)
		{
			twoLegs.SetActive(true);
			hasSecondLeg = false;
			sounds.audioDetach.Play();
			//instantiateBodyParts(leg);
			instantiateBodyParts(twoLegs);
			assignState();
			//detachWeaponLimbs();
		}
		
		
	}
	

	/*
	 * 
	 * This is checking if players have a torso, arms, and legs, and
	 * they want to detach the torso. If players detach the torso, 
	 * everything will fall apart. 
	 * 
	 * 
	 * */
	public void checkForDifferentLimbs()
	{

		if (hasArm && !hasSecondArm)
		{
			arm.SetActive(true);
			instantiateBodyParts(arm);
		}
		else if (hasArm && hasSecondArm)
		{
			twoArms.SetActive(true);
			arm.SetActive(true);
			instantiateBodyParts(twoArms);
			instantiateBodyParts(arm);
		}
		if (hasLeg && !hasSecondLeg)
		{
			leg.SetActive(true);
			instantiateBodyParts(leg);
		}
		else if (hasLeg && hasSecondLeg)
		{
			twoLegs.SetActive(true);
			leg.SetActive(true);
			instantiateBodyParts(twoLegs);
			instantiateBodyParts(leg);
		}
		hasTorso = false;
		hasArm = false;
		hasLeg = false;
		hasSecondArm = false;
		hasSecondLeg = false;
	}

	public void instantiateBodyParts(GameObject limbs)
	{
		pos = player.transform.position;
		limbs.transform.position = pos;
	}
	
	/*
	 *
	 * Handle sound effects here. Play the sound when players go left or right. Stop the sound when 
	 * players are not moving or pressing the arrow key
	 * 
	 * 
	 * */
	private void handleSounds()
	{
		if(Input.GetAxisRaw("Horizontal") == 1 && !playSound)
		{
			playSoundDifferentLimbs();
			playSound = true;
		}
		else if (Input.GetAxisRaw("Horizontal") == 0 || (Input.GetAxisRaw("Horizontal") == 1 && (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Xbox_LeftButton"))) ||
				(Input.GetAxisRaw("Horizontal") == -1 && (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Xbox_LeftButton"))))
		{

			playSound = false;
			stopSound();
		}
		else if (Input.GetAxisRaw("Horizontal") == -1 && !playSound)
		{
			playSoundDifferentLimbs();
			playSound = true;
		}	
		
		


	}

	/*
	 * Checking for the different limbs in order to play some sounds according to 
	 * the respective body states
	 * 
	 * */
	private void playSoundDifferentLimbs()
	{
		if(!checkLimbs.hasTorso)
		{
			sounds.audioHeadRoll.Play();
		}
		else if(checkLimbs.hasTorso && (!checkLimbs.hasLeg && !checkLimbs.hasSecondLeg)){
			sounds.audioFoot.Stop();
			sounds.audioTorso.Play();
		}
		else if(checkLimbs.hasTorso && (checkLimbs.hasLeg || checkLimbs.hasSecondLeg))
		{
			sounds.audioTorso.Stop();
			sounds.audioFoot.Play();
		}


	}

	/*
	 * This is to stop the sound from playing when players release the keya
	 * 
	 * 
	 * */
	private void stopSound()
	{
		if(!checkLimbs.hasTorso)
		{
			sounds.audioHeadRoll.Stop();
		}
		else if(checkLimbs.hasTorso && (!checkLimbs.hasLeg && !checkLimbs.hasSecondLeg)){
			sounds.audioTorso.Stop();
		}
		else if(checkLimbs.hasTorso && (checkLimbs.hasLeg || checkLimbs.hasSecondLeg))
		{
			sounds.audioFoot.Stop();
		}

	}

}