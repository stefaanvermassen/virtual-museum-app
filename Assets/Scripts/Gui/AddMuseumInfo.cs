using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// This class controls the UI to add info to a museum
/// </summary>
public class AddMuseumInfo : MonoBehaviour
{

    public InputField museumName;
    public InputField museumDescription;
    public GameObject panel;
    public Museum museum;

    /// <summary>
    /// This method initializes the values from the UI
    /// with the values available in the Museum
    /// </summary>
    public void Start()
    {
        museumName.text = museum.museumName;
        museumDescription.text = museum.description;
    }

    /// <summary>
    /// This will save the new Museum info when
    /// the confirm button is pressed
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void Save(int useless)
    {
        museum.museumName = museumName.text;
        museum.description = museumDescription.text;

        Storage.Instance.setStoreMode(Storage.StoreMode.Only_Local);
        Storage.Instance.Save(museum);

        panel.SetActive(false);
    }
}
