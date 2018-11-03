using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
	
public class AquariusAsciiBinary : MonoBehaviour {
	

	private ListFromTextAsset myList;
	private string biton = "1";
	private string bitoff = "0";
	private int currentChr = 0;
	public List<GameObject> Objects;
	private int slower = 0;
	// Use this for initialization
	void Start () {
		myList = this.GetComponent<ListFromTextAsset>();
		int binarylistlength = myList.item.Count;
		for (int i = 0; i < binarylistlength; i++) {
			 this.GetComponent<ListFromTextAsset>().itemText = myList.item [i];
			GenerateGrid (i);
		}

	}

	public GameObject gridSpace;
	GameObject[,] boardArray = new GameObject[9,9];



	void Update () {
		
		Ray detectobject = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (detectobject, out hit, 100)) 
		{
			Debug.DrawLine (detectobject.origin, hit.point);

		}
	/*	slower++;
		if (slower > 100) {
			currentChr++;
			slower = 0;
		}

		if (currentChr < 21 && currentChr > 10) {
			int oldChr = currentChr - 1;
			this.Objects [oldChr].SetActive(false);
			this.Objects [currentChr].SetActive(true);

			}
		else{currentChr = 1;}

*/
	}

	void GenerateGrid(int n)

	{
		myList = this.GetComponent<ListFromTextAsset>();
		string editingText = myList.itemText;
		int numberOfLetters = 0;
		foreach (char letter in editingText)
		{
			numberOfLetters++;
		}
		GameObject Block = new GameObject ();
		Block.name = n.ToString ();
		int z = 0;
		int x = 0;
		for (int i = 0; i < numberOfLetters; i++) 
		{		//	Debug.Log (i  + " " + x + " " + z);
				char[] currentChr = editingText.ToCharArray ();
				if (currentChr[i].ToString () == "1") {	
				// multiply vector3 x & z * 2 for cubes.
				boardArray [x, z] = Instantiate (gridSpace, new Vector3 (x, 0, z),gridSpace.transform.rotation);
					boardArray [x, z].transform.SetParent(Block.transform);
					boardArray [x, z].name = i.ToString() + "_"+"x" + (x).ToString() + "z" + (z).ToString();
				}
			x++;
			if(x == 9){
				x = 1;
				z++;
				}
		}

		Objects.Add (Block);
		Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/Prefabs/" + Block.name + ".prefab");
		PrefabUtility.ReplacePrefab(Block, prefab);
		AssetDatabase.Refresh();
		Block.SetActive(false);

	}
}
