using UnityEngine;
using System.Collections;

public class GridGenerator : MonoBehaviour {
	public GameObject gridSpace;
	GameObject[,] boardArray = new GameObject[10,10];


	void Start () {

		GenerateGrid ();
	}
	void Update () {

		Ray detectobject = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (detectobject, out hit, 100)) 
		{
			Debug.DrawLine (detectobject.origin, hit.point);

		}
	}

	void GenerateGrid()

	{

		for (int i = 0; i < boardArray.GetLength(0); i++) 
		{
			for (int j = 0; j < boardArray.GetLength (1); j++) 
			{

				boardArray[i,j] = Instantiate(gridSpace, new Vector3(j * 2,0,i * 2), Quaternion.identity);

			}
		}
	}
}
