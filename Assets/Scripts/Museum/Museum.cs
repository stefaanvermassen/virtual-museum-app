using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Museum : MonoBehaviour, Storable<Museum, MuseumData> {

    public List<MuseumTile> tiles = new List<MuseumTile>();
    public List<MuseumObject> objects = new List<MuseumObject>();
    public List<MuseumArt> art = new List<MuseumArt>();
    public string author;
    public string museumName;
    public string description;

    public Material frontMaterial;
    public Material backMaterial;

    public Texture2D debugTexture;

    public MuseumData Save() {
        var tileData = new List<MuseumTileData>();
        foreach (var t in tiles)
            tileData.Add(t.Save());
        var artData = new List<MuseumArtData>();
        foreach (var a in art)
            artData.Add(a.Save());
        var objectData = new List<MuseumObjectData>();
        foreach (var o in objects)
            objectData.Add(o.Save());
        return new MuseumData(tileData, artData, objectData, author, museumName, description);
    }
    public void Load(MuseumData data) {
        Clear();
        foreach (var tileData in data.Tiles)
            SetTile(tileData.WallStyle, tileData.FloorStyle, tileData.CeilingStyle, tileData.X, tileData.Y, tileData.Z);
        foreach (var artData in data.Art)
            AddArt(artData.ArtID, artData.X, artData.Y, artData.Z, artData.Orientation);
        foreach (var objectData in data.Objects)
            AddObject(objectData.ObjectID, objectData.X, objectData.Y, objectData.Z, objectData.Angle);
        author = data.Author;
        museumName = data.MuseumName;
        description = data.Description;
    }

    public void Clear() {
        foreach (var t in tiles) {
            t.Remove();
            Destroy(t.gameObject);
        }
        tiles.Clear();
        foreach (var a in art) {
            a.Remove();
            Destroy(a.gameObject);
        }
        art.Clear();
        foreach (var o in objects) {
            o.Remove();
            Destroy(o.gameObject);
        }
        objects.Clear();
    }

    public void AddArt(int artID, int x, int y, int z, int orientation) {
        if (ContainsTile(x, y, z) && (
                    (orientation == 0 && !ContainsTile(x,y,z-1)) ||
                    (orientation == 1 && !ContainsTile(x-1,y,z)) ||
                    (orientation == 2 && !ContainsTile(x,y,z+1)) ||
                    (orientation == 3 && !ContainsTile(x+1,y,z))
                )
            ) {
            RemoveArt(x, y, z);
            GameObject o = new GameObject();
            MuseumArt a = o.AddComponent<MuseumArt>();
            a.x = x;
            a.y = y;
            a.z = z;
            a.orientation = orientation;
            a.material = frontMaterial;
            a.texture = debugTexture;
            a.artID = artID;
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

    public void AddObject(int objectID, int x, int y, int z, float angle) {
        if (ContainsTile(x, y, z)) {
            RemoveObject(x, y, z);
            var ob = new GameObject();
            var museumObject = ob.AddComponent<MuseumObject>();
            museumObject.objectID = objectID;
            museumObject.x = x;
            museumObject.y = y;
            museumObject.z = z;
            museumObject.angle = angle;
            objects.Add(museumObject);
        }
    }

    public void RemoveObject(int x, int y, int z) {
        MuseumObject toRemove = null;
        foreach (MuseumObject o in objects) {
            if (o.x == x && o.y == y && o.z == z) {
                toRemove = o;
                break;
            }
        }
        if (toRemove != null) {
            objects.Remove(toRemove);
            toRemove.Remove();
            Destroy(toRemove.gameObject);
        }
    }

    public void SetTile(int wallStyle = 0, int floorStyle = 0, int ceilingStyle = 0, int x = 0, int y = 0, int z = 0) {
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
        tile.frontMaterial = frontMaterial;
        tile.backMaterial = backMaterial;
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
            RemoveArt(x, y, z);
            RemoveObject(x, y, z);
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
