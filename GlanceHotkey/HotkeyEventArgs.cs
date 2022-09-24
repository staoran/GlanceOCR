namespace GlanceHotkey
{
    public class HotkeyEventArgs : EventArgs
    {
        private readonly string _name;

        internal HotkeyEventArgs(string name)
        {
            _name = name;
        }

        public string Name => _name;

        public bool Handled { get; set; }
    }
}