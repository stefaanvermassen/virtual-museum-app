using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using API;

public class Art : Savable<Art, ArtData>
{

    public int ID { get; set; }
    public string name;
    public string description;
    public User owner;
    public List<string> tags = new List<string>();
    public List<string> genres = new List<string>();
	public Texture2D image { get; set; };


    public Art() {
        owner = new User();
    }

    /// <summary>
    /// Create an ArtData for serialization.
    /// </summary>
    /// <returns>The ArtData</returns>
    public ArtData Save()
    {
        var userData = owner.Save();
        //var imageData = image.EncodeToPNG();
        return new ArtData(ID, name, description, userData, tags, genres, null);
    }

    /// <summary>
    /// Load an ArtData in this art.
    /// </summary>
    /// <param name="data">The ArtData</param>
    public void Load(ArtData data)
    {
        ID = data.ID;
        name = data.Name;
        description = data.Description;
        owner.Load(data.Owner);
        tags = data.Tags;
        genres = data.Genres;
        image = new Texture2D(1, 1);
        image.LoadImage(data.Image);
    }

    public string getFolder() {
        return "Art";
    }
    public string getFileName() {
        return name;
    }
    public string getExtension(){
        return ".art";   
    }
    public void SaveRemote() {
        //TODO: implement
    }
    public void LoadRemote(string identifier) {
        ArtworkController.Instance.GetArtwork(
            identifier, 
            success: (art) => {
                ID = art.ArtWorkID;
                name = art.Name;
                Debug.Log(name);
                },
            error: (error) => { Debug.Log("NOPE"); }
        );
        ArtworkController.Instance.GetArtworkData(
            identifier,
            success: (art) => {
                image = new Texture2D(1, 1);
                image.LoadImage(art);
            },
            error: (error) => { Debug.Log("NOPE"); }
        ); 
    }
    public DateTime LastModified(string identifier) {
        return DateTime.Now;
    }
}
