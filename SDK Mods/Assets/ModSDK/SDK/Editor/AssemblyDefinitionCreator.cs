// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using UnityEngine;

public static class AssemblyDefinitionCreator
{
    public static bool WriteAssemblyDefinition(string assetPath, AssemblyDefinition assemblyDefinition)
    {
        try
        {
            var json = JsonUtility.ToJson(assemblyDefinition, true);
            File.WriteAllText(assetPath, json, System.Text.Encoding.UTF8);
            return true;
        }
        catch (IOException)
        {
            return false;
        }
    }

    public static AssemblyDefinition GetAssemblyDefinition(string assetPath)
    {
        try
        {
            var json = File.ReadAllText(assetPath);
            var asmdef = new AssemblyDefinition();
            JsonUtility.FromJsonOverwrite(json, asmdef);
            return asmdef;
        }
        catch (IOException)
        {
            return null;
        }
    }

    public class AssemblyDefinition
    {
        public string name;
        public string[] references;
        public string[] includePlatforms;
        public string[] excludePlatforms;
        public bool allowUnsafeCode;
        public bool overrideReferences;
        public string[] preCompiledReferences;
        public bool autoReferenced;
        public string[] defineConstraints;
        public VersionDefine[] versionDefines;
        public bool useGUIDs;

        public AssemblyDefinition()
        {
            references = Array.Empty<string>();
            includePlatforms = Array.Empty<string>();
            excludePlatforms = Array.Empty<string>();
            preCompiledReferences = Array.Empty<string>();
            defineConstraints = Array.Empty<string>();
            versionDefines = Array.Empty<VersionDefine>();
        }
    }

    [System.Serializable]
    public class VersionDefine
    {
        public string name;
        public string expression;
        public string define;
    }
}
