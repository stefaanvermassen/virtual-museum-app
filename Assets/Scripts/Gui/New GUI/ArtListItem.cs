using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ArtListItem : MonoBehaviour {
	public int artID;
	public string artTitle;
	public string artArtist;
	public string artDescription;

	public bool owner;

	public Image thumbnail;
	public Image fullResImage;
	public Art artWork;

	public GUIControl artPopUp;

	public GUIControl popUpNormal;
	public Text popUpTitle;
	public Text popUpArtist;
	public Text popUpDescription;
	public Image popUpImage;

	// When you're the owner
	public ArtEditPanel popUpOwner;
	public ArtList list;

	public void UpdateLabels() {
		Text[] labels = GetComponentsInChildren<Text> ();
		foreach (Text label in labels) {
			if(label.name.Contains ("Title")) {
				if(artTitle.Length < 35) {
					label.text = artTitle;
				} else {
					label.text = artTitle.Substring(0,32) + "...";
				}
			} else if(label.name.Contains ("Artist")) {
				if(artArtist.Length < 55) {
					label.text = "by " + artArtist;
				} else {
					label.text = "by " + artArtist.Substring(0,52) + "...";
				}
			} else if(label.name.Contains ("Description")) {
				if(artDescription.Length < 63) {
					label.text = artDescription;
				} else {
					label.text = artDescription.Substring(0,60) + "...";
				}
			} else if(label.name.Contains ("Tag")) {
				if(owner) {
					label.text = "MY ART";
				} else {
					label.text = "";
				}
			}
		}

		if (!owner || Application.loadedLevelName.Equals("BuildMuseum")) {
			ImageHighlightButton butt = GetComponentInChildren<ImageHighlightButton>();
			butt.gameObject.SetActive(false);
		}

		if (artWork.image!= null) {
			SetThumbnail(artWork.image);
		} else if (artWork.imageFile != null) {
			Texture2D texture = new Texture2D (1, 1);
			texture.LoadImage (artWork.imageFile);
			SetThumbnail(texture);
		}

	}

	public void SetThumbnail(Texture2D texture) {
		LayoutElement le = thumbnail.GetComponent<LayoutElement>();
		SetPreferredDimensions (le, texture.width, texture.height, 70, 70);
		thumbnail.type = Image.Type.Simple;
		thumbnail.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		thumbnail.color = Color.white;
	}
	
	public void OnClick() {
		if (!Application.loadedLevelName.Equals ("BuildMuseum")) {
			MainMenuActions actions = FindObjectOfType<MainMenuActions> ();
			if (actions != null) {
				actions.currentArtID = artID;
			}
			if (owner) {
				popUpOwner.gameObject.SetActive (true);
				popUpNormal.gameObject.SetActive (false);
				popUpOwner.artListItem = this;
				artPopUp.FlipCloseOpen ();
			} else {
				popUpOwner.gameObject.SetActive (false);
				popUpNormal.gameObject.SetActive (true);
				artPopUp.FlipCloseOpen ();
				popUpTitle.text = artTitle;
				popUpArtist.text = "by " + artArtist;
				popUpDescription.text = artDescription;
				LoadBigImage (popUpImage, 540, 330);
			}
		} else {
			BuildMuseumActions actions = FindObjectOfType<BuildMuseumActions> ();
			if (actions != null) {
				actions.SetArt (artID);
				actions.artCollectionPopUp.Close ();
			}
		}
	}

	public void LoadBigImage(Image bigImage, int leWidth, int leHeight) {
		SetEmptyBigImage(bigImage, thumbnail.mainTexture.width, thumbnail.mainTexture.height, leWidth, leHeight);

		API.ArtworkSizes size = API.ArtworkSizes.DESKTOP_LARGE;
		if (Screen.height > 1200) {
			size = API.ArtworkSizes.DESKTOP_LARGE;
		} else {
			size = API.ArtworkSizes.MOBILE_LARGE;
		}
		API.ArtworkController control = API.ArtworkController.Instance;
		control.GetArtworkData(artID.ToString(), (art) => {
			Texture2D texture = new Texture2D(1, 1);
			texture.LoadImage(art);
			SetBigImage (bigImage, texture, leWidth, leHeight);
		},
		(error) => {
			print ("Error loading big image");
		}, 
		size);
	}

	public static void SetEmptyBigImage(Image bigImage, int unscaledWidth, int unscaledHeight, int leWidth, int leHeight) {
		Image bigImageContainer = bigImage.transform.parent.GetComponent<Image> ();
		bigImage.sprite = bigImageContainer.sprite;
		bigImage.type = bigImageContainer.type;
		bigImage.color = bigImageContainer.color;
		LayoutElement le = bigImage.GetComponent<LayoutElement>();
		SetPreferredDimensions (le, unscaledWidth, unscaledHeight, leWidth, leHeight);
	}

	public static void SetBigImage(Image bigImage, Texture2D texture, int leWidth, int leHeight) {
		LayoutElement le = bigImage.GetComponent<LayoutElement>();
		SetPreferredDimensions (le, texture.width, texture.height, leWidth, leHeight);
		bigImage.type = Image.Type.Simple;
		bigImage.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), Vector2.zero);
		bigImage.color = Color.white;
	}

	public static void SetPreferredDimensions(LayoutElement le, int width, int height, int maxWidth, int maxHeight) {
		float leWidth = (float)width;
		float leHeight = (float)height;
		float leMaxWidth = (float)maxWidth;
		float leMaxHeight = (float)maxHeight;
		if (leWidth != leMaxWidth) {
			leHeight *= leMaxWidth/leWidth;
			leWidth = leMaxWidth;
		}
		if (leHeight > leMaxHeight) {
			leWidth *= leMaxHeight / leHeight;
			leHeight = leMaxHeight;
		}
		le.preferredWidth = (int)leWidth;
		le.preferredHeight = (int)leHeight;
		RectTransform[] rts = le.GetComponentsInChildren<RectTransform> ();
		foreach (RectTransform rt in rts) {
			rt.sizeDelta = new Vector2(leWidth, leHeight);
		}
	}

	public void ShowQR() {
		list.generator.GenerateQR (artWork);
		list.popUpQR.FlipCloseOpen ();
	}
}
