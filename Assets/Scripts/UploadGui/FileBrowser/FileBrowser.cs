using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class FileBrowser
{
    private Text directoryLabel;
    private InputField searchField;
    private GameObject directoryButton;
    private GameObject fileButton;
    private Texture2D fileTexture, folderTexture, backTexture;
    private Transform directoryView;
    private Transform fileView;
    private Button cancelButton;
    private Button acceptButton;
    private GameObject fileBrowser;

    private DirectoryInfo previousDirectory, currentDirectory;
    private string selectedFile = "";
    private string prevSearch = "";

    private readonly string[] imageExtensions;
    private bool done = false;
    private bool cancel = false;

    private enum Type
    {
        FOLDER, FILE, CANCEL, ACCEPT
    };

    public FileBrowser(Text directoryLabel, InputField searchField, GameObject directoryButton, GameObject fileButton,
                        Texture2D fileTexture, Texture2D folderTexture, Texture2D backTexture, Transform directoryView,
                        Transform fileView, Button cancelButton, Button acceptButton, GameObject fileBrowser, string[] imageExtensions)
    {
        this.directoryLabel = directoryLabel;
        this.searchField = searchField;
        this.directoryButton = directoryButton;
        this.fileButton = fileButton;
        this.fileTexture = fileTexture;
        this.folderTexture = folderTexture;
        this.backTexture = backTexture;
        this.directoryView = directoryView;
        this.fileView = fileView;
        this.cancelButton = cancelButton;
        this.acceptButton = acceptButton;
        this.fileBrowser = fileBrowser;
        this.imageExtensions = imageExtensions;
        initialize();
    }

    private void initialize()
    {
        currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
        cancelButton.onClick.AddListener(() => handleClick("", Type.CANCEL));
        acceptButton.onClick.AddListener(() => handleClick(selectedFile, Type.ACCEPT));
    }

    public void Update()
    {
        if (directoryLabel == null)
        {
            Debug.Log("DirectoryLabel is null.");
        }
        else if (currentDirectory == null)
        {
            Debug.Log("CurrentDirectory is null.");
        }
        directoryLabel.text = currentDirectory.FullName;
        string search = getSearch();
        drawFiles(search);
        drawDirectories();
    }

    public string getSelected()
    {
        return (done) ? selectedFile : "";
    }

    public bool isCanceled()
    {
        return cancel;
    }

    private string getSearch()
    {
        return searchField.text;
    }

    private void drawFiles(string search)
    {
        if (previousDirectory == currentDirectory && prevSearch == search)
        {
            return;
        }

        // Remove the buttons
        fileView.DetachChildren();
        // The selected file is now invalid
        selectedFile = "";

        IEnumerable<FileInfo> files = getFileList(search.Length != 0, search);
        foreach (FileInfo file in files)
        {
            addFileButton(file);
        }

        prevSearch = search;
    }

    private void drawDirectories()
    {
        if (previousDirectory == currentDirectory)
        {
            return;
        }

        // Remove the buttons
        directoryView.DetachChildren();

        if (currentDirectory.Parent != null)
        {
            addDirectoryButton(currentDirectory.Parent, backTexture);
        }

        DirectoryInfo[] directories = currentDirectory.GetDirectories();
        foreach (DirectoryInfo dir in directories)
        {
            addDirectoryButton(dir, folderTexture);
        }

        previousDirectory = currentDirectory;
    }

    private void addFileButton(FileInfo file)
    {
        Texture2D texture = fileTexture;
        Texture2D image = new Texture2D(0, 0);
        image.LoadImage(File.ReadAllBytes(file.FullName));

        if (image != null)
        {
            texture = image;
        }

        addButton(fileButton, file.Name, file.FullName, texture, fileView, Type.FILE);
    }

    private void addDirectoryButton(DirectoryInfo dir, Texture2D texture)
    {
        addButton(directoryButton, dir.Name, dir.FullName, texture, directoryView, Type.FOLDER);
    }


    private void addButton(GameObject but, string name, string fullName, Texture2D texture, Transform view, Type type)
    {
        GameObject b = GameObject.Instantiate(but) as GameObject;
        FBButton button = b.GetComponent<FBButton>();
        button.label.text = name;
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        button.icon.sprite = sprite;
        b.transform.SetParent(view);
        string info = fullName;
        button.button.onClick.AddListener(() => handleClick(info, type));
    }

    private void handleClick(string name, Type type)
    {
        switch (type)
        {
            case Type.FOLDER:
                currentDirectory = new DirectoryInfo(name);
                // Clear search
                searchField.text = "";
                break;
            case Type.FILE:
                selectedFile = name;
                break;
            case Type.CANCEL:
                Debug.Log("Cancel was hit.");
                cancel = true;
                break;
            case Type.ACCEPT:
                if (name == "")
                {
                    Debug.Log("A file has to be selected.");
                }
                else
                {
                    done = true;
                    Debug.Log(name + " was chosen.");
                }
                break;
            default:
                break;
        }
    }

    // Only returns directories, drives and images
    private IEnumerable<FileInfo> getFileList(bool recursive, string searchPattern)
    {
        IEnumerable<FileInfo> files = Enumerable.Empty<FileInfo>();
        FileComparer comparer = new FileComparer();
        if (searchPattern.Length == 0)
        {
            searchPattern = "*";
        }
        else if (!searchPattern.Contains("*"))
        {
            searchPattern = "*" + searchPattern + "*";
        }



        foreach (string ext in imageExtensions)
        {
            files = files.Union(searchDirectory(currentDirectory, searchPattern + ext, recursive), comparer);
        }

        return files;
    }

    private FileInfo[] searchDirectory(DirectoryInfo dir, string sp, bool recursive)
    {
        return dir.GetFiles(sp, (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }
}

class FileComparer : IEqualityComparer<FileInfo>
{

    public bool Equals(FileInfo x, FileInfo y)
    {
        return x.FullName == y.FullName;
    }

    public int GetHashCode(FileInfo obj)
    {
        return obj.GetHashCode();
    }
}