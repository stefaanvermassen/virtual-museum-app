using UnityEngine;
using System.Collections;
using Scanning;

public class QRGenerator : MonoBehaviour {

    public ArtFilter RequestedArtFilter { get; set; }
    public Color32[] QRCodeImage;

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
        QRCodeImage = qrCode.Image;
        Debug.Log("QRCode image required");

        Texture2D qrTexture = new Texture2D(256, 256);
        Debug.Log("number of pixels in array:" + QRCodeImage.Length);
        qrTexture.SetPixels32(0,0,256,256,QRCodeImage);

        GUI.DrawTexture(new Rect(0, 0, 256, 256), qrTexture, ScaleMode.ScaleToFit, true, 10.0f);
        Debug.Log("Texture drawn");
    }

	// Update is called once per frame
	void Update () {
	    //this method can be left empty
	}
}
