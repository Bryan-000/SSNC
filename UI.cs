using HarmonyLib;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SSNC;

public class UI
{
    public static Dictionary<string, string> supportedSlot = new()
    {
        {"Slot 1", "Slot1"},
        {"Slot 2", "Slot2"},
        {"Slot 3", "Slot3"},
        {"Slot 4", "Slot4"},
        {"Slot 5", "Slot5"},
        {"Slot M", "Slot11"},
    };

    public static void Load()
    { // game plan: enable and disable grid so it fucking loads the things and then put the shit on that ffs please and do this on each scene load
        GameObject OptionsMenu = Tools.ObjFindIncludeInactive("Canvas/OptionsMenu");
        GameObject SaveSlots = Tools.ObjFindIncludeInactive("Canvas/OptionsMenu/Save Slots");
        GameObject Grid = Tools.ObjFindIncludeInactive("Canvas/OptionsMenu/Save Slots/Grid");

        OptionsMenu.SetActive(true);
        SaveSlots.SetActive(true);
        OptionsMenu.SetActive(false);
        SaveSlots.SetActive(false);



        int index = 0;
        foreach (Transform Slot in Grid.transform)
        {
            index++;
            if (Slot.name == "Slot Row") continue;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     // whoever reads this, you have a free ticket for me to suck u off

            Transform SlotText = Slot.Find("Slot Text");
            if (!Plugin.UseDiff)
            {
                string key = SlotText.GetComponent<TextMeshProUGUI>().text;
                string displaytext = supportedSlot.ContainsKey(key)
                ? File.ReadAllText(Path.Combine(GameProgressSaver.BaseSavePath, supportedSlot[key], "SlotName.txt"))
                : "ERROR";

                UnityEngine.Object.Destroy(SlotText.GetComponent<TextMeshProUGUI>());
                SlotText.GetComponent<RectTransform>().anchoredPosition = new(120f, 0f);
                var field = SlotText.gameObject.AddComponent<TMP_InputField>();
                var image = UIbuilder.Image("Graphic", SlotText, new(0f, 0f, 225f, 47.5f), UIbuilder.Small_Fill, new(0f, 0f, 0f));
                UIbuilder.Image("Graphic bg", image.transform, new(0f, 0f, 225f, 47.5f), UIbuilder.Small_Border);
                var text = UIbuilder.Text("Text", "", SlotText, new(0f, 0f, 200f, 47.5f), 27, align: TextAlignmentOptions.Center);
                var placeholder = UIbuilder.Text("Placeholder", displaytext, SlotText, new(0f, 0f, 200f, 47.5f), 27, align: TextAlignmentOptions.Center);
                text.enableWordWrapping = placeholder.enableWordWrapping = true;

                field.textComponent = text;
                field.placeholder = placeholder;
                field.targetGraphic = image;
                field.onSelect.AddListener(_ => text.color = new(.55f, .55f, .55f));
                field.onDeselect.AddListener(_ => text.color = new(1f, 1f, 1f));
                field.onEndEdit.AddListener(typed =>
                {
                    if (supportedSlot.ContainsKey(key))
                    {
                        Plugin.Log.Debug($"is supported slot, {typed}");
                        string[] splitPoints = ["\\n", "\\r", "[n]", "[N]"];
                        Tools.Change_Slot_Name((typed.Length > 60 ? typed[..60] : typed).Split(splitPoints, StringSplitOptions.None), supportedSlot[key]);
                    }
                    else
                    {
                        Plugin.Log.Error("TRIED TO SAVE TO AN UNSUPPORTED SAVE SLOT.");
                        HudMessageReceiver.Instance?.SendHudMessage("ERROR: THIS SAVE SLOT IS UNSUPPORTED BY SSNC, PERHAPS IT IS MODDED.\nDUE TO THIS ERROR THE NAME INPUTED WAS <color=#f00><b>NOT</b></color> SAVED.");
                    }
                });
            }
            else
            {
                var placeholder = SlotText.gameObject.GetComponent<TextMeshProUGUI>();
                var key = placeholder.text;
                string displaytext = supportedSlot.ContainsKey(key)
                    ? File.ReadAllText(Path.Combine(GameProgressSaver.BaseSavePath, supportedSlot[key], "SlotName.txt"))
                    : "ERROR";
                if (!supportedSlot.ContainsKey(key)) continue;

                RectTransform RectSlotWrapper = Tools.Make<RectTransform>("SlotText Wrapper", Slot);
                var field = RectSlotWrapper.gameObject.AddComponent<TMP_InputField>();
                var text = UIbuilder.Text("Text", displaytext, RectSlotWrapper, new(0f, 0f, 200f, 47.5f), 27, align: TextAlignmentOptions.Center);
                placeholder.gameObject.name = "Placeholder";
                var OGcol = placeholder.color;
                SlotText.parent = RectSlotWrapper;
                placeholder.text = displaytext;
                text.enableWordWrapping = placeholder.enableWordWrapping = true;
                text.enableAutoSizing = placeholder.enableAutoSizing = true;
                text.fontSizeMax = placeholder.fontSizeMax = 27f;

                var RectText = SlotText.gameObject.GetComponent<RectTransform>();
                RectSlotWrapper.anchoredPosition = new(120f, 0f);
                RectSlotWrapper.anchorMin = RectSlotWrapper.anchorMax = new(0f, .5f);
                RectText.anchorMin = RectText.anchorMax = new(.5f, .5f);
                RectText.pivot = new(.5f, .5f);
                RectText.anchoredPosition = new(0, 0);
                RectText.sizeDelta = new(200f, 47.5f);

                field.onFocusSelectAll = false;
                field.textComponent = text;
                field.placeholder = placeholder;
                field.targetGraphic = text;
                Color c = placeholder.color;
                var result = Vector3.Lerp(new(c.r, c.g, c.b), new(.5f, .5f, .5f), .5f);
                Color darkened = new(result.x, result.y, result.z);
                field.onSelect.AddListener(_ => {
                    placeholder.color = darkened;
                    text.color = OGcol;
                }); 
                field.onDeselect.AddListener(_ => {
                    placeholder.color = text.color = OGcol;
                });
                field.onEndEdit.AddListener(typed =>
                {
                    if (!string.IsNullOrEmpty(typed) || typed.Length != 0 || typed != "")
                    {
                        if (supportedSlot.ContainsKey(key))
                        {
                            Plugin.Log.Debug($"is supported slot,\n{typed}");
                            string[] splitPoints = ["\\n", "\\r"];
                            Tools.Change_Slot_Name(typed.Split(splitPoints, StringSplitOptions.None), supportedSlot[key]);
                        }
                        else
                        {
                            Plugin.Log.Error("TRIED TO SAVE TO AN UNSUPPORTED SAVE SLOT.");
                            HudMessageReceiver.Instance?.SendHudMessage("ERROR: THIS SAVE SLOT IS UNSUPPORTED BY SSNC, PERHAPS IT IS MODDED.\nDUE TO THIS ERROR THE NAME INPUTED WAS <color=#f00><b>NOT</b></color> SAVED.");
                        }
                    }
                });
            }
        }
    }
}

/// <summary> Class used for building UI. </summary>
public class UIbuilder
{
    /// <summary> Sprites used in the UI. </summary>
    public static Sprite Large_Fill, Small_Fill, Large_Border, Small_Border, Large_Border_Black;

    /// <summary> The thing used for changing the color of buttons when you hover over them or click them. </summary>
    private static ColorBlock Fill, Border;

    /// <summary> Used for finding the assigned ColorBlock and PixelsPerUnit of that sprite as different sprites act differently in ULTRA_REVAMP UI. </summary>
    public static Dictionary<Sprite, (ColorBlock, float)> spriteValues = [];

    /// <summary> The TextMeshPro Font used by ULTRAKILL. </summary>
    public static TMP_FontAsset Font;

    /// <summary> Add's a Sprite's colors to the spriteColors Dick(:3)tionary. </summary>
    /// <param name="color">The ColorBlock for the Sprite.</param>
    /// <param name="sprites">The Sprite to set a ColorBlock for.</param>
    public static void Add(ColorBlock color, float PPU, params Sprite[] sprites) {
        foreach (Sprite sprite in sprites) {
            if (sprite == null) {
                Plugin.Log.Error($"Failed to add Sprite to spriteColors, Sprite doesnt exist. (SPRITE NAME: {sprite.name})");
                continue;
            }
            spriteValues.Add(sprite, (color, PPU));
        }
    }

    /// <summary> Assigns the according Sprites and TextMeshPro Font, then sets up the ColorBlock's.</summary>
    public static void Load()
    {
        Fill = Border = ColorBlock.defaultColorBlock;
        Border.highlightedColor = Border.selectedColor = new(.5f, .5f, .5f, 1f);
        Fill.pressedColor = Border.pressedColor = new(1f, 0f, 0f, 1f);

        Large_Fill = Tools.SearchForA<Sprite>(with_the_name: "Round_FillLarge");
        Small_Fill = Tools.SearchForA<Sprite>(with_the_name: "Round_FillSmall");
        Add(Fill, 4.05f, Large_Fill);
        Add(Fill, 5.4f, Small_Fill);
        Large_Border = Tools.SearchForA<Sprite>(with_the_name: "Round_BorderLarge");
        Small_Border = Tools.SearchForA<Sprite>(with_the_name: "Round_BorderSmall");
        Large_Border_Black = Tools.SearchForA<Sprite>(with_the_name: "Round_BorderLargeBlack");
        Add(Border, 4.05f, Large_Border, Large_Border_Black);
        Add(Border, 5.4f, Small_Border);
        Font = Tools.SearchForA<TMP_FontAsset>(with_the_name: "VCR_OSD_MONO_1");
    }

    /// <summary> Makes a new GameObject and adds RectTransform + any other component. </summary>
    /// <typeparam name="T">The Component Added to the GameObject after RectTransform.</typeparam>
    /// <param name="name">Name of the GameObject.</param>
    /// <param name="parent">Parent of the GameObject</param>
    /// <param name="pos">Position and Scale of the GameObject.</param>
    /// <param name="act">Action with param T and is ran right before returning.</param>
    /// <returns>T.</returns>
    public static T Create<T>(string name, Transform parent, Pos pos, Action<T> act) where T : Component
    {
        RectTransform trans = Tools.Make<RectTransform>(name, parent);
        trans.anchorMin = pos.Min;
        trans.anchorMax = pos.Max; 
        trans.anchoredPosition = pos.Position; // "piece of shit.Position" frfr
        trans.sizeDelta = pos.Scale;

        T component = trans.gameObject.AddComponent<T>();
        act(component);
        return component;
    }

    /// <summary> Creates a GameObject with an Image component with the corresponding Sprite. </summary>
    /// <param name="name">Name of the GameObject which contains the Image component.</param>
    /// <param name="parent">Parent of the GameObject which contains the Image component.</param>
    /// <param name="pos">Position and Scale of the GameObject and Image.</param>
    /// <param name="sprite">Sprite assigned to the Image.</param>
    /// <param name="color">Color of the Image.</param>
    /// <param name="fillCenter">Image.fillCenter.</param>
    /// <param name="obj"></param>
    /// <returns>The Image component.</returns>
    public static Image Image(string name, Transform parent, Pos pos, Sprite sprite = null, Color? color = null, bool fillCenter = true) =>
        Create<Image>(name, parent, pos, image =>
        {
            image.sprite = sprite ?? Large_Fill;
            image.fillCenter = fillCenter;
            (_, float PPU) = spriteValues[sprite ?? Large_Fill];
            image.pixelsPerUnitMultiplier = PPU;
            image.color = color ?? Color.white; // funni :3 (cuz like its white, cum is white, cum color.)
            image.type = UnityEngine.UI.Image.Type.Sliced; // so many words (⸝⸝๑﹏๑⸝⸝)
        });

    /// <summary> Creates a GameObject with a Button(and Image) component with the corresponding action to complete on click. </summary>
    /// <param name="name">Name of the GameObject which contains the Button component.</param>
    /// <param name="parent">Parent of the GameObject which contains the Button component.</param>
    /// <param name="pos">Position and Scale of the GameObject and Button.</param>
    /// <param name="sprite">Sprite assigned to the Button.</param>
    /// <param name="color">Color of the Button.</param>
    /// <param name="OnClick">Action ran once the Button is clicked.</param>
    /// <returns>The Button component.</returns>
    public static Button Button(string name, Transform parent, Pos pos, Sprite sprite = null, Color? color = null, Action OnClick = null)
    {
        Image image = Image(name, parent, pos, sprite, color);
        Button button = image.gameObject.AddComponent<Button>();

        button.onClick.AddListener(() => OnClick());
        button.targetGraphic = image;
        (ColorBlock colorBlock, float PPU) = spriteValues[sprite ?? Large_Fill];
        button.colors = colorBlock;
        return button;
    }

    /// <summary> Creates a GameObject with a TextMeshProUGUI component with the corresponding text. </summary>
    /// <param name="name">Name of the GameObject which contains the TextMeshProUGUI component.</param>
    /// <param name="Text">The text to be displayed.</param>
    /// <param name="parent">Parent of the GameObject which contains the TextMeshProUGUI component.</param>
    /// <param name="pos">Position and Scale of the GameObject and TextMeshProUGUI component.</param>
    /// <param name="size">Size of the text.</param>
    /// <param name="color">Color of the text.</param>
    /// <param name="align">Alignment of the text.</param>
    /// <returns>The TextMeshProUGUI component.</returns>
    public static TextMeshProUGUI Text(string name, string Text, Transform parent, Pos pos, int size = 16, Color? color = null, TextAlignmentOptions align = TextAlignmentOptions.Left) =>
        Create<TextMeshProUGUI>(name, parent, pos, text => 
        {
            text.text = Text;
            text.font = Font;
            text.fontSize = size;
            text.color = color ?? Color.black;
            text.alignment = align;
        });
}

/// <summary> Struct used for Position and Scale of UI(ik Rect exists but i just wanted to do funni comments). </summary>
/// <remarks> Creates a new Pos using 4 Vector2's. </remarks>
/// <param name="Pos">The position of the pivot of this RectTransform relative to the anchor reference point.
/// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
/// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
/// <param name="_Scale">The size of this RectTransform relative to the distances between the anchors.
/// <para> If the anchors are together, sizeDelta is the same as size.</para>
/// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
/// <param name="Minium">The normalized position in the parent RectTransform that the lower left corner is anchored to.</param>
/// <param name="Maxium">The normalized position in the parent RectTransform that the upper right corner is anchored to.</param>
public struct Pos(Vector2 Pos, Vector2 _Scale, Vector2 Minium, Vector2 Maxium)
{
    // holy fuck xml fucking sucks i almost killed myself over why i couldnt type "&" in 2 fucking summaries (position **&** scale)
    /// <summary> The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference. </para> </summary>
    public Vector2 Position = Pos;

    /// <summary> The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent. </para></summary>
    public Vector2 Scale = _Scale;

    /// <summary> The normalized position in the parent RectTransform that the lower left corner is anchored to. </summary>
    public Vector2 Min = Minium;

    /// <summary> The normalized position in the parent RectTransform that the upper right corner is anchored to. </summary>
    public Vector2 Max = Maxium;

    /// <summary> Creates a new Pos using 3 Vector2's. </summary>
    /// <param name="Pos">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Scale">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Anchor">The normalized position in the parent RectTransform that the middle is anchored to.</param>
    public Pos(Vector2 Pos, Vector2 Scale, Vector2 Anchor) : this(Pos, Scale, Anchor, Anchor) { }

    /// <summary> Creates a new Pos using 2 Vector2's. </summary>
    /// <param name="Pos">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Scale">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    public Pos(Vector2 Pos, Vector2 Scale) : this(Pos, Scale, new(.5f, .5f)) { }

    /// <summary> Creates a new Pos using 4 floats and 2 Vector2's. </summary>
    /// <param name="X">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Y">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Width">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Height">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Minium">The normalized position in the parent RectTransform that the lower left corner is anchored to.</param>
    /// <param name="Maxium">The normalized position in the parent RectTransform that the upper right corner is anchored to.</param>
    public Pos(float X, float Y, float Width, float Height, Vector2 Minium, Vector2 Maxium) : this(new(X, Y), new(Width, Height), Minium, Maxium) { }

    /// <summary> Creates a new Pos with 4 floats and 1 Vector2. </summary>
    /// <param name="X">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Y">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Width">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Height">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Anchor">The normalized position in the parent RectTransform that the middle is anchored to.</param>
    public Pos(float X, float Y, float Width, float Height, Vector2 Anchor) : this(X, Y, Width, Height, Anchor, Anchor) { }

    /// <summary> Creates a new Pos using 8 floats. </summary>
    /// <param name="X">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Y">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Width">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Height">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="MinX">The normalized position in the parent RectTransform that the lower left corner is anchored to.</param>
    /// <param name="MinY">The normalized position in the parent RectTransform that the lower left corner is anchored to.</param>
    /// <param name="MaxX">The normalized position in the parent RectTransform that the upper right corner is anchored to.</param>
    /// <param name="MaxY">The normalized position in the parent RectTransform that the upper right corner is anchored to.</param>
    public Pos(float X, float Y, float Width, float Height, float MinX, float MinY, float MaxX, float MaxY) : this(new Vector2(X, Y), new(Width, Height), new(MinX, MinY), new(MaxX, MaxY)) { }

    /// <summary> Creates a new Pos using 6 floats. </summary>
    /// <param name="X">Read Summary of Pos.Position.</param>
    /// <param name="Y">Read Summary of Pos.Position.</param>
    /// <param name="Width">Read Summary of Pos.Scale.</param>
    /// <param name="Height">Read Summary of Pos.Scale.</param>
    /// <param name="AnchorX">The normalized position in the parent RectTransform that the middle is anchored to.</param>
    /// <param name="AnchorY">The normalized position in the parent RectTransform that the middle is anchored to.</param>
    public Pos(float X, float Y, float Width, float Height, float AnchorX, float AnchorY) : this(X, Y, Width, Height, new(AnchorX, AnchorY)) { }

    /// <summary> Creates a new Pos using 4 floats. </summary>
    /// <param name="X">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Y">The position of the pivot of this RectTransform relative to the anchor reference point.
    /// <para> The Anchored Position is the position of the pivot of the RectTransform taking into consideration the anchor reference point. </para>
    /// <para> The anchor reference point is the position of the anchors.If the anchors are not together, Unity estimates the four anchor positions using the pivot placement as a reference.</para></param>
    /// <param name="Width">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    /// <param name="Height">The size of this RectTransform relative to the distances between the anchors.
    /// <para> If the anchors are together, sizeDelta is the same as size.</para>
    /// <para> If the anchors are in each of the four corners of the parent, the sizeDelta is how much bigger or smaller the rectangle is compared to its parent.</para></param>
    public Pos(float X, float Y, float Width, float Height) : this(X, Y, Width, Height, new(.5f, .5f)) { }

    #region operators

    /// <summary> Adds a Pos to another Pos. (made cuz boredom) </summary>
    /// <param name="a">Pos to be added to.</param>
    /// <param name="b">Pos to add.</param>
    /// <returns>Adds? how fucking hard is it to understand?</returns>
    public static Pos operator +(Pos a, Pos b) =>
        new(a.Position + b.Position, a.Scale + b.Scale);

    /// <summary> Subtracts a Pos from another Pos. (made cuz boredom) </summary>
    /// <param name="a">Pos to be subtracted from.</param>
    /// <param name="b">Pos to subtract.</param>
    /// <returns>YOU DONT DESERVE LIFE</returns>
    public static Pos operator -(Pos a, Pos b) =>
        new(a.Position - b.Position, a.Scale - b.Scale);

    /// <summary> Multiples a Pos by a Pos. </summary>
    /// <param name="a">Pos to be Multiplied.</param>
    /// <param name="b">Pos to Multiply by.</param>
    /// <returns>Multiples? what else can i say bro</returns>
    public static Pos operator *(Pos a, Pos b) =>
        new(a.Position * b.Position, a.Scale * b.Scale);

    /// <summary> Divides a Pos by a Pos. </summary>
    /// <param name="a">Pos to be Divided.</param>
    /// <param name="b">Pos to Divide by.</param>
    /// <returns>WHAT THE ACTUAL FUCK DO YOU WANT ME TO PUT HERE??? A MATH LESSOJN??!?!</returns>
    public static Pos operator /(Pos a, Pos b) =>
        new(a.Position / b.Position, a.Scale / b.Scale);

    /// <summary> Whether Pos a is larger then Pos b.</summary>
    /// <param name="a">Pos to check if larger.</param>
    /// <param name="b">Pos to check if smaller.</param>
    /// <returns>just- just.. JUST GO AWAY</returns>
    public static bool operator >(Pos a, Pos b) =>
        Compare(a, b);

    /// <summary> Whether Pos a is smaller then Pos b.</summary>
    /// <param name="a">Pos to check if smaller.</param>
    /// <param name="b">Pos to check if larger.</param>
    /// <returns>why lord.. why....</returns>
    public static bool operator <(Pos a, Pos b) =>
        Compare(b, a);

    /// <summary> Compare Pos size with another Pos. (DICK SIZE COMPARISON!!!)</summary>
    /// <param name="a">Pos to check if larger.</param>
    /// <param name="b">Pos to check if smaller.</param>
    /// <returns>Whether Pos a is bigger then Pos b.. what? thats how math works get the fuck away from me you animal..</returns>
    private static bool Compare(Pos a, Pos b)
    {
        int comparison = Comparison(a, b);

        return comparison == 1 || comparison == 2;
    }

    /// <summary> Compare Pos size with another Pos. (DICK SIZE COMPARISON!!!)</summary>
    /// <param name="a">Pos to check if larger.</param>
    /// <param name="b">Pos to check if smaller.</param>
    /// <returns>a less than b: 0 <para>a equals b: 1</para>a more then b: 2</returns>
    private static int Comparison(Pos a, Pos b)
    {
        int count = 0;
        if (a.Position.x > b.Position.x) count++;
        if (a.Position.y > b.Position.y) count++;
        if (a.Scale.x > b.Scale.x) count++;
        if (a.Scale.y > b.Scale.y) count++;
        if (a.Min.x > b.Min.x) count++;
        if (a.Min.y > b.Min.y) count++;
        if (a.Max.x > b.Max.x) count++;
        if (a.Max.y > b.Max.y) count++;

        return count <= 3 ? 0 : count >= 6 ? 2 : 1;
    }

    /// <summary> Whether the Pos is equal to the other. </summary>
    /// <param name="a">Pos to compare.</param>
    /// <param name="b">Pos to compare.</param>
    /// <returns>True if the Pos's are equal, false if not.</returns>
    public static bool operator ==(Pos a, Pos b)
    {
        int count = 0;
        if (a.Position == b.Position) count++;
        if (a.Scale == b.Scale) count++;
        if (a.Min == b.Min) count++;
        if (a.Max == b.Max) count++;
        return count == 4;
    }

    /// <summary> Whether the Pos isn't equal to the other. </summary>
    /// <param name="a">Pos to compare.</param>
    /// <param name="b">Pos to compare.</param>
    /// <returns>True if the Pos's aren't equal, false if they are.</returns>
    public static bool operator !=(Pos a, Pos b) =>
        !(a == b);

    public static new bool Equals(object objthis, object other) {
        // somewhat english version
        if (objthis is not Pos _this || other is not Pos otherPos) return false;
        return otherPos == _this;
        // not english version >:3
        /*return
            objthis is not 
            Pos?false:other 
            is not Pos?false:(Pos)objthis==(Pos)other;*/
    }

    /// <summary> Whether an object equals this Pos. </summary>
    /// <param name="other">object to compare.</param>
    /// <returns>Whether the object equals this Pos, returns false if (object is not Pos).<para>(thats like the actual code for this btw)</para></returns>
    public override bool Equals(object other) {
        if (other is not Pos otherPos) return false;
        return otherPos == this;
    }

    /// <summary> Gets the hash code or whatever.. (idfk what this is i just know i need to override it for == and != operators)</summary>
    /// <param name="p">Position to get the hash code of..</param>
    /// <returns>The hash code of p..</returns>
    public static int GetHashCode(Pos p) =>
        HashCode.Combine(p.Position, p.Scale, p.Min, p.Max);

    /// <summary> Gets the hash code or whatever.. (idfk what this is i just know i need to override it for == and != operators)</summary>
    /// <returns>The hash code..</returns>
    public override int GetHashCode() =>
        HashCode.Combine(Position, Scale, Min, Max);

    #endregion

    /// <summary> Change one of the current values. </summary>
    /// <param name="position">Position to override with.</param>
    /// <param name="scale">Scale to override with.</param>
    /// <param name="min">Min to override with.</param>
    /// <param name="max">Max to override with.</param>
    /// <returns>The Pos with the overrided values.</returns>
    public Pos Change(Vector2? position = null, Vector2? scale = null, Vector2? min = null, Vector2? max = null)
    {
        Position = position ?? Position;
        Scale = scale ?? Scale;
        Min = min ?? Min;
        Max = max ?? Max;

        return this;
    }

    /// <summary> Default used in UI. </summary>
    /// <returns>The default Pos.</returns>
    public static Pos Default() => new(0f, 0f, 100f, 100f);

    /// <summary> LessDefault, it's the default but u can customize Position. </summary>
    /// <param name="pos">Position.</param>
    /// <returns>The less-default Pos.</returns>
    public static Pos LD(Vector2 pos) => new(pos.x, pos.y, 100f, 100f);

    /// <summary> LessDefault, it's the default but u can customize Position. </summary>
    /// <param name="x">X Position.</param>
    /// <param name="y">Y Position.</param>
    /// <returns>The less-default Pos.</returns>
    public static Pos LD(float x, float y) => new(x, y, 100f, 100f);

    /// <summary> Zero. (0F, 0F, 0F, 0F)</summary>
    /// <returns>Pos(0F, 0F, 0F, 0F)</returns>
    public static Pos Zero() => new(0f, 0f, 0f, 0f);

    /// <summary> Converts a RectTransform component to a Pos. </summary>
    /// <param name="rectTransform">RectTransform component to convert.</param>
    /// <param name="OVERRIDE_ANCHOREDPOSITION">OVERRIDES THE FUCKING ANCHORED POSITION!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <param name="OVERRIDE_SIZEDELTA">OVERRIDES THE FUCKING SIZE DELTA!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <param name="OVERRIDE_ANCHORMIN">OVERRIDES THE FUCKING ANCHORED MIN!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <param name="OVERRIDE_ANCHORMAX">OVERRIDES THE FUCKING ANCHORED MAX!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <returns>The RectTransform after being converted.</returns>
    public static Pos RectTransform(RectTransform rectTransform, Vector2? OVERRIDE_ANCHOREDPOSITION = null, Vector2? OVERRIDE_SIZEDELTA = null, Vector2? OVERRIDE_ANCHORMIN = null, Vector2? OVERRIDE_ANCHORMAX = null) => 
        new(OVERRIDE_ANCHOREDPOSITION ?? rectTransform.anchoredPosition, OVERRIDE_SIZEDELTA ?? rectTransform.sizeDelta, OVERRIDE_ANCHORMIN ?? rectTransform.anchorMin, OVERRIDE_ANCHORMAX ?? rectTransform.anchorMax);

    /// <summary> Converts a Rect component to a Pos. </summary>
    /// <param name="rect">Rect component to convert.</param>
    /// <param name="OVERRIDE_POSITION">OVERRIDES THE FUCKING POSITION!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <param name="OVERRIDE_SIZE">OVERRIDES THE FUCKING SIZE!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <param name="OVERRIDE_MIN">OVERRIDES THE FUCKING MIN!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <param name="OVERRIDE_MAX">OVERRIDES THE FUCKING MAX!!! KILL THE FILTHS! KILL THE FILTHS! KILL THE FILTHS!</param>
    /// <returns>The Rect after being converted.</returns>
    public static Pos Rect(Rect rect, Vector2? OVERRIDE_POSITION = null, Vector2? OVERRIDE_SIZE = null, Vector2? OVERRIDE_MIN = null, Vector2? OVERRIDE_MAX = null) =>
        new(OVERRIDE_POSITION ?? rect.position, OVERRIDE_SIZE ?? rect.size, OVERRIDE_MIN ?? rect.min, OVERRIDE_MAX ?? rect.max);
}

/*/// <summary> Bool but like better for me. (pronounced "n-oo-lea")</summary>
public struct Nulea
{
    public bool b;
    public string s
    {
        get => b.ToString();

        set
        {

        }
    };

    public Nulea(bool b) => this.b = b;
    public Nulea(string s) : this(Boolean.TryParse(s, out var b)) { }

    // i want this to return nul after its been inverted like bool = !bool; but ~Nulea 
    public static Nulea operator ~(Nulea nul) => nul.Not();

    public static implicit operator bool(Nulea nul) => nul.b;
    public static implicit operator Nulea(bool b) => new(b);
    public static implicit operator Nulea(string s) => new(s);

    public Nulea Not() => 
        this = !this;
}*/