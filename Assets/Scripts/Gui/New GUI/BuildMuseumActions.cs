using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class BuildMuseumActions : MonoBehaviour {

	public int tool;
	public ImageHighlightButton[] toolButtons;
	public DrawController drawController;
	public Museum museum;

	void Start() {
		SetTool (1); // Pan tool
	}

	public void BackToMain() {
		Application.LoadLevel ("MainMenuScene");
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			BackToMain();
		}
		for (int i = 0; i < toolButtons.Length; i++) {
			if(tool == i) {
				toolButtons[i].Highlight(true);
				//toolButtons[i].Select();
			} else {
				toolButtons[i].Highlight(false);
			}
		}
	}

	public void SetTool(int tool) {
		this.tool = tool;
		drawController.SetTool(tool);
	}

	public void Preview() {
		var data = museum.Save();
		Stream TestFileStream = File.Create(Application.persistentDataPath + "/test.bin");
		BinaryFormatter serializer = new BinaryFormatter();
		serializer.Serialize(TestFileStream, data);
		TestFileStream.Close();
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Preview;
		Application.LoadLevel("WalkingController");
	}
}
