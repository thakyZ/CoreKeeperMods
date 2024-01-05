// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;

using PugMod;

using UnityEditor;

using UnityEngine;

public class ModBuilderSettings : ScriptableObject
{
  public ModMetadata metadata = new ModMetadata
  {
    guid = Guid.NewGuid().ToString("N"),
    name = "MyMod",
  };

  public string modPath = "Assets/Mod";

  public bool forceReimport = true;
  public bool buildBundles = true;
  public bool buildLinux = false;

  public bool buildBurst = true;
  public bool cachedBuild = false;

  public List<ModAsset> assets;

  [HideInInspector]
  public bool lastBuildLinux = false;

  [Serializable]
  public struct ModAsset
  {
    public string path;
    public string hash;
  }

  private void OnValidate()
  {
    if (string.IsNullOrEmpty(modPath))
    {
      var path = AssetDatabase.GetAssetPath(this);
      modPath = Path.GetDirectoryName(path);
    }
  }
}