using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Analytics : MonoBehaviour {

    public static Analytics instance = null;
    private GameObject analyticsPanel;
    private Text ammoPicked, pistolPicked, shotgunPicked, pistolUsed, shotgunUsed, en1Killed, en2Killed, levelsPlayed, foodLeft;
    private Player ps;
    private bool pressed;
    [HideInInspector]public int ammo, pistol, shotgun, pused, sused, en1, en2;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        InitAnalytics();
    }

    // Use this for initialization
    void Start () {
        

	}

    public void InitAnalytics()
    {
        analyticsPanel = GameObject.Find("Analytics");
        ammoPicked = GameObject.Find("Ammo picked").GetComponent<Text>();
        pistolPicked = GameObject.Find("Pistol picked").GetComponent<Text>();
        shotgunPicked = GameObject.Find("Shotgun picked").GetComponent<Text>();
        pistolUsed = GameObject.Find("Pistol used").GetComponent<Text>();
        shotgunUsed = GameObject.Find("Shotgun used").GetComponent<Text>();
        en1Killed = GameObject.Find("Enemy1 killed").GetComponent<Text>();
        en2Killed = GameObject.Find("Enemy2 killed").GetComponent<Text>();
        levelsPlayed = GameObject.Find("Levels Played").GetComponent<Text>();
        foodLeft = GameObject.Find("Food left").GetComponent<Text>();
        analyticsPanel.SetActive(false);
        ps = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update() {
        if ( (Input.GetKeyDown(KeyCode.LeftShift)) && (!pressed))
        {
            analyticsPanel.SetActive(true);
            pressed = true;
        } else if ((Input.GetKeyDown(KeyCode.LeftShift)) && (pressed))
        {
            analyticsPanel.SetActive(false);
            pressed = false;
        }

        if (pressed)
        {
            ammoPicked.text = "Ammunition picked up: " + ammo;
            pistolPicked.text = "Pistol picked up: " + pistol;
            shotgunPicked.text = "Shotgun picked up: " + shotgun;
            pistolUsed.text = "Pistol used: " + pused;
            shotgunUsed.text = "Shotgun used: " + sused;
            en1Killed.text = "Enemy 1 killed: " + en1;
            en2Killed.text = "Enemy 2 killed: " + en2;
            levelsPlayed.text = "Levels played: " + GameManager.instance.level;
            foodLeft.text = "Food left: " + ps.food;
        }
	}
}
