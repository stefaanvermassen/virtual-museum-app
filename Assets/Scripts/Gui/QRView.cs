using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QRView : MonoBehaviour
{
    public GameObject panel;
    public Texture2D image;
    public Image QR;
    private bool set;
    private Statistics statistics;
    private string screenName = "QR";

    /// <summary>
    /// Initialize the QR view
    /// </summary>
    private void Start()
    {
        set = false;
        QR.enabled = false;
        
        // Register screen
        statistics = Statistics.getInstance();
        statistics.RegisterScreen(screenName);
    }

    /// <summary>
    /// Draws the image
    /// </summary>
    private void Update()
    {
        if (!set && image != null)
        {
            set = true;
            QR.enabled = true;
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

        // Remove screen and report the button clicked
        statistics.ButtonClicked("Close");
        statistics.RemoveScreen();

    }

    /// <summary>
    /// Closes the panel and saves the QRcode
    /// </summary>
    /// <param name="useless">Useless parameter, needed for onClickListener</param>
    public void Save(int useless)
    {
        // TODO: Implement save
        panel.SetActive(false);

        // Report button clicked
        statistics.ButtonClicked("Save");
        statistics.RemoveScreen();
    }
}
