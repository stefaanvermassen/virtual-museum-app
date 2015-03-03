﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

public class NewFileBrowser : MonoBehaviour
{

    public Text directoryLabel;
    public InputField searchField;
    public GameObject directoryButton;
    public GameObject fileButton;
    public Texture2D fileTexture, folderTexture, backTexture;
    public Transform directoryView;
    public Transform fileView;

    private DirectoryInfo previousDirectory, currentDirectory;
    private string selectedFile = "";
    private string prevSearch = "";
    private Thread thread;

    private readonly string[] imageExtensions = { ".png", ".bmp", ".jpg", ".jpeg" };

    private enum Type
    {
        FOLDER, FILE
    };

    private void Start()
    {
        currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());
    }

    private void Update()
    {
        directoryLabel.text = currentDirectory.FullName;
        string search = getSearch();
        drawFiles(search);
        drawDirectories();
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

        fileView.DetachChildren();
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
        GameObject b = Instantiate(but) as GameObject;
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
                break;
            case Type.FILE:
                selectedFile = name;
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