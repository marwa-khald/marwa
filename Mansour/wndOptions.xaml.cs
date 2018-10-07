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
    /// Interaction logic for wndOptions.xaml
    /// </summary>
    public partial class wndOptions : Window
    {
        public wndOptions()
        {
            InitializeComponent();
        }
        public FontFamily InputFontFamily
        {
            get
            {
                return (FontFamily)cmbInputFonts.SelectedItem;
            }
            set
            {
                cmbInputFonts.SelectedItem = value;
            }
        }
        public FontFamily OutputFontFamily
        {
            get
            {
                return (FontFamily)cmbOutputFonts.SelectedItem;
            }
            set
            {
                cmbOutputFonts.SelectedItem = value;
            }
        }
        public double InputFontSize
        {
            get
            {
                return double.Parse(cmbInputFontSize.Text);
            }
            set
            {
                cmbInputFontSize.Text = value.ToString();
            }
        }
        public double OutputFontSize
        {
            get { return double.Parse(cmbOutputFontSize.Text); }
            set { cmbOutputFontSize.Text = value.ToString(); }
        }
        public Brush InputTextColor
        {
            get
            {
                return btnInputColour.Background;
            }
            set
            {
                btnInputColour.Background = value;
            }
        }
        public Brush OutputTextColor
        {
            get
            {
                return btnOutputColour.Background;
            }
            set
            {
                btnOutputColour.Background = value;
            }
        }
        public Brush InputBackground
        {
            get
            {
                return btnInputBackground.Background;
            }
            set
            {
                btnInputBackground.Background = value;
            }
        }
        public Brush OutputBackground
        {
            get
            {
                return btnOutputBackground.Background;
            }
            set
            {
                btnOutputBackground.Background = value;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (FontFamily Font in Fonts.SystemFontFamilies)
            {
                cmbInputFonts.Items.Add(Font);
                cmbOutputFonts.Items.Add(Font);
            }
            
        }

        private void btnInputColour_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker NewColor = new ColorPicker(((SolidColorBrush)btnInputColour.Background).Color);
            if (NewColor.ShowDialog() == true)
            {
                btnInputColour.Background = new SolidColorBrush(NewColor.PickedColor);
            }
        }

        private void btnInputBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker NewColor = new ColorPicker(((SolidColorBrush)btnInputBackground.Background).Color);
            if (NewColor.ShowDialog() == true)
            {
                btnInputBackground.Background = new SolidColorBrush(NewColor.PickedColor);
            }
        }

        private void btnOutputColour_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker NewColor = new ColorPicker(((SolidColorBrush)btnOutputColour.Background).Color);
            if (NewColor.ShowDialog() == true)
            {
                btnOutputColour.Background = new SolidColorBrush(NewColor.PickedColor);
            }
        }

        private void btnOutputBackground_Click(object sender, RoutedEventArgs e)
        {
            ColorPicker NewColor = new ColorPicker(((SolidColorBrush)btnOutputBackground.Background).Color);
            if (NewColor.ShowDialog() == true)
            {
                btnOutputBackground.Background = new SolidColorBrush(NewColor.PickedColor);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
