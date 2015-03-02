/*
 * Based on C# File Browser
 * https://www.assetstore.unity3d.com/en/#!/content/18308
 */

#define thread //comment out this line if you would like to disable multi-threaded search
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.IO;
#if thread
using System.Threading;
#endif


public class FileBrowser
{
    //public 
    //Optional Parameters
    public string name = "Upload a File"; //Just a name to identify the file browser with
    //GUI Options
    public int layoutType { get { return layout; } } //returns the current Layout type
    public Texture2D fileTexture, directoryTexture, backTexture, driveTexture; //textures used to represent file types
    public GUIStyle backStyle, cancelStyle, selectStyle, searchStyle; //styles used for specific buttons
    public Color selectedColor = new Color(0.5f, 0.5f, 0.9f); //the color of the selected file
    public bool isVisible { get { return visible; } } //check if the file browser is currently visible
    //File Options
    public string searchPattern = "*"; //search pattern used to find files
    //Output
    public FileInfo outputFile; //the selected output file
    //Search
    public bool showSearch = false; //show the search bar
    public bool searchRecursively = false; //search current folder and sub folders
    //Protected	
    //GUI
    protected Vector2 fileScroll = Vector2.zero, folderScroll = Vector2.zero, driveScroll = Vector2.zero;
    protected Color defaultColor;
    protected int layout;
    protected Rect guiSize;
    protected GUISkin oldSkin;
    protected bool visible = false;
    //Search
    protected string searchBarString = ""; //string used in search bar
    protected string prevSearchBarString = "";
    protected bool isSearching = false; //do not show the search bar if searching
    //File Information
    protected DirectoryInfo currentDirectory;
    protected List<FileInformation> files;
    protected DirectoryInformation[] directories, drives;
    protected DirectoryInformation parentDir;
    protected bool getFiles = true, showDrives = false;
    protected int selectedFile = -1;
    //Threading
    protected float startSearchTime = 0f;
#if thread
    protected Thread t;
#endif
    //Private
    private readonly string[] imageExtensions = { "png", "gif", "bmp", "jpg", "jpeg" };
    private const int REQ_SEARCH_LENGTH = 3;
    private bool done = false;

    //Constructors
    public FileBrowser(string directory, int layoutStyle, Rect guiRect) { currentDirectory = new DirectoryInfo(directory); layout = layoutStyle; guiSize = guiRect; }
#if (UNITY_IPHONE || UNITY_ANDROID || UNITY_BLACKBERRY || UNITY_WP8)
		public FileBrowser(string directory,int layoutStyle):this(directory,layoutStyle,new Rect(0,0,Screen.width,Screen.height)){}
		public FileBrowser(string directory):this(directory,1){}
#else
    public FileBrowser(string directory, int layoutStyle) : this(directory, layoutStyle, new Rect(Screen.width * 0.125f, Screen.height * 0.125f, Screen.width * 0.75f, Screen.height * 0.75f)) { }
    public FileBrowser(string directory) : this(directory, 0) { }
#endif
    public FileBrowser(Rect guiRect) : this() { guiSize = guiRect; }
    public FileBrowser(int layoutStyle) : this(Directory.GetCurrentDirectory(), layoutStyle) { }
    public FileBrowser() : this(Directory.GetCurrentDirectory()) { }

    //set variables
    public void setDirectory(string dir) { currentDirectory = new DirectoryInfo(dir); }
    public void setLayout(int l) { layout = l; }
    public void setGUIRect(Rect r) { guiSize = r; }


    // Gui function to be called during OnGUI
    public bool draw()
    {
        if (getFiles)
        {
            getFileList(currentDirectory);
            getFiles = false;
        }

        switch (layout)
        {
            case 0:
                drawNonMobile();
                break;
            case 1:
            default:
                drawMobile();
                break;
        }

        return done;
    }

    private void drawNonMobile()
    {
        GUILayout.BeginArea(guiSize);
        GUILayout.BeginVertical("box");
        // Draw statusbar on top
        GUILayout.BeginHorizontal("box");
        GUILayout.Space(10);
        GUILayout.Label(currentDirectory.FullName);
        GUILayout.FlexibleSpace();
        if (showSearch)
        {
            drawSearchField();
            GUILayout.Space(10);
        }
        GUILayout.EndHorizontal();
        // Draw the rest of the  panel
        GUILayout.BeginHorizontal("box");
        // Draw tree on left side
        GUILayout.BeginVertical(GUILayout.MaxWidth(300));
        folderScroll = GUILayout.BeginScrollView(folderScroll);
        if (showDrives)
        {
            foreach (DirectoryInformation di in drives)
            {
                if (di.button()) { getFileList(di.d); }
            }
        }
        else
        {
            if ((backStyle != null) ? parentDir.button(backStyle) : parentDir.button())
                getFileList(parentDir.d);
        }
        foreach (DirectoryInformation di in directories)
        {
            if (di.button()) { getFileList(di.d); }
        }
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        // Draw found items
        GUILayout.BeginVertical("box");
        fileScroll = GUILayout.BeginScrollView(fileScroll);
        for (int fi = 0; fi < files.Count; fi++)
        {
            if (selectedFile == fi)
            {
                defaultColor = GUI.color;
                GUI.color = selectedColor;
            }
            if (files[fi].button())
            {
                outputFile = files[fi].f;
                selectedFile = fi;
            }
            if (selectedFile == fi)
                GUI.color = defaultColor;
        }
        GUILayout.EndScrollView();
        // Draw the bar containing the buttons
        GUILayout.BeginHorizontal();
        if ((cancelStyle == null) ? GUILayout.Button(new GUIContent("Cancel")) : GUILayout.Button(new GUIContent("Cancel"), cancelStyle))
        {
            outputFile = null;
            done = true;
        }

        if ((selectStyle == null) ? GUILayout.Button(new GUIContent("Select")) : GUILayout.Button(new GUIContent("Select"), selectStyle)) { done = true; }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    private void drawMobile()
    {
        GUILayout.BeginArea(guiSize);
        GUILayout.BeginVertical("box");
        if (showSearch)
        {
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            drawSearchField();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        fileScroll = GUILayout.BeginScrollView(fileScroll);

        if (showDrives)
        {
            GUILayout.BeginHorizontal();
            foreach (DirectoryInformation di in drives)
            {
                if (di.button()) { getFileList(di.d); }
            }
            GUILayout.EndHorizontal();
        }
        else
        {
            if ((backStyle != null) ? parentDir.button(backStyle) : parentDir.button())
                getFileList(parentDir.d);
        }


        foreach (DirectoryInformation di in directories)
        {
            if (di.button()) { getFileList(di.d); }
        }
        for (int fi = 0; fi < files.Count; fi++)
        {
            if (selectedFile == fi)
            {
                defaultColor = GUI.color;
                GUI.color = selectedColor;
            }
            if (files[fi].button())
            {
                outputFile = files[fi].f;
                selectedFile = fi;
            }
            if (selectedFile == fi)
                GUI.color = defaultColor;
        }
        GUILayout.EndScrollView();

        if ((selectStyle == null) ? GUILayout.Button(new GUIContent("Select")) : GUILayout.Button(new GUIContent("Select"), selectStyle)) { done = true; }
        if ((cancelStyle == null) ? GUILayout.Button(new GUIContent("Cancel")) : GUILayout.Button(new GUIContent("Cancel"), cancelStyle))
        {
            outputFile = null;
            done = true;
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    protected void drawSearchField()
    {
        GUILayout.Label(new GUIContent("Search: ", "4 characters minimum"));
        searchBarString = GUILayout.TextField(searchBarString, GUILayout.MinWidth(150));
        if (searchBarString.Length > REQ_SEARCH_LENGTH)
        {
            if (prevSearchBarString != searchBarString)
            {
                isSearching = true;
#if thread
                startSearchTime = Time.time;
                t = new Thread(threadSearchFileList);
                t.Start(true);
#else

            searchFileList(currentDirectory);
#endif
            }
            prevSearchBarString = searchBarString;
        }
        else
        {
            isSearching = false;
            getFileList(currentDirectory);
        }
    }

    // Only returns directories, drives and images
    public void getFileList(DirectoryInfo di)
    {
        //set current directory
        currentDirectory = di;
        //get parent
        parentDir = new DirectoryInformation((di.Parent == null) ? di : di.Parent, backTexture);
        showDrives = di.Parent == null;

        //get drives
        string[] drvs = System.IO.Directory.GetLogicalDrives();
        drives = new DirectoryInformation[drvs.Length];
        for (int v = 0; v < drvs.Length; v++)
        {
            drives[v] = new DirectoryInformation(new DirectoryInfo(drvs[v]), driveTexture);
        }

        //get directories
        DirectoryInfo[] dia = di.GetDirectories();
        directories = new DirectoryInformation[dia.Length];
        for (int d = 0; d < dia.Length; d++)
        {
            directories[d] = new DirectoryInformation(dia[d], directoryTexture);
        }

        //get files
        files = new List<FileInformation>();
        foreach (string s in imageExtensions)
        {
            //FileInfo[] fia = di.GetFiles(searchPattern + "?.png");
            FileInfo[] fia = searchDirectory(di, searchPattern + "?." + s, false);
            foreach (FileInfo info in fia)
            {
                FileInformation finfo = new FileInformation(info, fileTexture);
                if (!files.Contains(finfo))
                {
                    files.Add(finfo);
                }
            }
        }
    }

    protected void searchFileList(DirectoryInfo di)
    {
        files.Clear();
        foreach (string s in imageExtensions)
        {
            FileInfo[] fia = searchDirectory(di, (searchBarString.IndexOf("*") >= 0) ? searchBarString + "?." + s : "*" + searchBarString + "?." + s, searchRecursively);
            for (int f = 0; f < fia.Length; f++)
            {
                FileInformation finfo = new FileInformation(fia[f], fileTexture);
                if (!files.Contains(finfo))
                {
                    Debug.Log(fia[f].Name);
                    files.Add(finfo);
                }
            }
        }
    }

    protected void threadSearchFileList()
    {
        searchFileList(currentDirectory);
    }

    //search a directory by a search pattern, this is optionally recursive
    public FileInfo[] searchDirectory(DirectoryInfo di, string sp, bool recursive)
    {
        return di.GetFiles(sp, (recursive) ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
    }

    public float brightness(Color c) { return c.r * .3f + c.g * .59f + c.b * .11f; }

    //to string
    public override string ToString()
    {
        return "Name: " + name + "\nVisible: " + isVisible.ToString() + "\nDirectory: " + currentDirectory + "\nLayout: "
            + layout.ToString() + "\nGUI Size: " + guiSize.ToString() + "\nDirectories: " + directories.Length.ToString() + "\nFiles: " + files.Count.ToString();
    }
}

public class FileInformation : System.Object
{
    public FileInfo f;
    private string name;
    private Texture2D img;
    private GUIContent gc;


    public FileInformation(FileInfo f, Texture2D img)
    {
        this.f = f;
        name = f.Name;
        this.img = img;
    }

    private GUIContent getGUIContent()
    {
        if (gc == null)
        {
            Texture2D image = new Texture2D(84, 94);
            image.LoadImage(File.ReadAllBytes(f.FullName));
            if (image != null)
            {
                img = image;
            }
            gc = new GUIContent(name, img);
        }

        return gc;
    }

    public bool button() { return GUILayout.Button(getGUIContent()); }

    public override bool Equals(System.Object obj)
    {
        if (obj == null)
        {
            return false;
        }

        return (f.Name == ((FileInformation)obj).f.Name);
    }

    public bool Equals(FileInformation info)
    {
        // If parameter is null return false:
        if (info == null)
        {
            return false;
        }

        // Return true if the fields match:
        return (f.Name == info.f.Name);
    }

    public override int GetHashCode()
    {
        return f.Name.GetHashCode();
    }
}

public class DirectoryInformation
{
    public DirectoryInfo d;
    private string name;
    private Texture2D img;
    private GUIContent gc;

    public DirectoryInformation(DirectoryInfo d, Texture2D img)
    {
        this.d = d;
        name = d.Name;
        this.img = img;
    }

    private GUIContent getGUIContent()
    {
        if (gc == null)
        {
            if (img == null)
            {
                gc = new GUIContent(name);
            }
            else
            {
                gc = new GUIContent(name, img);
            }
        }

        return gc;
    }

    public bool button() { return GUILayout.Button(getGUIContent()); }
    public bool button(GUIStyle gs) { return GUILayout.Button(getGUIContent(), gs); }
}