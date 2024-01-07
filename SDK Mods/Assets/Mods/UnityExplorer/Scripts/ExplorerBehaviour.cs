﻿using System.Reflection;
using UnityEngine;
using UnityExplorer.UI;
using UniverseLib;
using UInputManager = UniverseLib.Input.InputManager;

namespace UnityExplorer
{
    public class ExplorerBehaviour : MonoBehaviour
    {
        internal static ExplorerBehaviour Instance { get; private set; }

#if CPP
        public ExplorerBehaviour(System.IntPtr ptr) : base(ptr) { }
#endif

        internal static void Setup()
        {
#if CPP
            ClassInjector.RegisterTypeInIl2Cpp<ExplorerBehaviour>();
#endif

            GameObject obj = new("ExplorerBehaviour");
            DontDestroyOnLoad(obj);
            obj.hideFlags = HideFlags.HideAndDontSave;
            Instance = obj.AddComponent<ExplorerBehaviour>();
        }

        internal void Update()
        {
            ExplorerCore.Update();
        }

        // For editor, to clean up objects

        internal void OnDestroy()
        {
            OnApplicationQuit();
        }

        internal bool quitting;

        internal void OnApplicationQuit()
        {
            if (quitting) return;
            quitting = true;

            if (UE_UIManager.UIRoot != null)
            {
                TryDestroy(UE_UIManager.UIRoot.transform.root.gameObject);
            }

            TryDestroy((typeof(Universe).Assembly.GetType("UniverseLib.UniversalBehaviour")
                .GetProperty("Instance", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null, null)
                as Component).gameObject);

            TryDestroy(this.gameObject);
        }

        internal void TryDestroy(GameObject obj)
        {
            try
            {
                if (obj)
                    Destroy(obj);
            }
            catch { }
        }
    }
}
