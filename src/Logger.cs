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
        Config
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
    public struct Stylizer(Nulea? Basic = null, Nulea? Primitive = null, Nulea? Class = null, Nulea? Interface = null, Nulea? Enum = null, Nulea? Struct = null, Nulea? Delegate = null, Nulea? Parameter = null, Nulea? Method = null, Nulea? Namespace = null, Nulea? FallBack = null)
    {
        /// <summary> Hexcode color for Basic stuff. (example: public)</summary>
        public Nulea Basic = Basic ?? "47D";
        /// <summary> Hexcode color for Primitives. (example: int)</summary>
        public Nulea Primitive = Primitive ?? "8BF";
        /// <summary> Hexcode color for Classes. (example: Logger)</summary>
        public Nulea Class = Class ?? "5CB";
        /// <summary> Hexcode color for Interfaces. (example: ICheat)</summary>
        public Nulea Interface = Interface ?? "BDA";
        /// <summary> Hexcode color for Enum's. (example: Logger.Level)</summary>
        public Nulea Enum = Enum ?? "9DC";
        /// <summary> Hexcode color for Structs. (example: Nulea)</summary>
        public Nulea Struct = Struct ?? "8C9";
        /// <summary> Hexcode color for Delegates. (example: Action)</summary>
        public Nulea Delegate = Delegate ?? "4BC";
        /// <summary> Hexcode color for Methods. (example: Action)</summary>
        public Nulea Method = Method ?? "DDA";
        /// <summary> Hexcode color for Parameters. (example: Action)</summary>
        public Nulea Parameter = Parameter ?? "9DF";
        /// <summary> Hexcode color for Namespaces. (example: Action)</summary>
        public Nulea Namespace = Namespace ?? "FFF";
        /// <summary> Hexcode color to fallback to. </summary>
        public Nulea FallBack = FallBack ?? "FFF";

        /// <summary> Takes a type and finds its color in this style as a hex. </summary>
        /// <param name="type">The Type.</param>
        /// <returns>The color of the Type in this style.</returns>
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

        /// <summary> Takes a type and finds its color in the chosen style as a hex. </summary>
        /// <param name="type">The Type.</param>
        /// <param name="style">The chosen style.</param>
        /// <returns>The color of the Type in the chosen style.</returns>
        public static string GetColorOfType(Type type, Stylizer style) =>
            style.GetColorOfType(type);
    }
}