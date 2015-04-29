using UnityEngine;
using System.Collections;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MuseumLoader : MonoBehaviour {

	public static int museumID = -1;
	public enum MuseumAction{Visit, Preview, Edit};
	public static MuseumAction currentAction = MuseumAction.Preview;
	public Museum museum;

	void OnEnable() {
		Catalog.Refresh ();
	}

	// Use this for initialization
	void Start () {
		MainMenuActions menu = FindObjectOfType<MainMenuActions> ();
		if (menu != null) { // This the default museum.
			TextAsset asset = Resources.Load("defaultMuseum_bin") as TextAsset;
			Stream s = new MemoryStream(asset.bytes);
			BinaryFormatter deserializer = new BinaryFormatter ();
			MuseumData data = (MuseumData)deserializer.Deserialize (s);
			s.Close ();
			museum.Load (data);
		} else {
			if (currentAction == MuseumAction.Preview) { // Preview mode
				if (File.Exists (Application.persistentDataPath + "/test.bin")) {
					Stream TestFileStream = File.OpenRead (Application.persistentDataPath + "/test.bin");
					BinaryFormatter deserializer = new BinaryFormatter ();
					MuseumData data = (MuseumData)deserializer.Deserialize (TestFileStream);
					TestFileStream.Close ();
					museum.Load (data);
				}
			} else if (currentAction == MuseumAction.Visit || currentAction == MuseumAction.Edit) { // Visit mode
				if (museumID > 0)
					Storage.Instance.LoadRemote (museum, museumID.ToString ());
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
