using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	public GameObject gameManager;
    public GameObject analyticsManager;
	// Instantiates the game manager
	void Awake () {
		if (GameManager.instance == null) {
			Instantiate(gameManager);
		}

        if (Analytics.instance == null)
        {
            Instantiate(analyticsManager);
        }
	}
}
