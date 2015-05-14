using API;
using HTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// This class controls the UI to login
/// </summary>
public class LoginView : StatisticsBehaviour
{

    public CustomInputField usernameField;
    public CustomInputField passwordField;
    public GameObject panel;
    public Toast toast;

    public void Start()
    {
        StartStatistics("Login");
    }

    /// <summary>
    /// This logs in the user
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void Login(int useless)
    {
        string username = usernameField.text;
        string password = passwordField.text;

        if (String.IsNullOrEmpty(password)
            || String.IsNullOrEmpty(username))
        {
            toast.Notify("Please fill in all fields");
            return;
        }

        API.UserController userController = API.UserController.Instance;
        Request response = userController.Login(username, password, (success) =>
        {
            SessionManager.Instance.LoginUser(success);
            toast.Notify("Login successful!");
			Application.LoadLevel("MainMenuScene");
            panel.SetActive(false);
            ClosingButton("Login");
        }, (error) =>
        {
			toast.Notify("Login failed. Please try again...");
        });
    }

	public void Register() {
		Application.LoadLevel ("Register");
	}
}
