﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewGame : MonoBehaviour
{

    string[] buttons = new string[4] { "Start", "Level Select", "Credits", "Exit" };

    int selected = 0;
    bool inputVertical = false;
    bool userHasHitReturn = false;

    public Texture2D lvlSelButton;
    public Texture2D quitButton;
    public Texture2D playButton;
    public Texture2D creditsButton;


    public bool clickedStart = false;
    private bool aPressed = false;

    private AsyncOperation async = null; // When assigned, load is in progress.
    private LevelManager lvlmanager;
    private Loading loading;
    public bool showtext = false;
    public float delay;
    float Timesincestart = 0f;

    int menuSelection(string[] buttonsArray, int selectedItem, string direction)
    {
        if (direction == "up")
        {
            if (selectedItem == 0)
            {
                selectedItem = buttonsArray.Length - 1;
            }
            else {
                selectedItem -= 1;
            }
        }
        if (direction == "down")
        {
            if (selectedItem == buttonsArray.Length - 1)
            {
                selectedItem = 0;
            }
            else {
                selectedItem += 1;
            }
        }
        return selectedItem;
    }

    void Update()
    {
        //knowing what players scroll through
        if (Input.GetAxisRaw("Horizontal") == 1)
        {
            if (inputVertical == false)
            {
                selected = menuSelection(buttons, selected, "down");
            }
            inputVertical = true;

        }

        if (Input.GetAxisRaw("Horizontal") == -1)
        {
            if (inputVertical == false)
            {
                selected = menuSelection(buttons, selected, "up");
            }
            inputVertical = true;

        }

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            inputVertical = false;
        }

        //If players pressed the A button on xbox. 
        if (Input.GetButtonDown("Xbox_AButton"))
        {
            aPressed = true;
        }
        else aPressed = false;

        if (delay > Timesincestart && showtext == false)
        {
            Timesincestart += Time.deltaTime;
        }

        if (Timesincestart >= delay)
        {
            showtext = true;
        }

    }




    void OnGUI()
    {

        if (showtext == true)
        {
            GUI.SetNextControlName(buttons[0]);
            Event e = Event.current;

            if (GUI.Button(new Rect(Screen.width / 2 -300, Screen.height / 2 -50, playButton.width / 2, playButton.height / 2), playButton))
            {
                PlayerPrefs.SetInt("Level", 1);
                Application.LoadLevel("LoadingScene");
                Debug.Log("Clicked Start");

            }

            //If players select the start. 
            if (GUI.GetNameOfFocusedControl() == "Start" && (e.keyCode == KeyCode.Return || aPressed))
            {
                PlayerPrefs.SetInt("Level", 1);
                Application.LoadLevel("LoadingScene");

            }

            GUI.SetNextControlName(buttons[1]);

            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 -50, playButton.width / 2, playButton.height / 2), lvlSelButton))
            {
                PlayerPrefs.SetInt("Level", 13);
                Application.LoadLevel("LoadingScene");
                //when selected Options button
                Debug.Log("Clicked Options");

            }

            //Players select the levels they want
            if (GUI.GetNameOfFocusedControl() == "Level Select" && (e.keyCode == KeyCode.Return || aPressed))
            {
                PlayerPrefs.SetInt("Level", 13);
                Application.LoadLevel("LoadingScene");

            }
			GUI.SetNextControlName(buttons[2]);
            if (GUI.Button(new Rect(Screen.width / 2, Screen.height / 2 -50, playButton.width / 2, playButton.height / 2), creditsButton))
            {
                PlayerPrefs.SetInt("Level", 14);
                Application.LoadLevel("LoadingScene");
                //when selected Options button
                Debug.Log("Clicked Options");

            }

            //Players select the levels they want
            if (GUI.GetNameOfFocusedControl() == "Credits" && (e.keyCode == KeyCode.Return || aPressed))
            {
                PlayerPrefs.SetInt("Level", 14);
                Application.LoadLevel("LoadingScene");

            }
            GUI.SetNextControlName(buttons[3]);

            if (GUI.Button(new Rect(Screen.width / 2 + 150, Screen.height / 2 -50, playButton.width / 2, playButton.height / 2), quitButton))
            {
                Application.Quit();
                //when selected Exit button
                Debug.Log("Exit");

            }

            //If players select exit. 
            if (GUI.GetNameOfFocusedControl() == "Exit" && (e.keyCode == KeyCode.Return || aPressed))
            {

                Application.Quit();
            }
            GUI.FocusControl(buttons[selected]);

        }
    }
    }


