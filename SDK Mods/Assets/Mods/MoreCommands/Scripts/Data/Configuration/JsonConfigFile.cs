// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
#nullable enable

using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using CoreLib.Data.Configuration;
using MoreCommands.Data.Converter;
using PugMod;
using Logger = MoreCommands.Util.Logger;

namespace MoreCommands.Data.Configuration {
  public class JsonConfigFile<T> where T : IConfiguration {
    private readonly LoadedMod ownerMetadata;

    private static Encoding UTF8NoBom => new UTF8Encoding(false);

    private T? context;

    /// <summary>
    ///     Returns the ConfigEntryBase values that the ConfigFile contains.
    ///     <para>Creates a new array when the property is accessed. Thread-safe.</para>
    /// </summary>
    public T? Context {
      get {
         return context;
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

    /// <summary>
    /// Generate user-readable comments for each of the settings in the saved .cfg file.
    /// </summary>
    public bool GenerateSettingDescriptions { get; set; } = true;

    /// <inheritdoc cref="JsonConfig" />
    public JsonConfigFile(string configPath, bool saveOnInit) : this(configPath, saveOnInit, null!) { }

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

    #region Save/Load

    /// <summary>
    /// Reloads the config from disk. Unsaved changes are lost.
    /// </summary>
    public void Reload() {

      var fileData = API.ConfigFilesystem.Read(ConfigFilePath);
      var fileText = UTF8NoBom.GetString(fileData);

      try {
        context = JsonSerializer.Deserialize<T>(fileText, JsonBase.JsonSerializerOptions);
      } catch (Exception exception) {
        Logger.Error($"Failed to deserialize the json object for context of type {typeof(T)}\n{exception.Message}\n{exception.StackTrace}");
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
        var jsonData = JsonSerializer.Serialize(context, typeof(T), JsonBase.JsonSerializerOptions);
        stringBuilder.Append(jsonData);
      } catch (Exception exception) {
        Logger.Error($"Failed to serialize the json object for context of type {typeof(T)}\n{exception.Message}\n{exception.StackTrace}");
        return;
      }

      var fileData = UTF8NoBom.GetBytes(stringBuilder.ToString());
      API.ConfigFilesystem.Write(ConfigFilePath, fileData);
    }

    internal static char[] PathSeparatorChars => new[] { '/', '\\' };

    public static string? GetDirectoryName(string? path) {
      if (path == string.Empty) {
        throw new ArgumentException("Invalid path");
      }

      if (path is null) {
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
    public event EventHandler? ConfigReloaded;

    /// <summary>
    ///     Fired when one of the settings is changed.
    /// </summary>
    public event EventHandler<SettingChangedEventArgs>? SettingChanged;

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
          Logger.Error($"Failed to run callback.\n{exception.Message}\n{exception.StackTrace}");
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
          Logger.Error($"Failed to run callback.\n{exception.Message}\n{exception.StackTrace}");
        }
      }
    }

    #endregion
  }
}
