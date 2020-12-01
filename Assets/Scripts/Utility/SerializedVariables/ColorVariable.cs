using UnityEngine;

namespace Utility {

  [CreateAssetMenu(menuName = "Serialized Variables/ColorVariable")]
  public class ColorVariable : ScriptableObject {
    [SerializeField] private Color color = Color.white;
    public Color Color => color;
  }
}