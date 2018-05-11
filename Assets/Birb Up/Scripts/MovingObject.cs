using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//abstract class to be used as base for both player and enemy movement scripts
public abstract class MovingObject : MonoBehaviour {

	public float moveTime = 0.1f;
	public LayerMask blockingLayer;

	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;



	//*** MOVEMENT BASE ***//



	// Use this for initialization
	protected virtual void Start () {
		boxCollider = GetComponent<BoxCollider2D>();
		rb2D = GetComponent<Rigidbody2D>();
		inverseMoveTime = 1f / moveTime;
	}

	// sets up the smooth movement from the start position to the end position
	protected bool Move (int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2(xDir, yDir);

		boxCollider.enabled = false; // temporarily disables box collider so that it wont collide with itself or other colliders while moving
		hit = Physics2D.Linecast (start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			StartCoroutine(SmoothMovement(end));
			return true;
		}

		return false;
	}

	// moves the character smoothly from its current position to the end position
	protected IEnumerator SmoothMovement(Vector3 end) {
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition(newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	// checks for possibility of movement
	protected virtual void AttemptMove <T> (int xDir, int yDir)
		where T : Component {
		RaycastHit2D hit;
		bool canMove = Move (xDir, yDir, out hit);

		if (hit.transform == null) {
			return;
		}

		T hitComponent = hit.transform.GetComponent<T>();

		if (!canMove && hitComponent != null) {
			OnCantMove(hitComponent);
		}
	}

	// sets up OnCantMove function to be declared in character scripts
	protected abstract void OnCantMove <T> (T component)
		where T : Component;
}
