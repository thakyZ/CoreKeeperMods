// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

using UnityEditor;

using UnityEngine;

public class TextInputPopUp : EditorWindow
{
  private string _inputString;
  private static Action<string> _callback;

  private string _textFieldLabel;
  private string _submit;
  private string _cancel;

  public static void ShowWindow(string title, string textFieldLabel, string submit, string cancel, string inputText, Action<string> callbackFunction)
  {
    _callback = callbackFunction;
    TextInputPopUp window = (TextInputPopUp)EditorWindow.GetWindow(typeof(TextInputPopUp));
    window.titleContent = new GUIContent(title);
    window.minSize = window.maxSize = new Vector2(400, 100);
    window.Show();

    window._textFieldLabel = textFieldLabel;
    window._submit = submit;
    window._cancel = cancel;
    window._inputString = inputText;
  }

  private void OnGUI()
  {
    GUILayout.BeginHorizontal(); // text group

    GUILayout.FlexibleSpace();

    _inputString = EditorGUILayout.TextField(_textFieldLabel, _inputString, GUILayout.Width(350));

    GUILayout.FlexibleSpace();

    GUILayout.EndHorizontal(); // text group end

    GUILayout.Space(20);

    GUILayout.BeginHorizontal(); // button group

    GUILayout.FlexibleSpace();

    if (GUILayout.Button(_submit, GUILayout.Width(100f)))
    {
      if (_callback != null)
        _callback(_inputString);

      this.Close();
    }

    GUILayout.Space(10);

    if (GUILayout.Button(_cancel, GUILayout.Width(100f)))
    {
      this.Close();
    }

    GUILayout.FlexibleSpace();

    GUILayout.EndHorizontal(); // button group end
  }
}