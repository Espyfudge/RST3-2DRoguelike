using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

	public int playerDamage;
	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;
    public int hp;

	private Animator animator;
	private Transform target;
	private bool skipMove;

	// basic enemy setup
	protected override void Start () {
		GameManager.instance.AddEnemyToList(this);
		animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
        if (gameObject.name.Contains("Enemy1"))
            hp = 2;
        if (gameObject.name.Contains("Enemy2"))
            hp = 3;
		base.Start();
	}

    public void quickRemove()
    {
        GameManager.instance.RemoveEnemyFromList(this);
    }

	//*** ENEMY MOVEMENT ***//



	// basic enemy movement, makes sure enemies can only move once every other turn
	protected override void AttemptMove <T> (int xDir, int yDir) {
		if (skipMove) {
			skipMove = false;
			return;
		}

		base.AttemptMove <T> (xDir, yDir);

		skipMove = true;
	}

	// moves enemies towards the player
	public void MoveEnemy() {
		int xDir = 0;
		int yDir = 0;

		if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon) {
			yDir = target.position.y > transform.position.y ? 1 : -1;
		}
		else {
			xDir = target.position.x > transform.position.x ? 1 : -1;
		}

		AttemptMove <Player> (xDir, yDir);
	}



	//*** ENEMY COMBAT ***//



	// controls enemy attacking player
	// will activate if enemy is next to player
	protected override void OnCantMove <T> (T component) {
		Player hitPlayer = component as Player;

		animator.SetTrigger("enemyAttack");

		SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

		hitPlayer.LoseFood(playerDamage);
	}

}
