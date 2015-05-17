using UnityEngine;
using System.Collections;
using Scanning;
using System.Threading;
using UnityEngine.UI;

public class QRCam : StatisticsBehaviour {

    public Texture2D Encoded;
    public WebCamTexture CamTexture;
    private Rect PaneRect;
    private QRScanner Scanner = new QRScanner();
	public Image view;
	public MeshRenderer quad;
	public Thread ScanThread;
	public Toast toast;


    void OnGUI()
    {
        //GUI.DrawTexture(PaneRect, CamTexture, ScaleMode.ScaleAndCrop);
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
			quad.material.SetTexture(0,CamTexture);
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

	  StartScanning ();
      
      StartStatistics("QRCam");
   }

	void StartScanning(){
		Scanner.done = false;
		ScanThread = new Thread (Scanner.Scan);
		ScanThread.Start ();
		AsyncLoader.CreateAsyncLoader (
		startup: () => {},
		isDone: () => {return Scanner.done;},
		whileLoading: () => {},
		whenDone: () => {
			if(Scanner.success){
				toast.Notify("Virtual Museum Code detected! Art has been added to your collection!");
				var filter = new ArtFilter();
				filter.Configure(Scanner.code);
				filter.Collect();
               	var cc = API.CreditController.Instance;
        		var sharedlinkcreditmodel = new API.CreditModel(){ Action = API.CreditActions.SCANNEDARTWORK};
        		cc.AddCredit(sharedlinkcreditmodel, (info) => {
        			if (info.CreditsAdded) { // check if credits are added
        				toast.Notify("Thank you for scanning an artwork. Your total amount of tokens is: " + info.Credits);
        			} else {
        				Debug.Log("No tokens added for new build museum action.");
        			}
        		}, (error) => {
        			Debug.Log("An error occured when adding tokens for the user.");
        		});
				StartScanning();
			}else{
				toast.Notify("This is not a Virtual Museum Code, please try again!");
				StartScanning();
			}
		},
		interval: 10);
	}

    void Update()
    {
        if (Scanner.Color == null) //scanner.Color is set to null when no QR code could be found and decoded
        {
            Scanner.Color = CamTexture.GetPixels32();
        }
    }

	public void Back(){
		Application.LoadLevel ("MainMenuScene");
	}
}
