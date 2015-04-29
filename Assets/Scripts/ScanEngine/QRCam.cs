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
		
		//TEST CODE HERE

		API.ArtworkFilterController c = API.ArtworkFilterController.Instance;
		API.ArtWorkFilter f = new API.ArtWorkFilter ();
		f.ArtistID = 1;

		Hashtable t = new Hashtable ();
		t.Add ("tag", "collection1");
		ArrayList l = new ArrayList ();
		l.Add (t);
		f.Values = l;

		HTTP.Request r = c.CreateArtWorkFilter (f);

		c.CreateArtWorkFilter (f, 
		                    (art)=> {
			Debug.Log ("Filter added");
		}, 
		(error) => {
			Debug.Log ("Failed to add filter.");
		}
		);

		//END TEST CODE HERE


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
