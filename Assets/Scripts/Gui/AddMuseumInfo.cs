using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This class controls the UI to add info to a museum
/// </summary>
public class AddMuseumInfo : MonoBehaviour {

    public InputField museumAuthor;
    public InputField museumName;
    public InputField museumDescription;
    public GameObject panel;
    public Museum museum;

    /// <summary>
    /// This method initializes the values from the UI
    /// with the values available in the Museum
    /// </summary>
    private void Start()
    {
        museumAuthor.text = museum.ownerID;
        museumName.text = museum.museumName;
        museumDescription.text = museum.description;
	}

    /// <summary>
    /// This will save the new Museum info when
    /// the confirm button is pressed
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void save(int useless)
    {
        museum.ownerID = museumAuthor.text;
        museum.museumName = museumName.text;
        museum.description = museumDescription.text;

        panel.SetActive(false);
    }
}
