namespace SSNC;

using HarmonyLib;
using plog.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;


/// <summary> Logs stuff idfk man. </summary>
public class Logger
{
    /// <summary> The logger to record with. </summary>
    public plog.Logger log;

    /// <summary> Extra tags to add with each recorded log. </summary>
    public Tag[] Tags;

    /// <summary> The amount of logs to store before writing them to the logs file. </summary>
    public int WriteToFile = 30;

    /// <summary> What to write to the log file in the next update. </summary>
    public List<string> toWrite = [];

    /// <summary> File path to write logs to. </summary>
    public string logfilepath = null;

    /// <summary> Creates a new Log with a name and extra tags. </summary>
    /// <param name="Log">Logger for the Log.</param>
    /// <param name="tags">Any extra tags to add to each log.</param>
    /// <param name="WriteToFile">How many logs to store on ur memory before writing them to a file.</param>
    public Logger(plog.Logger Log, Tag[] tags, int WriteToFile = 30, string logfilepath = null) =>
        Load(Log, tags, WriteToFile, logfilepath);

    /// <summary> Creates a new Log with a name and extra tags. </summary>
    /// <param name="name">Name of the Log.</param>
    /// <param name="Tags">Any extra tags to add to each log.</param>
    /// <param name="WriteToFile">How many logs to store on ur memory before writing them to a file.</param>
    public Logger(string name, Tag[] Tags, int WriteToFile = 30) =>
        Load(new(name), Tags, WriteToFile);

    /// <summary> Creates a new Log with a name and extra tags. </summary>
    /// <param name="name">Name of the Log.</param>
    public Logger(string name, int WriteToFile = 30) : this(name, null, WriteToFile) { }

    /// <summary> Loads the Logger. </summary>
    /// <param name="Log">plog.Logger to use as a base.</param>
    /// <param name="tags">Any extra tags to add to each log.</param>
    /// <param name="WriteToFile">How many logs to store on ur memory before writing them to a file.</param>
    protected void Load(plog.Logger Log, Tag[] tags, int WriteToFile = 30, string logfilepath = null)
    {
        log = Log;
        Info($"Logger created...");
        Tags = tags;
        Info($"Tags set...", eT: Tags);

        this.WriteToFile = WriteToFile;
        this.logfilepath = logfilepath ?? Path.Combine(Path.GetDirectoryName(Plugin.Instance.Location), "logs", $"Log {DateTime.Now:MM-dd-yyyy hh-mm-ss tt}.txt");
    }

    /// <summary> "Flushes" all currently stored logs to a file. (name def not stolen from jaket)</summary>
    public void Flush()
    {
        string logs = Path.GetDirectoryName(logfilepath);
        if (!Directory.Exists(logs)) Directory.CreateDirectory(logs);
        logfilepath ??= Path.Combine(logs, $"Log {DateTime.Now:MM-dd-yyyy hh-mm-ss tt}.txt");

        File.AppendAllLines(logfilepath, toWrite);
        toWrite.Clear();
    }

    /// <summary> Records a log, writes it to a file, adds up your tags, and just makes it nicer. </summary>
    /// <param name="level">Level to be logged. (certain levels change the appearence of the log message)</param>
    /// <param name="msg">Message to be written.</param>
    /// <param name="StackTrace">The text to be shown when the log is clicked on. (called stackTrace since thats what its usually used for)</param>
    /// <param name="IncludeStackTrace">Whether to include the StackTrace after your StackTrace string.</param>
    /// <param name="extraTags">Extra tags to add to the log.</param>
    public void Record(Level level, string msg, string StackTrace = null, bool IncludeStackTrace = true, Tag[] extraTags = null)
    {
        if (IncludeStackTrace)
            StackTrace += $"\nOuter:\n{StackTraceUtility.ExtractStackTrace()}";

        Tag[] tags = Tags ?? [];
        if (extraTags != null) tags.AddRangeToArray(extraTags);

        BasicRecord(msg, level, tags, StackTrace);
    }

    //btw eT means extraTags
    //not the alien

    // TODO: add summaries

    /// <summary> Records a log level with the Off level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Off(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Off, $"<color=#A9A9A9>{message}</color>", context, IST, eT);

    /// <summary> Records a log level with the Debug level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Debug(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Debug, $"<color=#BBB>{message}</color>", context, IST, eT);

    /// <summary> Records a log level with the Info level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Info(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Info, $"<size=20>[i]</size> <color=#EEF>{message}</color>", context, IST, eT);

    /// <summary> Records a log level with the Fine level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Fine(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Fine, $"<size=20>[i]</size> <color=#EEF>{message}</color>", context, IST, eT);

    /// <summary> Records a log level with the Warning level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Warning(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Warning, $"<color=#F00><size=24>[!]</color> {message}", context, IST, eT);

    /// <summary> Records a log level with the Error level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Error(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Error, $"<color=#F00><size=24>[!!!]</color> {message}", context, IST, eT);

    /// <summary> Records a log level with the Exception level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Exception(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Exception, message, context, IST, eT);

    /// <summary> Records a log level with the Exception level. (certain levels change the appearence of the log message)</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Message to be written.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Exception(Exception exception, string message = null, Tag[] eT = null)
    {
        string msg = message ?? $"{exception}";
        string con = message == null ? "" : $"{exception}";
        Record(Level.Exception, msg, con, true, eT);
    }

    /// <summary> Records a log level with the CommandLine level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void CommandLine(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.CommandLine, message, context, IST, eT);

    /// <summary> Records a log level with the Config level. (certain levels change the appearence of the log message)</summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="context">The text to be shown when the log is clicked on.</param>
    /// <param name="IST">Whether to show stacktrace after the context.</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Config(string message, string context = null, bool IST = false, Tag[] eT = null) => Record(Level.Config, message, context, IST, eT);

    // non-base ultrakill log voids

    /// <summary> Literally just sets the msg to be written to a log file and does plog.Logger.Record();, it's really basic. </summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="level">Level to be logged. (certain levels change the appearence of the log message)</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    /// <param name="stackTrace">The text to be shown when the log is clicked on. (called stackTrace since thats what its usually used for)</param>
    /// <param name="context">i have no idea</param>
    public void BasicRecord(string message, Level level, IEnumerable<Tag> eT = null, string stackTrace = null, object context = null) {
        string nonUKMESSAGE = $"[{log.Tag.Name}] {(eT != null ? string.Join(' ', from tag in eT select $"[{tag.Name}]") + " " : string.Empty)}| [{DateTime.Now:MM/dd/yyyy hh:mm:ss tt}] | [{level}]: {Regex.Replace(message, "<.*?>", string.Empty)}{(stackTrace != null ? $" stackTrace: {stackTrace}" : string.Empty)}";
        toWrite.Add(nonUKMESSAGE);
        if (toWrite.Count >= WriteToFile) Flush();
        Plugin.BepLog.Log(Translate_Level_To_Bep_Level[level] | BepInEx.Logging.LogLevel.Message, nonUKMESSAGE);
        log.Record(message, Translate_Level_To_Plog_Level[level], eT, stackTrace, context);
    }

    /// <summary> Records a log level with the Extreme level. (certain levels change the appearence of the log message)</summary>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">Message to be written.</param>
    public void Extreme(Exception exception, string message) => // just use this for whenever something REALLY FUCKING BAD happens, like smt that can just casually FUCK OVER THE WHOLE MOD
        Extra($"<size=20><color=#f00>[<size=16>{message}</size>]</color></size>", (exception ?? new ArgumentNullException("exception")).ToString(), Level.Extreme, [new("EXTREME")]);

    /// <summary> Records a log but lets you add an extra message when its clicked on. </summary>
    /// <param name="message">Message to be written.</param>
    /// <param name="extra">The text to be shown when the log is clicked on. (called stackTrace since thats what its usually used for)</param>
    /// <param name="level">Level to be logged. (certain levels change the appearence of the log message)</param>
    /// <param name="eT">Extra tags to add to the log.</param>
    public void Extra(string message, string extra, Level level = Level.Info, Tag[] eT = null) =>
        Record(level, message, extra, true, eT);

    /// <summary> Skins a script segment by segment and logs before each one. (you have to do () => before each segment tho)</summary>
    /// <param name="start">Message to be put at the start. (example: "start: Completed Segment 1")</param>
    /// <param name="OVERRIDE_LOG">Overrides how the log is formatted. (default is "[0]: Completed Segment [1]", [0] is where start is gonna be put, and [1] is the segment number)</param>
    /// <param name="level">Level to be logged. (certain levels change the appearence of the log message)</param>
    /// <param name="Segments">Each segment to log before.</param>
    public void Skin(string start, string OVERRIDE_LOG = null, Level level = Level.Info, params Action[] Segments)
    {
        string say = OVERRIDE_LOG ?? $"[0]: Completed Segment [1]";

        int i = 0;
        foreach (Action Segment in Segments) {
            i++; Record(level, Say(say, start, i.ToString()));
            Segment();
        }
    }

    /// <summary> Converts a string like "[0]: Completed Segment [1]" into "Log: Completed Segment 5". (what [#] is replaced with is decided by the string[] params aka phrases)</summary>
    /// <param name="say">String to convert.</param>
    /// <param name="phrases">What phrases to replace [#]'s with.</param>
    /// <returns>Returns the converted string.</returns>
    public string Say(string say, params string[] phrases)
    {
        // incase say is like "wasupp [ :3" so it doesnt try to turn " :3" into a phrase
        if (!say.Contains('[') || !say.Contains(']')) {
            Error($"While Running SSNC.Logger.Say(string say, params string[] phrases), it failed due to no '[', or no ']' chars in string say...\nsay: {say}\nphrases:\n{string.Join("  |-|-|  ", phrases)}");
            return say;
        }

        StringBuilder builder = new();
        string[] splitted = say.Split('[', ']');
        int i = 0;
        foreach (string part in splitted)
        {
            i++;
            builder.Append(i == 1 ? part : int.TryParse(part, out int phraseIndex) ? phraseIndex > 0 ? phrases[phraseIndex] : phrases[0] : phrases[0]);
            if (i == 2) i = 0;
        }

        return builder.ToString();
    }

    /// <summary> Gets the name of a method with ✨colors✨. (example: <color=#BBB><color=#0FC><color=#FFF>COAT</color>.<color=#FFF>Net</color>.<color=#4CB>Networking</color>.<color=#DDA>Send</color><color=#FFF>(COAT.Content</color>.<color=#FFF><color=#9DC>PacketType</color> <color=#9DF>packetType</color>, System</color>.<color=#FFF><color=#5CB>Action</color><COAT.IO</color>.<color=#FFF><color=#5CB>Writer</color> <color=#9DF>Writer</color>> <color=#9DF>cons</color>, System</color>.<color=#FFF><color=#5CB>Action</color><System</color>.<color=#FFF><color=#8BF>IntPtr</color> <color=#9DF>IntPtr</color>, System</color>.<color=#FFF><color=#8BF>Int32</color> <color=#9DF>Int32</color>> <color=#9DF>result</color>, System</color>.<color=#FFF><color=#8BF>Int32</color> <color=#9DF>size</color>);</color></color></color>)</summary>
    /// <param name="Class">The type that the method is within.</param>
    /// <param name="Method">The name of the method to get a full name of with ✨colors✨.</param>
    /// <param name="Style">The style of how Primitives, Classes, Interfaces, Enum's, Structs, and Delegates look.</param>
    /// <returns>The methods name with ✨colors✨. (such as for this it'd be SSNC.Logger.GetNameOfMethod() but with ✨colors✨)</returns>
    public string GetNameOfMethod(Type type, string Method, Stylizer? Style = null) =>
        GetNameOfMethod(type.GetMethod(Method, (BindingFlags)60), Style);

    /// <summary> Gets the name of a method with ✨colors✨. </summary>
    /// <param name="method">The method to get a name of with ✨colors✨.</param>
    /// <param name="Style">The style of how Primitives, Classes, Interfaces, Enum's, Structs, and Delegates look.</param>
    /// <returns>The methods name with ✨colors✨. (such as for this it'd be SSNC.Logger.GetNameOfMethod() but with ✨colors✨)</returns>
    public string GetNameOfMethod(MethodBase method, Stylizer? Style = null)
    {
        if (method == null)
        {
            Exception(new ArgumentNullException(nameof(method)), "ARGUMENTNULLEXCEPTION WHILE TRYING TO GETNAMEOFMETHOD");
            return "ARGUMENTNULLEXCEPTION WHILE TRYING TO GETNAMEOFMETHOD (stack trace: " + StackTraceUtility.ExtractStackTrace() + " )";
        }

        Stylizer style = Style ?? new();
        Type Class = method?.DeclaringType ?? typeof(Logger);
        string parameterHasArguments(ParameterInfo param) =>
            param.ParameterType.GetGenericArguments().Length == 0 ? string.Empty :
            $"<{string.Join(", ",
            from argument in param.ParameterType.GetGenericArguments() select $"{argument?.Namespace ?? string.Empty}</color>.<color=#FFF>{style.GetColorOfType(argument)}{(argument.Name.IndexOf('`') != -1 ? argument.Name[..argument.Name.IndexOf('`')] : argument.Name)}</color> <color=#{style.Parameter}>{argument.Name}</color>"
            )}>";
        string parameters = string.Join<string>(", ", from param in method.GetParameters() select $"{param.ParameterType?.Namespace ?? string.Empty}</color>.<color=#FFF>{style.GetColorOfType(param.ParameterType)}{(param.ParameterType.Name.IndexOf('`') != -1 ? param.ParameterType.Name[..param.ParameterType.Name.IndexOf('`')] : param.ParameterType.Name)}</color>{parameterHasArguments(param)} <color=#{style.Parameter}>{param.Name}</color>");
        return $"<color=#0FC><color=#FFF>{(Class?.Namespace ?? "namespace").Replace(".", "</color>.<color=#FFF>")}</color>.<color=#{style.Class}>{Class?.Name ?? "class"}</color>.<color=#{style.Method}>{method?.Name ?? "method"}</color><color=#FFF>({parameters});</color></color>";
    }

    /// <summary> Log level. (certain levels change the appearence of the log message)</summary>
    public enum Level
    {
        Off,
        Debug,
        Info,
        Fine,
        Warning,
        Error,
        Exception,
        Extreme,
        CommandLine,
        Config // result: <color=#0FC><color=#FFF>COAT</color>.<color=#FFF>Net</color>.<color=#00000000>Networking</color>.<color=#00000000>Send</color><color=#FFF>(COAT.Content</color>.<color=#FFF><color=#00000000>PacketType</color> <color=#00000000>packetType</color>, System</color>.<color=#FFF><color=#00000000>Action</color><COAT.IO</color>.<color=#FFF><color=#00000000>Writer</color> <color=#00000000>Writer</color>> <color=#00000000>cons</color>, System</color>.<color=#FFF><color=#00000000>Action</color><System</color>.<color=#FFF><color=#00000000>IntPtr</color> <color=#00000000>IntPtr</color>, System</color>.<color=#FFF><color=#00000000>Int32</color> <color=#00000000>Int32</color>> <color=#00000000>result</color>, System</color>.<color=#FFF><color=#00000000>Int32</color> <color=#00000000>size</color>);</color></color>
    }

    /// <summary> Translates SSNC.Logger.Level into plog.Models.Level. </summary>
    public static Dictionary<Level, plog.Models.Level> Translate_Level_To_Plog_Level = new()
    {
        {Level.Off, plog.Models.Level.Off},
        {Level.Debug, plog.Models.Level.Debug},
        {Level.Info, plog.Models.Level.Info},
        {Level.Fine, plog.Models.Level.Fine},
        {Level.Warning, plog.Models.Level.Warning},
        {Level.Error, plog.Models.Level.Error},
        {Level.Exception, plog.Models.Level.Exception},
        {Level.Extreme, plog.Models.Level.Exception},
        {Level.CommandLine, plog.Models.Level.CommandLine},
        {Level.Config, plog.Models.Level.Config}
    };

    /// <summary> Translates SSNC.Logger.Level into BepInEx.Logging.LogLevel. </summary>
    public static Dictionary<Level, BepInEx.Logging.LogLevel> Translate_Level_To_Bep_Level = new()
    {
        {Level.Off, BepInEx.Logging.LogLevel.None},
        {Level.Debug, BepInEx.Logging.LogLevel.Debug},
        {Level.Info, BepInEx.Logging.LogLevel.Info},
        {Level.Fine, BepInEx.Logging.LogLevel.Info},
        {Level.Warning, BepInEx.Logging.LogLevel.Warning},
        {Level.Error, BepInEx.Logging.LogLevel.Error},
        {Level.Exception, BepInEx.Logging.LogLevel.Error},
        {Level.Extreme, BepInEx.Logging.LogLevel.Fatal},
        {Level.CommandLine, BepInEx.Logging.LogLevel.Info},
        {Level.Config, BepInEx.Logging.LogLevel.Info}
    };

    /// <summary> Struct used for customizing the style of how Primitives, Classes, Interfaces, Enum's, Structs, and Delegates look in SSNC.Log.GetNameMethod(). </summary>
    /// <param name="Primitive">Hexcode color for Primitives.</param>
    /// <param name="Class">Hexcode color for Classes.</param>
    /// <param name="Interface">Hexcode color for Interfaces.</param>
    /// <param name="Enum">Hexcode color for Enum's.</param>
    /// <param name="Struct">Hexcode color for Structs.</param>
    /// <param name="Delegate">Hexcode color for Delegates.</param>
    public struct Stylizer(Hex? Basic = null, Hex? Primitive = null, Hex? Class = null, Hex? Interface = null, Hex? Enum = null, Hex? Struct = null, Hex? Delegate = null, Hex? Parameter = null, Hex? Method = null, Hex? Namespace = null, Hex? FallBack = null)
    {
        /// <summary> Hexcode color for Basic stuff. (example: public)</summary>
        public Hex Basic = Basic ?? "47D";
        /// <summary> Hexcode color for Primitives. (example: int)</summary>
        public Hex Primitive = Primitive ?? "8BF";
        /// <summary> Hexcode color for Classes. (example: Logger)</summary>
        public Hex Class = Class ?? "5CB";
        /// <summary> Hexcode color for Interfaces. (example: ICheat)</summary>
        public Hex Interface = Interface ?? "BDA";
        /// <summary> Hexcode color for Enum's. (example: Logger.Level)</summary>
        public Hex Enum = Enum ?? "9DC";
        /// <summary> Hexcode color for Structs. (example: Hex)</summary>
        public Hex Struct = Struct ?? "8C9";
        /// <summary> Hexcode color for Delegates. (example: Action)</summary>
        public Hex Delegate = Delegate ?? "4BC";
        /// <summary> Hexcode color for Methods. (example: Action)</summary>
        public Hex Method = Method ?? "DDA";
        /// <summary> Hexcode color for Parameters. (example: Action)</summary>
        public Hex Parameter = Parameter ?? "9DF";
        /// <summary> Hexcode color for Namespaces. (example: Action)</summary>
        public Hex Namespace = Namespace ?? "FFF";
        /// <summary> Hexcode color to fallback to. </summary>
        public Hex FallBack = FallBack ?? "FFF";

        /// <summary> Takes a type and finds its color in this style as a hex. </summary>
        /// <param name="type">The Type.</param>
        /// <returns>The color of the Type in this style.</returns>
#pragma warning disable IDE0251 // Make member 'readonly'
        public string GetColorOfType(Type type) =>
            $"<color=#{(
                type.IsPrimitive ? Primitive // if type is primitive, such as int, float, char, bool.
                : type.IsClass ? Class // if the type is a class, like SSNC.Logger
                : type.IsInterface ? Interface // if the type is an interface, such as IEnumerable, IList, IDictionary
                                               // (the "I" doesnt make it an interface, its just part of the name so u KNOW its an inferace)
                : type.IsEnum ? Enum // if the type is a enum, like DayOfWeek, ConsoleColor, ConsoleKey
                : type.IsValueType ? Struct // if the type is a struct, such as DateTime, TimeSpan, Guid
                : type.IsSubclassOf(typeof(Delegate)) || type == typeof(Delegate) ? Delegate // if the type is a delegate, like Action, Func<>, Predicate<>
                : FallBack).ToString()}>"; // fallback if neither of those
#pragma warning restore IDE0251 // Make member 'readonly'
        // fuck you just because it CAN be readonly doesnt mean i WANT it to be readonly u piece of sh9it

        /// <summary> Takes a type and finds its color in the chosen style as a hex. </summary>
        /// <param name="type">The Type.</param>
        /// <param name="style">The chosen style.</param>
        /// <returns>The color of the Type in the chosen style.</returns>
        public static string GetColorOfType(Type type, Stylizer style) =>
            style.GetColorOfType(type);
    }

    /// <summary> Struct used by Stylizer to pick colors. </summary>
    /// <remarks> Makes a Hex with 4 Bytes. </remarks>
    /// <param name="r">Red value.</param>
    /// <param name="g">Green value.</param>
    /// <param name="b">Blue value.</param>
    /// <param name="a">Alpha value.</param>
    public struct Hex(byte r, byte g, byte b, byte a)
    {
        public static bool fuckoffupieceofshit = false;
        /// <summary> Red value. </summary>
        public byte Red = r;
        /// <summary> Green value. </summary>
        public byte Green = g;
        /// <summary> Blue value. </summary>
        public byte Blue = b;
        /// <summary> Alpha value. </summary>
        public byte Alpha = a;
        /// <summary> Red value. </summary>
        public byte r
        {
            readonly get => Red;
            set { Red = value; }
        }
        /// <summary> Green value. </summary>
        public byte g
        {
            readonly get => Green;
            set { Green = value; }
        }
        /// <summary> Blue value. </summary>
        public byte b
        {
            readonly get => Blue;
            set { Blue = value; }
        }
        /// <summary> Alpha value. </summary>
        public byte a
        {
            readonly get => Alpha;
            set { Alpha = value; }
        }

        /// <summary> Red Color. </summary>
        public static readonly Hex red = "#FF0000";
        /// <summary> Green Color. </summary>
        public static readonly Hex green = "#00FF00";
        /// <summary> Bright Teal Color. </summary>
        public static readonly Hex brightTeal = "#01F9C6";
        /// <summary> Blue Color. </summary>
        public static readonly Hex blue = "#0000FF";
        /// <summary> White Color. </summary>
        public static readonly Hex white = "#FFFFFF";
        /// <summary> Black Color. </summary>
        public static readonly Hex black = "#000000";
        /// <summary> Yellow Color. </summary>
        public static readonly Hex yellow = "#FFFF00";
        /// <summary> Cyan Color. </summary>
        public static readonly Hex cyan = "#00FFFF";
        /// <summary> Magent Color. </summary>
        public static readonly Hex magent = "#FF00FF";
        /// <summary> Gray Color. </summary>
        public static readonly Hex gray = "#7F7F7F";
        /// <summary> Grey Color. </summary>
        public static readonly Hex grey = gray;
        /// <summary> Clear Color. </summary>
        public static readonly Hex clear = "#00"; // this is only 2 characters for AA cuz its clear, we dont need no color
        /// <summary> Transparent Color. </summary>
        public static readonly Hex transparent = clear;

        /// <summary> Makes a Hex with 3 Integers. </summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        public Hex(int r, int g, int b) : this(r, g, b, 255) { }

        /// <summary> Makes a Hex with 4 Integers. </summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        public Hex(int r, int g, int b, int a) : this((byte)r, (byte)g, (byte)b, (byte)a) { }

        /// <summary> Makes a Hex with 3 Bytes. </summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        public Hex(byte r, byte g, byte b) : this(r, g, b, (byte)255) { }

        /// <summary> Makes a Hex with either one of 4 Bytes. ((r, g, &amp; b: default to 0) (a: default to 255))</summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        public Hex(byte? r = null, byte? g = null, byte? b = null, byte? a = null) : this(r ?? 0, g ?? 0, b ?? 0, a ?? 255) { }

        /// <summary> Makes a Hex with either one of 4 Integers. ((r, g, &amp; b: default to 0) (a: default to 255))</summary>
        /// <param name="r">Red value.</param>
        /// <param name="g">Green value.</param>
        /// <param name="b">Blue value.</param>
        /// <param name="a">Alpha value.</param>
        public Hex(int? r = null, int? g = null, int? b = null, int? a = null) : this(r ?? 0, g ?? 0, b ?? 0, a ?? 255) { }

        /// <summary> Adds each color channel in the Hex by each color channel in another Hex. </summary>
        /// <param name="a">Hex to be Added.</param>
        /// <param name="b">Hex to Add by.</param>
        /// <returns>The Added Hex.</returns>
        public static Hex operator +(Hex a, Hex b) =>
            new(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);

        /// <summary> Adds each color channel in the Hex by a byte. </summary>
        /// <param name="a">Hex to be Added.</param>
        /// <param name="b">Byte to Add by.</param>
        /// <returns>The Added Hex.</returns>
        public static Hex operator +(Hex a, byte b) => 
            new(a.r + b, a.g + b, a.b + b, a.a + b);

        /// <summary> Substracts each color channel in the Hex by each color channel in another Hex. </summary>
        /// <param name="a">Hex to be Substracted.</param>
        /// <param name="b">Hex to Substract by.</param>
        /// <returns>The Substracted Hex.</returns>
        public static Hex operator -(Hex a, Hex b) =>
            new(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);

        /// <summary> Substracts each color channel in the Hex by a byte. </summary>
        /// <param name="a">Hex to be Substracted.</param>
        /// <param name="b">Byte to Substract by.</param>
        /// <returns>The Substracted Hex.</returns>
        public static Hex operator -(Hex a, byte b) =>
            new(a.r - b, a.g - b, a.b - b, a.a - b);

        /// <summary> Multiples each color channel in the Hex by each color channel in another Hex. </summary>
        /// <param name="a">Hex to be Multiplied.</param>
        /// <param name="b">Hex to Multiply by.</param>
        /// <returns>The Multiplied Hex.</returns>
        public static Hex operator *(Hex a, Hex b) =>
            new(a.r * b.r, a.g * b.g, a.b * b.b, a.a * b.a);

        /// <summary> Multiples each color channel in the Hex by a byte. </summary>
        /// <param name="a">Hex to be Multiplied.</param>
        /// <param name="b">Byte to Multiply by.</param>
        /// <returns>The Multiplied Hex.</returns>
        public static Hex operator *(Hex a, byte b) =>
            new(a.r * b, a.g * b, a.b * b, a.a * b);

        /// <summary> Divides each color channel in the Hex by each color channel in another Hex. </summary>
        /// <param name="a">Hex to be Divided.</param>
        /// <param name="b">Hex to Divide by.</param>
        /// <returns>The Divided Hex.</returns>
        public static Hex operator /(Hex a, Hex b) =>
            new(a.r / b.r, a.g / b.g, a.b / b.b, a.a / b.a);

        /// <summary> Divides each color channel in the Hex by a byte. </summary>
        /// <param name="a">Hex to be Divided.</param>
        /// <param name="b">Byte to Divide by.</param>
        /// <returns>The Divided Hex.</returns>
        public static Hex operator /(Hex a, byte b) =>
            new(a.r / b, a.g / b, a.b / b, a.a / b);

        /// <summary> Divides each color channel in the Hex by a byte. </summary>
        /// <param name="a">Hex to be Divided.</param>
        /// <param name="b">Byte to Divide by.</param>
        /// <returns>The Divided Hex.</returns>
        public static float[] operator /(Hex a, float b) =>
            [ a.r / b, a.g / b, a.b / b, a.a / b ];

        /// <summary> Checks if a Hex is equal to another. </summary>
        /// <param name="left">Left Hex to check.</param>
        /// <param name="right">Right Hex to check.</param>
        /// <returns>Whether the Hex's are equal or not.</returns>
        public static bool operator ==(Hex left, Hex right) =>
            left.Equals(right);

        /// <summary> Checks if a Hex isn't equal to another. </summary>
        /// <param name="left">Left Hex to check.</param>
        /// <param name="right">Right Hex to check.</param>
        /// <returns>Whether the Hex's aren't equal.</returns>
        public static bool operator !=(Hex left, Hex right) =>
            !left.Equals(right);

        /// <summary> Checks if the Left Hex is greater than the Right Hex. </summary>
        /// <param name="left">Left Hex to check.</param>
        /// <param name="right">Right Hex to check.</param>
        /// <returns>Whether the Left Hex is greater than the Right Hex.</returns>
        public static bool operator >(Hex left, Hex right) =>
            Compare(left, right) >= 3;

        /// <summary> Checks if the Left Hex is less than the Right Hex. </summary>
        /// <param name="left">Left Hex to check.</param>
        /// <param name="right">Right Hex to check.</param>
        /// <returns>Whether the Left Hex is less than the Right Hex.</returns>
        public static bool operator <(Hex left, Hex right) =>
            Compare(left, right) <= 2;

        /// <summary> Compares how many values in the Left Hex are greater than ones in the Right Hex.</summary>
        /// <param name="left">Left Hex to compare.</param>
        /// <param name="right">Right Hex to compare.</param>
        /// <returns>The amount of values in the Left Hex that are greater than ones in the Right Hex.</returns>
        public static int Compare(Hex left, Hex right)
        {
            int count = 0;
            if (left.Red > right.Red) count++;
            if (left.Green > right.Green) count++;
            if (left.Blue > right.Blue) count++;
            if (left.Alpha > right.Alpha) count++;
            return count;
        }

        /// <summary> Darkens a color byte based on the alpha. </summary>
        /// <param name="colorByte">The color byte to be darken'd.</param>
        /// <param name="Alpha">Alpha to darken by.</param>
        /// <returns>The colorByte once it has been darken'd by the alpha.</returns>
        public static byte DarkenBasedOnAlpha(byte? colorByte, byte? Alpha) =>
            (byte)((colorByte ?? 255) * ((Alpha ?? 255) / 255f));

        /// <summary> Darkens a color byte based on this Hex's alpha. </summary>
        /// <param name="colorByte">The color byte to be darken'd.</param>
        /// <returns>The colorByte once it has been darken'd by the alpha.</returns>
        public byte DarkenBasedOnAlpha(byte? colorByte) =>
            DarkenBasedOnAlpha(colorByte ?? 255, Alpha);

        /// <summary> Converts an RGBA Hex into RGB. (darkens based on alpha. it also still has alpha channel, just it doesnt do anything.)</summary>
        /// <param name="h">Hex to convert.</param>
        /// <returns>The new converted Hex.</returns>
        public static Hex ToRGB(Hex h) =>
            new(h.DarkenBasedOnAlpha(h.Red), h.DarkenBasedOnAlpha(h.Green), h.DarkenBasedOnAlpha(h.Blue));

        /// <summary> Converts this RGBA Hex into RGB. (darkens based on alpha. it also still has alpha channel, just it doesnt do anything.)</summary>
        /// <returns>The new converted Hex.</returns>
        public readonly Hex ToRGB() => ToRGB(this);

        /// <summary> Converts an RGBA Hex into an array of floats representing CMYK. (darkens based on alpha.)</summary>
        /// <param name="h">Hex to convert.</param>
        /// <returns>An array of floats containing CMYK values.</returns>
        public static float[] ToCMYK(Hex h)
        {
            h = ToRGB(h);
            float Red = h.r / 255f, Green = h.g / 255f, Blue = h.b / 255f,
                Black/*(K)*/ = Mathf.Min(1 - Red, 1 - Green, 1 - Blue), 
                Cyan/*(C)*/ = (1 - Red - Black) / (1 - Black),
                Magenta/*(M)*/ = (1 - Green - Black) / (1 - Black),
                Yellow/*(Y)*/ = (1 - Blue - Black) / (1 - Black);
            return [Cyan/*(C)*/, Magenta/*(M)*/, Yellow/*(Y)*/, Black/*(K)*/];
        }

        /// <summary> Converts this RGBA Hex into an array of floats representing CMYK. (darkens based on alpha.)</summary>
        /// <returns>An array of floats containing CMYK values.</returns>
        public readonly float[] ToCMYK() =>
            ToCMYK(this);

        /// <summary> Converts an RGBA Hex into an array of floats representing HSV. (darkens based on alpha.)</summary>
        /// <param name="h">Hex to convert.</param>
        /// <returns>An array of floats containing HSV values.</returns>
        public static float[] ToHSV(Hex h) {
            Color.RGBToHSV(ToRGB(h), out var H, out var S, out var V); // im too fucking lazy to figure this shit out or ask gpt to do it for me
            return [H, S, V];
        }

        /// <summary> Converts this RGBA Hex into an array of floats representing HSV. (darkens based on alpha.)</summary>
        /// <returns>An array of floats containing HSV values.</returns>
        public readonly float[] ToHSV() =>
            ToHSV(this);

        /// <summary> Checks if a object is equal to this Hex. </summary>
        /// <param name="A">Object to check.</param>
        /// <returns>Whether the object is a Hex and if its equal to this Hex.</returns>
        public override readonly bool Equals(object A)
        {
            if (A is not Hex a) return false;
            return a.r == r && a.g == g && a.b == b && a.a == Alpha;
        }

        /// <summary> Gets the hash code or whatever.. (idfk what this is i just know i need to override it for == and != operators)</summary>
        /// <returns>The hash code..</returns>
        public override readonly int GetHashCode() =>
            HashCode.Combine(Red, Green, Blue, Alpha);

        /// <summary> Converts the Hex into a string. </summary>
        /// <param name="format">Format to be converted into.</param>
        /// <returns>The string defition of this Hex</returns>
        public readonly string ToString(string format = "X2")
        {
            Plugin.Log.Debug("active tostring(format) " + format);
            if (format[0] != 'x' || format != "rgb" || format != "rgba") format = format.ToUpper();
            string point = format.Length == 1 ? "" : format.Substring(1);
            string sub(byte val, string format) => val.ToString($"{format[0]}{point}");
            string product = format switch
            {
                "HEX" => $"{Red:X2}{Green:X2}{Blue:X2}{Alpha:X2}",
                "A" => $"{Alpha:D3}", // 255
                "rgb" => $"{Red:D} {Green:D} {Blue:D}", // 1 249 198
                "RGB" => $"{Red:D3}{Green:D3}{Blue:D3}", // 001249198
                "rgba" => $"{Red:D} {Green:D} {Blue:D} {Alpha:D}", // 1 249 198 255
                "RGBA" => $"{Red:D3}{Green:D3}{Blue:D3}{Alpha:D3}", // 001249198255
                "#AA" => $"{Alpha:X2}", // FF
                "#RGB" => $"{Red.ToString("X2")[0]}{Green.ToString("X2")[0]}{Blue.ToString("X2")[0]}",// 0FC
                "#RGBA" => $"{Red.ToString("X2")[0]}{Green.ToString("X2")[0]}{Blue.ToString("X2")[0]}{Alpha.ToString("X2")[0]}", // 0FCF
                "#RRGGBB" => $"{Red:X2}{Green:X2}{Blue:X2}", // 01F9C6
                "#RRGGBBAA" => $"{Red:X2}{Green:X2}{Blue:X2}{Alpha:X2}", // 01F9C6FF
                "G" => $"{sub(Red, "G")}{sub(Green, "G")}{sub(Blue, "G")}{sub(Alpha, "G")}",
                "D" => $"{sub(Red, "D")} {sub(Green, "D")} {sub(Blue, "D")} {sub(Alpha, "D")}",
                "X" => $"{sub(Red, "X")}{sub(Green, "X")}{sub(Blue, "X")}{sub(Alpha, "X")}",
                "x" => $"{sub(Red, "x")}{sub(Green, "x")}{sub(Blue, "x")}{sub(Alpha, "x")}",
                "N" => $"{sub(Red, "N")}{sub(Green, "N")}{sub(Blue, "N")}{sub(Alpha, "N")}",
                "F" => $"{sub(Red, "F")}{sub(Green, "F")}{sub(Blue, "F")}{sub(Alpha, "F")}",
                "E" => $"{sub(Red, "E")}{sub(Green, "E")}{sub(Blue, "E")}{sub(Alpha, "E")}",
                "P" => $"{sub(Red, "P")}{sub(Green, "P")}{sub(Blue, "P")}{sub(Alpha, "P")}",
                "C" => $"{sub(Red, "C")}{sub(Green, "C")}{sub(Blue, "C")}{sub(Alpha, "C")}",
                _ => $"{Red:X2}{Green:X2}{Blue:X2}{Alpha:X2}"
            };
            return product;
        }

        public override string ToString() { Plugin.Log.Debug("active tostring()"); return ToString("HEX"); }

        /// <summary> Implicit operator so Hex's can be used like strings. </summary>
        /// <param name="h">The Hex to convert into a string.</param>
        /// <returns>The Hex as a string.</returns>
        public static implicit operator string(Hex h) 
        {
            Plugin.Log.Debug("active implicit string opeerator");
            if (!fuckoffupieceofshit) return h.ToString("HEX");
            else return $"{h.Red:X2}{h.Green:X2}{h.Blue:X2}{h.Alpha:X2}";
        }

        /// <summary> Implicit operator so strings can be used like Hex's. </summary>
        /// <param name="s">The string to convert into a Hex.</param>
        /// <returns>The string as a Hex. (returns FFF if your string isnt a supported type)</returns>
        public static implicit operator Hex(string s)
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

        /// <summary> Implicit operator so Hex's can be used like intergers. </summary>
        /// <param name="h">The Hex to convert into an interger.</param>
        /// <returns>The Hex as an interger.</returns>
        public static implicit operator int(Hex h) =>
            int.Parse(h, System.Globalization.NumberStyles.HexNumber);

        /// <summary> Implicit operator so intergers can be used like Hex's. </summary>
        /// <param name="h">The interger to convert into an Hex.</param>
        /// <returns>The interger as an Hex.</returns>
        public static implicit operator Hex(int i)
        {
            string hexI = i.ToString("X2");
            bool Is(params int[] poss) {
                foreach (int pos in poss) if (hexI.Length == pos) return true;
                return false;
            }
            if (!Is(2, 3, 4, 6, 8)) hexI = $"0{hexI}";

            return hexI;
        }

        /// <summary> Implicit operator so Hex's can be used like Color's. </summary>
        /// <param name="h">The Hex to convert into a Color.</param>
        public static implicit operator Color(Hex h) => 
            new((h/255f)[0], (h/255f)[1], (h/255f)[2], (h/255f)[3]);

        /// <summary> Implicit operator so Color's can be used like Hex's. </summary>
        /// <param name="h">The Color to convert into a Hex.</param>
        public static implicit operator Hex(Color c)
        {
            int toInt(float val) => (int)Math.Round(Mathf.Clamp(val * 255f, 0, 255));
            Hex HexCol = new(toInt(c.r), toInt(c.g), toInt(c.b), toInt(c.a));
            return HexCol;
        }

        /// <summary> Implicit operator so Hex's can be used like UniversalColor's. (Darkens colors based on alpha due to UniversalColor's not supporting alpha channels) </summary>
        /// <param name="h">The Hex to convert into a UniversalColor.</param>
        public static implicit operator UniversalColor(Hex h) => 
            new(h.DarkenBasedOnAlpha(h.Red), h.DarkenBasedOnAlpha(h.Green), h.DarkenBasedOnAlpha(h.Blue));

        /// <summary> Implicit operator so UniversalColor's can be used like Hex's. </summary>
        /// <param name="c">The UniversalColor to convert into a Hex.</param>
        public static implicit operator Hex(UniversalColor c) =>
            new(c.Red, c.Green, c.Blue);
    }
}