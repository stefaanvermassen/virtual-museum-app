using UnityEngine;
using System.Collections;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MuseumLoader : MonoBehaviour {

	public static int museumID = -1;
	public Museum museum;

	// Use this for initialization
	void Start () {
		if(museumID == -1) { // Preview mode
			if (File.Exists(Application.persistentDataPath + "/test.bin")) {
				Stream TestFileStream = File.OpenRead(Application.persistentDataPath + "/test.bin");
				BinaryFormatter deserializer = new BinaryFormatter();
				MuseumData data = (MuseumData)deserializer.Deserialize(TestFileStream);
				TestFileStream.Close();
				museum.Load(data);
			}
		} else { // Visit mode
			Storage.Instance.LoadRemote(museum, museumID.ToString());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
