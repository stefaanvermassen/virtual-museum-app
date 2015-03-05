using UnityEngine;
using System.Collections;

public class InputAdapter {
	//methods to override in private class to implement behaviour of GUIControl
	//button
	public void onClick(){
		//do nothing
	}
	//checkbox
	public void onCheck(bool check){
		//do nothing
	}
	//radiobutton
	public void onCheck(int index){
		//do nothing
	}

	//TODO use unity's input listener 
	//nextBtn = GameObject.Find ("Next");
//	nextBtn.GetComponent<Button> ().onClick.RemoveAllListeners ();
//	nextBtn.GetComponent<Button> ().onClick.AddListener (SceneTransition);
}
