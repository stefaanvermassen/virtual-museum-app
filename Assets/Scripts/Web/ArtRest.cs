using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using SimpleJSON;
using UnityEngine.UI;


public class ArtRest : MonoBehaviour
{
	private List<ArtGUIInterface> allArt;
	private WWW www;
	//thread safe 
	IEnumerator  getAllArt (GUIControl content)
	{
		string imageArtworkUrl;
		www = new WWW (BASE_URL+ARTWORK);
		yield return www;
		JSONNode node = JSON.Parse (www.text);
		allArt = new List<ArtGUIInterface> ();
		ArtGUIInterface newArtCatalogItem;
		content.removeAllChildren ();
		//set content position
		foreach (JSONNode child in node.Children) {
			imageArtworkUrl = BASE_URL+ARTWORK + "/" + child ["ArtWorkID"];
			www = new WWW (imageArtworkUrl);
			yield return www;
			newArtCatalogItem = new ArtGUIInterface (child ["ArtWorkID"], child ["ArtistID"], child ["Name"],www.texture);
			allArt.Add (newArtCatalogItem);
			catalogItemFromGUIInterface (newArtCatalogItem,content);
		}

	
	}
	//post the edited art 
	IEnumerator  postArt (GUIControl content)
	{
		string artworkUrl = "http://api.awesomepeople.tv/api/artwork";
		www = new WWW (artworkUrl);
		yield return www;
	}
	public void postArt(ArtGUIInterface art){

	}
	private GUIControl catalogItemFromGUIInterface (ArtGUIInterface art,GUIControl content)
	{
		GUIControl item = GUIControl.init (GUIControl.types.CatalogItem);
		content.add (item);

		//change image
		//get the image from the catalog item
		Image image = item.transform.Find ("Preview").gameObject.GetComponent<Image> ();
		image.enabled = true;
		image.sprite = Sprite.Create (art.Thumbnail,new Rect(0, 0, art.Thumbnail.width, art.Thumbnail.height), Vector2.zero);

		//change text
		Transform inputFields = item.transform.Find ("InputBox/InputFields").transform;
		//name
		InputField field = inputFields.GetChild(0).GetComponent<InputField> ();
		field.text = art.Name;

		//ID
		field = inputFields.GetChild(1).GetComponent<InputField> ();
		field.text = art.Id;
		//artist
		field = inputFields.GetChild(2).GetComponent<InputField> ();
		field.text = art.ArtistID;
		return item;
	}

	public void fillCatalogWithAllArt (GUIControl content)
	{

		StartCoroutine (getAllArt (content));

	}


	private const string BASE_URL = "http://api.awesomepeople.tv/";
	private const string TOKEN = "Token";
	private const string ARTWORK = "api/artwork";
}