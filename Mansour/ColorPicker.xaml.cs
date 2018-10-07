using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Mansour
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : Window
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        public ColorPicker(Color InitialColor)
        {
            InitializeComponent();
            Red = InitialColor.R;
            Green = InitialColor.G;
            Blue = InitialColor.B;
        }

        private byte Red, Green, Blue;

        public Color PickedColor
        {
            get
            {
                return Color.FromRgb(Red, Green, Blue);
            }
        }

        private void txtRed_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtRed.Text.Length > 3 || !byte.TryParse(txtRed.Text, out Red))
            {
                txtRed.Text = ((SolidColorBrush)SelectedColor.Background).Color.R.ToString();
            }
            else
            {
                SelectedColor.Background = new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
            }
        }

        private void txtGreen_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtGreen.Text.Length > 3 || !byte.TryParse(txtGreen.Text, out Green))
            {
                txtGreen.Text = ((SolidColorBrush)SelectedColor.Background).Color.G.ToString();
            }
            else
            {
                SelectedColor.Background = new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
            }
        }

        private void txtBlue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtBlue.Text.Length > 3 || !byte.TryParse(txtBlue.Text, out Blue))
            {
                txtBlue.Text = ((SolidColorBrush)SelectedColor.Background).Color.G.ToString();
            }
            else
            {
                SelectedColor.Background = new SolidColorBrush(Color.FromRgb(Red, Green, Blue));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtRed.Text = Red.ToString();
            txtGreen.Text = Green.ToString();
            txtBlue.Text = Blue.ToString();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
