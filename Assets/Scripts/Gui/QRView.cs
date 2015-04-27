using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QRView : StatisticsBehaviour
{
    public GameObject panel;
    public Texture2D image;
    public Image QR;
    private bool set;

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
        if (!set && image != null)
        {
            set = true;
            QR.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
        }
    }

    /// <summary>
    /// Closes the panel
    /// </summary>
    /// <param name="useless">Useless parameter, needed for onClickListener</param>
    public void Close(int useless)
    {
        panel.SetActive(false);
        ClosingButton("Close");
    }

    /// <summary>
    /// Closes the panel and saves the QRcode
    /// </summary>
    /// <param name="useless">Useless parameter, needed for onClickListener</param>
    public void Save(int useless)
    {
        // TODO: Implement save
        panel.SetActive(false);
        ClosingButton("Save");
    }
}
