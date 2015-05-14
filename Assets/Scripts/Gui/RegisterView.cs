using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using HTTP;
using API;

/// <summary>
/// This class controls the UI to login
/// </summary>
public class RegisterView : StatisticsBehaviour
{

    public CustomInputField usernameField;
    public CustomInputField emailField;
    public CustomInputField passwordField;
    public CustomInputField confirmPasswordField;
    public Toast toast;
    public GameObject panel;

    public void Start()
    {
        StartStatistics("Register");
    }

    /// <summary>
    /// This logs in the user
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void Register(int useless)
    {
        string username = usernameField.text;
        string email = emailField.text;
        string password = passwordField.text;

        if (String.IsNullOrEmpty(password)
            || String.IsNullOrEmpty(username)
            || String.IsNullOrEmpty(email))
        {
            toast.Notify("Please fill in all fields");
            return;
        }

        if (password != confirmPasswordField.text) {
            toast.Notify("Passwords are different");
            passwordField.text = "";
            confirmPasswordField.text = "";
            return;
        }

        API.UserController userController = API.UserController.Instance;
        Request response = null;
        response = userController.CreateUser(username, email, password, (success) =>
        {
            SessionManager.Instance.LoginUser(success);
            toast.Notify("Successfully registered!");
			Application.LoadLevel("MainMenuScene");
            panel.SetActive(false);
            ClosingButton("Register");
        }, (error) =>
        {
            string errorMessage = response.response.Object["Message"] as string;
            Hashtable modelState = response.response.Object["ModelState"] as Hashtable;
            
            foreach (DictionaryEntry e in modelState)
            {
                foreach (string value in (ArrayList) e.Value)
                {
                    errorMessage += " " + value;
                }
            }
            
            toast.Notify(errorMessage);
        });
    }

	public void Login() {
		Application.LoadLevel ("Login");
	}
}
