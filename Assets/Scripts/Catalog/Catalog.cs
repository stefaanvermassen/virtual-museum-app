using UnityEngine;
using System.Collections;

public static class Catalog {

    private static string[] objects = new string[] { "texmonkey", "Vase1" };

    public static GameObject GetObject(int objectID) {
        return Resources.Load<GameObject>(objects[objectID]);
    }

}
