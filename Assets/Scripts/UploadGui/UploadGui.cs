using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UploadGui : MonoBehaviour
{
    // For fileBrowser
    public Text directoryLabel;
    public InputField searchField;
    public GameObject directoryButton;
    public GameObject fileButton;
    public Texture2D fileTexture, folderTexture, backTexture;
    public Transform directoryView;
    public Transform fileView;
    public Button cancelButton;
    public Button acceptButton;
    public GameObject fileBrowserObject;

    // UploadGUI
    public GameObject browseButton;
    public InputField pathField;
    public Image thumbnail;
    public GameObject uploadGui;
    public Button cancelULButton;
    public Button uploadButton;

    private FileBrowser fileBrowser;
    private byte[] uploadableFile;
    private IEnumerator www;
    private string accessToken;
    private readonly string[] imageExtensions = { ".png", ".bmp", ".jpg", ".jpeg" };
    private const string BASE_URL = "http://api.awesomepeople.tv/";
    private const string TOKEN = "Token";
    private const string ARTWORK = "api/artwork";
    private const string MIME = "image/";

    private enum Type
    {
        CANCEL, UPLOAD
    };

    // Use this for initialization
    private void Start()
    {
        pathField.text = Directory.GetCurrentDirectory();
        cancelULButton.onClick.AddListener(() => handleClick(Type.CANCEL));
        uploadButton.onClick.AddListener(() => handleClick(Type.UPLOAD));
        fileBrowserObject.SetActive(false);
        thumbnail.enabled = false;
    }

    private void handleClick(Type type)
    {
        switch (type)
        {
            case Type.CANCEL:
                Debug.Log("Cancel was hit.");
                exit();
                break;
            case Type.UPLOAD:
                string selected = pathField.text;
                foreach (string s in imageExtensions)
                {
                    if (selected.EndsWith(s))
                    {
                        //Upload selected file
                        if (accessToken == "") { stubLogin(); }
                        uploadableImage();
                        Debug.Log("Uploaded " + selected + " successful!");
                        exit();
                    }
                }

                Debug.Log("Please select an image file!");
                break;
            default:
                break;
        }
    }

    private void exit()
    {
        Application.Quit();
    }


    public void createFileBrowser()
    {
        fileBrowser = new FileBrowser(directoryLabel, searchField, directoryButton, fileButton, fileTexture, folderTexture, backTexture, directoryView,
                fileView, cancelButton, acceptButton, fileBrowserObject, imageExtensions);
        fileBrowserObject.SetActive(true);
        uploadGui.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (fileBrowser != null)
        {
            if (fileBrowser.isCanceled())
            {
                killBrowser();
            }
            else
            {
                fileBrowser.Update();

                // Update our GUI
                string selected = fileBrowser.getSelected();
                if (selected != "")
                {
                    killBrowser();
                    Debug.Log(selected);
                    pathField.text = selected;

                    thumbnail.enabled = true;
                    Texture2D image = new Texture2D(0, 0);
                    uploadableFile = File.ReadAllBytes(selected);
                    image.LoadImage(uploadableFile);
                    thumbnail.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);
                }
            }
        }
    }

    private void killBrowser()
    {
        uploadGui.SetActive(true);
        fileBrowserObject.SetActive(false);
        fileBrowser = null;
    }

    /*
     * http://api.awesomepeople.tv/Token
     * {"grant_type":"password",
     * "username":"username",
     * "password":"password"}
     */
    private void stubLogin()
    {
        WWW www = sendPost(BASE_URL + TOKEN, new string[] { "grant_type", "username", "password" }, new string[] { "password", "museum@awesomepeople.tv", "@wesomePeople_20" });
        JSONNode node = JSON.Parse(www.text);
        accessToken = node["access_token"];
    }

    private void uploadableImage()
    {
        string[] splitted = pathField.text.Split(new char[]{'.'});
        string mime = splitted[splitted.Length - 1];
        splitted = pathField.text.Split(new char[] { '/', '\\' });
        string name = splitted[splitted.Length - 1];
        WWW www = sendImagePost(BASE_URL + ARTWORK, pathField.text, name, mime);
        Debug.Log(www.text);
    }


    private WWW sendImagePost(string url, string imageLocation, string name, string mime)
    {
        WWWForm form = new WWWForm();
        form.AddBinaryData(imageLocation, uploadableFile, name, MIME + mime);

        return postForm(url, form);
    }

    private WWW sendPost(string url, string[] name, string[] value)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < name.Length; i++)
        {
            form.AddField(name[i], value[i]);
        }

        return postForm(url, form);
    }

    private WWW postForm(string url, WWWForm form)
    {
        Dictionary<string, string> headers = form.headers;
        byte[] rawData = form.data;

        WWW www = new WWW(url, rawData, headers);
        while (!www.isDone) ;
        return www;
    }
}
