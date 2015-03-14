using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AddMuseumInfo {

    private InputField museumAuthor;
    private InputField museumName;
    private InputField museumDescription;
    private Museum museum;

    public AddMuseumInfo(Museum museum, InputField museumAuthor, InputField museumName, InputField museumDescription)
    {
        this.museum = museum;
        this.museumAuthor = museumAuthor;
        this.museumName = museumName;
        this.museumDescription = museumDescription;
    }

	// Use this for initialization
    protected void Start()
    {
        museumAuthor.text = museum.author;
        museumName.text = museum.museumName;
        museumDescription.text = museum.description;
	}

    public void save()
    {
        museum.author = museumAuthor.text;
        museum.museumName = museumName.text;
        museum.description = museumDescription.text;
        Debug.Log("New data written");
    }
}
