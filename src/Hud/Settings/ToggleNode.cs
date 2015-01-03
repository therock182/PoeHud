namespace PoeHUD.Hud.Settings
{
    public sealed class ToggleNode
    {
        public ToggleNode() {}

        public ToggleNode(bool value)
        {
            Value = value;
        }

        public bool Value { get; set; }

        public static implicit operator bool(ToggleNode node)
        {
            return node.Value;
        }

        public static implicit operator ToggleNode(bool value)
        {
            return new ToggleNode(value);
        }
    }
}