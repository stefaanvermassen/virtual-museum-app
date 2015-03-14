using UnityEngine;
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

    public Storage()
    {
        //empty constructor
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
    public void Save<T>(Storable<T, Data<T>> st)
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
    * */
    public void Load<T>(Storable<T, Data<T>> st, string path)
    {

        //if no internet, only load locally
        if (!internet())
        {
            if (checkType(st, path))
            {
                LoadLocal<T>(st, path);
            }
        }

        //if internet, check date of last changed
        else
        {
            //TODO: compare dates and decide to load or not (e.g. carrier and settings only lan: don't load but prompt user warning
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
    private string RootFolder = "/3DVirtualMuseum";
    private string MuseumFolder = "/museums/";
    private string MuseumFileExtension = ".mus";

    // Update is called once per frame
    void Update()
    {}

    private void SaveRemote<T>(Storable<T, Data<T>> st)
    {
        //TODO: implement
        //idea: make extra methods on Storable? 
        throw new System.NotImplementedException();
    }


    private void SaveLocal<T>(Storable<T, Data<T>> st)
    {
        //Require data to save
        var data = st.Save();
        //Require path where to save
        string path = getPath(st);
        //create file and save data
        Stream TestFileStream = File.Create(path);//does this overwrite existing files? -> Yes !
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(TestFileStream, data);
        TestFileStream.Close();
        Debug.Log("Data Saved Locally.");
    }

    private void LoadRemote<T>(Storable<T, Data<T>> st, string path)
    {
        //TODO: implement
        //idea: make extra methods on Storable? 
        throw new System.NotImplementedException();
    }

    private void LoadLocal<T>(Storable<T, Data<T>> st, string path)
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


    private string getPath<T>(Storable<T, Data<T>> st)
    {
        string path = Application.persistentDataPath + RootFolder;
        //if type is Museum
        if (typeof(T) == typeof(Museum))
        {
            string museumName = ((Museum)st).museumName.Replace(' ', '_'); //replace all spaces in the museum name with underscores
            path += MuseumFolder + museumName + MuseumFileExtension;
        }

        return path;
    }

    private bool checkType<T>(Storable<T, Data<T>> st, string path)
    {
        //if type is Museum
        if (typeof(T) == typeof(Museum))
        {
            string[] splitPath = path.Split('.');
            //is the file of type museum?
            return splitPath[splitPath.Length - 1].Equals( MuseumFileExtension );
        }

        return false;
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
