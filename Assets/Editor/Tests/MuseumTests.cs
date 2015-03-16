using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class MuseumTests {

    private static int TEST_CASES = 10;
    private static int SEED = 123;

    public MuseumTests() {
        Random.seed = SEED;
    }

    int RandomInt(int from, int until) {
        return (int)(Random.value * (from+until) - from);
    }

    string RandomString(int minLength, int maxLength) {
        string s = "";
        int length = RandomInt(minLength, maxLength);
        for (int i = 0; i < length; i++) {
            char c = (char)RandomInt(0, 255);
            s += c;
        }
        return s;
    }

    void DestroyEverything() {
        var objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var o in objects) GameObject.DestroyImmediate(o);
    }

    [Test]
    public void ChangingMuseumInformation_ChangingMuseumName_NameChanged() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            var name = RandomString(1, 100);
            museum.museumName = name;
            Assert.AreEqual(museum.museumName, name, "Name should be correctly set to "+name+" but it's "+museum.museumName);
        }
        DestroyEverything();
    }

    [Test]
    public void ChangingMuseumInformation_ChangingMuseumDescription_DescriptionChanged() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            var description = RandomString(1, 1000);
            museum.description = description;
            Assert.AreEqual(museum.description, description, "Description should be correctly set to " + description + " but it's " + museum.description);
        }
        DestroyEverything();
    }

    [Test]
    public void PlacingAndRemovingTiles_PlacingSingleTile_TilePlaced() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            int wallStyle = RandomInt(0, 100);
            int floorStyle = RandomInt(0, 100);
            int ceilingStyle = RandomInt(0, 100);
            int x = RandomInt(-100, 100);
            int y = 0;
            int z = RandomInt(-100, 100);
            Assert.IsFalse(museum.ContainsTile(x, y, z), "Museum should not contain tile.");
            museum.SetTile(wallStyle, floorStyle, ceilingStyle, x, y, z);
            Assert.IsTrue(museum.ContainsTile(x, y, z), "Museum should contain tile.");
            var tile = museum.GetTile(x, y, z);
            Assert.AreEqual(tile.wallStyle, wallStyle, "Wallstyle not correctly set");
            Assert.AreEqual(tile.floorStyle, floorStyle, "Floorstyle not correctly set");
            Assert.AreEqual(tile.ceilingStyle, ceilingStyle, "Ceilingstyle not correctly set");
            museum.Clear();
        }
        DestroyEverything();
    }

    [Test]
    public void PlacingAndRemovingTiles_PlacingSingleTile_AllTileWallsArePresent() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            int x = RandomInt(-100, 100);
            int y = 0;
            int z = RandomInt(-100, 100);
            museum.SetTile(0, 0, 0, x, y, z);
            var tile = museum.GetTile(x, y, z);
            Assert.IsTrue(tile.left && tile.right && tile.front && tile.back, "Tile should have no neighbours");
            museum.Clear();
        }
        DestroyEverything();
    }

    public void PlacingAndRemovingTiles_PlacingNeighbours_WallsDisappear() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            int x = RandomInt(-100, 100);
            int y = 0;
            int z = RandomInt(-100, 100);
            museum.SetTile(0, 0, 0, x, y, z);
            var tile = museum.GetTile(x, y, z);
            museum.SetTile(0, 0, 0, x-1, y, z);
            Assert.IsFalse(tile.left, "Tile has left neighbour now");
            museum.SetTile(0, 0, 0, x+1, y, z);
            Assert.IsFalse(tile.right, "Tile has right neighbour now");
            museum.SetTile(0, 0, 0, x, y, z-1);
            Assert.IsFalse(tile.back, "Tile has back neighbour now");
            museum.SetTile(0, 0, 0, x, y, z+1);
            Assert.IsFalse(tile.front, "Tile has front neighbour now");
            museum.Clear();
        }
        DestroyEverything();
    }

    public void PlacingAndRemovingTiles_RemoveTile_TileIsGone() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            int x = RandomInt(-100, 100);
            int y = 0;
            int z = RandomInt(-100, 100);
            museum.SetTile(0, 0, 0, x, y, z);
            museum.RemoveTile(x, y, z);
            Assert.IsFalse(museum.ContainsTile(x, y, z), "Tile should be erased");
            museum.Clear();
        }
        DestroyEverything();
    }

    public void PlacingAndRemovingTiles_RemoveTileWhereObjectIsLocated_ObjectRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 10, 0, 15);
        museum.AddObject(0, 10, 0, 15, 0);
        museum.RemoveTile(10, 0, 15);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Object should be gone");
        DestroyEverything();
    }

    public void PlacingAndRemovingTiles_RemoveTileWhereArtIsLocated_ArtRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 10, 0, 15);
        museum.AddArt(0, 10, 0, 15, 0);
        museum.RemoveTile(10, 0, 15);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Art should be gone");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingObjects_AddObjectWithoutTile_ObjectNotCreated() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should not contain this object");
        museum.AddObject(0, 10, 0, 15, 0);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should still not contain this object, because there is no tile");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingObjects_AddObjectOnTile_CorrectObjectCreated() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 10, 0, 15);
        museum.AddObject(0, 10, 0, 15, 1.5f);
        Assert.IsTrue(museum.ContainsObject(10, 0, 15), "Museum should contain this object");
        var o = museum.GetObject(10, 0, 15);
        Assert.AreEqual(o.angle, 1.5f, "Angle not correctly set");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingObjects_RemoveObject_ObjectRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 10, 0, 15);
        museum.AddObject(0, 10, 0, 15, 0);
        museum.RemoveObject(10, 0, 15);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Object should be erased");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingArt_AddArtWithoutTile_ArtNotAdded() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.AddArt(0, 10, 0, 15, 0);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Art should not be placed");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingArt_AddArtOnTile_ArtAdded() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 10, 0, 15);
        museum.AddArt(0, 10, 0, 15, 0);
        Assert.IsTrue(museum.ContainsArt(10, 0, 15), "Art should be placed");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingArt_RemoveArt_ArtRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 10, 0, 15);
        museum.AddArt(0, 10, 0, 15, 0);
        museum.RemoveArt(10, 0, 15);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Art should be removed");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingArt_PlacingArtOnWalls_ArtAdded() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 0, 0, 0);
        museum.AddArt(0, 0, 0, 0, 0);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 0");
        museum.RemoveArt(0, 0, 0);
        museum.AddArt(0, 0, 0, 0, 1);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 1");
        museum.RemoveArt(0, 0, 0);
        museum.AddArt(0, 0, 0, 0, 2);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 2");
        museum.RemoveArt(0, 0, 0);
        museum.AddArt(0, 0, 0, 0, 3);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 3");
        DestroyEverything();
    }

    [Test]
    public void AddingAndRemovingArt_PlacingArtWithoutWall_ArtNotAdded() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 0, 0, 0);
        museum.SetTile(0, 0, 0, 0, 0, -1);
        museum.AddArt(0, 0, 0, 0, 0);
        Assert.IsFalse(museum.ContainsArt(0, 0, 0), "Art should not be placeable when there is no wall, orientation 0");
        museum.SetTile(0, 0, 0, -1, 0, 0);
        museum.AddArt(0, 0, 0, 0, 1);
        Assert.IsFalse(museum.ContainsArt(0, 0, 0), "Art should not be placeable when there is no wall, orientation 1");
        museum.SetTile(0, 0, 0, 0, 0, 1);
        museum.AddArt(0, 0, 0, 0, 2);
        Assert.IsFalse(museum.ContainsArt(0, 0, 0), "Art should not be placeable when there is no wall, orientation 2");
        museum.SetTile(0, 0, 0, 1, 0, 0);
        museum.AddArt(0, 0, 0, 0, 3);
        Assert.IsFalse(museum.ContainsArt(0, 0, 0), "Art should not be placeable when there is no wall, orientation 3");
        DestroyEverything();
    }

    [Test]
    public void SerializingMuseum_LoadingSavedMuseum_MuseumIsLoadedWithCorrectInformation() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 2, 3, 0, 0, 0);
        museum.SetTile(4, 5, 6, 1, 0, 1);
        museum.AddObject(123, 0, 0, 0, 0.5f);
        museum.AddArt(456, 1, 0, 1, 3);

        var museumData = museum.Save();
        museum.Clear();
        Assert.IsFalse(museum.ContainsTile(0, 0, 0), "Museum should be empty: tile 000");
        Assert.IsFalse(museum.ContainsTile(1,0,1),"Museum should be empty: tile 101");
        Assert.IsFalse(museum.ContainsObject(0,0,0),"Museum should be empty: object");
        Assert.IsFalse(museum.ContainsArt(1,0,1),"Museum should be empty: art");
        museum.Load(museumData);
        Assert.IsTrue(museum.ContainsTile(0, 0, 0), "Museum should be loaded: tile 000");
        Assert.IsTrue(museum.ContainsTile(1, 0, 1), "Museum should be loaded: tile 101");
        Assert.IsTrue(museum.ContainsObject(0, 0, 0), "Museum should be loaded: object");
        Assert.IsTrue(museum.ContainsArt(1, 0, 1), "Museum should be loaded: art");

        Assert.AreEqual(museum.GetTile(0, 0, 0).wallStyle, 1);
        Assert.AreEqual(museum.GetTile(0, 0, 0).floorStyle, 2);
        Assert.AreEqual(museum.GetTile(0, 0, 0).ceilingStyle, 3);
        Assert.AreEqual(museum.GetTile(1, 0, 1).wallStyle, 4);
        Assert.AreEqual(museum.GetTile(1, 0, 1).floorStyle, 5);
        Assert.AreEqual(museum.GetTile(1, 0, 1).ceilingStyle, 6);
        Assert.AreEqual(museum.GetObject(0, 0, 0).objectID, 123);
        Assert.AreEqual(museum.GetObject(0, 0, 0).angle, 0.5f);
        Assert.AreEqual(museum.GetArt(1, 0, 1).artID, 456);
        Assert.AreEqual(museum.GetArt(1, 0, 1).orientation, 3);
        DestroyEverything();
    }

}
