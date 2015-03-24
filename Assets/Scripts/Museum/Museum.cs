using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using API;

/// <summary>
/// Internal museum representation. This can load and save museum 
/// representations and has methods to modify the museum.
/// </summary>
public class Museum : MonoBehaviour, Storable<Museum, MuseumData> {

    public List<MuseumTile> tiles = new List<MuseumTile>();
    public List<MuseumObject> objects = new List<MuseumObject>();
    public List<MuseumArt> art = new List<MuseumArt>();
    public string ownerID;
    public string museumName;
    public string description;

    public Material frontMaterial;
    public Material backMaterial;

    public Texture2D debugTexture;

    /// <summary>
    /// Create a MuseumData for serialization.
    /// </summary>
    /// <returns>The MuseumData</returns>
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
        return new MuseumData(tileData, artData, objectData, ownerID, museumName, description);
    }

    /// <summary>
    /// Load a MuseumData inside this museum.
    /// </summary>
    /// <param name="data"></param>
    public void Load(MuseumData data) {
        Clear();
        foreach (var tileData in data.Tiles)
            SetTile(tileData.WallStyle, tileData.FloorStyle, tileData.CeilingStyle, tileData.X, tileData.Y, tileData.Z);
        foreach (var artData in data.Art)
            AddArt(artData.Art.ID, artData.X, artData.Y, artData.Z, artData.Orientation);
        foreach (var objectData in data.Objects)
            AddObject(objectData.ObjectID, objectData.X, objectData.Y, objectData.Z, objectData.Angle);
        ownerID = data.OwnerID;
        museumName = data.MuseumName;
        description = data.Description;
    }

    /// <summary>
    /// Remove everything inside this museum.
    /// </summary>
    public void Clear() {
        foreach (var t in tiles) {
            t.Remove();
            Util.Destroy(t.gameObject);
        }
        tiles.Clear();
        foreach (var a in art) {
            a.Remove();
			Util.Destroy(a.gameObject);
        }
        art.Clear();
        foreach (var o in objects) {
            o.Remove();
			Util.Destroy(o.gameObject);
        }
        objects.Clear();
    }

    /// <summary>
    /// Add art, only works when it is placed on a wall.
    /// </summary>
    /// <param name="artID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="orientation">0, 1, 2 or 3, decides direction of painting</param>
    public void AddArt(int artID, int x, int y, int z, int orientation) {
        if (ContainsTile(x, y, z) && (
                    (orientation == 0 && !ContainsTile(x,y,z-1)) ||
                    (orientation == 1 && !ContainsTile(x-1,y,z)) ||
                    (orientation == 2 && !ContainsTile(x,y,z+1)) ||
                    (orientation == 3 && !ContainsTile(x+1,y,z))
                )
            ) {
            RemoveArt(x, y, z);
            MuseumArt ma = new GameObject().AddComponent<MuseumArt>();
            Art a = new Art();
            a.ID = artID;

            ma.x = x;
            ma.y = y;
            ma.z = z;
            ma.orientation = orientation;
            ma.material = frontMaterial;
            ma.texture = debugTexture;
            ma.art = a;
            art.Add(ma);
        }
    }

    /// <summary></summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>True if the coordinate contains art</returns>
    public bool ContainsArt(int x, int y, int z) {
        foreach(MuseumArt a in art){
            if (a.x == x && a.y == y && a.z == z) return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the art at position x,y,z. Returns null when there is none.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>Art at the coordinate, or null when there is none.</returns>
    public MuseumArt GetArt(int x, int y, int z) {
        foreach (MuseumArt a in art) {
            if (a.x == x && a.y == y && a.z == z) return a;
        }
        return null;
    }

    /// <summary>
    /// Removes the art at the coordinate.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
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
			Util.Destroy(toRemove.gameObject);
        }
    }

    /// <summary>
    /// Add an object, only works if there is already a tile at the coordinate.
    /// </summary>
    /// <param name="objectID"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="angle"></param>
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

    /// <summary>
    /// Removes the object at x,y,z if it exists.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
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
			Util.Destroy(toRemove.gameObject);
        }
    }

    /// <summary></summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>True if the coordinate contains an object.</returns>
    public bool ContainsObject(int x, int y, int z) {
        foreach (MuseumObject o in objects) {
            if (o.x == x && o.y == y && o.z == z) return true;
        }
        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>The object at x,y,z if it exists, null otherwise.</returns>
    public MuseumObject GetObject(int x, int y, int z) {
        foreach (MuseumObject o in objects) {
            if (o.x == x && o.y == y && o.z == z) return o;
        }
        return null;
    }

    /// <summary>
    /// Sets a tile using a wall, floor and ceiling-style at a position.
    /// </summary>
    /// <param name="wallStyle"></param>
    /// <param name="floorStyle"></param>
    /// <param name="ceilingStyle"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
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

    /// <summary>
    /// Remove the tile at x,y,z and everything it might contain.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public void RemoveTile(int x, int y, int z) {
        var tile = GetTile(x, y, z);
        if (tile != null) {
            RemoveArt(x, y, z);
            RemoveObject(x, y, z);
            tiles.Remove(tile);
            tile.Remove();
			Util.Destroy(tile.gameObject);
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

    /// <summary>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>True if there is a tile at x,y,z.</returns>
    public bool ContainsTile(int x, int y, int z) {
        return GetTile(x, y, z) != null;
    }

    /// <summary>
    /// Returns the tile at position x,y,z.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns>The tile at position x,y,z if it exists, null otherwise.</returns>
    public MuseumTile GetTile(int x, int y, int z) {
        foreach (MuseumTile tile in tiles) {
            if (tile.x == x && tile.y == y && tile.z == z) return tile;
        }
        return null;
    }
}
