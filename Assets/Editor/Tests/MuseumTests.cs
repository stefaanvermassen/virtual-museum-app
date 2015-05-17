using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class MuseumTests {

    private static int TEST_CASES = 10;
    private static int SEED = 123;

    [SetUp]
    public void Init() {
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

    [TearDown]
    public void DestroyEverything() {
        var objects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var o in objects) {
            GameObject.DestroyImmediate(o);
        }
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
    }

    [Test]
    public void PlacingAndRemovingTiles_PlacingSingleTile_AllTileWallsArePresent() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            int x = RandomInt(-100, 100);
            int y = 0;
            int z = RandomInt(-100, 100);
            museum.SetTile(1, 1, 1, x, y, z);
            var tile = museum.GetTile(x, y, z);
            Assert.IsTrue(tile.left && tile.right && tile.front && tile.back, "Tile should have no neighbours");
            museum.Clear();
        }
    }

    [Test]
    public void PlacingAndRemovingTiles_PlacingNeighbours_WallsDisappear() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
		Catalog.Refresh ();
        for (int i = 0; i < TEST_CASES; i++) {
            int x = RandomInt(1, 100);
            int y = 0;
            int z = RandomInt(1, 100);
            museum.SetTile(1, 1, 1, x, y, z);
            var tile = museum.GetTile(x, y, z);
            museum.SetTile(1, 1, 1, x-1, y, z);
            Assert.IsFalse(tile.left, "Tile has left neighbour now");
            museum.SetTile(1, 1, 1, x+1, y, z);
            Assert.IsFalse(tile.right, "Tile has right neighbour now");
            museum.SetTile(1, 1, 1, x, y, z-1);
            Assert.IsFalse(tile.back, "Tile has back neighbour now");
            museum.SetTile(1, 1, 1, x, y, z+1);
            Assert.IsFalse(tile.front, "Tile has front neighbour now");
            museum.Clear();
        }
    }

    [Test]
    public void PlacingAndRemovingTiles_RemoveTile_TileIsGone() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        for (int i = 0; i < TEST_CASES; i++) {
            int x = RandomInt(-100, 100);
            int y = 0;
            int z = RandomInt(-100, 100);
            museum.SetTile(1, 1, 1, x, y, z);
            museum.RemoveTile(x, y, z);
            Assert.IsFalse(museum.ContainsTile(x, y, z), "Tile should be erased");
            museum.Clear();
        }
    }

    [Test]
    public void PlacingAndRemovingTiles_RemoveTileWhereObjectIsLocated_ObjectRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 1, 1, 10, 0, 15);
        museum.AddObject(0, 10, 0, 15, 0);
        museum.RemoveTile(10, 0, 15);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Object should be gone");
    }

    [Test]
    public void PlacingAndRemovingTiles_RemoveTileWhereArtIsLocated_ArtRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 1, 1, 10, 0, 15);
        museum.AddArt(0, new Vector3(10, 0, 15), new Vector3(1, 1, 1), 1);
        museum.RemoveTile(10, 0, 15);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Art should be gone");
    }

    [Test]
    public void PlacingAndRemovingTiles_DoNothing_CenterTileAvailable() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.Start();
        Assert.IsTrue(museum.ContainsTile(0,0,0), "Center tile should be initialized");
    }

    [Test]
    public void PlacingAndRemovingTiles_RemoveTileAtCenter_TileNotRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.Start();
        museum.RemoveTile(0, 0, 0);
        Assert.IsTrue(museum.ContainsTile(0, 0, 0), "Center tile should not be removed");
    }

    [Test]
    public void AddingAndRemovingObjects_AddObjectWithoutTile_ObjectNotCreated() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should not contain this object");
        museum.AddObject(0, 10, 0, 15, 0);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should still not contain this object, because there is no tile");
    }

    [Test]
    public void AddingAndRemovingObjects_AddObjectOnTile_CorrectObjectCreated() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 1, 1, 10, 0, 15);
        museum.AddObject(0, 10, 0, 15, 1.5f);
        Assert.IsTrue(museum.ContainsObject(10, 0, 15), "Museum should contain this object");
        var o = museum.GetObject(10, 0, 15);
        Assert.AreEqual(o.angle, 1.5f, "Angle not correctly set");
    }

    [Test]
    public void AddingAndRemovingObjects_RemoveObject_ObjectRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 1, 1, 10, 0, 15);
        museum.AddObject(0, 10, 0, 15, 0);
        museum.RemoveObject(10, 0, 15);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Object should be erased");
    }

    [Test]
    public void AddingAndRemovingArt_AddArtWithoutTile_ArtNotAdded() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.AddArt(0, new Vector3(10, 0, 15), new Vector3(0,0,0), 1);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Art should not be placed");
    }

    [Test]
    public void AddingAndRemovingArt_AddArtOnTile_ArtAdded() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 1, 1, 10, 0, 15);
        museum.AddArt(0, new Vector3(10, 0.5f, 14.5f), new Vector3(1, 1, 1), 1);
        Assert.IsTrue(museum.ContainsArt(10, 0, 15), "Art should be placed");
    }

    [Test]
    public void AddingAndRemovingArt_RemoveArt_ArtRemoved() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 1, 1, 10, 0, 15);
        museum.AddArt(0, new Vector3(10, 0, 15), new Vector3(1, 1, 1), 1);
        museum.RemoveArt(10, 0, 15);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Art should be removed");
    }

    [Test]
    public void SerializingMuseum_LoadingSavedMuseum_MuseumIsLoadedWithCorrectInformation() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(1, 2, 3, 1, 1, 1);
        museum.SetTile(4, 5, 6, 1, 0, 1);
        museum.AddObject(123, 1, 1, 1, 0.5f);
        museum.AddArt(456, new Vector3(1, 0.5f, 0.5f), new Vector3(1, 1, 1), 1);

        var museumData = museum.Save();
        museum.Clear();
        Assert.IsFalse(museum.ContainsTile(1, 1, 1), "Museum should be empty: tile 000");
        Assert.IsFalse(museum.ContainsTile(1,0,1),"Museum should be empty: tile 101");
        Assert.IsFalse(museum.ContainsObject(1, 1, 1),"Museum should be empty: object");
        Assert.IsFalse(museum.ContainsArt(1,0,1),"Museum should be empty: art");
        museum.Load(museumData);
        Assert.IsTrue(museum.ContainsTile(1, 1, 1), "Museum should be loaded: tile 000");
        Assert.IsTrue(museum.ContainsTile(1, 0, 1), "Museum should be loaded: tile 101");
        Assert.IsTrue(museum.ContainsObject(1, 1, 1), "Museum should be loaded: object");
        Assert.IsTrue(museum.ContainsArt(1, 0, 1), "Museum should be loaded: art");

        Assert.AreEqual(museum.GetTile(1, 1, 1).wallStyle, 1);
        Assert.AreEqual(museum.GetTile(1, 1, 1).floorStyle, 2);
        Assert.AreEqual(museum.GetTile(1, 1, 1).ceilingStyle, 3);
        Assert.AreEqual(museum.GetTile(1, 0, 1).wallStyle, 4);
        Assert.AreEqual(museum.GetTile(1, 0, 1).floorStyle, 5);
        Assert.AreEqual(museum.GetTile(1, 0, 1).ceilingStyle, 6);
        Assert.AreEqual(museum.GetObject(1, 1, 1).objectID, 123);
        Assert.AreEqual(museum.GetObject(1, 1, 1).angle, 0.5f);
        Assert.AreEqual(museum.GetArt(1, 0, 1).art.ID, 456);
    }

}
