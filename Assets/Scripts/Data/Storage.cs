using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// This class is implemented using the Singleton design and is used to abstract saving and loading to local or remote storage
/// </summary>
public class Storage : MonoBehaviour {

    /* ******************
     * Singleton Code   *
     *********************/

    //unique instance
    private static Storage STORAGE;
 
    /// <summary>
    /// Get the instance of storage to work with
    /// </summary>
    public static Storage Instance
    {
        get
        {
            if (STORAGE == null)
            {
                STORAGE = new Storage();
                STORAGE.LoadPlayerPrefs();
            }
        
            return STORAGE;
        }
    }

    /// <summary>
    ///  Use this for initialization
    /// </summary>
    void Start()
    {
        DontDestroyOnLoad(gameObject); //This object will persist between all scenes-> the Storage singleton will not be destroyed
    }

    /* *****************************************
     * Public methods for Saving and Loading   *
     *******************************************/

  
    /// <summary>
    /// Method to Save a Storable object, the player preferences and application runtime platform will decide wether this is done local or remote or both
    /// </summary>
    /// <param name="st">The object that will be serialized and saved (should be of type Savable)</param>
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

    /// <summary>
    /// Method to load a Storable object from storage
    /// </summary>
    /// <param name="st">The object where data will be loaded to.</param>
    /// <param name="identification">unique string identification of the object</param>
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

    /// <summary>
    /// Use this to serialize and save an object that is not a monobehaviour
    /// </summary>
    /// <param name="data">the object to be serialized and saved</param>
    public void Save(SavableData data)
    {
        //TODO
        throw new NotImplementedException();
    }
    /// <summary>
    /// Method that reads data from storage and loads it into an object, this method is only used for objects that are not monobehaviors
    /// </summary>
    /// <param name="identification">unique string identification of the object (eg: "objectX:id:01")</param>
    /// <returns>object with the loaded data in its fields</returns>
    public SavableData Load(string identification)
    {
        //TODO
        throw new NotImplementedException();
    }

    /* ******************
     * Storage Settings *
     *********************/

    public enum StoreMode { Only_Local, Only_Remote, Always_Local_Remote_On_Wifi, Local_And_Remote };
    private static string STORE_MODE_STRING = "StoreMode";
    public StoreMode Mode;

    /// <summary>
    /// Method to request the Modus of storing
    /// </summary>
    /// <returns>Modus of Storing (enumtype)</returns>
    public StoreMode getStoreMode()
    {
        return Mode;
    }

    /// <summary>
    /// Method to change the Modus of storing
    /// </summary>
    /// <param name="m">Desired modus of storing</param>
    public void setStoreMode(StoreMode m)
    {
        if (storeModeAllowed(m))
        {
            Mode = m;
            SavePlayerPrefs();
        }
    }

    /// <summary>
    /// Checks if the store modus is allowed (e.g. Local storing is not allowed/possible on Browser)
    /// </summary>
    /// <param name="m">Deisred modus of storing</param>
    /// <returns>Modus is allowed or not</returns>
    public bool storeModeAllowed(StoreMode m)
    {
        //TODO: check if modus is possible on this platform ! (e.g. no local on browser)
        throw new NotImplementedException();
    }

    /// <summary>
    /// Load playerpreferences for storage from playerpreferences (is in local file system or cookies on browser)
    /// </summary>
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

    /// <summary>
    /// Save playerpreferences for storage to file system or cookies
    /// </summary>
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

    protected Storage()
    {
    }

    // Update is called once per frame
    void Update()
    {}


    /// <summary>
    /// Helper function used to save a Savable object Remotely
    /// </summary>
    /// <param name="st">The object to be saved remotely</param>
    private void SaveRemote<T>(Savable<T, Data<T>> st)
    {
        st.SaveRemote();
    }

    /// <summary>
    /// Helper function to save a savable object locally
    /// </summary>
    /// <param name="st">The object to be saved</param>
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

    /// <summary>
    /// Helper function to load a savable object remotely
    /// </summary>
    /// <param name="st">Object where the data should be loaded to</param>
    /// <param name="identifier">string to identify the object on the backend</param>
    private void LoadRemote<T>(Savable<T, Data<T>> st, string identifier)
    {
        st.LoadRemote(identifier);
    }

    /// <summary>
    /// Helper function to load data from a file to an object
    /// </summary>
    /// <param name="st">The object where the data will be loaded to</param>
    /// <param name="path">the path of the file to load from</param>
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

    /// <summary>
    /// Helper function that checks if the file Extension of the desired file fits the object where data should be loaded to
    /// </summary>
    /// <param name="st">Object where data will be loaded to</param>
    /// <param name="path">File where we want to load data from</param>
    /// <returns>if these are of the same type (file extension matches)</returns>
    private bool checkFileExtension<T>(Savable<T, Data<T>> st, string path)
    {
        string[] splitPath = path.Split('.');
        return splitPath[splitPath.Length - 1].Equals( st.getExtension() );
    }

    /// <summary>
    /// Helper function to decide where an object will be saved locally
    /// </summary>
    /// <param name="identification">string for identification of the object on the server</param>
    /// <returns>path to the desired file</returns>
    private string findPath(string identification)
    {
        //TODO: identification will probably be a user/museum combo or another unique identifier for the server: find the path of this file locally (if it exists)
        return identification;
    }

    /// <summary>
    /// Helper function to check the date an object was last modified, this can be used to check if the remote object has changed since the last local save or not. If it is up to date no pull from the server is needed
    /// </summary>
    /// <param name="path">path of the file we want to have the last modified data from</param>
    /// <returns>Last modified date</returns>
    private DateTime lastModified(string path)
    {
        if (File.Exists(path)) return File.GetLastWriteTimeUtc(path);
        else return new DateTime(); // default constructor returns 1/1/0001 12:00:00 AM.
    }

    /// <summary>
    /// Helper function that checks if the application has internet via Local Area Network
    /// </summary>
    /// <returns>if there is a LAN connection</returns>
    public bool lan()
    {
        return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
    }

    /// <summary>
    /// Helper function that checks if the application has internet via a Carrier Data Network
    /// </summary>
    /// <returns>if there is a 4G connection</returns>
    public bool carrier()
    {
        return (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
    }

    /// <summary>
    /// Helper function that checks if the application has an internet connection
    /// </summary>
    /// <returns>if there is an internet connection</returns>
    public bool internet()
    {
        return lan() || carrier();
    }

    
}
