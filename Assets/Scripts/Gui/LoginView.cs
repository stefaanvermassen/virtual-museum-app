using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using HTTP;

/// <summary>
/// This class controls the UI to login
/// </summary>
public class LoginView : MonoBehaviour
{

    public InputField emailField;
    public InputField passwordField;
    public GameObject panel;

    /// <summary>
    /// This logs in the user
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void Login(int useless)
    {
        string email = emailField.text;
        string password = passwordField.text;

        API.UserController userController = API.UserController.Instance;
        Request response = userController.Login(email, password);

        panel.SetActive(false);
    }
}
