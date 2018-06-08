using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float levelStartDelay = 2f;
    public float turnDelay = .1f;
    public static GameManager instance = null;
    public int playerFoodPoints = 100;
    public int playerAmmo = 0;
    public int playerPistol = 0;
    public int playerShotgun = 0;
    [HideInInspector] public bool playersTurn = true;
    public bool doingSetup = true;
    
    private Text levelText;
    private Text restartText;
    private GameObject levelImage;
    private BoardManager boardScript;
    [HideInInspector] public int level = 1;
    private List<Enemy> enemies;
    private bool enemiesMoving;


    //*** GAME SETUP ***//



    // Awake sets up before Start
    void Awake() {
        //turns this script into a singleton
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        

        DontDestroyOnLoad(gameObject); // makes sure this object is not removed upon sceneloading
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();

        
    }

    // makes sure scene is only loaded once
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization() {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // increases level count and runs the function that initalises the game
    static private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        instance.level++;
        instance.InitGame();
        Analytics.instance.InitAnalytics();
    }

    // initalises game, including the introductory "day #" message, and running the function to set up a new board
    void InitGame() {
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        restartText = GameObject.Find("RestartText").GetComponent<Text>();
        restartText.gameObject.SetActive(false);
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
    }

    // hides the "day #" image and sets the doingSetup bool to false, so the player can move
    private void HideLevelImage() {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    //*** ENEMY TURN CONTROL ***//



    // Update is called once per frame
    // checks if the enemy can move
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;
        
        StartCoroutine(MoveEnemies());
    }

    // shows the game over screen
    public void GameOver()
    {
        if (level == 1)
        {
            levelText.text = "After " + level + " day, you starved.";
        } else
        {
            levelText.text = "After " + level + " days, you starved.";
        }
       
        levelImage.SetActive(true);
        restartText.gameObject.SetActive(true);
        //enabled = false;
    }

    public void RestartGame()
    {
        instance.level = 0;
        instance.playerFoodPoints = 100;
        
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    // adds enemies on the field to the enemies list
    public void AddEnemyToList(Enemy script) {
		enemies.Add(script);
	}

    public void RemoveEnemyFromList(Enemy script)
    {
        enemies.Remove(script);
    }

	// controls the enemy movement with a delay between each enemy. sets the players turn to true when enemies are done moving
	IEnumerator MoveEnemies() {
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds(turnDelay);
		}

		for (int i = 0; i < enemies.Count; i++) {
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}

		instance.playersTurn = true;
		enemiesMoving = false;
	}
}
