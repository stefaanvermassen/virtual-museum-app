using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class AddMuseumInfo : MonoBehaviour {

    public InputField museumAuthor;
    public InputField museumName;
    public InputField museumDescription;
    public GameObject panel;
    public Museum museum;

    private void Start()
    {
        museumAuthor.text = museum.author;
        museumName.text = museum.museumName;
        museumDescription.text = museum.description;
	}

    public void save(int useless)
    {
        museum.author = museumAuthor.text;
        museum.museumName = museumName.text;
        museum.description = museumDescription.text;

        panel.SetActive(false);
    }
}
