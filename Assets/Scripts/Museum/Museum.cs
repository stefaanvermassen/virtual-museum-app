using UnityEngine;
using System.Collections.Generic;

public class Museum : MonoBehaviour {

    public List<MuseumTile> tiles = new List<MuseumTile>();
    public List<GameObject> objects = new List<GameObject>();
    public List<MuseumArt> art = new List<MuseumArt>();
    public string author;
    //public string name;
    public string description;

    public Material material;

	void Start () {
	}

    public void AddArt(int x, int y, int z, int orientation, Texture2D texture) {
        RemoveArt(x, y, z);
        if(ContainsTile(x,y,z)){
            GameObject o = new GameObject();
            MuseumArt a = o.AddComponent<MuseumArt>();
            a.x = x;
            a.y = y;
            a.z = z;
            a.orientation = orientation;
            a.material = material;
            a.texture = texture;
            art.Add(a);
        }
    }

    public bool ContainsArt(int x, int y, int z) {
        foreach(MuseumArt a in art){
            if (a.x == x && a.y == y && a.z == z) return true;
        }
        return false;
    }

    public void RemoveArt(int x, int y, int z) {
        MuseumArt toRemove = null;
        foreach (MuseumArt a in art) {
            if (a.x == x && a.y == y && a.z == z) {
                toRemove = a;
                break;
            }
        }
        if (toRemove != null) {
            art.Remove(toRemove);
            toRemove.Remove();
            Destroy(toRemove.gameObject);
        }
    }

    public void AddObject(GameObject ob, Vector3 position, Vector3 rotation) {
        RemoveObject(position, 1);
        var clone = (GameObject) Instantiate(ob,position,Quaternion.Euler(rotation));
        objects.Add(clone);
    }

    public void RemoveObject(Vector3 position, float maxDistance) {
        GameObject toRemove = null;
        foreach (GameObject o in objects) {
            if (Vector3.Distance(o.transform.position, position) < maxDistance) {
                toRemove = o;
                break;
            }
        }
        if (toRemove != null) objects.Remove(toRemove);
        Destroy(toRemove);
    }

    public void SetTile(int x = 0, int y = 0, int z = 0, int wallStyle = 0, int floorStyle = 0, int ceilingStyle = 0) {
        RemoveTile(x, y, z);
        GameObject tileObject = new GameObject();
        tileObject.transform.parent = transform.parent;
        tileObject.transform.localPosition = new Vector3(x, y, z);
        var tile = tileObject.AddComponent<MuseumTile>();
        tiles.Add(tile);
        tile.x = x;
        tile.y = y;
        tile.z = z;
        tile.wallStyle = wallStyle;
        tile.floorStyle = floorStyle;
        tile.ceilingStyle = ceilingStyle;
        tile.material = material;
        var leftTile = GetTile(x - 1, y, z);
        var rightTile = GetTile(x + 1, y, z);
        var frontTile = GetTile(x, y, z + 1);
        var backTile = GetTile(x, y, z - 1);
        if (leftTile == null) tile.left = true;
        else leftTile.right = false;
        if (rightTile == null) tile.right = true;
        else rightTile.left = false;
        if (frontTile == null) tile.front = true;
        else frontTile.back = false;
        if (backTile == null) tile.back = true;
        else backTile.front = false;
        if (leftTile != null) leftTile.UpdateEdges();
        if (rightTile != null) rightTile.UpdateEdges();
        if (backTile != null) backTile.UpdateEdges();
        if (frontTile != null) frontTile.UpdateEdges();
    }

    public void RemoveTile(int x, int y, int z) {
        var tile = GetTile(x, y, z);
        if (tile != null) {
            tiles.Remove(tile);
            tile.Remove();
            Destroy(tile.gameObject);
            var leftTile = GetTile(x - 1, y, z);
            var rightTile = GetTile(x + 1, y, z);
            var frontTile = GetTile(x, y, z + 1);
            var backTile = GetTile(x, y, z - 1);
            if (leftTile != null) leftTile.right = true;
            if (rightTile != null) rightTile.left = true;
            if (frontTile != null) frontTile.back = true;
            if (backTile != null) backTile.front = true;
            if (leftTile != null) leftTile.UpdateEdges();
            if (rightTile != null) rightTile.UpdateEdges();
            if (backTile != null) backTile.UpdateEdges();
            if (frontTile != null) frontTile.UpdateEdges();
        }
    }

    public bool ContainsTile(int x, int y, int z) {
        return GetTile(x, y, z) != null;
    }

    public MuseumTile GetTile(int x, int y, int z) {
        foreach (MuseumTile tile in tiles) {
            if (tile.x == x && tile.y == y && tile.z == z) return tile;
        }
        return null;
    }
	
	void Update () {
	
	}
}
