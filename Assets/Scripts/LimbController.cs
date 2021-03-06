﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LimbController : MonoBehaviour
{

	[SerializeField]
	public GameObject leg, arm, torso, twoLegs, twoArms;
	[SerializeField]
	public Sprite[] bodyStates;

	List<GameObject> armsList;
	List<GameObject> torsoList;
	List<GameObject> legsList;

	public AudioClip attachSound;


	private float timer;
	private LevelManager mylevelmanager;

	string objectTag;
	private Animator myAnimator;
	//Animator for the different states
	private GameObject[] nearbyLimbsofType;
	public bool hasTorso, hasArm, hasSecondArm, hasLeg, hasSecondLeg;
	public bool hasBoot, hasTorch, hasShovel, hasPickaxe;
	private Player player;
	private float minimumDistance = 2.5f;
	private Vector3 pos;
	private Sound sounds;
	private bool playSound;
	private LimbController checkLimbs;
	private bool dpadY = false;
	private bool dpadX = false;

	private Transform oldPos;

	// Use this for initialization
	void Start ()
	{
		objectTag = "";
		armsList = new List<GameObject> ();
		legsList = new List<GameObject> ();
		torsoList = new List<GameObject> ();
		myAnimator = GetComponent<Animator> ();
		player = GetComponent<Player> ();
		bodyStates = new Sprite[10];
		torso = GameObject.Find ("Torso");
		arm = GameObject.Find ("Arm");
		leg = GameObject.Find ("Leg");
		twoArms = GameObject.Find ("Arm");
		twoLegs = GameObject.Find ("Leg");
		sounds = player.GetComponent<Sound> ();
		playSound = false;
		assignState ();
		mylevelmanager = FindObjectOfType<LevelManager> ();

	}

	void Update ()
	{
		if (player.enabled) {
		
			addLimbsToLists ();
			if (Input.GetKeyDown (KeyCode.Z) || Input.GetButtonDown ("Xbox_BButton")) {
				
				whichLimb ();
			} else if (Input.GetAxisRaw ("XBox_DPadX") != 0 || Input.GetAxisRaw ("XBox_DPadY") != 0 ||
			           Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.A) || Input.GetKeyDown (KeyCode.S) || Input.GetKeyDown (KeyCode.D)) {
			
				detach ();
			}

			//Players automatically has a pickaxe if they have an arm
			if (!hasArm && !hasSecondArm)
				hasPickaxe = false;
			//This is to make sure the axis on the xbox is pressed only once. 
			if (Input.GetAxisRaw ("XBox_DPadY") == 0) {
				dpadY = false;
			}
			if (Input.GetAxisRaw ("XBox_DPadX") == 0) {
				dpadX = false;
			}

			//handleSounds ();
		
		}
		timer += Time.deltaTime;
			
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
	private void assignState () //assigns the state of the sprite
	{
		if (hasTorso && hasLeg && hasSecondLeg && hasArm && hasSecondArm) {
			myAnimator.SetInteger ("state", 5);
		} else if (hasTorso && hasLeg && !hasSecondLeg && hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 8);
		} else if (hasTorso && hasLeg && hasSecondLeg && !hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 7);
		} else if (hasTorso && hasLeg && !hasSecondLeg && !hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 6);
		} else if (hasTorso && hasLeg && !hasSecondLeg && hasArm && hasSecondArm) {
			myAnimator.SetInteger ("state", 4);
		} else if (hasTorso && !hasLeg && !hasSecondLeg && hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 2);
		} else if (hasTorso && !hasLeg && !hasSecondLeg && hasArm && hasSecondArm) {
			myAnimator.SetInteger ("state", 3);
		} else if (!hasTorso && !hasLeg && !hasSecondLeg && !hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 0);
		} else if (hasTorso && !hasLeg && !hasSecondLeg && !hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 1);
		} else if (hasTorso && hasLeg && hasSecondLeg && hasArm && !hasSecondArm) {
			myAnimator.SetInteger ("state", 9);
		} else if (!hasTorso)
			myAnimator.SetInteger ("state", 0);
	}

	/*
	 * 
	 * finds and attaches the nearest limb
	 * 
	 * 
	 * */
	private void whichLimb ()
	{
		if (attachableLimbNearby ("arm")) {
			sounds.audioAttach.Play ();
			attachLimb (armsList);
			if (player.state == 1 || player.state == 2) {
				player.bumpPlayer (new Vector2 (1.8f, 0f));
			}

		} else if (attachableLimbNearby ("leg")) {
			sounds.audioAttach.Play ();
			attachLimb (legsList);

		} else if (attachableLimbNearby ("torso")) {
			sounds.audioAttach.Play ();
			attachLimb (torsoList);
			player.bumpPlayer (new Vector2 (.6f, 0f));

		}

	}

	/*
	* 
	* This returns true if player is near a certain limb object
		* 
		* 
		* */
	bool attachableLimbNearby (string listToUse)
	{
		
		List<GameObject> whichList = new List<GameObject> ();


		if (listToUse == "torso") {
			whichList = torsoList;

		} else if (listToUse == "arm" || listToUse == "pickaxe") {

			whichList = armsList;

		} else if (listToUse == "leg") {

			whichList = legsList;

		}
		
		//Checking if players are near an arm or legs they can attach to
		for (int i = 0; i < whichList.Count; ++i) {

			if (Vector3.Distance (transform.position, whichList [i].transform.position) <= minimumDistance) {
				if (whichList [i].tag == "torso" && !hasTorso && (whichList [i].gameObject.activeSelf == true)) {
					torso = whichList [i].gameObject;
					objectTag = "torso";
					return true;
				} else if (whichList [i].tag == "arm" && !hasArm && !hasSecondArm && hasTorso && (whichList [i].gameObject.activeSelf == true)) {
					arm = whichList [i].gameObject;
					objectTag = "arm";
					return true;
				} else if (whichList [i].tag == "arm" && hasArm && !hasSecondArm && hasTorso && (whichList [i].gameObject.activeSelf == true)) {
					twoArms = whichList [i].gameObject;
					objectTag = "arm";
					return true;
				} else if (whichList [i].tag == "leg" && !hasLeg && !hasSecondLeg && hasTorso && (whichList [i].gameObject.activeSelf == true)) {
					leg = whichList [i].gameObject;
					objectTag = "leg";
					return true;
				} else if (whichList [i].tag == "leg" && hasLeg && !hasSecondLeg && hasTorso && (whichList [i].gameObject.activeSelf == true)) {
					twoLegs = whichList [i].gameObject;
					objectTag = "leg";
					return true;
				}
			}
		}
		return false;
	}

	/*
	 * 
	 * Multiple Limbs store into an array list
	 * 
	 * 
	 * */

	void addLimbsToLists ()
	{
		Transform[] hinges = GameObject.FindObjectsOfType (typeof(Transform)) as Transform[];
		foreach (Transform go in hinges) {
			if (go.tag == "arm") {
				armsList.Add (go.gameObject);
			} else if (go.tag == "torso") {
				torsoList.Add (go.gameObject);
			} else if (go.tag == "leg") {
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
	public void attachLimb (List<GameObject> limb)
	{

		if (!hasTorso && objectTag == "torso") {
			hasTorso = true;
			assignState ();
			sendToOblivion (torso);


		} else if (hasTorso && objectTag == "arm") {
			
			if (!hasArm) {
				hasArm = true;
				sendToOblivion (arm);
				arm.GetComponent<Player> ().enabled = false;

			} else if (hasArm && !hasSecondArm) {
				hasSecondArm = true;
				sendToOblivion (twoArms);
				twoArms.GetComponent<Player> ().enabled = false;

			}
			hasPickaxe = true;
			assignState ();
		} else if (objectTag == "leg" && hasTorso) {
			if (!hasLeg) {
				hasLeg = true;
				sendToOblivion (leg);
				leg.GetComponent<Player> ().enabled = false;

			} else if (hasLeg && !hasSecondLeg) {

				hasSecondLeg = true;
				sendToOblivion (twoLegs);
				twoLegs.GetComponent<Player> ().enabled = false;

			}
			assignState ();
		}
	}

	private void sendToOblivion (GameObject banishThis)
	{
		banishThis.transform.Translate (new Vector3 (999999, 999999, 99999));
	}

	/*
	 * 
	 * This is the detach part.
	 * Checks if player has the necessary parts to detach
	 * Then, the limb will respawn where players are at. 
	 * 
	 * 
	 * */
	public void detach ()
	{
		if ((Input.GetKeyDown (KeyCode.W) || Input.GetAxisRaw ("XBox_DPadY") == 1) && hasTorso) {
			if (dpadY == false) {
				if (player.isClimbing) {
					player.isClimbing = false;
				}
		
				mylevelmanager.detachLimb ();
				torso.SetActive (true); 
				checkForDifferentLimbs ();
				instantiateBodyParts (torso);
				hasTorso = false;
				assignState ();
				sounds.audioDetachTorso.Play ();
				player.isClimbing = false;
				dpadY = true;
				player.bumpPlayer (new Vector2 (.6f, 0f));
			}
		}
		if ((Input.GetKeyDown (KeyCode.A) || Input.GetAxisRaw ("XBox_DPadX") == 1 || Input.GetAxisRaw ("XBox_DPadX") == -1 || Input.GetKeyDown (KeyCode.D))
		    && hasArm && !hasSecondArm) {
			if (dpadX == false) {
				mylevelmanager.detachLimb ();
				arm.SetActive (true);
				instantiateBodyParts (arm);
				hasArm = false;
				assignState ();
				sounds.audioDetachArm.Play ();
				//player.bumpPlayer (new Vector2 (.6f, 0f));

				//Change animations?
				dpadX = true;

			}
			
		}
		if ((Input.GetKeyDown (KeyCode.A) || Input.GetAxisRaw ("XBox_DPadX") == 1 || Input.GetAxisRaw ("XBox_DPadX") == -1 || Input.GetKeyDown (KeyCode.D))
		    && hasArm && hasSecondArm) {
			if (dpadX == false) {
				mylevelmanager.detachLimb ();
				if (player.isClimbing) {
					player.isClimbing = false;
				}
				twoArms.SetActive (true);
				instantiateBodyParts (twoArms);
				hasSecondArm = false;
				assignState ();
				sounds.audioDetachArm.Play ();
				//Change animations here?
				player.bumpPlayer (new Vector2 (.6f, 0f));

				dpadX = true;

			}
			
			
			
		} else if ((Input.GetKeyDown (KeyCode.S) || Input.GetAxisRaw ("XBox_DPadY") == -1) && hasLeg && !hasSecondLeg) {
			if (dpadY == false) {
				mylevelmanager.detachLimb ();
				leg.SetActive (true);
				hasLeg = false;
				instantiateBodyParts (leg);
				assignState ();
				sounds.audioDetachLeg.Play ();
				player.bumpPlayer (new Vector2 (.6f, 0f));

				dpadY = true;
			}
		} else if ((Input.GetKeyDown (KeyCode.S) || Input.GetAxisRaw ("XBox_DPadY") == -1) && hasSecondLeg) {
			if (dpadY == false) {
				mylevelmanager.detachLimb ();
				twoLegs.SetActive (true);
				hasSecondLeg = false;
				sounds.audioDetachLeg.Play ();
				instantiateBodyParts (twoLegs);
				assignState ();
				player.bumpPlayer (new Vector2 (.6f, 0f));

				dpadY = true;
			}
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
	public void checkForDifferentLimbs ()
	{

		if (hasArm && !hasSecondArm) {
			arm.SetActive (true);
			instantiateBodyParts (arm);
		} else if (hasArm && hasSecondArm) {
			twoArms.SetActive (true);
			arm.SetActive (true);
			instantiateBodyParts (twoArms);
			instantiateBodyParts (arm);
		}
		if (hasLeg && !hasSecondLeg) {
			leg.SetActive (true);
			instantiateBodyParts (leg);
		} else if (hasLeg && hasSecondLeg) {
			twoLegs.SetActive (true);
			leg.SetActive (true);
			instantiateBodyParts (twoLegs);
			instantiateBodyParts (leg);
		}
		hasTorso = false;
		hasArm = false;
		hasLeg = false;
		hasSecondArm = false;
		hasSecondLeg = false;
	}

	/*
	
		If players detach an arm or leg, that will set the game object active
		and it will detach near the player. 
	
	*/
	private void instantiateBodyParts (GameObject limb)
	{
		//pos = player.transform.position;
		pos = new Vector3 (player.transform.position.x, player.transform.position.y + 3f,
			player.transform.position.z);
		limb.transform.position = pos;
		if (limb.tag != "torso") {
			limb.GetComponent<Controller2D> ().collisions.below = false;
		}
	}
	
	/*
	 *
	 * Handle sound effects here. Play the sound when players go left or right. Stop the sound when 
	 * players are not moving or pressing the arrow key
	 * 
	 * 
	 * 
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
	 * 
	private void playSoundDifferentLimbs()
	{
		if(!hasTorso)
		{
			sounds.audioHeadRoll.Play();
		}
		else if(hasTorso && (!hasLeg && !hasSecondLeg)){
			sounds.audioFoot.Stop();
			sounds.audioHeadRoll.Stop();
			sounds.audioTorso.Play();
		}
		else if(hasTorso && (hasLeg || hasSecondLeg))
		{
			sounds.audioHeadRoll.Stop();
			sounds.audioTorso.Stop();
			sounds.audioFoot.Play();
		}


	}*/

	/*
	 * This is to stop the sound from playing when players release the keya
	 * 
	 * 
	 * 
	private void stopSound()
	{
		if(!hasTorso)
		{
			sounds.audioHeadRoll.Stop();
		}
		else if(hasTorso && (!hasLeg && !hasSecondLeg)){
			sounds.audioTorso.Stop();
		}
		else if(hasTorso && (hasLeg || hasSecondLeg))
		{
			sounds.audioFoot.Stop();
		}

	}*/

}