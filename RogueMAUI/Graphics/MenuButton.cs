using SkiaSharp;

namespace RogueMAUI.Graphics;

public class MenuButton
{
    public string Text { get; set; }
    public Action OnClick { get; set; }
    public SKRect Bounds { get; set; }

    public MenuButton(string text, Action onClick)
    {
        Text = text;
        OnClick = onClick;
    }

    public bool Contains(float x, float y) => Bounds.Contains(x, y);
}