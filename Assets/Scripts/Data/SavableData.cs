using UnityEngine;
using System.Collections;
using System;


public interface SavableData {

    string getFolder();
    string getFileName();
    string getExtension();

    //These methods should contain server API calls
    void SaveRemote();
    void LoadRemote(string identifier);
    DateTime LastModified(string identifier);
}
