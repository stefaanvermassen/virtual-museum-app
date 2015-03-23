using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;


public class ArtRest : MonoBehaviour
{
	private List<ArtGUIInterface> allArt;
	private WWW www;
	//thread safe 
	IEnumerator  getAllArt (GUIControl content)
	{
		string imageArtworkUrl;
		API.UserController uc = API.UserController.Instance;
		API.ArtworkController ac = API.ArtworkController.Instance;
		allArt = new List<ArtGUIInterface> ();
		ArtGUIInterface newArtCatalogItem;
		content.removeAllChildren ();
		ac.getAllArtworks (success: (response) => {
			foreach(API.ArtWork child in response) {
				ac.getArtwork(child.ArtWorkID.ToString(), success:(texture) => {
					newArtCatalogItem = new ArtGUIInterface (child.ArtWorkID.ToString(), child.ArtistID.ToString(), child.Name, texture);
					allArt.Add (newArtCatalogItem);
					catalogItemFromGUIInterface (newArtCatalogItem,content);
				}, error:(error) => {Debug.Log("An error occured while loading artwork with ID: " + child.ArtWorkID.ToString());});

			}
		},
							error: (error) => {
			Debug.Log("An error occured while loading all artworks");
		}); 

		yield return null;
	}
	//post the edited art 
	IEnumerator  postArt (GUIControl content)
	{
		yield return null;
	}
	public void postArt(ArtGUIInterface art){
		API.ArtWork artWork = new API.ArtWork () {
			ArtWorkID = 1,
			ArtistID = 1,
			Name = "Feliciaan"
		};
		API.ArtworkController ac = API.ArtworkController.Instance;
		ac.updateArtWork (artWork, ((response) => {
			Debug.Log ("Updating Artwork successfull");}), 
		                  ((error) => {
			Debug.Log ("An error occured");}));
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