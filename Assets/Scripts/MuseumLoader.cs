using UnityEngine;
using System.Collections;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MuseumLoader : MonoBehaviour {

	public static int museumID = -1;
	public enum MuseumAction{Visit, Preview, Edit};
	public static MuseumAction currentAction = MuseumAction.Preview;
	public Museum museum;


	/// <summary>
	/// Raises the enable event.
	/// An ideal event to resfresh the catalog, with data from the server
	/// </summary>
	void OnEnable() {
		Catalog.Refresh ();
	}

	/// <summary>
	/// Start this instance.
	/// 
	/// </summary>
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
			if (currentAction == MuseumAction.Preview || currentAction == MuseumAction.Edit) { // Preview mode
				if (File.Exists (Application.persistentDataPath + "/test.bin")) {
					LoadFromFile(Application.persistentDataPath + "/test.bin");
				} else if (museumID > 0) {
					Storage.Instance.LoadRemote (museum, museumID.ToString ());
				}
			} else if (currentAction == MuseumAction.Visit) { // Visit mode
				if (museumID > 0) {
					Storage.Instance.LoadRemote (museum, museumID.ToString ());
				}
			}
		}
	}

	public static void CreateTempMuseum(Museum museum) {
		var data = museum.Save();
		Stream TestFileStream = File.Create(Application.persistentDataPath + "/test.bin");
		BinaryFormatter serializer = new BinaryFormatter();
		serializer.Serialize(TestFileStream, data);
		TestFileStream.Close();
	}

	public static void DeleteTempMuseum() {
		if (File.Exists (Application.persistentDataPath + "/test.bin")) {
			File.Delete(Application.persistentDataPath + "/test.bin");
		}
	}

	void LoadFromFile(string path) {
		if (File.Exists (path)) {
			Stream TestFileStream = File.OpenRead (path);
			BinaryFormatter deserializer = new BinaryFormatter ();
			MuseumData data = (MuseumData)deserializer.Deserialize (TestFileStream);
			TestFileStream.Close ();
			museum.Load (data);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
