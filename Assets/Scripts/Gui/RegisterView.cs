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
public class RegisterView : MonoBehaviour
{

    public InputField usernameField;
    public InputField emailField;
    public InputField passwordField;
    public InputField confirmPasswordField;
    public GameObject panel;

    /// <summary>
    /// This logs in the user
    /// </summary>
    /// <param name="useless">Does nothing but is needed to be seen as a ClickListener</param>
    public void Register(int useless)
    {
        string username = usernameField.text;
        string email = emailField.text;
        string password = passwordField.text;
        Toast toast = gameObject.AddComponent<Toast>();

        if (String.IsNullOrEmpty(password)
            || String.IsNullOrEmpty(username)
            || String.IsNullOrEmpty(email))
        {
            toast.Notify("Please fill in all fields");
            return;
        }

        if (password != confirmPasswordField.text) {
            toast.Notify("Passwords differ");
            passwordField.text = "";
            confirmPasswordField.text = "";
            return;
        }

        API.UserController userController = API.UserController.Instance;
        Request response = userController.CreateUser(username, email, password, (success) =>
        {
            SessionManager.Instance.LoginUser(success);
            toast.Notify("Successfully registered!");
        }, (error) =>
        {
            toast.Notify("Login failed. Please try again...");
            return;
        });

        panel.SetActive(false);
    }
}
