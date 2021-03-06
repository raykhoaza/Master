using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private SwitchControl ControlScript;
    private GameObject back1;
    private GameObject player;
    private GameObject follower;
    private GameObject Wreck;
    private GameObject breakable;
    private Sound sounds;
    public GameObject deathParticle;
	public GameObject exit;
    public GameObject WreckParticle;
    public GameObject detachParticle;
    private GameObject[] Legs;
    private GameObject[] Arms;
    Animator playerAnimator;
    float minimumDistance = 10.5f;
    private LinkedList Limbs;
	private PressurePlateController pp;
	public string [] Levels;
	private Loading loading;
    private Vector3 offSet;
	
    public class Node {
        public Node next;
        public Node last;
        public GameObject data;
        public Vector3 position;
    }
    //LinkedList data structure to hold gameobjects and position
    public class LinkedList
    {
        private Node head;
        private Node last;
        //attach a new gameobject to the last element
        public void append(GameObject tmp) {
            Node toAdd = new Node();
            toAdd.data = tmp;
            toAdd.position = tmp.transform.position;
            if (head == null) {
                head = toAdd;
                last = toAdd;
                toAdd.next = null;
            } else {
                last.next = toAdd;
                last = toAdd;
                toAdd.next = null;
            }
        }
        //find the position of a game object
        public Vector3 getPosition(GameObject tmp) {
            Node iter = head;
            Vector3 position = Vector3.zero;
            while(head != null) {
                if(iter.data == tmp) {
                    position = iter.position;
                    return position;
                } else {
                    iter = iter.next;
                }
            }
            return position;
        }
    }

    public float respawnDelay;
    // Use this for initialization
    void Start()
    {
        Limbs = new LinkedList();
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player.transform.position);
        offSet = player.transform.position + (player.transform.forward * 985.0f);
        follower = GameObject.FindGameObjectWithTag("follower");
        Wreck = GameObject.Find("BreakTerrain");
		exit = GameObject.Find("Exit");
        back1 = GameObject.Find("Background");
        breakable = GameObject.FindGameObjectWithTag("breakable");
        ControlScript = player.GetComponent<SwitchControl>();
        //find all objects with tags "leg" and "arm"
        Legs = GameObject.FindGameObjectsWithTag("leg");
        Arms = GameObject.FindGameObjectsWithTag("arm");
        playerAnimator = player.GetComponent<Animator>();
        sounds = player.GetComponent<Sound>();

        Levels = new string[]{"AlexFerr2DLevel", "Showcase"};
        for (int i = 0; i < Legs.Length; i++) 
        {
            Limbs.append(Legs[i]);
        }

        for (int i = 0; i < Arms.Length; i++) 
        {
            Limbs.append(Arms[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
		
        if (Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Xbox_XButton"))
        {
            playerAnimator.SetTrigger("attack");
        }
		if(breakable != null){
			if(Vector3.Distance(breakable.transform.position, player.transform.position) <= minimumDistance)
			{
				playerAnimator.SetLayerWeight(5, 1);
			}
		}
        else{
            playerAnimator.SetLayerWeight(5, 0);
        }
		
    }

    public void respawnPlayer()
    {
        StartCoroutine("respawnPlayerCo");
    }

    public void detachLimb()
    {
     
        Instantiate(detachParticle, player.transform.position , player.transform.rotation);
    }

    public void destroyWall()
    {
        Wreck.SetActive(false);
        Instantiate(WreckParticle, Wreck.transform.position, Wreck.transform.rotation);
    }

    public IEnumerator respawnPlayerCo()
    {
        Instantiate(deathParticle, player.transform.position, player.transform.rotation);
        //player.SetActive(false);
        player.GetComponent<Renderer>().enabled = false;
        follower.GetComponent<Renderer>().enabled = false;
        follower.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
		//SceneManager.LoadScene ("ninja");
		Scene scene = SceneManager.GetActiveScene(); 
		SceneManager.LoadScene(scene.name);

        //Application.LoadLevel(Application.loadedLevel);
        yield return new WaitForSeconds(respawnDelay);
        follower.SetActive(true);
        back1.GetComponent<Renderer>().enabled = true;
        follower.GetComponent<Renderer>().enabled = true;
        player.SetActive(true);
        player.GetComponent<Renderer>().enabled = true;
        //Instantiate(respawnParticle, currentCheckpoint.transform.position, currentCheckpoint.transform.rotation);
        //player.transform.position = currentCheckpoint.transform.position;
    }

    public void respawnLimb(string target)
    {
        GameObject tmp = GameObject.Find(target);
        Instantiate(deathParticle, tmp.transform.position, tmp.transform.rotation);
        tmp.transform.position = Limbs.getPosition(tmp);
        ControlScript.switchToHead();
    }
	
	
	

}