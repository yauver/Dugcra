using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public static class SaveAndLoadManager
{
    //save/load:
    //inventory
    //!grids!
    //characters
    //player info
    //global inventory
    //quests
    //dialogue

    public static string windowsSaveLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    public static string gridSaveFolder = "Saves";

    public static string SaveLocation(string name)
    {
        string saveLocation = gridSaveFolder + "/";

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }

        saveLocation += name + ".json";

        return saveLocation;
    }

    public static void CheckDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);            
        }
    }

    public static string SaveTo(string directory, string fileName)
    {
        string saveLocation = directory + "/" + fileName;
        return saveLocation;
    }

    //Grid saving and loading
    #region
    public static void SaveGrid(Grid grid, bool savePacked)
    {
        Save save = new Save();
        List<Save.Tiles> t = new List<Save.Tiles>();
        Save.Tiles ti;
        for (int x = 0; x < Grid.gridSize; x++)
        {
            for (int y = 0; y < Grid.gridSize; y++)
            {
                ti.tile = grid.tiles[x, y];
                ti.pos = new WorldPos(x, y);
                t.Add(ti);
            }
        }
        save.ToArray(t);

        File.WriteAllText(SaveLocation(grid.ToString()), save.saveToString(savePacked));
    }

    public static bool LoadGrid(Grid grid)
    {
        if (!File.Exists(SaveLocation(grid.ToString())))
        {
            return false;
        }

        Stream file = new FileStream(SaveLocation(grid.ToString()), FileMode.Open);
        StreamReader text = new StreamReader(file);
        string t = text.ReadToEnd();
        file.Close();
        text.Close();

        Save save = JsonUtility.FromJson<Save>(t);

        try
        {
            foreach (var item in save.ToDictionary())
            {
                grid.tiles[item.Key.x, item.Key.y] = item.Value;
            }
        }
        catch (Exception)
        {
            Debug.Log(grid);
        }
        grid.update = true;
        return true;
    }
    #endregion
}
