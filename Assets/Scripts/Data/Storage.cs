using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/**
 * This class is implemented using the Singleton design and is used to abstract saving and loading to local or remote storage
 * */
public class Storage : MonoBehaviour {

    /* ******************
     * Singleton Code   *
     *********************/

    //unique instance
    private static Storage STORAGE;

    /**
    * Method to get the instance of storage to work with
    * */
    public static Storage get()
    {
        if (STORAGE == null)
        {
            STORAGE = new Storage();
            STORAGE.LoadPlayerPrefs();
        }
        return STORAGE;
    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject); //This object will persist between all scenes-> the Storage singleton will not be destroyed
    }

    /* *****************************************
     * Public methods for Saving and Loading   *
     *******************************************/
    /**
    * Method to Save a Storable object, the player preferences and application runtime platform will decide wether this is done local or remote or both
    * */
    public void Save<T>(Savable<T, Data<T>> st)
    {
        switch (Mode)
        {
            case StoreMode.Only_Local:
                SaveLocal(st);
                break;
            case StoreMode.Local_And_Remote:
                SaveLocal(st);
                if (internet()) SaveRemote(st);
                break;
            case StoreMode.Only_Remote:
                if (internet()) SaveRemote(st);
                break;
            case StoreMode.Always_Local_Remote_On_Wifi:
                SaveLocal(st);
                if (lan()) SaveRemote(st);
                break;

        }
    }

    /**
    * Method to load a Storable object from storage
    *
    * */
    public void Load<T>(Savable<T, Data<T>> st, string identification)
    {
        string path;

        //if possibly stored locally but no internet
        bool firstCase = (Mode == StoreMode.Local_And_Remote || Mode == StoreMode.Always_Local_Remote_On_Wifi) && !internet();
        //if only locally stored        
        bool secondCase = Mode == StoreMode.Only_Local;
        //if only use data on lan mode and no lan available
        bool thirdCase = Mode == StoreMode.Always_Local_Remote_On_Wifi && !lan();

        //load locally
        if (firstCase || secondCase || thirdCase)
        {
            path = findPath(identification);
            if (checkFileExtension(st, path))
            {
                LoadLocal<T>(st, path);
                return;
            }
            else throw new FileLoadException("Wrong file extension, data could not be loaded into this class.");
        }

        //if only stored remote
        if (Mode == StoreMode.Only_Remote)
        {
            LoadRemote(st, identification);
            return;
        }


        //if stored remote and locally and internet is available and permitted to use (in case of only lan)

        //compare last modified
        path = findPath(identification);
        DateTime dtLocal = File.GetLastWriteTime(path);
        DateTime dtRemote = st.LastModified(identification);

        //local file most recent
        if (dtLocal.CompareTo(dtRemote) <= 0)
        {
            if (checkFileExtension(st, path))
            {
                LoadLocal<T>(st, path);
                return;
            }
            else throw new FileLoadException("Wrong file extension, data could not be loaded into this class.");
        }
        //remote file most recent
        else
        {
            LoadRemote(st, identification);
            return;
        }
    }

    /* ******************
     * Storage Settings *
     *********************/

    public enum StoreMode { Only_Local, Only_Remote, Always_Local_Remote_On_Wifi, Local_And_Remote };
    private static string STORE_MODE_STRING = "StoreMode";
    public StoreMode Mode;

    /**
     * Method to request the Modus of storing
     * */
    public StoreMode getStoreMode()
    {
        return Mode;
    }

    /**
     * Method to change the Modus of storing
     * */
    public void setStoreMode(StoreMode m)
    {
        Mode = m;
        SavePlayerPrefs();
    }

    //Load playerpreferences for storage
    private void LoadPlayerPrefs()
    {
        int storeMode = PlayerPrefs.GetInt(STORE_MODE_STRING,-1); //returns default value -1 if there is no StoreMode playerpref found (first launch of app)
        if (storeMode == -1)
        {
            //Define defaults for each platform
            switch (Application.platform)
            {
                //mobile
                case RuntimePlatform.Android:
                case RuntimePlatform.IPhonePlayer:
                    Mode = Storage.StoreMode.Always_Local_Remote_On_Wifi;
                    break;
                //web
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    Mode = Storage.StoreMode.Only_Remote;
                    break;
                //desktop and development environment
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    Mode = Storage.StoreMode.Local_And_Remote;
                    break;
            }
            SavePlayerPrefs();
        }
        else Mode = (Storage.StoreMode)storeMode;
    }

    //Save playerpreferences for storage
    private void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt(STORE_MODE_STRING, (int)Mode);
        PlayerPrefs.Save();
    }


    /* *****************************
    * Helper variables and methods *
    ********************************/

    //non-static variables
    private string RootFolder = Application.persistentDataPath + "/3DVirtualMuseum/";

    public Storage()
    {
    }

    // Update is called once per frame
    void Update()
    {}

    private void SaveRemote<T>(Savable<T, Data<T>> st)
    {
        st.SaveRemote();
    }


    private void SaveLocal<T>(Savable<T, Data<T>> st)
    {
        //Require data to save
        var data = st.Save();
        //Build path where to save
        string path = RootFolder + st.getFolder() + "/" + st.getFileName() +"." +st.getExtension();
        //create file and save data
        Stream TestFileStream = File.Create(path);//does this overwrite existing files? -> Yes !
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(TestFileStream, data);
        TestFileStream.Close();
        Debug.Log("Data Saved Locally.");
    }

    private void LoadRemote<T>(Savable<T, Data<T>> st, string identifier)
    {
        st.LoadRemote(identifier);
    }

    private void LoadLocal<T>(Savable<T, Data<T>> st, string path)
    {
        if (File.Exists(path))
        {
            Stream TestFileStream = File.OpenRead(Application.persistentDataPath + "/test.bin");
            BinaryFormatter deserializer = new BinaryFormatter();
            Data<T> data = (Data<T>)deserializer.Deserialize(TestFileStream);
            TestFileStream.Close();
            st.Load(data);
        }
        throw new FileNotFoundException("Could not load data because file does not exist. ("+path+")");
    }

    private bool checkFileExtension<T>(Savable<T, Data<T>> st, string path)
    {
        string[] splitPath = path.Split('.');
        return splitPath[splitPath.Length - 1].Equals( st.getExtension() );
    }

    private string findPath(string identification)
    {
        //TODO: identification will probably be a user/museum combo or another unique identifier for the server: find the path of this file locally (if it exists)
        return identification;
    }

    private DateTime lastModified(string path)
    {
        if (File.Exists(path)) return File.GetLastWriteTimeUtc(path);
        else return new DateTime(); // default constructor returns 1/1/0001 12:00:00 AM.
    }

    public bool lan()
    {
        return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
    }

    public bool carrier()
    {
        return (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
    }

    public bool internet()
    {
        return lan() || carrier();
    }

    
}
