using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Scanning;
using System.IO;

public class QRGenerator : MonoBehaviour {

    public ArtFilter RequestedArtFilter { get; set; }
    public QRView view;

	public void GenerateQR(Art art) {
		RequestedArtFilter = new ArtFilter();
		RequestedArtFilter.ArtistID = ""+art.owner.ID;
		RequestedArtFilter.ArtId = ""+art.ID;
		//RequestedArtFilter.Genres.Add("testGenre1");
		//RequestedArtFilter.Genres.Add("testGenre2");
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
