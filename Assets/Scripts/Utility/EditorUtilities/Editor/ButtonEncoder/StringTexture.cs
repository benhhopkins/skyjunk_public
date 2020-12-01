using UnityEngine;

namespace Utility {
  public class Base64StringTexture {
    private string textureString = "";
    protected Texture2D texture;

    public Base64StringTexture(string textureString) {
      this.textureString = textureString;
      texture = Base64ToTexture(textureString);
    }

    public Texture2D Texture {
      get {
        if (texture == null) {
          texture = Base64ToTexture(textureString);
        }
        return texture;
      }
    }

    private static Texture2D Base64ToTexture(string base64) {
      Texture2D t = new Texture2D(1, 1);
      t.hideFlags = HideFlags.HideAndDontSave;
      t.LoadImage(System.Convert.FromBase64String(base64));
      return t;
    }
  }

  public class StringImage : Base64StringTexture {
    public StringImage(string textureString) :
      base(textureString) { }

    public void Draw() {
      GUI.DrawTexture(new Rect(0, 0, texture.width, texture.height), texture);
    }

    public void Draw(Vector2 v) {
      GUI.DrawTexture(new Rect(v.x, v.y, texture.width, texture.height), texture);
    }

    public void Draw(Rect r) {
      GUI.DrawTexture(r, texture);
    }

    public void DrawAsBox() {
      GUILayout.Box(texture);
    }
  }
}