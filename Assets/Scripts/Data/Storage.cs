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
    public void Save<T, D>(Savable<T, D> st) where D : Data<T>
    {
        switch (Mode)
        {
            case StoreMode.Only_Local:
                SaveLocal<T,D>(st);
                break;
            case StoreMode.Local_And_Remote:
                SaveLocal<T, D>(st);
                if (Internet()) SaveRemote<T, D>(st);
                break;
            case StoreMode.Only_Remote:
                if (Internet()) SaveRemote<T, D>(st);
                break;
            case StoreMode.Always_Local_Remote_On_Wifi:
                SaveLocal<T, D>(st);
                if (Lan()) SaveRemote<T, D>(st);
                break;
            default:
                //This should not happen, but sonar wants a default case, so I'll throw an exception, because this would be a VERY unexpected outcome
                throw new Exception("Unexpected SaveMode in Storage component");

        }
    }

    /// <summary>
    /// Method to load a storable object from storage which uses integers for identification
    /// </summary>
    /// <param name="st">Object where data will be loaded to</param>
    /// <param name="identification">idintifier</param>
    public void Load<T, D>(Savable<T, D> st, int identification) where D : Data<T>
    {
        Load<T,D>(st, ""+identification);
    }

    /// <summary>
    /// Method to load a Storable object from storage which uses strings for identification
    /// </summary>
    /// <param name="st">The object where data will be loaded to.</param>
    /// <param name="identification">unique string identification of the object</param>
    public void Load<T, D>(Savable<T, D> st, string identification) where D : Data<T>
    {
        string path;

        //if possibly stored locally but no internet
        bool firstCase = (Mode == StoreMode.Local_And_Remote || Mode == StoreMode.Always_Local_Remote_On_Wifi) && !Internet();
        //if only locally stored        
        bool secondCase = Mode == StoreMode.Only_Local;
        //if only use data on lan mode and no lan available
        bool thirdCase = Mode == StoreMode.Always_Local_Remote_On_Wifi && !Lan();

        //load locally
        if (firstCase || secondCase || thirdCase)
        {
            path = FindPath(identification);

            LoadLocal<T,D>(st, path);
            return;

        }

        //if only stored remote
        if (Mode == StoreMode.Only_Remote)
        {
            LoadRemote(st, identification);
            return;
        }


        //if stored remote and locally and internet is available and permitted to use (in case of only lan)

        //compare last modified
        path = FindPath(identification);
        DateTime dtLocal = File.GetLastWriteTime(path);
        DateTime dtRemote = st.LastModified(identification);

        Debug.Log("STORAGE: Local File Last Modified: " + dtLocal.ToString());
        Debug.Log("STORAGE: Remote File Last Modified: " + dtRemote.ToString());

        //local file most recent
        if (dtLocal.CompareTo(dtRemote) > 0)
        {
            
            LoadLocal<T,D>(st, path);
            return;

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
        switch (Mode)
        {
        case StoreMode.Only_Local:
            SaveLocal(data);
            break;
        case StoreMode.Local_And_Remote:
            SaveLocal(data);
            if (Internet()) SaveRemote(data);
            break;
        case StoreMode.Only_Remote:
            if (Internet()) SaveRemote(data);
            break;
        case StoreMode.Always_Local_Remote_On_Wifi:
            SaveLocal(data);
            if (Lan()) SaveRemote(data);
            break;
        default:
            //This should not happen, but sonar wants a default case, so I'll throw an exception, because this would be a VERY unexpected outcome
            throw new Exception("Unexpected SaveMode in Storage component");


        }
    }


    /// <summary>
    /// Method that reads data from storage and loads it into an object, this method is only used for objects that are not monobehaviors
    /// </summary>
    /// 
    /// <param name="data">Object where data should be loaded to (this is needed to get the type)</param>
    /// <param name="identification">unique string identification of the object (eg: "objectX:id:01")</param>
    /// <returns>object with the loaded data in its fields</returns>
    public SavableData Load(SavableData data, int identification)
    {
        return Load(data, "" + identification);
    }


    /// <summary>
    /// Method that reads data from storage and loads it into an object, this method is only used for objects that are not monobehaviors
    /// </summary>
    /// 
    /// <param name="data">Object where data should be loaded to (this is needed to get the type)</param>
    /// <param name="identification">unique string identification of the object (eg: "objectX:id:01")</param>
    /// <returns>object with the loaded data in its fields</returns>
    public SavableData Load(SavableData data, string identification)
    {
        string path;

        //if possibly stored locally but no internet
        bool firstCase = (Mode == StoreMode.Local_And_Remote || Mode == StoreMode.Always_Local_Remote_On_Wifi) && !Internet();
        //if only locally stored        
        bool secondCase = Mode == StoreMode.Only_Local;
        //if only use data on lan mode and no lan available
        bool thirdCase = Mode == StoreMode.Always_Local_Remote_On_Wifi && !Lan();

        //load locally
        if (firstCase || secondCase || thirdCase)
        {
            path = FindPath(identification);

            data = LoadLocal(data, path);
            return data;
          
        }

        //if only stored remote
        if (Mode == StoreMode.Only_Remote)
        {
            data = LoadRemote(data, identification);
            return data;
        }


        //if stored remote and locally and internet is available and permitted to use (in case of only lan)

        //compare last modified
        path = FindPath(identification);
        DateTime dtLocal = File.GetLastWriteTime(path);
        DateTime dtRemote = data.LastModified(identification);

        //local file most recent
        if (dtLocal.CompareTo(dtRemote) <= 0)
        {

            data = LoadLocal(data, path);
            return data;
            
        }
        //remote file most recent
        else
        {
            data = LoadRemote(data, identification);
            return data;
        }
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
    public StoreMode GetStoreMode()
    {
        return Mode;
    }

    /// <summary>
    /// Method to change the Modus of storing
    /// </summary>
    /// <param name="m">Desired modus of storing</param>
    public void SetStoreMode(StoreMode m)
    {
        if (StoreModeAllowed(m))
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
    public bool StoreModeAllowed(StoreMode m)
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
                default:
                    //Unsupported platform, should not happen, but Sonar wants default case, so I'll throw an exception
                    throw new Exception("Unexpected Platform, Storage doesn't know where to save stuff, how did you manage to run this app on a magical platform?");
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


    /// <summary>
    /// Helper function used to save a Savable object (Data from a MonoBehavior) Remotely
    /// </summary>
    /// <param name="st">The object to be saved remotely</param>
    public void SaveRemote<T,D>(Savable<T,D> savable) where D : Data<T>
    {
        savable.SaveRemote();
        Debug.Log("STORAGE: Data Saved Remotely.");
    }

    /// <summary>
    /// Helper function to save a savable object (Data from a MonoBehavior) locally
    /// </summary>
    /// <param name="st">The object to be saved</param>
    public void SaveLocal<T,D>(Savable<T, D> savable) where D : Data<T>
    {
        //Require data to save
        var data = savable.Save();
        //Build path where to save
        string path = RootFolder + savable.getFolder();
        bool folderExists = Directory.Exists(path);
        if (!folderExists) Directory.CreateDirectory(path);
        //create file and save data
        path += "/" + savable.getFileName() + "." + savable.getExtension();
        Stream TestFileStream = File.Create(path);//does this overwrite existing files? -> Yes !
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(TestFileStream, data);
        TestFileStream.Close();
        Debug.Log("STORAGE: Data Saved Locally to " + path);
    }

    /// <summary>
    /// Helper function to save SavableData (non Monobehaviour object) remotely
    /// </summary>
    /// <param name="data">data to be saved</param>
    public void SaveRemote(SavableData data)
    {
        data.SaveRemote();
        Debug.Log("STORAGE: Data Saved Remotely.");
    }


    /// <summary>
    /// Helper function to save SavableData (non Monobehaviour object) locally
    /// </summary>
    /// <param name="data">data to be saved</param>
    public void SaveLocal(SavableData data)
    {
        //Build path where to save
        string path = RootFolder + data.getFolder();
        bool folderExists = Directory.Exists(path);
        if (!folderExists) Directory.CreateDirectory(path);
        //create file and save data
        path += "/" + data.getFileName() + "." + data.getExtension();
        Stream TestFileStream = File.Create(path);//does this overwrite existing files? -> Yes !
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(TestFileStream, data);
        TestFileStream.Close();
        Debug.Log("STORAGE: Data Saved Locally.");
    }



    /// <summary>
    /// Helper function to load a savable object remotely
    /// </summary>
    /// <param name="st">Object where the data should be loaded to</param>
    /// <param name="identifier">string to identify the object on the backend</param>
    public void LoadRemote<T>(Savable<T, Data<T>> st, string identifier)
    {
        st.LoadRemote(identifier);
    }

    /// <summary>
    /// Helper function to load data from a file to an object
    /// </summary>
    /// <param name="st">The object where the data will be loaded to</param>
    /// <param name="path">the path of the file to load from</param>
    public void LoadLocal<T, D>(Savable<T, D> savable, string path) where D : Data<T>
    {
        if (File.Exists(path))
        {
            if(CheckFileExtension<T,D>(savable,path)){
                Stream TestFileStream = File.OpenRead(path);
                BinaryFormatter deserializer = new BinaryFormatter();
                D data = (D)deserializer.Deserialize(TestFileStream);
                TestFileStream.Close();
                savable.Load(data);
            }else throw new FileLoadException("Wrong file extension, data could not be loaded into this class.");
        }
        else throw new FileNotFoundException("Could not load data because file does not exist. ("+path+")");
    }


    /// <summary>
    /// Helper function to load savable data from remote server into data object
    /// </summary>
    /// <param name="st">Object where the data should be loaded to</param>
    /// <param name="identifier">string to identify the object on the backend</param>
    public SavableData LoadRemote(SavableData data, string identifier)
    {
        data.LoadRemote(identifier);
        Debug.Log("STORAGE: Data Loaded Remotely.");
        return data;
    }

    /// <summary>
    /// Helper function to load savable data from local file into data object
    /// </summary>
    /// <param name="st">The object where the data will be loaded to</param>
    /// <param name="path">the path of the file to load from</param>
    public SavableData LoadLocal(SavableData data, string path)
    {
        if (File.Exists(path))
        {
            if(CheckFileExtension(data,path)){
                Stream TestFileStream = File.OpenRead(path);
                BinaryFormatter deserializer = new BinaryFormatter();
                data = (SavableData)deserializer.Deserialize(TestFileStream);
                TestFileStream.Close();
                Debug.Log("STORAGE: Data Loaded Locally.");
                return data;
            }
            else throw new FileLoadException("Wrong file extension, data could not be loaded into this class.");
        }
        else throw new FileNotFoundException("Could not load data because file does not exist. (" + path + ")");
    }



    /// <summary>
    /// Helper function that checks if the file Extension of the desired file fits the object where data should be loaded to
    /// </summary>
    /// <param name="st">Object where data will be loaded to</param>
    /// <param name="path">File where we want to load data from</param>
    /// <returns>if these are of the same type (file extension matches)</returns>
    private bool CheckFileExtension<T, D>(Savable<T, D> savable, string path) where D : Data<T>
    {
        Debug.Log("STORAGE: Checking file extension");
        string[] splitPath = path.Split('.');
        return splitPath[splitPath.Length - 1].Equals(savable.getExtension());
    }

    /// <summary>
    /// Helper function that checks if the file Extension of the desired file fits the object where data should be loaded to
    /// </summary>
    /// <param name="st">Object where data will be loaded to</param>
    /// <param name="path">File where we want to load data from</param>
    /// <returns>if these are of the same type (file extension matches)</returns>
    private bool CheckFileExtension(SavableData data, string path)
    {
        Debug.Log("STORAGE: Checking file extension");
        string[] splitPath = path.Split('.');
        return splitPath[splitPath.Length - 1].Equals(data.getExtension());
    }

    /// <summary>
    /// Helper function to decide where an object will be saved locally
    /// </summary>
    /// <param name="identification">string for identification of the object on the server</param>
    /// <returns>path to the desired file</returns>
    private string FindPath(string identification)
    {
        //TODO: identification will probably be a user/museum combo or another unique identifier for the server: find the path of this file locally (if it exists)
        return identification;
    }

    /// <summary>
    /// Helper function to check the date an object was last modified, 
    /// this can be used to check if the remote object has changed since the last local save or not. If it is up to date no pull from the server is needed
    /// </summary>
    /// <param name="path">path of the file we want to have the last modified data from</param>
    /// <returns>Last modified date</returns>
    private DateTime LastModified(string path)
    {
        if (File.Exists(path)) return File.GetLastWriteTimeUtc(path);
        else return new DateTime(); // default constructor returns 1/1/0001 12:00:00 AM.
    }

    /// <summary>
    /// Helper function that checks if the application has internet via Local Area Network
    /// </summary>
    /// <returns>if there is a LAN connection</returns>
    public bool Lan()
    {
        return (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
    }

    /// <summary>
    /// Helper function that checks if the application has internet via a Carrier Data Network
    /// </summary>
    /// <returns>if there is a 4G connection</returns>
    public bool Carrier()
    {
        return (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork);
    }

    /// <summary>
    /// Helper function that checks if the application has an internet connection
    /// </summary>
    /// <returns>if there is an internet connection</returns>
    public bool Internet()
    {
        return Lan() || Carrier();
    }

}
