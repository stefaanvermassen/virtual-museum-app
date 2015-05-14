using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpWindow : GUIControl
{
	public Text header;
	public Text info;
	public Button ok;
	public GUIControl placeHolder;
	private static PopUpWindow instance;

	public enum MessageType
	{
		INFO=1,
		WARNING=2,
		ERROR=3
	}

	void Start ()
	{
		PopUpWindow.instance = this;
		ok.onClick.AddListener (() => Replace (placeHolder));
        screenName = "PopUpWindow";
	}

	/// <summary>
	/// Instances the show message.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="message">Message.</param>
	public void InstanceShowMessage (MessageType type, string message){
		header.text = type.ToString ();
		info.text = message;
		//for some reason, the button is set to non-active after klicking
		ok.gameObject.SetActive (true);
		Replace (placeHolder);
		OnTop ();

	}

	/// <summary>
	/// Shows the message.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="message">Message.</param>
	public static void ShowMessage (MessageType type, string message)
	{
		if (instance != null) {
			instance.InstanceShowMessage(type,message);
		}
        else
        {
			Debug.Log ("No PopUpWindow present in the scene.");
		}
	}
}
