using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QRView : StatisticsBehaviour
{
    public GameObject panel;
    public Texture2D image;
    public Image QR;

    public bool set;

    /// <summary>
    /// Initialize the QR view
    /// </summary>
    protected void Start()
    {
        set = false;
        StartStatistics("QR");
    }

    /// <summary>
    /// Draws the image
    /// </summary>
    private void Update()
    {
        if (!set && image != null) {
			set = true;
			QR.type = Image.Type.Simple;
			QR.color = Color.white;
			QR.sprite = Sprite.Create (image, new Rect (0, 0, image.width, image.height), Vector2.zero);
		} else if(!set && panel == null) {
			Image imageContainer = QR.transform.parent.GetComponent<Image> ();
			QR.sprite = imageContainer.sprite;
			QR.type = imageContainer.type;
			QR.color = imageContainer.color;
		}
    }

    /// <summary>
    /// Closes the panel
    /// </summary>
    /// <param name="useless">Useless parameter, needed for onClickListener</param>
    public void Close(int useless)
    {
        if(panel != null) panel.SetActive(false);
        ClosingButton("Close");
    }

    /// <summary>
    /// Closes the panel and saves the QRcode
    /// </summary>
    /// <param name="useless">Useless parameter, needed for onClickListener</param>
    public void Save(int useless)
    {
        // TODO: Implement save
        //panel.SetActive(false);
        ClosingButton("Save");
    }

	public void Save() {
		Save (0);
	}

	public void Close() {
		Close (0);
	}
}
