namespace SSNC;

using plog.Models;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary> Struct used by Stylizer to pick colors. </summary>
/// <remarks> Makes a Nulea with 4 Bytes. </remarks>
/// <param name="r">Red value.</param>
/// <param name="g">Green value.</param>
/// <param name="b">Blue value.</param>
/// <param name="a">Alpha value.</param>
public struct Nulea(byte r, byte g, byte b, byte a)
{
    /// <summary> Red value. </summary>
    public byte r = r;
    /// <summary> Green value. </summary>
    public byte g = g;
    /// <summary> Blue value. </summary>
    public byte b = b;
    /// <summary> Alpha value. </summary>
    public byte a = a;

    /// <summary> Red Color. </summary>
    public static readonly Nulea red = "#FF0000";
    /// <summary> Green Color. </summary>
    public static readonly Nulea green = "#00FF00";
    /// <summary> Bright Teal Color. </summary>
    public static readonly Nulea brightTeal = "#01F9C6";
    /// <summary> Blue Color. </summary>
    public static readonly Nulea blue = "#0000FF";
    /// <summary> White Color. </summary>
    public static readonly Nulea white = "#FFFFFF";
    /// <summary> Black Color. </summary>
    public static readonly Nulea black = "#000000";
    /// <summary> Yellow Color. </summary>
    public static readonly Nulea yellow = "#FFFF00";
    /// <summary> Cyan Color. </summary>
    public static readonly Nulea cyan = "#00FFFF";
    /// <summary> Magent Color. </summary>
    public static readonly Nulea magent = "#FF00FF";
    /// <summary> Gray Color. </summary>
    public static readonly Nulea gray = "#7F7F7F";
    /// <summary> Grey Color. </summary>
    public static readonly Nulea grey = gray;
    /// <summary> Clear Color. </summary>
    public static readonly Nulea clear = "#00"; // this is only 2 characters for AA cuz its clear, we dont need no color
    /// <summary> Transparent Color. </summary>
    public static readonly Nulea transparent = clear;

    /// <summary> Makes a Nulea with 3 Integers. </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    public Nulea(int r, int g, int b) : this(r, g, b, 255) { }

    /// <summary> Makes a Nulea with 4 Integers. </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="a">Alpha value.</param>
    public Nulea(int r, int g, int b, int a) : this((byte)r, (byte)g, (byte)b, (byte)a) { }

    /// <summary> Makes a Nulea with 3 Bytes. </summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    public Nulea(byte r, byte g, byte b) : this(r, g, b, (byte)255) { }

    /// <summary> Makes a Nulea with either one of 4 Bytes. ((r, g, &amp; b: default to 0) (a: default to 255))</summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="a">Alpha value.</param>
    public Nulea(byte? r = null, byte? g = null, byte? b = null, byte? a = null) : this(r ?? 0, g ?? 0, b ?? 0, a ?? 255) { }

    /// <summary> Makes a Nulea with either one of 4 Integers. ((r, g, &amp; b: default to 0) (a: default to 255))</summary>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="a">Alpha value.</param>
    public Nulea(int? r = null, int? g = null, int? b = null, int? a = null) : this(r ?? 0, g ?? 0, b ?? 0, a ?? 255) { }

    /// <summary> Adds each color channel in the Nulea by each color channel in another Nulea. </summary>
    /// <param name="a">Nulea to be Added.</param>
    /// <param name="b">Nulea to Add by.</param>
    /// <returns>The Added Nulea.</returns>
    public static Nulea operator +(Nulea a, Nulea b) =>
        new(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);

    /// <summary> Adds each color channel in the Nulea by a byte. </summary>
    /// <param name="a">Nulea to be Added.</param>
    /// <param name="b">Byte to Add by.</param>
    /// <returns>The Added Nulea.</returns>
    public static Nulea operator +(Nulea a, byte b) =>
        new(a.r + b, a.g + b, a.b + b, a.a + b);

    /// <summary> Substracts each color channel in the Nulea by each color channel in another Nulea. </summary>
    /// <param name="a">Nulea to be Substracted.</param>
    /// <param name="b">Nulea to Substract by.</param>
    /// <returns>The Substracted Nulea.</returns>
    public static Nulea operator -(Nulea a, Nulea b) =>
        new(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);

    /// <summary> Substracts each color channel in the Nulea by a byte. </summary>
    /// <param name="a">Nulea to be Substracted.</param>
    /// <param name="b">Byte to Substract by.</param>
    /// <returns>The Substracted Nulea.</returns>
    public static Nulea operator -(Nulea a, byte b) =>
        new(a.r - b, a.g - b, a.b - b, a.a - b);

    /// <summary> Multiples each color channel in the Nulea by each color channel in another Nulea. </summary>
    /// <param name="a">Nulea to be Multiplied.</param>
    /// <param name="b">Nulea to Multiply by.</param>
    /// <returns>The Multiplied Nulea.</returns>
    public static Nulea operator *(Nulea a, Nulea b) =>
        new(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);

    /// <summary> Multiples each color channel in the Nulea by a byte. </summary>
    /// <param name="a">Nulea to be Multiplied.</param>
    /// <param name="b">Byte to Multiply by.</param>
    /// <returns>The Multiplied Nulea.</returns>
    public static Nulea operator *(Nulea a, byte b) =>
        new(a.r * b, a.g * b, a.b * b, a.a * b);

    /// <summary> Divides each color channel in the Nulea by each color channel in another Nulea. </summary>
    /// <param name="a">Nulea to be Divided.</param>
    /// <param name="b">Nulea to Divide by.</param>
    /// <returns>The Divided Nulea.</returns>
    public static Nulea operator /(Nulea a, Nulea b) =>
        new(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);

    /// <summary> Divides each color channel in the Nulea by a byte. </summary>
    /// <param name="a">Nulea to be Divided.</param>
    /// <param name="b">Byte to Divide by.</param>
    /// <returns>The Divided Nulea.</returns>
    public static Nulea operator /(Nulea a, byte b) =>
        new(a.r / b, a.g / b, a.b / b, a.a / b);

    /// <summary> Divides each color channel in the Nulea by a byte. </summary>
    /// <param name="a">Nulea to be Divided.</param>
    /// <param name="b">Byte to Divide by.</param>
    /// <returns>The Divided Nulea.</returns>
    public static float[] operator /(Nulea a, float b) =>
        [a.r / b, a.g / b, a.b / b, a.a / b];

    /// <summary> Checks if a Nulea is equal to another. </summary>
    /// <param name="left">Left Nulea to check.</param>
    /// <param name="right">Right Nulea to check.</param>
    /// <returns>Whether the Nulea's are equal or not.</returns>
    public static bool operator ==(Nulea left, Nulea right) =>
        left.Equals(right);

    /// <summary> Checks if a Nulea isn't equal to another. </summary>
    /// <param name="left">Left Nulea to check.</param>
    /// <param name="right">Right Nulea to check.</param>
    /// <returns>Whether the Nulea's aren't equal.</returns>
    public static bool operator !=(Nulea left, Nulea right) =>
        !left.Equals(right);

    /// <summary> Checks if the Left Nulea is greater than the Right Nulea. </summary>
    /// <param name="left">Left Nulea to check.</param>
    /// <param name="right">Right Nulea to check.</param>
    /// <returns>Whether the Left Nulea is greater than the Right Nulea.</returns>
    public static bool operator >(Nulea left, Nulea right) =>
        Compare(left, right) >= 3;

    /// <summary> Checks if the Left Nulea is less than the Right Nulea. </summary>
    /// <param name="left">Left Nulea to check.</param>
    /// <param name="right">Right Nulea to check.</param>
    /// <returns>Whether the Left Nulea is less than the Right Nulea.</returns>
    public static bool operator <(Nulea left, Nulea right) =>
        Compare(left, right) <= 2;

    /// <summary> Compares how many values in the Left Nulea are greater than ones in the Right Nulea.</summary>
    /// <param name="left">Left Nulea to compare.</param>
    /// <param name="right">Right Nulea to compare.</param>
    /// <returns>The amount of values in the Left Nulea that are greater than ones in the Right Nulea.</returns>
    public static int Compare(Nulea left, Nulea right)
    {
        int count = 0;
        if (left.r > right.r) count++;
        if (left.g > right.g) count++;
        if (left.b > right.b) count++;
        if (left.a > right.a) count++;
        return count;
    }

    /// <summary> Darkens a color byte based on the alpha. </summary>
    /// <param name="colorByte">The color byte to be darken'd.</param>
    /// <param name="Alpha">Alpha to darken by.</param>
    /// <returns>The colorByte once it has been darken'd by the alpha.</returns>
    public static byte DarkenBasedOnAlpha(byte? colorByte, byte? Alpha) =>
        (byte)((colorByte ?? 255) * ((Alpha ?? 255) / 255f));

    /// <summary> Darkens a color byte based on this Nulea's alpha. </summary>
    /// <param name="colorByte">The color byte to be darken'd.</param>
    /// <returns>The colorByte once it has been darken'd by the alpha.</returns>
    public byte DarkenBasedOnAlpha(byte? colorByte) =>
        DarkenBasedOnAlpha(colorByte ?? 255, a);

    /// <summary> Converts an RGBA Nulea into RGB. (darkens based on alpha. it also still has alpha channel, just it doesnt do anything.)</summary>
    /// <param name="h">Nulea to convert.</param>
    /// <returns>The new converted Nulea.</returns>
    public static Nulea ToRGB(Nulea h) =>
        new(h.DarkenBasedOnAlpha(h.r), h.DarkenBasedOnAlpha(h.g), h.DarkenBasedOnAlpha(h.b));

    /// <summary> Converts this RGBA Nulea into RGB. (darkens based on alpha. it also still has alpha channel, just it doesnt do anything.)</summary>
    /// <returns>The new converted Nulea.</returns>
    public readonly Nulea ToRGB() => ToRGB(this);

    /// <summary> Converts an RGBA Nulea into an array of floats representing CMYK. (darkens based on alpha.)</summary>
    /// <param name="h">Nulea to convert.</param>
    /// <returns>An array of floats containing CMYK values.</returns>
    public static float[] ToCMYK(Nulea h)
    {
        h = ToRGB(h);
        float Red = h.r / 255f, Green = h.g / 255f, Blue = h.b / 255f,
            Black/*(K)*/ = Mathf.Min(1 - Red, 1 - Green, 1 - Blue),
            Cyan/*(C)*/ = (1 - Red - Black) / (1 - Black),
            Magenta/*(M)*/ = (1 - Green - Black) / (1 - Black),
            Yellow/*(Y)*/ = (1 - Blue - Black) / (1 - Black);
        return [Cyan/*(C)*/, Magenta/*(M)*/, Yellow/*(Y)*/, Black/*(K)*/];
    }

    /// <summary> Converts this RGBA Nulea into an array of floats representing CMYK. (darkens based on alpha.)</summary>
    /// <returns>An array of floats containing CMYK values.</returns>
    public readonly float[] ToCMYK() =>
        ToCMYK(this);

    /// <summary> Converts an RGBA Nulea into an array of floats representing HSV. (darkens based on alpha.)</summary>
    /// <param name="h">Nulea to convert.</param>
    /// <returns>An array of floats containing HSV values.</returns>
    public static float[] ToHSV(Nulea h)
    {
        Color.RGBToHSV(ToRGB(h), out var H, out var S, out var V); // im too fucking lazy to figure this shit out or ask gpt to do it for me
        return [H, S, V];
    }

    /// <summary> Converts this RGBA Nulea into an array of floats representing HSV. (darkens based on alpha.)</summary>
    /// <returns>An array of floats containing HSV values.</returns>
    public readonly float[] ToHSV() =>
        ToHSV(this);

    /// <summary> Checks if a object is equal to this Nulea. </summary>
    /// <param name="A">Object to check.</param>
    /// <returns>Whether the object is a Nulea and if its equal to this Nulea.</returns>
    public override readonly bool Equals(object A)
    {
        if (A is not Nulea h) return false;
        return h.r == r && h.g == g && h.b == b && h.a == a;
    }

    /// <summary> Gets the hash code or whatever.. (idfk what this is i just know i need to override it for == and != operators)</summary>
    /// <returns>The hash code..</returns>
    public override readonly int GetHashCode() =>
        HashCode.Combine(r, g, b, a);

    /// <summary> Converts the Nulea into a string. </summary>
    /// <param name="format">Format to be converted into.</param>
    /// <returns>The string defition of this Nulea</returns>
    public readonly string ToString(string format = "X2")
    {
        Plugin.Log.Debug("active tostring(format) " + format);
        if (format[0] != 'x' || format != "rgb" || format != "rgba") format = format.ToUpper();
        string point = format.Length == 1 ? "" : format.Substring(1);
        string sub(byte val, string format) => val.ToString($"{format[0]}{point}");
        string product = format switch
        {
            "HEX" => $"{r:X2}{g:X2}{b:X2}{a:X2}",
            "A" => $"{a:D3}", // 255
            "rgb" => $"{r:D} {g:D} {b:D}", // 1 249 198
            "RGB" => $"{r:D3}{g:D3}{b:D3}", // 001249198
            "rgba" => $"{r:D} {g:D} {b:D} {a:D}", // 1 249 198 255
            "RGBA" => $"{r:D3}{g:D3}{b:D3}{a:D3}", // 001249198255
            "#AA" => $"{a:X2}", // FF
            "#RGB" => $"{r.ToString("X2")[0]}{g.ToString("X2")[0]}{b.ToString("X2")[0]}",// 0FC
            "#RGBA" => $"{r.ToString("X2")[0]}{g.ToString("X2")[0]}{b.ToString("X2")[0]}{a.ToString("X2")[0]}", // 0FCF
            "#RRGGBB" => $"{r:X2}{g:X2}{b:X2}", // 01F9C6
            "#RRGGBBAA" => $"{r:X2}{g:X2}{b:X2}{a:X2}", // 01F9C6FF
            "G" => $"{sub(r, "G")}{sub(g, "G")}{sub(b, "G")}{sub(a, "G")}",
            "D" => $"{sub(r, "D")} {sub(g, "D")} {sub(b, "D")} {sub(a, "D")}",
            "X" => $"{sub(r, "X")}{sub(g, "X")}{sub(b, "X")}{sub(a, "X")}",
            "x" => $"{sub(r, "x")}{sub(g, "x")}{sub(b, "x")}{sub(a, "x")}",
            "N" => $"{sub(r, "N")}{sub(g, "N")}{sub(b, "N")}{sub(a, "N")}",
            "F" => $"{sub(r, "F")}{sub(g, "F")}{sub(b, "F")}{sub(a, "F")}",
            "E" => $"{sub(r, "E")}{sub(g, "E")}{sub(b, "E")}{sub(a, "E")}",
            "P" => $"{sub(r, "P")}{sub(g, "P")}{sub(b, "P")}{sub(a, "P")}",
            "C" => $"{sub(r, "C")}{sub(g, "C")}{sub(b, "C")}{sub(a, "C")}",
            _ => $"{r:X2}{g:X2}{b:X2}{a:X2}"
        };
        return product;
    }

    public override string ToString() { Plugin.Log.Debug("active tostring()"); return ToString("HEX"); }

    /// <summary> Implicit operator so Nulea's can be used like strings. </summary>
    /// <param name="h">The Nulea to convert into a string.</param>
    /// <returns>The Nulea as a string.</returns>
    public static implicit operator string(Nulea h) => h.ToString("HEX");

    /// <summary> Implicit operator so strings can be used like Nulea's. </summary>
    /// <param name="s">The string to convert into a Nulea.</param>
    /// <returns>The string as a Nulea. (returns FFF if your string isnt a supported type)</returns>
    public static implicit operator Nulea(string s)
    {
        if (s.StartsWith('#')) s = s[1..];
        static byte con(string str) => Convert.ToByte(int.Parse(str.Length == 1 ? $"{str}0" : str, System.Globalization.NumberStyles.HexNumber));
        // aa
        if (s.Length == 2) return new(a: con(s[..2]));
        // rgb
        if (s.Length == 3) return new(con(s[0].ToString()), con(s[1].ToString()), con(s[2].ToString()));
        // rgba
        if (s.Length == 4) return new(con(s[0].ToString()), con(s[1].ToString()), con(s[2].ToString()), con(s[3].ToString()));
        // rrggbb
        if (s.Length == 6) return new(con(s[..2]), con(s.Substring(2, 2)), con(s.Substring(4, 2)));
        // rrggbbaa
        if (s.Length == 8) return new(con(s[..2]), con(s.Substring(2, 2)), con(s.Substring(4, 2)), con(s.Substring(6, 2)));
        // fallback
        return new();
    }

    /// <summary> Implicit operator so Nulea's can be used like intergers. </summary>
    /// <param name="h">The Nulea to convert into an interger.</param>
    /// <returns>The Nulea as an interger.</returns>
    public static implicit operator uint(Nulea h) =>
        (uint)h.r << 24 | (uint)h.g << 16 | (uint)h.b << 8 | h.a;

    /// <summary> Implicit operator so intergers can be used like Nulea's. </summary>
    /// <param name="h">The interger to convert into an Nulea.</param>
    /// <returns>The interger as an Nulea.</returns>
    public static implicit operator Nulea(uint i) => new(
        r: (byte)(i >> 24),
        g: (byte)(i >> 16),
        b: (byte)(i >> 8),
        a: (byte)i
    );

    /// <summary> Implicit operator so Nulea's can be used like Color's. </summary>
    /// <param name="h">The Nulea to convert into a Color.</param>
    public static implicit operator Color(Nulea h) =>
        new(h.r / 255f, h.g / 255f, h.b / 255f, h.a / 255f);

    /// <summary> Implicit operator so Color's can be used like Nulea's. </summary>
    /// <param name="h">The Color to convert into a Nulea.</param>
    public static implicit operator Nulea(Color c)
    {
        int toInt(float val) => (int)Math.Round(Mathf.Clamp(val * 255f, 0, 255));
        Nulea HexCol = new(toInt(c.r), toInt(c.g), toInt(c.b), toInt(c.a));
        return HexCol;
    }

    /// <summary> Implicit operator so Nulea's can be used like UniversalColor's. (Darkens colors based on alpha due to UniversalColor's not supporting alpha channels) </summary>
    /// <param name="h">The Nulea to convert into a UniversalColor.</param>
    public static implicit operator UniversalColor(Nulea h) =>
        new(h.DarkenBasedOnAlpha(h.r), h.DarkenBasedOnAlpha(h.g), h.DarkenBasedOnAlpha(h.b));

    /// <summary> Implicit operator so UniversalColor's can be used like Nulea's. </summary>
    /// <param name="c">The UniversalColor to convert into a Nulea.</param>
    public static implicit operator Nulea(UniversalColor c) =>
        new(c.Red, c.Green, c.Blue);
}