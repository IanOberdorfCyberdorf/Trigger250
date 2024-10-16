using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Forms;
using WindowsInput;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO.Ports;

namespace Trigger250
{
    public partial class MainWindow : Window
    {
        private string com = "COM8";
        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 9000;
        private uint _currentModifiers;
        private uint _currentKey;
        private NotifyIconWrapper _trayIcon;
        private Logger _logger;
        private SerialCom _serialCom;
        private KeyBoardWedge _keyboardWedge;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            // Initialize the logger
            _logger = new Logger("app.log");

            // Initialize the tray icon
            _trayIcon = new NotifyIconWrapper();
            _trayIcon.OnExit += Exit_Click;
            _trayIcon.OnConfigure += Configure_Click;

            this.Visibility = Visibility.Hidden; // Hide the main window
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Main window Loaded");
            LoadHotkeySettings(); // Load hotkey settings first
            RegisterHotKeys(); // Register hotkeys after loading settings
            InitializeSerialCommunication();
            _trayIcon.Show(); // Show tray icon
        }

        private void LoadHotkeySettings()
        {
            try
            {
                var settings = File.ReadAllText("hotkeySettings.json");
                var hotkey = JsonConvert.DeserializeObject<Hotkey>(settings);
                _currentModifiers = hotkey.Modifiers;
                _currentKey = hotkey.Key;

                // Update the current hotkey label
                UpdateHotkeyLabel(hotkey);
            }
            catch (Exception ex)
            {
                _logger.Log($"Failed to load hotkey settings: {ex.Message}");
                // Use default if settings don't exist
                _currentModifiers = User32.MOD_CTRL | User32.MOD_ALT;
                _currentKey = (uint)Keys.D; // Default to Ctrl + Alt + D
            }
        }

        private void RegisterHotKeys()
        {
            var handle = new WindowInteropHelper(this).Handle;
            UnregisterHotKeys(); // Unregister any existing hotkeys

            // Attempt to register the hotkey
            bool isRegistered = User32.RegisterHotKey(handle, HOTKEY_ID, _currentModifiers, _currentKey);

            if (isRegistered)
            {
                _logger.Log($"Hotkey registered successfully: Modifiers: {_currentModifiers}, Key: {((Keys)_currentKey)}");
                Debug.WriteLine($"Hotkey registered successfully: Modifiers: {_currentModifiers}, Key: {((Keys)_currentKey)}");
            }
            else
            {
                _logger.Log($"Failed to register hotkey: Modifiers: {_currentModifiers}, Key: {((Keys)_currentKey)}");
                Debug.WriteLine($"Failed to register hotkey: Modifiers: {_currentModifiers}, Key: {((Keys)_currentKey)}");
            }
        }

        private void InitializeSerialCommunication()
        {
            _serialCom = new SerialCom(com); // Initialize SerialCom instance

            try
            {
                // Before opening, check if the port is already in use
                var portNames = SerialPort.GetPortNames();
                if (!portNames.Contains(com))
                {
                    Debug.WriteLine(com + " is not available.");
                    return;
                }

                _serialCom.Open(); // Open the serial port
                Debug.WriteLine("Serial port opened successfully");
                _serialCom.Close();
            }
            catch (UnauthorizedAccessException)
            {
                Debug.WriteLine("Access denied to the COM port.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SerialCom_DataReceived(object sender, DataReceivedEventArgs e)
        {
            // Use Dispatcher to update the UI
            Dispatcher.Invoke(() =>
            {
                // For example, update a Label or TextBox
                //receivedDataLabel.Content = e.Data; // Assuming you have a Label named receivedDataLabel
                Debug.WriteLine("Serial Data: " + e.Data);
            });
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var source = PresentationSource.FromVisual(this) as HwndSource;
            if (source != null)
            {
                source.AddHook(WndProc);
            }
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
            {
                Debug.WriteLine("Combo pressed");
                string data = null;
                bool waiting = true;

                try
                {
                    _keyboardWedge = new KeyBoardWedge();
                    _serialCom.Open();
                    _serialCom.SendCommand("S");
                    while(waiting)
                    {
                        data = _serialCom.Get();
                        if (data != null)
                        {
                            waiting = false;
                        }
                    }
                    _serialCom.Close();
                    string wedge = _keyboardWedge.KeyboardWedgeString(data);
                    SendKeysToFocusedApp(wedge);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                

                handled = true;
            }
            return IntPtr.Zero;
        }

        private string convertData(string data)
        {
            string keyWedge = "Here's your processed string";
            return keyWedge;
        }

        private void SendKeysToFocusedApp(string keys)
        {
            var inputSimulator = new InputSimulator();

            foreach (char c in keys)
            {
                inputSimulator.Keyboard.TextEntry(c);
                System.Threading.Thread.Sleep(10); // Delay between keystrokes
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true; // Cancel the closing event
            this.Visibility = Visibility.Hidden; // Hide the window
        }

        private void UpdateHotkeyLabel(Hotkey hotkey)
        {
            string modifiers = string.Empty;

            if ((hotkey.Modifiers & User32.MOD_CTRL) != 0)
                modifiers += "Ctrl + ";
            if ((hotkey.Modifiers & User32.MOD_ALT) != 0)
                modifiers += "Alt + ";

            string keyName = ((Keys)hotkey.Key).ToString();

            hotkeyLabel.Content = $"Current Hotkey: {modifiers}{keyName}";
        }

        private void UnregisterHotKeys()
        {
            var handle = new WindowInteropHelper(this).Handle;
            User32.UnregisterHotKey(handle, HOTKEY_ID);
        }

        private void Configure_Click(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown(); // Explicitly specify WPF Application
        }

        private async void Test_Click(object sender, EventArgs e)
        {
            //string response = await SendCommandAndWaitForResponse("I2");
            //if (_serialCom.Test_Result(response))
            //{
            //    System.Windows.MessageBox.Show("TLD Connected");
            //};
        }

        private void Hotkey_Captured(uint newModifiers, uint newKey)
        {
            // Unregister the old hotkey
            UnregisterHotKeys();

            // Save the new hotkey settings
            var hotkey = new Hotkey { Modifiers = newModifiers, Key = newKey };
            File.WriteAllText("hotkeySettings.json", JsonConvert.SerializeObject(hotkey));

            // Register the new hotkey
            RegisterHotKeys();

            UpdateHotkeyLabel(hotkey); // Update the label with the new hotkey

            _logger.Log($"New hotkey registered: Modifiers: {newModifiers}, Key: {((Keys)newKey)}");
            Debug.WriteLine($"New hotkey registered: Modifiers: {newModifiers}, Key: {((Keys)newKey)}");
        }

        private class Hotkey
        {
            public uint Modifiers { get; set; }
            public uint Key { get; set; }
        }

        private void ChangeComboButton_Click(object sender, EventArgs e)

        {

            var captureWindow = new HotkeyCaptureWindow();

            captureWindow.HotkeyCaptured += Hotkey_Captured;

            captureWindow.Show(); // Show the capture window

        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            KeyBoardWedge keyBoardWedge = new KeyBoardWedge();
            keyBoardWedge.Show();
        }

        private void ShowWindow(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal; // Restore the window if it was minimized
        }

        private void ExitApplication(object sender, EventArgs e)
        {
            _trayIcon.Dispose(); // Clean up the tray icon
            _serialCom.Dispose();
            System.Windows.Application.Current.Shutdown(); // Close the application
        }

        protected override void OnClosed(EventArgs e)
        {
            // Clean up the NotifyIcon
            _trayIcon.Dispose();
            base.OnClosed(e);
        }

        //private async Task<string> SendCommandAndWaitForResponse(string command)
        //{
        //    //if (_serialCom == null)
        //    //{
        //    //    throw new InvalidOperationException("SerialCom is not initialized.");
        //    //}

        //    var tcs = new TaskCompletionSource<string>();

        //    // Event handler for data received
        //    EventHandler<DataReceivedEventArgs> dataReceivedHandler = null;

        //    dataReceivedHandler = (sender, e) =>
        //    {
        //        // Set the result in the TaskCompletionSource when data is received
        //        tcs.SetResult(e.Data);

        //        // Unsubscribe from the event
        //        _serialCom.DataReceived -= dataReceivedHandler;
        //    };

        //    // Subscribe to the DataReceived event
        //    _serialCom.DataReceived += dataReceivedHandler;

        //    try
        //    {
        //        _serialCom.Open();

        //        // Send the command to the serial device
        //        _serialCom.SendCommand(command);

        //        // Wait for the response
        //        return await tcs.Task;

        //        _serialCom.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.ToString();
        //    }
            
        //}
    }

    public class NotifyIconWrapper : IDisposable
    {
        private readonly NotifyIcon _notifyIcon;

        public event EventHandler OnConfigure;
        public event EventHandler OnExit;

        public NotifyIconWrapper()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
            };

            // Create a context menu and assign it to NotifyIcon
            var contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Configure", null, (s, e) => OnConfigure?.Invoke(this, EventArgs.Empty));
            contextMenu.Items.Add("Exit", null, (s, e) => OnExit?.Invoke(this, EventArgs.Empty));

            _notifyIcon.ContextMenuStrip = contextMenu; // Set the context menu here
        }

        public void Show()
        {
            _notifyIcon.Visible = true;
        }

        public void Dispose()
        {
            _notifyIcon.Dispose();
        }
    }

    public static class User32
    {
        public const uint MOD_ALT = 0x0001; // Alt key
        public const uint MOD_CTRL = 0x0002; // Control key

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}