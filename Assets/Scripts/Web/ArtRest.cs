using UnityEngine;
using System.Collections.Generic;
using System.Collections;

using GUI;

public class ArtRest : MonoBehaviour
{
	private List<ArtGUIInterface> allArt;
	private WWW www;
	//thread safe 
	IEnumerator  getAllArt ()
	{
		string url = "http://api.awesomepeople.tv/api/artwork";
		 www = new WWW (url);
		yield return www;
		Debug.Log (www.text );

	
	}

	public void fillCatalogWithAllArt (GUIControl content)
	{
		StartCoroutine (getAllArt());

	}

}