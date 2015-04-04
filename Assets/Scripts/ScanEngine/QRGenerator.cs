using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scanning;

public class QRGenerator : MonoBehaviour {

    public ArtFilter RequestedArtFilter { get; set; }
    public Image GUIImage;

    public QRGenerator(ArtFilter filter)
    {
        RequestedArtFilter = filter;
        
    }

	// Use this for initialization
    void Start()
    {
        Debug.Log("Start is called");
        RequestedArtFilter = new ArtFilter();
        RequestedArtFilter.ArtistName = "testArtist";
        RequestedArtFilter.Tags.Add("testTag1");
        RequestedArtFilter.Tags.Add("testTag2");
        RequestedArtFilter.Genres.Add("testGenre1");
        RequestedArtFilter.Genres.Add("testGenre2");
        Debug.Log("ArtFilter made");
        QRScanner scanner = new QRScanner();
        QRCode qrCode = (QRCode)scanner.MakeScannable(null, RequestedArtFilter);
        Color32[] QRCodeImage = qrCode.Image;
        Debug.Log("QRCode image required");


        Texture2D qrTexture = new Texture2D(256, 256);
        Debug.Log("number of pixels in array:" + QRCodeImage.Length);
        qrTexture.SetPixels32(0,0,256,256,QRCodeImage);

        Debug.Log("Got image:" + GUIImage.minHeight);
        GUIImage.color = Color.blue;
        GUIImage.sprite = Sprite.Create(qrTexture, new Rect(0, 0, 256, 256), Vector2.zero);
    }

	// Update is called once per frame
	void Update () {
	    //this method can be left empty
	}
}
