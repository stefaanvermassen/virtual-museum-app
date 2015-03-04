using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

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
    private readonly string[] imageExtensions = { ".png", ".bmp", ".jpg", ".jpeg" };

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
                string selected = fileBrowser.getSelected();
                if (selected != "")
                {
                    //Upload selected file
                    Debug.Log("Uploaded " + selected + " successful!");
                    exit();
                }
                else
                {
                    selected = pathField.text;
                    foreach (string s in imageExtensions)
                    {
                        if (selected.EndsWith(s))
                        {
                            //Upload selected file
                            Debug.Log("Uploaded " + selected + " successful!");
                            exit();
                        }
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
    }

    // Update is called once per frame
    private void Update()
    {
        if (fileBrowser != null)
        {
            fileBrowser.Update();
            
            // Update our GUI
            string selected = fileBrowser.getSelected();
            if (selected != "")
            {
                Debug.Log(selected);
                pathField.text = selected;

                thumbnail.enabled = true;
                Texture2D image = new Texture2D(0, 0);
                image.LoadImage(File.ReadAllBytes(selected));
                thumbnail.sprite = Sprite.Create(image, new Rect(0, 0, folderTexture.width, folderTexture.height), Vector2.zero);
            }
        }
    }
}
