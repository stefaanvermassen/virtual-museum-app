using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class Storage : MonoBehaviour {

    public enum StoreMode { Only_Local, Only_Remote, Always_Local_Remote_On_Wifi, Local_And_Remote };

    private static Storage STORAGE;
    private string STORE_MODE_STRING = "StoreMode";
    public static StoreMode MODE;

    private string MUSEUM_FOLDER ="/museums/";

    public Storage()
    {
    }

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(gameObject); //This object will persist between all scenes
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static Storage get()
    {
        if (STORAGE == null)
        {
            STORAGE = new Storage();
            LoadPlayerPrefs();
        }
        return STORAGE;
    }

    public void Save<T>(Storable<T, Data<T>> st)
    {
        if (MODE == StoreMode.Only_Local || MODE == StoreMode.Local_And_Remote || MODE == StoreMode.Always_Local_Remote_On_Wifi)
        {
            SaveLocal(st);
        }

        //refactor: aparte calls voor leesbaarheid -> Done !

        bool wifi = (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork);
        bool internet = wifi || ( Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)

        if(MODE == StoreMode.Always_Local_Remote_On_Wifi && wifi){
            SaveRemote(st);
        }

        if(MODE == StoreMode.Only_Remote || MODE == StoreMode.Local_And_Remote)
        {
            if(internet) SaveRemote(st);
        }

    }

    private void SaveRemote<T>(Storable<T, Data<T>> st)
    {
        //TODO: implement
        //idea: make extra methods on Storable? 
        throw new System.NotImplementedException();
    }


    public void SaveLocal<T>(Storable<T,Data<T>> st)
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
        Debug.Log("Data Saved");
    }


    private string getPath<T>(Storable<T, Data<T>> st)
    {
        string path = Application.persistentDataPath + "/3DVirtualMuseum";
        //if type is Museum
        if (typeof(T) == typeof(Museum))
        {
            string museumName = ((Museum)st).museumName.Replace(' ', '_');
            path += MUSEUM_FOLDER + museumName + ".mus";
        }

        return path;
    }

    public void Load()
    {
    }

    public static StoreMode getStoreMode()
    {
        return MODE;
    }

    public static void setStoreMode(StoreMode m)
    {
        MODE = m;
        SavePlayerPrefs();
    }

    //Load playerpreferences for storage
    public static void LoadPlayerPrefs()
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
                    MODE = Storage.StoreMode.Always_Local_Remote_On_Wifi;
                    break;
                //web
                case RuntimePlatform.WebGLPlayer:
                case RuntimePlatform.OSXWebPlayer:
                case RuntimePlatform.WindowsWebPlayer:
                    MODE = Storage.StoreMode.Only_Remote;
                    break;
                //desktop and development environment
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.LinuxPlayer:
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.OSXEditor:
                    MODE = Storage.StoreMode.Local_And_Remote;
                    break;
            }
            SavePlayerPrefs();
        }
        else MODE = (Storage.StoreMode)storeMode;
    }

    //Save playerpreferences for storage
    public static void SavePlayerPrefs()
    {
        PlayerPrefs.SetInt(STORE_MODE_STRING, (int)MODE);
    }
}
