using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QRView : MonoBehaviour
{
    public GameObject panel;
    public Texture2D image;
    public Image QR;
    private bool set;

    /// <summary>
    /// Initialize the QR view
    /// </summary>
    private void Start()
    {
        set = false;
        QR.enabled = false;
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

    // TODO: Implement save
}
