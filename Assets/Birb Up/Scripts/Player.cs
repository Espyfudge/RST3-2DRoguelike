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
    public Text ammoText;
    public Text pistolText;
    public Text shotgunText;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	private Animator animator;
	public int food;
	private Vector2 touchOrigin = -Vector2.one;
    private int ammunition;
    private int pistolUses;
    private int shotgunUses;

    private KeyCode pistol = KeyCode.Alpha1;
    private KeyCode shotgun = KeyCode.Alpha2;
    private int facing = 1;
    private SpriteRenderer sr;
    private int blockLayer;
    

	// Use this for initialization
	protected override void Start () {

        
		animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
		food = GameManager.instance.playerFoodPoints;
        ammunition = GameManager.instance.playerAmmo;
        pistolUses = GameManager.instance.playerPistol;
        shotgunUses = GameManager.instance.playerShotgun;

        blockLayer = (1 << 8);

		foodText.text = "Food: " + food;
        ammoText.text = "Bullets: " + ammunition;
        pistolText.text = "[1]\nPistol: " + pistolUses;
        shotgunText.text = "[2]\nShotgun: " + shotgunUses;

        base.Start();
	}



    //*** MOVEMENT ***//



    // Update is called once per frame
    void Update() {

        Debug.Log(GameManager.instance.playersTurn);
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

            ShootEnemy();
        }

        
	}

	// runs when a player moves, consumes food and moves them in the direction
	protected override void AttemptMove <T> (int xDir, int yDir) {
		food--;
		foodText.text = "Food: " + food;
        ammoText.text = "Bullets: " + ammunition;
        pistolText.text = "[1]\nPistol: " + pistolUses;
        shotgunText.text = "[2]\nShotgun: " + shotgunUses;

        base.AttemptMove <T> (xDir, yDir);

		RaycastHit2D hit;
		if (Move(xDir, yDir, out hit)) {
			SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
		}

        if ( (xDir < 0) && (facing == 1) )
        {
            sr.flipX = true;
            facing = -1;
        }

        if ( (xDir > 0) && (facing == -1) )
        {
            sr.flipX = false;
            facing = 1;
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

        // when creating inventory change this so that the icon displays in the inventory bar with amount of food (use only fruit icon to represent both)
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

        if (other.tag == "Ammo")
        {
            // when creating inventory, change this so that the icon displays in the inventory bar with the amount
            ammunition += 3;
            ammoText.text = "+3 Bullets: " + ammunition;
            //SoundManager.instance.RandomizeSfx(ammoSound1, ammoSound2);
            Analytics.instance.ammo++;
            other.gameObject.SetActive(false);
        }

        if ( (other.tag == "Weapon") && (other.name.Contains("Pistol")) ) 
        {
            Analytics.instance.pistol++;
            pistolUses += 5;
            pistolText.text = "[1]\n+5 Pistol: " + pistolUses;
            other.gameObject.SetActive(false);
        }

        if ( (other.tag == "Weapon") && (other.name.Contains("Shotgun")) )
        {
            Analytics.instance.shotgun++;
            shotgunUses += 5;
            shotgunText.text = "[2]\n+5 Shotgun: " + shotgunUses;
            other.gameObject.SetActive(false);
        }
	}

    private void ShootEnemy()
    {
        if ( (Input.GetKeyDown(pistol)) && (ammunition > 0) && (pistolUses > 0))
        {
            Analytics.instance.pused++;
            animator.SetTrigger("playerShoot1");
            ammunition--;
            pistolUses--;
            ammoText.text = "Bullets: " + ammunition;
            pistolText.text = "[1]\nPistol: " + pistolUses;

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.right * facing, Mathf.Infinity , blockLayer);
            if ( hit.collider != null )
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.tag == "Enemy")
                {
                    Debug.Log("hit!");
                    hit.transform.GetComponent<Enemy>().hp--;
                    if (hit.transform.GetComponent<Enemy>().hp <= 0 )
                    {
                        if (hit.transform.gameObject.name.Contains("Enemy1"))
                            Analytics.instance.en1++;
                        if (hit.transform.gameObject.name.Contains("Enemy2"))
                            Analytics.instance.en2++;
                        hit.transform.GetComponent<Enemy>().quickRemove();
                        hit.transform.gameObject.SetActive(false);
                    }

                }
            }
            GameManager.instance.playersTurn = false;
        }

        if ((Input.GetKeyDown(shotgun)) && (ammunition > 1) && (shotgunUses > 0))
        {
            Analytics.instance.sused++;
            animator.SetTrigger("playerShoot2");
            ammunition -= 2;
            shotgunUses--;
            ammoText.text = "Bullets: " + ammunition;
            shotgunText.text = "[2]\nShotgun: " + shotgunUses;

            RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.right * facing, Mathf.Infinity, blockLayer);
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.name);
                if (hit.transform.tag == "Enemy")
                {
                    Debug.Log("hit!");
                    hit.transform.GetComponent<Enemy>().hp -= 2;
                    if (hit.transform.GetComponent<Enemy>().hp <= 0)
                    {
                        if (hit.transform.gameObject.name.Contains("Enemy1"))
                            Analytics.instance.en1++;
                        if (hit.transform.gameObject.name.Contains("Enemy2"))
                            Analytics.instance.en2++;
                        hit.transform.GetComponent<Enemy>().quickRemove();
                        hit.transform.gameObject.SetActive(false);
                    }

                }
            }
            GameManager.instance.playersTurn = false;
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
            food = 100;
            ammunition = 0;
            pistolUses = 0;
            shotgunUses = 0;
		}
	}

	public void OnDisable() {
		GameManager.instance.playerFoodPoints = food;
        GameManager.instance.playerAmmo = ammunition;
        GameManager.instance.playerPistol = pistolUses;
        GameManager.instance.playerShotgun = shotgunUses;
	}
}
