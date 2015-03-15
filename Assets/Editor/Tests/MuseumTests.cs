using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

[TestFixture]
public class MuseumTests {

    [Test]
    public void InformationTest() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.museumName = "Naam";
        museum.description = "Beschrijving";
        Assert.IsTrue(museum.museumName == "Naam", "Name should be correctly set");
        Assert.IsTrue(museum.description == "Beschrijving", "Description should be correctly set");
    }

    [Test]
    public void TileNeighbourTest()
    {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        Assert.IsFalse(museum.ContainsTile(1, 0, 2), "Museum should not contain tile.");
        museum.SetTile(0, 0, 0, 1, 0, 2);
        Assert.IsTrue(museum.ContainsTile(1,0,2), "Museum should contain tile.");
        var tile = museum.GetTile(1, 0, 2);
        Assert.IsTrue(tile.left && tile.right && tile.front && tile.back, "Tile should have no neighbours");
        museum.SetTile(0, 0, 0, 0, 0, 2);
        Assert.IsFalse(tile.left, "Tile has left neighbour now");
        museum.SetTile(0, 0, 0, 2, 0, 2);
        Assert.IsFalse(tile.right, "Tile has right neighbour now");
        museum.SetTile(0, 0, 0, 1, 0, 1);
        Assert.IsFalse(tile.back, "Tile has back neighbour now");
        museum.SetTile(0, 0, 0, 1, 0, 3);
        Assert.IsFalse(tile.front, "Tile has front neighbour now");
    }

    [Test]
    public void AddingObjectTest() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should not contain this object");
        museum.AddObject(0, 10, 0, 15, 0);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should still not contain this object, because there is no tile");
        Assert.IsFalse(museum.ContainsTile(10, 0, 15), "Museum should not contain this tile yet");
        museum.SetTile(0, 0, 0, 10, 0, 15);
        Assert.IsTrue(museum.ContainsTile(10, 0, 15), "Museum should contain this tile");
        museum.AddObject(0, 10, 0, 15, 0); 
        Assert.IsTrue(museum.ContainsObject(10, 0, 15), "Museum should contain this object");
        museum.RemoveObject(10, 0, 15);
        Assert.IsFalse(museum.ContainsObject(10, 0, 15), "Museum should not contain this object anymore");

        museum.AddObject(0, 10, 0, 15, 0);
        Assert.IsTrue(museum.ContainsObject(10, 0, 15), "Museum should contain this object");
        museum.RemoveTile(10, 0, 15);
        Assert.IsFalse(museum.ContainsTile(10, 0, 15), "Tile is removed");
        Assert.IsFalse(museum.ContainsTile(10, 0, 15), "Object should be gone after removing tile");
    }

    [Test]
    public void AddingArtTest() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Museum should not contain this art");
        museum.AddObject(0, 10, 0, 15, 0);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Museum should still not contain this art, because there is no tile");
        Assert.IsFalse(museum.ContainsTile(10, 0, 15), "Museum should not contain this tile yet");
        museum.SetTile(0, 0, 0, 10, 0, 15);
        Assert.IsTrue(museum.ContainsTile(10, 0, 15), "Museum should contain this tile");
        museum.AddArt(0, 10, 0, 15, 0);
        Assert.IsTrue(museum.ContainsArt(10, 0, 15), "Museum should contain this art");
        museum.RemoveArt(10, 0, 15);
        Assert.IsFalse(museum.ContainsArt(10, 0, 15), "Museum should not contain this art anymore");

        museum.AddArt(0, 10, 0, 15, 0);
        Assert.IsTrue(museum.ContainsArt(10, 0, 15), "Museum should contain this art");
        museum.RemoveTile(10, 0, 15);
        Assert.IsFalse(museum.ContainsTile(10, 0, 15), "Tile is removed");
        Assert.IsFalse(museum.ContainsTile(10, 0, 15), "Art should be gone after removing tile");
    }

    [Test]
    public void ArtOrientationTest() {
        var ob = new GameObject();
        var museum = ob.AddComponent<Museum>();
        museum.SetTile(0, 0, 0, 0, 0, 0);
        museum.AddArt(0, 0, 0, 0, 0);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 0");
        museum.AddArt(0, 0, 0, 0, 1);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 1");
        museum.AddArt(0, 0, 0, 0, 2);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 2");
        museum.AddArt(0, 0, 0, 0, 3);
        Assert.IsTrue(museum.ContainsArt(0, 0, 0), "Art in single tile should be placeable with orientation 3");
        museum.RemoveArt(0, 0, 0);
        Assert.IsFalse(museum.ContainsArt(0, 0, 0), "Art should be gone after removing");
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
    }

    [Test]
    public void SerializationTest() {
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
    }

}
