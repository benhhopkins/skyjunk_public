using System;
using UnityEngine;
using UnityEditor;

namespace Utility {

  public class ButtonEncoder : EditorWindow {
    string output = "";
    Texture2D inputTexture;

    Texture2D testTexture;

    [MenuItem("Window/Button Encoder")]
    static void Init() {
      ButtonEncoder window = (ButtonEncoder)EditorWindow.GetWindow(typeof(ButtonEncoder));
      window.Show();
    }

    void OnGUI() {
      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Input Texture:", EditorStyles.boldLabel);
      inputTexture = (Texture2D)EditorGUILayout.ObjectField(inputTexture, typeof(Texture2D), false);
      EditorGUILayout.Space();

      EditorGUILayout.LabelField("Output:", EditorStyles.boldLabel);
      GUILayout.TextArea(output);
      if (GUILayout.Button("Generate") && inputTexture != null) {
        byte[] bytes = inputTexture.EncodeToPNG();
        output = Convert.ToBase64String(bytes);

        testTexture = Base64ToTexture(output);
      }

      EditorGUILayout.Space();
      EditorGUILayout.LabelField("Test Decode:", EditorStyles.boldLabel);
      if (testTexture != null) {
        var lastRect = GUILayoutUtility.GetLastRect();
        GUI.DrawTexture(new Rect(lastRect.xMin, lastRect.yMax, testTexture.width, testTexture.height), testTexture);
      }
    }

    private static Texture2D Base64ToTexture(string base64) {
      Texture2D t = new Texture2D(1, 1);
      t.hideFlags = HideFlags.HideAndDontSave;
      t.LoadImage(System.Convert.FromBase64String(base64));
      return t;
    }
  }
}