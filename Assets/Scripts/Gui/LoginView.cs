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
public class LoginView : MonoBehaviour
{

    public InputField usernameField;
    public InputField passwordField;
    public GameObject panel;
    public Toast toast;

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
            panel.SetActive(false);
        }, (error) =>
        {
            toast.Notify("Login failed. Please try again...");
        });
    }
}
