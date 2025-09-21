namespace SSNC;

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[BepInPlugin("Bryan_-000-.SSNC", "SSNC", "1.0.0")]
public class PluginLoader : BaseUnityPlugin
{
    private void Awake() => SceneManager.sceneLoaded += (_, _) =>
    {
        if (Plugin.Instance == null) 
        {
            Tools.Make<Plugin>("SSNC").Location = Info.Location;
            Plugin.BepLog = Logger;
            Plugin.UseDiff = Config.Bind("Settings", "UseDiff", true, "If true, uses regular SSNC UI. If false, does an older version of SSNC'S UI that may be buggier.").Value;
        }
    };
}

/// <summary> The Mod. </summary>
public class Plugin : MonoBehaviour
{
    /// <summary> Logger used to log to ULTRAKILL and the log folder by the Mod. </summary>
    public static Logger Log;
    /// <summary> Logger used to log to BepInEx by the Mod. </summary>
    public static ManualLogSource BepLog;
    /// <summary> Logger used to log to BepInEx by the Mod. </summary>
    public static ManualLogSource UnityLog;

    /// <summary> Instance of the Mod, inside the scene "DontDestroyOnLoad" so it doesnt gets destoryed on a scene load. </summary>
    public static Plugin Instance;

    public static bool UseDiff;

    /// <summary> Location of the DLL in the pc. (example: C:\Program Files (x86)\Steam\steamapps\common\ULTRAKILL\BepInEx\plugins\SaveSlotNameChanger\SaveSlotNameChanger.dll)</summary>
    public string Location;

    /// <summary> Whether the Mod has done its stuff(shit). </summary>
    private bool DoneShit;

    /// <summary> Prevent the Mod from being vaporized on a scene load. </summary>
    private void Awake() =>
        DontDestroyOnLoad(Instance = this);

    /// <summary> Make the logger and setup DoShit to run once the game is loaded. </summary>
    private void Start()
    {
        Log = new("SSNC");

        SceneManager.sceneLoaded += (scene, mode) =>
        {
            if (SceneHelper.CurrentScene == "Main Menu") DoShit();
        };
    }

    /// <summary> Flush all logs to a file when the game is quit. </summary>
    private void OnApplicationQuit() => Log.Flush();

    /// <summary> Do shit, what else am I meant to say? </summary>
    private void DoShit()
    {
        if (DoneShit) return;

        UIbuilder.Load();
        foreach (var slotty in UI.supportedSlot)
        {
            string path = Path.Combine(GameProgressSaver.BaseSavePath, slotty.Value, "SlotName.txt");
            if (!File.Exists(path))
                Tools.Generate_Name_File(path, slotty.Key);
        }

        Log.Info("Loading UI");
        UI.Load();
        SceneManager.sceneLoaded += (scene, mode) =>
        {
            Log.Info($"Load UI after loading into scene {scene.name} | {mode}");
            UI.Load();
        };

        DoneShit = true;
    }
}

/// <summary> Class containing fun lil stuff used by the Mod. </summary>
public static class Tools
{
    #region Save Slot

    /// <summary> "Generates"/Creates a new text file within a SaveSlot, used for naming the SaveSlot. </summary>
    /// <param name="path">SaveSlot Path.</param>
    /// <param name="slotname">The slots name.</param>
    /// <returns>Whether it successfully "Generated"/Created the SaveSlot name file.</returns>
    public static bool Generate_Name_File(string path, string slotname)
    {
        if (File.Exists(path)) return true;

        string saveDirectory = Path.GetDirectoryName(path);
        if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);

        File.AppendAllLines(path, [slotname]);

        if (File.Exists(path)) {
            Plugin.Log.Info("Successfully generated new Save Slot Name File");
            return true;
        }
        else {
            Plugin.Log.Error("Failed to generate new Save Slot Name File");
            return false;
        }
    }

    /// <summary> Changes the name of a SaveSlot. </summary>
    /// <param name="newname">The new name that the SaveSlot's name should be set to.</param>
    /// <param name="slot">Name of the save slot's folder.</param>
    /// <param name="Path">SaveSlot Path. (DISCLAIMER: only put this param if you already have the path, to prevent needing to search for the path again.)</param>
    public static void Change_Slot_Name(string[] newname, string slot, string Path = null)
    {
        Plugin.Log.Info("Changing Save Slot("+slot+") Name: " + string.Join(" | ", newname));
        Path ??= System.IO.Path.Combine(GameProgressSaver.BaseSavePath, slot, "SlotName.txt");

        if (!File.Exists(Path) && !Generate_Name_File(Path, newname[0]))
        {
            Plugin.Log.Error("tried to change the name of a slot that doesnt have a name file..");
            Plugin.Log.Error("attempted to generate a new name file and failed");
            return;
        }

        File.WriteAllLines(Path, newname);
    }

    /// <summary> Gets the index of the SaveSlot based on its position in the UI. </summary>
    /// <param name="row">The SaveSlot UI.</param>
    /// <returns>The index of the SaveSlot if found, or -1 if it is not.</returns>
    public static int Get_Save_Slot_UI_Index(GameObject row)
    {
        int i = 0;
        foreach (Transform child in row.transform.parent)
        {
            i++;
            if (child.gameObject == row) return i;
        }
        return -1;
    }

    #endregion
    #region unity

    /// <summary> Cached Sprites from the users pc.<para>(THIS IS NOT SHARED BTW THIS JUST USED FOR THE "EDIT PENCIL" ICON) </para></summary>
    public static Dictionary<string, Sprite> CacheLoadedSprite = [];

    /// <summary> Loads a PNG from off the users pc and create a UnityEngine.Sprite from it.<para>(THIS IS NOT SHARED BTW THIS JUST USED FOR THE "EDIT PENCIL" ICON) </para></summary>
    /// <param name="filePath">Path of the PNG file to load.</param>
    /// <returns>The UnityEngine.Sprite if found from on the pc and loaded.<para>Unless the Sprite was cached, then it loads the Sprite from there.</para>If the Sprite wasnt found it will Log and return null.</returns>
    public static Sprite LoadSpriteFromDisk(string filePath)
    {
        if (CacheLoadedSprite.ContainsKey(filePath)) {
            Plugin.Log.Fine($"{Plugin.Log.GetNameOfMethod(typeof(Tools), "LoadSpriteFromDisk")} Sprite was cached, loading it from there...");
            return CacheLoadedSprite[filePath];
        }

        if (!filePath.EndsWith(".png"))
        {
            Plugin.Log.Error($"{Plugin.Log.GetNameOfMethod(typeof(Tools), "LoadSpriteFromDisk")} File isn't even a PNG. (filePath: {filePath})");
            return null;
        }

        if (!File.Exists(filePath)) {
            Plugin.Log.Error($"{Plugin.Log.GetNameOfMethod(typeof(Tools), "LoadSpriteFromDisk")} File Path doesnt exist. (filePath: {filePath})");
            return null;
        }

        byte[] imageBytes = File.ReadAllBytes(filePath);
        Texture2D texture = new(1, 1, TextureFormat.ARGB32, true) { filterMode = FilterMode.Point };

        if (!texture.LoadImage(imageBytes)) {
            Plugin.Log.Error($"{Plugin.Log.GetNameOfMethod(typeof(Tools), "LoadSpriteFromDisk")} Couldn't could bytes from {filePath} as a Texture2D.");
            return null;
        }

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new UnityEngine.Vector2(0.5f, 0.5f));
        CacheLoadedSprite.Add(filePath, sprite);
        return sprite;
    }

    /// <summary> Cached Objects. </summary>
    public static Dictionary<Type, UnityEngine.Object[]> Cache = [];

    /// <summary> Searches for an Object with that name. </summary>
    /// <typeparam name="type">Type of Object to search for.</typeparam>
    /// <param name="with_the_name">The name of the Object to search for.</param>
    /// <returns>The Object with the name, if not found; returns the default for the Type.</returns>
    public static type SearchForA<type>(string with_the_name) where type : UnityEngine.Object
    {
        Type _Type = typeof(type);
        if (!Cache.TryGetValue(_Type, out var cached))
        {
            cached = Resources.FindObjectsOfTypeAll<type>();
            Cache.Add(_Type, cached);
        }

        return (type)Array.Find(cached, s => s.name == with_the_name);
    }

    /// <summary> Makes a GameObject and adds the component of your choice. </summary>
    /// <typeparam name="component">Component that is added to the GameObject.</typeparam>
    /// <param name="name">Name of the GameObject.</param>
    /// <param name="parent">Parent of the GameObject.</param>
    /// <returns>The component that was added to the GameObject.</returns>
    public static component Make<component>(string name, Transform parent = null) where component : Component
    {
        GameObject obj = new(name);
        obj.transform.SetParent(parent ?? Plugin.Instance?.transform, false);
        return obj.AddComponent<component>();
    }

    /// <summary> Makes a GameObject. </summary>
    /// <param name="name">Name of the GameObject.</param>
    /// <param name="parent">Parent of the GameObject.</param>
    /// <returns>The GameObject.</returns>
    public static GameObject Make(string name, Transform parent = null)
    {
        GameObject obj = new(name);
        obj.transform.SetParent(parent ?? Plugin.Instance?.transform, false);
        return obj;
    }

    /// <summary> Finds a GameObject based on name/path, doesnt matter if its enabled or not. </summary>
    /// <param name="Path">The name/path of the GameObject to hunt down.</param>
    /// <param name="scene">The scene to hunt the GameObject down in. (defaults to scene 0 if null)</param>
    /// <returns>The GameObject if found. If it isn't found, returns null.</returns>
    public static GameObject ObjFindIncludeInactive(string Path, Scene? scene = null)
    {
        scene ??= SceneManager.GetSceneAt(0);
        string rootSearchObj = Path;
        int IndexOfSlash = Path.IndexOf('/');
        if (IndexOfSlash != -1)
        {
            rootSearchObj = Path[..IndexOfSlash];
            Path = Path[(IndexOfSlash + 1)..];
        }

        var search = (from gameObject in scene.Value.GetRootGameObjects() where gameObject.name == rootSearchObj select gameObject).FirstOrDefault();
        search = IndexOfSlash == -1 ? search : search.transform.Find(Path)?.gameObject;
        if (search != null) Plugin.Log.Info($"Found {search.name}"); else Plugin.Log.Error($"Couldn't find {rootSearchObj} / {Path}");
        return search;
    }

    /// <summary> Finds a GameObject based on name/path, doesnt matter if its enabled or not, and gets/adds the component(T). </summary>
    /// <typeparam name="T">Component to Get/Add.</typeparam>
    /// <param name="Path">The name/path of the GameObject to hunt down.</param>
    /// <param name="scene">The scene to hunt the GameObject down in. (defaults to scene 0 if null)</param>
    /// <returns>The GameObject's component(or the one added if it didnt exist) if found. If it isn't found, returns null.</returns>
    public static T ObjFindIncludeInactive<T>(string Path, Scene? scene = null) where T : Component {
        GameObject obj = ObjFindIncludeInactive(Path, scene);
        return obj == null ? null : obj.GetComponent<T>() ?? obj.AddComponent<T>();
    }

    /// <summary> Sets a GameObject and all of its children active. </summary>
    /// <param name="gameObject">The GameObject to activate.</param>
    /// <param name="active">Whether to deactivate or reactivate.</param>
    /// <returns>The GameObject once its active.</returns>
    public static GameObject SetCompletelyActive(GameObject gameObject, bool active) =>
        SetCompletelyActive(gameObject.transform, active).gameObject;

    /// <summary> Sets a Transform's GameObject and all of its children active. </summary>
    /// <param name="gameObject">The GameObject to activate.</param>
    /// <param name="active">Whether to deactivate or reactivate.</param>
    /// <returns>The Transform once its GameObject is active.</returns>
    public static Transform SetCompletelyActive(Transform transform, bool active)
    {
        transform.gameObject.SetActive(active);
        foreach (Transform child in transform)
            child.gameObject.SetActive(active);

        return transform;
    }

    /// <summary> Gets all children in a GameObject. </summary>
    /// <param name="parent">The GameObject parent to get children from.</param>
    /// <returns>An array of GameObjects filled with each child, returns null if there isnt any.</returns>
    public static GameObject[] GetChildren(GameObject parent)
    {
        GameObject[] result = (GameObject[])(from child in GetChildren(parent.transform) ?? [] select child.gameObject);
        return result == Enumerable.Empty<GameObject>() || result == null ? null : result; 
    }

    /// <summary> Gets all children in a transform. </summary>
    /// <param name="parent">The Transform parent to get children from.</param>
    /// <returns>An array of Transforms filled with each child, returns null if there isnt any.</returns>
    public static Transform[] GetChildren(Transform parent)
    {
        Transform[] children = null;
        foreach (Transform child in parent) children.AddToArray(child);
        return children;
    }

    #endregion
}