using System;

namespace UI {
  public interface ISelectable {
    bool Selected { get; }
    event Action<ISelectable> Pressed;
  }
}