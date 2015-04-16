using UnityEngine;
using System.Collections.Generic;

public static class Catalog {

    public static string[] objects = new string[] { "texmonkey", "Vase1", "Statue",
    "Lamp", "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey",
    "texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey","texmonkey"
    };

    public static string[] walls = new string[] {"Wall1", "Wall2", "Wall3"
    };

    public static string[] ceilings = new string[] {"Ceiling1", "Ceiling2", "Ceiling3"
    };

    public static string[] floors = new string[] {"Floor1", "Floor2", "Floor3", "Floor4"
    };

	public static string[] frames = new string[] {"Frame1", "Frame2"
	};

    private static Dictionary<int, GameObject> objectDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> wallDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> ceilingDictionary = new Dictionary<int, GameObject>();
    private static Dictionary<int, GameObject> floorDictionary = new Dictionary<int, GameObject>();
	private static Dictionary<int, GameObject> frameDictionary = new Dictionary<int, GameObject>();

    private static GameObject GetResource(int id, string[] names, string folder, Dictionary<int, GameObject> dictionary){
        if (!dictionary.ContainsKey(id)) {
            dictionary.Add(id, (GameObject)Resources.Load<GameObject>(folder+"/"+names[id]));
        }
        return dictionary[id];
    }

    public static GameObject GetObject(int objectID) {
        return GetResource(objectID, objects, "Objects", objectDictionary);
    }
    public static GameObject GetWall(int objectID) {
        return GetResource(objectID, walls, "Styles", wallDictionary);
    }
    public static GameObject GetCeiling(int objectID) {
        return GetResource(objectID, ceilings, "Styles", ceilingDictionary);
    }
    public static GameObject GetFloor(int objectID) {
        return GetResource(objectID, floors, "Styles", floorDictionary);
    }
	public static GameObject GetFrame(int objectID) {
		return GetResource(objectID, frames, "Frames", frameDictionary);
	}



}
