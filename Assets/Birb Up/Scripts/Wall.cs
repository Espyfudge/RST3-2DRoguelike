using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

	public Sprite dmgSprite;
	public int hp = 4;
	public AudioClip chopSound1;
	public AudioClip chopSound2;

	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}



	//*** WALL DESTRUCTION MANAGEMENT **//



	// runs when a player walks into an inner wall
	public void DamageWall (int loss) {
		SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
		spriteRenderer.sprite = dmgSprite;
		hp -= loss; // reduces hp every time the player runs into (and therefore chops) the wall
		if (hp <= 0) {
			gameObject.SetActive(false); // sets the gameobject to inactive once its hp falls below 1, so that the player can pass through
		}
	}
}
