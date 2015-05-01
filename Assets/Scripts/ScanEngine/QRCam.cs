using UnityEngine;
using System.Collections;
using Scanning;
using System.Threading;
using UnityEngine.UI;

public class QRCam : MonoBehaviour {

    public Texture2D Encoded;
    public WebCamTexture CamTexture;
    private Rect PaneRect;
    private QRScanner Scanner = new QRScanner();
    private Thread ScanThread;
	public QRView view;


    void OnGUI()
    {
        GUI.DrawTexture(PaneRect, CamTexture, ScaleMode.ScaleAndCrop);
    }

	// Use this for initialization
    void OnEnable()
    {
        if (CamTexture != null)
        {
            CamTexture.Play();
            Scanner.Width = CamTexture.width;
            Scanner.Height = CamTexture.height;
			Scanner.Color = CamTexture.GetPixels32();
        }
        
	}

    void OnDisable()
    {
        if (CamTexture != null)
        {
            CamTexture.Pause();
        }
    }

    void OnDestroy()
    {
        ScanThread.Abort();
        CamTexture.Stop();
    }

    // It's better to stop the thread by itself rather than abort it.
    void OnApplicationQuit()
    {
        Scanner.IsQuit = true;
    }
    
    void Start()
   {

	  Encoded = new Texture2D(256, 256);

	  PaneRect = new Rect(0, 0, Screen.width, Screen.height); //is this the panel or the entire screen?

	  CamTexture = new WebCamTexture();
	  CamTexture.requestedHeight = Screen.height; // 480;
	  CamTexture.requestedWidth = Screen.width; //640;
		

	  OnEnable();


	  ScanThread = new Thread(Scanner.Scan);
	  ScanThread.Start();
   }

    void Update()
    {
        if (Scanner.Color == null) //scanner.Color is set to null when no QR code could be found and decoded
        {
            Scanner.Color = CamTexture.GetPixels32();
        }
    }
}
