using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UploadGui : MonoBehaviour
{

    public GameObject browseButton;
    public InputField pathField;
    public Image thumbnail;
    public Transform uploadPanel;
    public GameObject uploadGui;
    public Button cancelULButton;
    public Button uploadButton;

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

    private FileBrowser fileBrowser;
    private string uploadableFile;
    private readonly string[] imageExtensions = { ".png", ".bmp", ".jpg", ".jpeg" };
    private IEnumerator www;
    private string accessToken;

    private enum Type
    {
        CANCEL, UPLOAD
    };

    // Use this for initialization
    private void Start()
    {
        pathField.text = Directory.GetCurrentDirectory();
        addBrowseButton();
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
                        stubLogin();
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

    private void addBrowseButton()
    {
        GameObject b = GameObject.Instantiate(browseButton) as GameObject;
        BrowseButton button = b.GetComponent<BrowseButton>();
        button.icon.sprite = Sprite.Create(folderTexture, new Rect(0, 0, folderTexture.width, folderTexture.height), Vector2.zero);
        b.transform.SetParent(uploadPanel);
        button.button.onClick.AddListener(() => createFileBrowser());
    }

    private void createFileBrowser()
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
                    byte[] file = File.ReadAllBytes(selected);
                    image.LoadImage(file);
                    thumbnail.sprite = Sprite.Create(image, new Rect(0, 0, image.width, image.height), Vector2.zero);

                    // Generate the file to upload
                    uploadableFile = Base64Encode(System.Text.Encoding.Default.GetString(file));
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

    private string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    /*
     * http://api.awesomepeople.tv/Token
     * {"grant_type":"password",
     * "username":"username",
     * "password":"password"}
     */
    private void stubLogin()
    {
        WWW www = sendPost("http://api.awesomepeople.tv/Token", new string[] { "grant_type", "username", "password" }, new string[] { "password", "museum@awesomepeople.tv", "@wesomePeople_20" });
        JSONNode node = JSON.Parse(www.text);
        accessToken = node["access_token"];
    }

    private WWW sendPost(string url, string[] name, string[] value)
    {
        WWWForm form = new WWWForm();
        for (int i = 0; i < name.Length; i++)
        {
            form.AddField(name[i], value[i]);
        }

        Dictionary<string, string> headers = form.headers;
        byte[] rawData = form.data;

        WWW www = new WWW(url, rawData, headers);
        while (!www.isDone) ;
        return www;
    }
}
