using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This class controls the UI to add info to a art
/// </summary>
public class ArtDetails : MonoBehaviour
{

    public InputField artName;
    public InputField artDescription;
    public Text artist;
    public GameObject panel;
    public Art art;

    /// <summary>
    /// This method initializes the values from the UI
    /// with the values available in the Art
    /// </summary>
    public void Start()
    {
        artName.text = art.name;
        artDescription.text = art.description;
        artist.text = art.owner.name;
    }

    /// <summary>
    /// This will save the new art info when
    /// the confirm button is pressed
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void save(int useless)
    {
        art.name = artName.text;
        art.description = artDescription.text;

        panel.SetActive(false);
    }
}
