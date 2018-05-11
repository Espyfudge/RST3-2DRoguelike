using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject {

	public int wallDamage = 1;
	public int pointsPerFood = 10;
	public int pointsPerSoda = 20;
	public float restartLevelDelay = 1f;
	public Text foodText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	private int food;
	private Vector2 touchOrigin = -Vector2.one;

	// Use this for initialization
	protected override void Start () {
		animator = GetComponent<Animator>();

		food = GameManager.instance.playerFoodPoints;

		foodText.text = "Food: " + food;

		base.Start();
	}



	//*** MOVEMENT ***//



	// Update is called once per frame
	void Update () {
		//checks if itś the player's turn
		if (!GameManager.instance.playersTurn) return;
		// controls the movement for the player
		int horizontal = 0;
		int vertical = 0;

		// controls movement for unity standalone or webplayer build, as well as editor.
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		if (horizontal != 0) {
			vertical = 0;
		}

		// controls movement for android and iOS devices.
		#else

		if (Input.touchCount > 0) {
			Touch myTouch = Input.touches[0];

			if (myTouch.phase == TouchPhase.Began) {
				touchOrigin = myTouch.position;
			}

			else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0) {
				Vector2 touchEnd = myTouch.position;
				float x = touchEnd.x - touchOrigin.x;
				float y = touchEnd.y - touchOrigin.y;
				touchOrigin.x = -1;
				if (Mathf.Abs(x) > Mathf.Abs(y)) {
					horizontal = x > 0 ? 1 : -1;
				}
				else {
					vertical = y > 0 ? 1 : -1;
				}
			}
		}

		#endif
		if (!GameManager.instance.doingSetup) {
			if ( horizontal != 0 || vertical != 0) {
				AttemptMove<Wall> (horizontal, vertical);
			}
		}
	}

	// runs when a player moves, consumes food and moves them in the direction
	protected override void AttemptMove <T> (int xDir, int yDir) {
		food--;
		foodText.text = "Food: " + food;

		base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move(xDir, yDir, out hit)) {
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

		CheckIfGameOver();

		GameManager.instance.playersTurn = false;
	}

	//damages the wall when the player walks into it
	protected override void OnCantMove <T> (T component) {
		Wall hitWall = component as Wall;
		hitWall.DamageWall(wallDamage);
		animator.SetTrigger("playerChop");
	}



	//*** FOOD AND ENDGAME ***//



	// detects whether the player has moved into a square containing food, soda, or the exit
	private void OnTriggerEnter2D (Collider2D other) {
		// activates the function that reloads the scene
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		}

		if (other.tag == "Food") { // gives the player extra food points on picking up food
			food += pointsPerFood;
			foodText.text = "+" + pointsPerFood + " Food: " + food;
			SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
			other.gameObject.SetActive(false);
		}
		else if (other.tag == "Soda") { // gives the player extra food points on picking up soda
			food += pointsPerSoda;
			foodText.text = "+" + pointsPerSoda + " Food: " + food;
			SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive(false);
		}
	}

	// reloads the scene
	private void Restart() {
		SceneManager.LoadScene(0);
	}

	// determines the loss of food when attacked by an enemy
	public void LoseFood (int loss) {
		animator.SetTrigger("playerHit");
		food -= loss;
		foodText.text = "-" + loss + " Food: " + food;
		CheckIfGameOver();
	}

	private void CheckIfGameOver() {
		if (food <= 0) {
			SoundManager.instance.PlaySingle(gameOverSound);
			SoundManager.instance.musicSource.Stop();
			GameManager.instance.GameOver();
		}
	}

	private void OnDisable() {
		GameManager.instance.playerFoodPoints = food;
	}
}
