using System;
using System.Windows;
using System.Windows.Input;

namespace Trigger250
{
    public partial class HotkeyCaptureWindow : Window
    {
        public event Action<uint, uint> HotkeyCaptured;

        public HotkeyCaptureWindow()
        {
            InitializeComponent();
            this.KeyDown += HotkeyCaptureWindow_KeyDown;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.Title = "Press your hotkey combination";
            this.Topmost = true;
            this.Show();
        }

        // Make sure to fully qualify KeyEventArgs with the correct namespace
        private void HotkeyCaptureWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            uint modifiers = 0;

            // Check if Control key is pressed
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                modifiers |= User32.MOD_CTRL;
            }

            // Check if Alt key is pressed
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                modifiers |= User32.MOD_ALT;
            }

            // Get the pressed key (if it's not a modifier key)
            if (e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl && e.Key != Key.LeftAlt && e.Key != Key.RightAlt)
            {
                uint key = (uint)KeyInterop.VirtualKeyFromKey(e.Key);

                // Fire the event with modifiers and key
                HotkeyCaptured?.Invoke(modifiers, key);
                this.Close(); // Close the capture window when a hotkey is captured
            }
        }
    }
}