// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using CoreLib.Data.Configuration;
using PugMod;

namespace MoreCommands.Data.Configuration {
  public class JsonConfigFile<T> where T : IConfiguration {
    private readonly LoadedMod ownerMetadata;

    private static Encoding UTF8NoBom { get; } = new UTF8Encoding(false);

    /// <inheritdoc cref="JsonConfig" />
    public JsonConfigFile(string configPath, bool saveOnInit) : this(configPath, saveOnInit, null) { }

    private T context;

    /// <summary>
    ///     Create a new config file at the specified config path.
    /// </summary>
    /// <param name="configPath">Full path to a file that contains settings. The file will be created as needed.</param>
    /// <param name="saveOnInit">If the config file/directory doesn't exist, create it immediately.</param>
    /// <param name="ownerMetadata">Information about the plugin that owns this setting file.</param>
    /// <param name="context"></param>
    public JsonConfigFile(string configPath, bool saveOnInit, LoadedMod ownerMetadata) {
      this.ownerMetadata = ownerMetadata;
      context = (T)Activator.CreateInstance(typeof(T));

      ConfigFilePath = configPath ?? throw new ArgumentNullException(nameof(configPath));

      if (API.ConfigFilesystem.FileExists(ConfigFilePath)) {
        Reload();
      } else if (saveOnInit) {
        Save();
      }
    }

    /// <summary>
    ///     Full path to the config file. The file might not exist until a setting is added and changed, or <see cref="Save" />
    ///     is called.
    /// </summary>
    public string ConfigFilePath { get; }

    /// <summary>
    ///     If enabled, writes the config to disk every time a value is set.
    ///     If disabled, you have to manually use <see cref="Save" /> or the changes will be lost!
    /// </summary>
    public bool SaveOnConfigSet { get; set; } = true;

    #region Save/Load

    /// <summary>
    ///     Returns the ConfigEntryBase values that the ConfigFile contains.
    ///     <para>Creates a new array when the property is accessed. Thread-safe.</para>
    /// </summary>
    public T Context {
      get {
         return context;
      }
    }

    /// <summary>
    /// Generate user-readable comments for each of the settings in the saved .cfg file.
    /// </summary>
    public bool GenerateSettingDescriptions { get; set; } = true;

    /// <summary>
    /// Reloads the config from disk. Unsaved changes are lost.
    /// </summary>
    public void Reload() {
      var options = new JsonSerializerOptions
      {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        MaxDepth = 1000,
      };

      var fileData = API.ConfigFilesystem.Read(ConfigFilePath);
      var fileText = UTF8NoBom.GetString(fileData);

      try {
        context = JsonSerializer.Deserialize<T>(fileText, options);
      } catch (Exception exception) {
        MoreCommandsMod.Log.LogError($"Failed to deserialize the json object for context of type {typeof(T)}\n{exception.Message}\n{exception.StackTrace}");
        return;
      }

      OnConfigReloaded();
    }

    /// <summary>
    /// Writes the config to disk.
    /// </summary>
    public void Save()
    {
      var directoryName = GetDirectoryName(ConfigFilePath);
      if (directoryName != null) {
        API.ConfigFilesystem.CreateDirectory(directoryName);
      }

      var stringBuilder = new StringBuilder();

      if (ownerMetadata != null) {
        stringBuilder.AppendLine($"// Settings file was created by plugin {ownerMetadata.Metadata.name}");
        stringBuilder.AppendLine();
      }

      try {
        var options = new JsonSerializerOptions {
          AllowTrailingCommas = true,
          WriteIndented = true,
          ReadCommentHandling = JsonCommentHandling.Skip,
          MaxDepth = 1000,
        };

        var jsonData = JsonSerializer.Serialize(context, typeof(T), options);
        stringBuilder.Append(jsonData);
      } catch (Exception exception) {
        MoreCommandsMod.Log.LogError($"[{MoreCommandsMod.NAME}]: Failed to serialize the json object for context of type {typeof(T)}\n{exception.Message}\n{exception.StackTrace}");
        return;
      }

      var fileData = UTF8NoBom.GetBytes(stringBuilder.ToString());
      API.ConfigFilesystem.Write(ConfigFilePath, fileData);
    }

    internal static readonly char[] PathSeparatorChars = new[] { '/', '\\' };

    public static string GetDirectoryName(string path) {
      if (path == string.Empty) {
        throw new ArgumentException("Invalid path");
      }

      if (path == null) {
        return null;
      }

      if (path.Trim().Length == 0) {
        throw new ArgumentException("Argument string consists of whitespace characters only.");
      }

      var num = path.LastIndexOfAny(PathSeparatorChars);

      if (num == 0) {
        num++;
      }

      if (num <= 0) {
        return string.Empty;
      }

      var text = path[..num];

      return text;
    }

    #endregion

    #region Events

    /// <summary>
    ///     An event that is fired every time the config is reloaded.
    /// </summary>
    public event EventHandler ConfigReloaded;

    /// <summary>
    ///     Fired when one of the settings is changed.
    /// </summary>
    public event EventHandler<SettingChangedEventArgs> SettingChanged;

    internal void OnSettingChanged(object sender, ConfigEntryBase changedEntryBase) {
      if (changedEntryBase == null) {
        throw new ArgumentNullException(nameof(changedEntryBase));
      }

      if (SaveOnConfigSet) {
        Save();
      }

      var settingChanged = SettingChanged;

      if (settingChanged == null) {
        return;
      }

      var args = new SettingChangedEventArgs(changedEntryBase);

      foreach (var callback in settingChanged.GetInvocationList().Cast<EventHandler<SettingChangedEventArgs>>()) {
        try {
          callback(sender, args);
        } catch (Exception exception) {
          MoreCommandsMod.Log.LogError($"[{MoreCommandsMod.NAME}]: Failed to run callback.\n{exception.Message}\n{exception.StackTrace}");
        }
      }
    }

    private void OnConfigReloaded() {
      var configReloaded = ConfigReloaded;

      if (configReloaded == null) {
        return;
      }

      foreach (var callback in configReloaded.GetInvocationList().Cast<EventHandler>()) {
        try {
          callback(this, EventArgs.Empty);
        } catch (Exception exception) {
          MoreCommandsMod.Log.LogError($"[{MoreCommandsMod.NAME}]: Failed to run callback.\n{exception.Message}\n{exception.StackTrace}");
        }
      }
    }

    #endregion
  }
}
