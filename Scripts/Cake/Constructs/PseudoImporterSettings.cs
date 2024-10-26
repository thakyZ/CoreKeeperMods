using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Cake.Common.IO;
using Cake.NekoBoiNick.CoreKeeperMods.Constructs;
using YamlDotNet.Serialization;

// cSpell:words NetStandard
// cSpell:ignoreRegexp /mod\.?io/
// cSpell:ignoreRegexp /[Mm[Ss][Cc]ore?[Ll]ib/

namespace Cake.NekoBoiNick.CoreKeeperMods.Constructs;

public enum AssetReferenceType : int {
  // ReSharper disable InconsistentNaming
  Unk_0 = 0,
  Unk_1 = 1,
  Unk_2 = 2,
  // ReSharper restore InconsistentNaming
  Script = 3,
}

public sealed class AssetReference {
  // ReSharper disable InconsistentNaming
  public int fileID { get; set; }
  public Guid? guid { get; set; } = null;
  public AssetReferenceType type { get; set; } = 0;
  // ReSharper restore InconsistentNaming
}

public sealed class PseudoImporterSettings {
  [YamlIgnore]
  private BuildContext? buildContext;
  // ReSharper disable InconsistentNaming
  [MemberNotNullWhen(true, nameof(IsValid))]
  public List<string>? includeGameAssemblies { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public List<string>? excludeGameAssemblies { get; set; }
  public int initializedToVersion { get; set; }
  public int includeAllAssemblies { get; set; }
  // ReSharper disable UnusedMember.Global
  public int m_ObjectHideFlags { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public AssetReference? m_CorrespondingSourceObject { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public AssetReference? m_PrefabInstance { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public AssetReference? m_PrefabAsset { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public AssetReference? m_GameObject { get; set; }
  public int m_Enabled { get; set; }
  public int m_EditorHideFlags { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public AssetReference? m_Script { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public string? m_Name { get; set; }
  public object? m_EditorClassIdentifier { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public string? sdkAssemblyPath { get; set; }
  [MemberNotNullWhen(true, nameof(IsValid))]
  public string? gameAssemblyPath { get; set; }
  // ReSharper restore UnusedMember.Global
  // ReSharper restore InconsistentNaming

  [YamlIgnore]
  public bool IsValid => m_CorrespondingSourceObject is not null
    && m_PrefabInstance is not null
    && m_PrefabAsset is not null
    && m_GameObject is not null
    && m_Script is not null
    && m_Name is not null
    && sdkAssemblyPath is not null
    && gameAssemblyPath is not null
    && includeGameAssemblies is not null
    && excludeGameAssemblies is not null;
  public static PseudoImporterSettings Init(BuildContext buildContext) {
    var importerSettings = PseudoAssetDatabase.LoadAssetAtPath(buildContext.SettingsPath, buildContext);
    if (importerSettings is not null) {
      importerSettings.buildContext = buildContext;
    } else {
      importerSettings = new PseudoImporterSettings {
        buildContext = buildContext,
      };
      if (!buildContext.DirectoryExists(buildContext.SettingsPath.GetDirectory())) {
        buildContext.CreateDirectory(buildContext.SettingsPath.GetDirectory());
      }
    }
    importerSettings.Initialize();
    return importerSettings;
  }

  private void Initialize() {
    if (buildContext is null || initializedToVersion >= buildContext.CurrentVersion || includeGameAssemblies is null || excludeGameAssemblies is null) {
      return;
    }

    if (initializedToVersion < 1) {
      includeGameAssemblies.Add("NaughtyAttributes.Core.dll");
      includeGameAssemblies.Add("DomainReloadHelper.Runtime.dll");
      includeGameAssemblies.Add("Pug.Base.dll");
      includeGameAssemblies.Add("Pug.ECS.Authoring.dll");
      includeGameAssemblies.Add("Pug.ECS.Components.dll");
      includeGameAssemblies.Add("Pug.ECS.ConditionExtensions.dll");
      includeGameAssemblies.Add("Pug.UnityExtensions.dll");
      includeGameAssemblies.Add("PugMod.SDK.Runtime.dll");
      includeGameAssemblies.Add("PugProperties.dll");

      excludeGameAssemblies.Add("mscorlib.dll");
      excludeGameAssemblies.Add("netstandard.dll");
      excludeGameAssemblies.Add("mcs.dll");
      excludeGameAssemblies.Add("^System.");
      excludeGameAssemblies.Add("^Mono.");
      excludeGameAssemblies.Add("^Unity");
      excludeGameAssemblies.Add("^Microsoft.");
      // Remove TraceManager and keep this?
      //excludeGameAssemblies.Add("^io.sentry.");
      //excludeGameAssemblies.Add("^Sentry.");
      excludeGameAssemblies.Add("CgSDK.dll");
      excludeGameAssemblies.Add("^Trivial.");
      excludeGameAssemblies.Add("^RoslynCSharp.");
      // Only one to use Roslyn and shouldn't ever be needed
      excludeGameAssemblies.Add("^PugMod.Loader");
      excludeGameAssemblies.Add("Assembly-CSharp.dll");
      // Included as source
      excludeGameAssemblies.Add("PugMod.SDK.dll");
      // Harmony included in SDK
      excludeGameAssemblies.Add("0Harmony.dll");
      excludeGameAssemblies.Add("^MonoMod.");
      // Necessary mod.io dll is included in SDK
      excludeGameAssemblies.Add("^modio.");

      // Included in SDK for now
      excludeGameAssemblies.Add("SpriteInstancing.dll");
    }

    if (initializedToVersion < 2) {
      includeGameAssemblies.Add("Microsoft.Bcl.AsyncInterfaces.dll");
      includeGameAssemblies.Add("System.Buffers.dll");
      includeGameAssemblies.Add("System.Memory.dll");
      includeGameAssemblies.Add("System.Numerics.Vectors.dll");
      includeGameAssemblies.Add("System.Text.Encodings.dll");
      includeGameAssemblies.Add("System.Text.Json.dll");
      includeGameAssemblies.Add("System.Runtime.CompilerServices.Unsafe.dll");
    }

    initializedToVersion = buildContext.CurrentVersion;
  }
}
