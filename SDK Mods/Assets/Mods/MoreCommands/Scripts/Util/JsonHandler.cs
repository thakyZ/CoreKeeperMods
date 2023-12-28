// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using PugMod;

namespace MoreCommands
{
    public class JsonHandler
    {
        private T LoadConfig<T>(string path)
        {
            T output;

            if (!API.ConfigFilesystem.FileExists(path))
            {
                try
                {
                    var jsonText = Encoding.UTF8.GetString(API.ConfigFilesystem.Read(path));
                    output = JsonConvert.DeserializeObject<T>(jsonText);
                }
                catch (Exception exception)
                {
                    Debug.Write($"[{MoreCommandsMod.NAME}] Failed to load or deserialize file at {path}:\n{exception.Message}\n{exception.StackTrace}");
                }
            }
            else
            {
                try
                {
                    output = default(T);
                    var jsonText = JsonConvert.SerializeObject(output);
                    API.ConfigFilesystem.Write(path, Encoding.UTF8.GetBytes(jsonText));
                    return output;
                }
                catch (Exception exception)
                {
                    Debug.Write($"[{MoreCommandsMod.NAME}] Failed to write or serialize file at {path}:\n{exception.Message}\n{exception.StackTrace}");
                }
            }

            throw new Exception("Failed spectacularly.")
        }
    }
}
