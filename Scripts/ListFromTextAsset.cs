using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

[RequireComponent(typeof(TextMesh))]

[ExecuteInEditMode]
public class ListFromTextAsset : MonoBehaviour
{
	public TextAsset TextFile;
    [SerializeField]
    private TextAsset oldTextFile;
	public bool LoadItems;
    public bool hasInit ;
    private string theWholeFileAsOneLongString;
	public List<string> item;
   
    public int currentItem = 0;
    private int previousItem = 0;

    private int oldItemCount = 0;

    [Multiline]
	public string itemText;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float percentOfItems;
    private float previousPercent;
 
    public  void Init()
	{
		if (TextFile != null)
		{
            oldTextFile = TextFile;
			//get the whole file
			theWholeFileAsOneLongString = TextFile.text;
			//get the table name
			string[] tableInput = theWholeFileAsOneLongString.Split("="[0]);         
			this.gameObject.name = tableInput[0];
			if (tableInput.Length >= 1)
			{                
				// population the data list
				item = new List<string>();
				//replace spaces
				tableInput[1] = tableInput[1].Replace(", ", ",");
				tableInput[1] = tableInput[1].Replace(" ,", ",");
				//replace line feeds
				tableInput[1] = tableInput[1].Replace("\n", "");
				// add items to list by spliting string by comma delimiter
				item.AddRange(tableInput[1].Split(","[0]));
				// iterate over items
				for (var i = 0; i < item.Count; i++)
				{
					//strip extra lua brackets 
					item[i] = item[i].Replace("{", "");
					item[i] = item[i].Replace("}", "");
					// remove extra quotes
					item[i] = item[i].Replace("\"", "");
					//remove leading space
					if (item[i].StartsWith(" ")){item[i] = item[i].Substring(1,item[i].Length-1);}
				}
                itemText =  item[0];
				hasInit = true;
                LoadItems = false;
				int kWords =  item.Count;
                currentItem = kWords;
            }
		}
        else
        {
            hasInit = false;
            LoadItems = false;
            currentItem = 0;
            percentOfItems = 0.0f;
        }
	}

	// only works in editor
	  void ListFolder() {
        string currentFile = AssetDatabase.GetAssetPath(TextFile);           
        string currentFolder = Path.GetDirectoryName(currentFile);
        DirectoryInfo dir = new DirectoryInfo(currentFolder);
       // Debug.Log(dir);
        FileInfo[] info = dir.GetFiles("*.*");
        foreach (FileInfo f in info)
        {
        }
    }

	void TextMeshUpdate(int i)
	{
        
        if (i >= item.Count-1) { i = item.Count-1; }
        currentItem = i;
      
        itemText = item[i];
		TextMesh tTextmesh = this.GetComponent<TextMesh>();
		if (!tTextmesh) { this.gameObject.AddComponent<TextMesh>(); }
		tTextmesh.alignment = TextAlignment.Center;
		if (tTextmesh == null){ return; }
			tTextmesh.text = itemText;
	}

	void  incrementcurrentItem(int currentItem)
	{
		if (currentItem > item.Count - 1 || currentItem <=-1)
		{
			currentItem = 0;
		}
		else
		{
			currentItem++;
		}     
	}

	void Update() {

        if (oldTextFile != TextFile)
        {
           
            hasInit = false;
            LoadItems = true;
        }
      

        if (LoadItems == true && hasInit == false)
		{  
        Init();
		}

        if (oldItemCount != item.Count)
        {
            oldItemCount = item.Count;
        }

		if (hasInit = true)
        {

            if (previousPercent != percentOfItems)
            {
                previousPercent = percentOfItems;
                currentItem = Mathf.FloorToInt(percentOfItems * item.Count);
                
            }
            
            if (previousItem != currentItem)
            {
                previousItem = currentItem;
                TextMeshUpdate(currentItem);
            }
           
        }     

	}

}
