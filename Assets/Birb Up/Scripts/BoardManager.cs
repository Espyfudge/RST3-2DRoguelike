using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

	[Serializable]
	public class Count {
		public int minimum;
		public int maximum;

		public Count (int min, int max) {
			minimum = min;
			maximum = max;
		}
	}

	public int columns = 8;
	public int rows = 8;
	public Count wallCount = new Count(5,8);
	public Count foodCount = new Count(1,4);
    public Count ammoCount = new Count(1, 3);
    public Count weaponCount = new Count(0,2);
	public GameObject exit;
	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] foodTiles;
    public GameObject[] ammoTiles;
    public GameObject[] weaponTiles;
	public GameObject[] enemyTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;
	private List <Vector3> gridPositions = new List<Vector3>();



	//*** PLAYING FIELD CREATION ***//



	// creates a 6x6 grid on the playing field
	void InitialiseList() {
		gridPositions.Clear();

		for (int x = 1; x < columns - 1; x++) {
			for (int y = 1; y < rows - 1; y++) {
				gridPositions.Add(new Vector3(x,y,0f));
			}
		}
	}

	// places the floor and outer wall tiles in the correct positions
	void BoardSetup() {
		boardHolder = new GameObject("Board").transform;

		for (int x = -1; x < columns + 1; x++) {
			for (int y = -1; y < rows + 1; y++) {
				GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
					if (x == -1 || x == columns || y == -1 || y == rows) {
						toInstantiate = outerWallTiles[Random.Range(0,outerWallTiles.Length)];
					}

					GameObject instance = Instantiate(toInstantiate, new Vector3(x,y,0f), Quaternion.identity) as GameObject;

					instance.transform.SetParent(boardHolder);
			}
		}
	}

	// returns a random position on the 6x6 grid
	Vector3 RandomPosition() {
		int randomIndex = Random.Range(0,gridPositions.Count);
		Vector3 randomPosition = gridPositions[randomIndex];
		gridPositions.RemoveAt(randomIndex); // removes this position from the list of gridpositions so that other items cant be placed on top of it
		return randomPosition;
	}

	// randomly positions tiles from tileArray onto grid
	void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum) {
		int objectCount = Random.Range(minimum, maximum + 1);

		for (int i = 0; i < objectCount; i++) {
			Vector3 randomPosition = RandomPosition(); // the random position where the tile will be placed is found using previous function
			GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
			Instantiate (tileChoice, randomPosition, Quaternion.identity);
		}
	}

	// initialises the entire board using all aforementioned functions
	// creates an 8x8 board with borders, inner wall tiles, food tiles, and enemies
	public void SetupScene(int level) {
		BoardSetup();
		InitialiseList();
		LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
		LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        LayoutObjectAtRandom(ammoTiles, ammoCount.minimum, ammoCount.maximum);
        LayoutObjectAtRandom(weaponTiles, weaponCount.minimum, weaponCount.maximum);
		int enemyCount = (int)Mathf.Log(level, 2f);
		LayoutObjectAtRandom(enemyTiles,enemyCount, enemyCount);
		Instantiate(exit, new Vector3(columns - 1, rows - 1 ,0F), Quaternion.identity);
	}
}
