using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Trigger250
{
    /// <summary>
    /// Interaction logic for KeyBoardWedge.xaml
    /// </summary>
    public partial class KeyBoardWedge : Window
    {
        public KeyBoardWedge()
        {
            InitializeComponent();
        }

        private void Preset_Checked(object sender, RoutedEventArgs e)
        {
            // Check which radio button was selected
            System.Windows.Controls.RadioButton selectedRadioButton = sender as System.Windows.Controls.RadioButton;

            if (selectedRadioButton != null)
            {
                string selectedOption = selectedRadioButton.Content.ToString();
                switch (selectedOption)
                {
                    case "SAP":
                        StringBox.Text = "[WT];[TAB];[L];[TAB];[W];[TAB];[H];[CR]";
                        DisableKeyboard();
                        break;
                    case "KBW-U":
                        StringBox.Text = "[WT];[TAB];[TAB];[L];[TAB];[W];[TAB];[H];[TAB]";
                        DisableKeyboard();
                        break;
                    case "KBW-F":
                        StringBox.Text = "[L];[TAB];[W];[TAB];[H];[CR]";
                        DisableKeyboard();
                        break;
                    case "Custom":
                        StringBox.Text = "";
                        EnableKeyboard();
                        break;
                    default:
                        break;
                }
            }
        }

        private void EnableKeyboard()
        {
            LengthButton.IsEnabled = true;
            WidthButton.IsEnabled = true;
            HeightButton.IsEnabled = true;
            WeightButton.IsEnabled = true;
            BarcodeButton.IsEnabled = true;
            TabButton.IsEnabled = true;
            SpaceButton.IsEnabled = true;
            CarriageReturnButton.IsEnabled = true;
            LineFeedButton.IsEnabled = true;
            SemiColonButton.IsEnabled = true;
            CommaButton.IsEnabled = true;
            XButton.IsEnabled = true;
        }

        private void DisableKeyboard()
        {
            LengthButton.IsEnabled = false;
            WidthButton.IsEnabled = false;
            HeightButton.IsEnabled = false;
            WeightButton.IsEnabled = false;
            BarcodeButton.IsEnabled = false;
            TabButton.IsEnabled = false;
            SpaceButton.IsEnabled = false;
            CarriageReturnButton.IsEnabled = false;
            LineFeedButton.IsEnabled = false;
            SemiColonButton.IsEnabled = false;
            CommaButton.IsEnabled = false;
            XButton.IsEnabled = false;
        }

        private void LengthButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[L]";
        }

        private void WidthButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[W]";
        }

        private void HeightButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[H]";
        }

        private void WeightButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[WT]";
        }

        private void TabButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[TAB]";
        }

        private void CRButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[CR]";
        }

        private void LFButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[LF]";
        }

        private void BarButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[BAR]";
        }

        private void SPButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[SP]";
        }

        private void SemiButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[;]";
        }

        private void ComaButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[,]";
        }

        private void XButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = StringBox.Text + "[X]";
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            StringBox.Text = "";
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public string KeyboardWedgeString(string data)
        {
            //remove serial response string
            string wedgeString = data.Substring(10);
            //separate string to several fields
            string[] separatedString= wedgeString.Split(' ');
            //return processed data
            return separatedString[0];
            //return wedgeString;
        }
    }
}
