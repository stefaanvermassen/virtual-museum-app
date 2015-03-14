using UnityEngine;
using System.Collections;

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MuseumLoader : MonoBehaviour {

	public Museum museum;

	// Use this for initialization
	void Start () {
		if (File.Exists(Application.persistentDataPath + "/test.bin")) {
			Stream TestFileStream = File.OpenRead(Application.persistentDataPath + "/test.bin");
			BinaryFormatter deserializer = new BinaryFormatter();
			MuseumData data = (MuseumData)deserializer.Deserialize(TestFileStream);
			TestFileStream.Close();
			museum.Load(data);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
