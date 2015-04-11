using UnityEngine;
using System.Collections;

public class BasicActions : MonoBehaviour {

	public void startGame()
	{
		Application.LoadLevel("BuildMuseum");
	}
	public void ShowtestMessage(){
		PopUpWindow.ShowMessage(PopUpWindow.MessageType.INFO, "This is a test info");
	}
}
