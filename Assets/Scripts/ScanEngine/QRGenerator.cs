using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scanning;
using System.IO;

public class QRGenerator : MonoBehaviour {

    public ArtFilter RequestedArtFilter { get; set; }
    public QRView view;

    public QRGenerator()
    {
        RequestedArtFilter = new ArtFilter();
        RequestedArtFilter.ArtistName = "testArtist";
        RequestedArtFilter.Tags.Add("testTag1");
        RequestedArtFilter.Tags.Add("testTag2");
        RequestedArtFilter.Genres.Add("testGenre1");
        RequestedArtFilter.Genres.Add("testGenre2");
    }

    void Start()
    {
		GenerateQR ();
    }

	public void GenerateQR() {
		view.image = null;
		view.set = false;
		QRScanner scanner = new QRScanner();
		QRCode qrCode = (QRCode)scanner.MakeScannable(null, RequestedArtFilter);
		Color32[] QRCodeImage = qrCode.Image;
		Texture2D qrTexture = new Texture2D(256, 256);
		qrTexture.SetPixels32(QRCodeImage);
		Color32[] pixels = qrTexture.GetPixels32();
		view.image = qrTexture;
		qrTexture.Apply();
	}


}
