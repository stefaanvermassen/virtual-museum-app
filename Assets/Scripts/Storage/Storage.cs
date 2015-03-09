using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Storage : MonoBehaviour{

    private static Storage storage;

    private BinaryFormatter BF;

    public Storage()
    {

    }

    /**
     * Require Storage object
     */
    public static Storage get()
    {
        if (storage == null)
        {
            storage = new Storage();
            storage.BF = new BinaryFormatter();
        }

        return storage;
    }

    /**
     * Method to save persistent objects in binary file
     * */
    public void Save(System.Object obj)
    {

        if (!obj.GetType().IsSerializable) throw new NotSerializableException("The object you are trying to save is of type: " + obj.GetType() + ", this type is not serializable.");
        //TODO: code to determine local and/or remote save
        bool local = true;
        bool remote = false;

        //TODO: Saver<Savable>

        //MUSEUM
        Museum museum = obj as Museum;
        if (museum != null && local) SaveMuseumLocal(museum);
        if (museum != null && remote) SaveMuseumRemote(museum);

    }

    //TODO: calls to this method should be organised properly
    private void LoadMuseum(string name)
    {
        string path = Application.persistentDataPath + "/museums/" + name + ".museum";
        if (File.Exists(path))
        {
            FileStream file = File.Open(path, FileMode.Open);
            //MuseumData data = (MusemData)BF.Deserialize(file);
            file.Close();
            //return data;
        }
    }

    private void SaveMuseumLocal(Museum museum)
    {
        
        FileStream file = File.Create(Application.persistentDataPath + "/museums/" + museum.name + ".museum"); //check: does this overwrite previous save?

        /*
        MuseumData  data = museum.Data;
        BF.Serialize(file, data);
         */
        file.Close();
    }

    private void SaveMuseumRemote(Museum museum)
    {
        //TODO: implement remote saving to server
    }

	void Start(){}
    void Update(){}
}

class NotSerializableException : Exception
{
    public NotSerializableException(string message):base(message)
    {
    }
}