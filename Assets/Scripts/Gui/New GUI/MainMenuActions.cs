using UnityEngine;
using System.Collections;
using System;

using UnityEngine.UI;

public class MainMenuActions : MonoBehaviour
{

	public int currentMuseumID = -1;
	public int currentArtID = -1;
	public Toast toast;

	void Start ()
	{
	}

	public void NewMuseum() {
		MuseumLoader.DeleteTempMuseum ();
		MuseumLoader.museumID = -1;
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Edit;
		Application.LoadLevel("BuildMuseum");
	}

	public void EditMuseum(int museumID) {
		MuseumLoader.DeleteTempMuseum ();
		MuseumLoader.museumID = museumID;
		MuseumLoader.currentAction = MuseumLoader.MuseumAction.Edit;
		Application.LoadLevel ("BuildMuseum");
	}

	public void VisitMuseum (bool owner)
	{
		VisitMuseum (currentMuseumID, owner);
	}

	public void EditMuseum ()
	{
		EditMuseum (currentMuseumID);
	}

	public void DeleteMuseum ()
	{
		DeleteMuseum (currentMuseumID);
	}

	public void DeleteMuseum (int museumID)
	{
		MuseumList list = FindObjectOfType<MuseumList> ();
		//toast.Notify ("Not yet implemented");
		var mc = API.MuseumController.Instance;
		mc.DeleteMuseum(museumID, (succes) => {
			//Debug.Log("Deleting musuem " + museumID + " succesfull");
			list.InitList();
			list.museumPopUp.Close ();
			toast.Notify ("This museum was deleted successfully.");
		}, (error) => {
			toast.Notify ("There was an error deleting this museum.");
			//Debug.Log("Deleting musuem " + museumID + " errored");
		});
	}

	public void ResetArtID ()
	{
		currentArtID = -1;
	}

	public void ResetMuseumID ()
	{
		currentMuseumID = -1;
	}

	public void SwitchAccount ()
	{
		Application.LoadLevel ("Login");
	}

	public void ExploreMuseums ()
	{
		Application.LoadLevel ("SearchMuseums");
	}

	public void ScanArt(){
		Application.LoadLevel ("Scan");
	}

	/// <summary>
	/// Visit the museum with the designated museum ID
	/// </summary>
	/// <param name="museumID">Museum ID</param>
	/// <param name="owner">Whether or not the current user is the owner of this museum</param>
	public void VisitMuseum (int museumID, bool owner)
	{
		MuseumLoader.museumID = museumID;
		MuseumLoader.DeleteTempMuseum ();
		if (owner) {
			MuseumLoader.currentAction = MuseumLoader.MuseumAction.Preview;
		} else {
			MuseumLoader.currentAction = MuseumLoader.MuseumAction.Visit;
		}
		Debug.Log ("test");
		Application.LoadLevel ("WalkingController");
	}

	public void visitRandomMuseum ()
	{
		var mc = API.MuseumController.Instance;
		mc.GetRandomMuseum((m) => {
			currentMuseumID=m.MuseumID;
			Debug.Log ("Random museum ID: "+currentMuseumID);
			if (currentMuseumID != -1) {
				VisitMuseum ( false);
			}
		});
	}

	public void ShowtestMessage ()
	{
		PopUpWindow.ShowMessage (PopUpWindow.MessageType.INFO, "This is a test info");
	}

	public void Exit ()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
		Application.Quit ();
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Exit ();
		}
	}
}
