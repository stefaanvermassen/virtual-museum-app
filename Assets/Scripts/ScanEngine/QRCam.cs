using UnityEngine;
using System.Collections;
using Scanning;
using System.Threading;
using UnityEngine.UI;

public class QRCam : MonoBehaviour {

    public Texture2D encoded;
    public WebCamTexture camTexture;
    public Image image;
    private Rect paneRect;
    private QRScanner scanner = new QRScanner();
    private Thread scanThread;


    void onGUI()
    {
        GUI.DrawTexture(paneRect, camTexture, ScaleMode.ScaleAndCrop);
    }

	// Use this for initialization
    void OnEnable()
    {
        if (camTexture != null)
        {
            camTexture.Play();
            scanner.Width = camTexture.width;
            scanner.Height = camTexture.height;
        }
        
	}

    void OnDisable()
    {
        if (camTexture != null)
        {
            camTexture.Pause();
        }
    }

    void OnDestroy()
    {
        scanThread.Abort();
        camTexture.Stop();
    }

    // It's better to stop the thread by itself rather than abort it.
    void OnApplicationQuit()
    {
        scanner.IsQuit = true;
    }
    
    void Start()
   {

      encoded = new Texture2D(256, 256);

      paneRect = new Rect(0, 0, Screen.width, Screen.height); //is this the panel or the entire screen?

      camTexture = new WebCamTexture();
      camTexture.requestedHeight = Screen.height; // 480;
      camTexture.requestedWidth = Screen.width; //640;

      OnEnable();
      image.material.mainTexture = camTexture;

      scanThread = new Thread(scanner.Scan);
      scanThread.Start();
   }

    void Update()
    {
        if (scanner.Color == null) //scanner.Color is set to null when no QR code could be found and decoded
        {
            scanner.Color = camTexture.GetPixels32();
        }
    }
}
