using UnityEngine;
using System.Collections.Generic;

public class Museum : MonoBehaviour {

    public List<MuseumTile> tiles = new List<MuseumTile>();
    public string author;
    public string name;
    public string description;

	void Start () {
	}

    public void SetTile(int x = 0, int y = 0, int z = 0, int wallStyle = 0, int floorStyle = 0, int ceilingStyle = 0) {
        RemoveTile(x, y, z);
        GameObject tileObject = new GameObject();
        tileObject.transform.position = new Vector3(x, y, z);
        var tile = tileObject.AddComponent<MuseumTile>();
        tiles.Add(tile);
        tile.x = x;
        tile.y = y;
        tile.z = z;
        tile.wallStyle = wallStyle;
        tile.floorStyle = floorStyle;
        tile.ceilingStyle = ceilingStyle;
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
        leftTile.UpdateEdges();
        rightTile.UpdateEdges();
        backTile.UpdateEdges();
        frontTile.UpdateEdges();
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
            leftTile.UpdateEdges();
            rightTile.UpdateEdges();
            frontTile.UpdateEdges();
            backTile.UpdateEdges();
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
