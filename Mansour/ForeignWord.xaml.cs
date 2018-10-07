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
using System.Data.OleDb;

namespace Mansour
{
    /// <summary>
    /// Interaction logic for ForeignWord.xaml
    /// </summary>
    public partial class ForeignWord : Window
    {
        public ForeignWord()
        {
            InitializeComponent();
        }

        private void btnAuto_Click(object sender, RoutedEventArgs e)
        {
            string Word = Tashkeel.Remove(txtWord.Text);
            txtDiacritics.Text = Tashkeel.Guess(Word);
            txtWord.Text = Tashkeel.SetTashkeel(Word, txtDiacritics.Text);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        byte WordClass
        {
            get
            {
                if (radName.IsChecked == true)
                {
                    return 1;
                }
                else if (radTime.IsChecked == true)
                {
                    return 2;
                }
                else if (radPlace.IsChecked == true)
                {
                    return 3;
                }
                else if (radTech.IsChecked == true)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
        }

        private void txtWord_LostFocus(object sender, RoutedEventArgs e)
        {
            StringBuilder TempText = new StringBuilder();
            string Diac = "َُِّ";
            for (int i = 0; i < txtWord.Text.Length - 1; i++)
            {
                if (Diac.Contains(txtWord.Text[i])) continue;

                switch (txtWord.Text[i + 1])
                {
                    case 'ا':
                    case 'ى':
                    case 'َ'://فتحة
                        TempText.Append('1');
                        break;
                    case 'و':
                    case 'ُ'://ضمة
                        TempText.Append('2');
                        break;
                    case 'ي':
                    case 'ِ'://كسرة
                        TempText.Append('3');
                        break;
                    case 'ْ'://سكون
                        TempText.Append('0');
                        break;
                    case 'ّ'://حركة مشددة
                        if (i + 2 < txtWord.Text.Length)
                        {
                            switch (txtWord.Text[i + 1])
                            {
                                case 'َ'://فتحة
                                    TempText.Append('4');
                                    break;
                                case 'ُ'://ضمة
                                    TempText.Append('5');
                                    break;
                                case 'ِ'://كسرة
                                    TempText.Append('6');
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        switch (txtWord.Text[i])
                        {
                            case 'ا':
                            case 'و':
                            case 'ي':
                                TempText.Append('7');
                                break;
                        }
                        break;
                }
            }
            txtDiacritics.Text = TempText.ToString();
        }

        private void txtDiacritics_LostFocus(object sender, RoutedEventArgs e)
        {
            txtWord.Text = Tashkeel.SetTashkeel(Tashkeel.Remove(txtWord.Text), txtDiacritics.Text);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtWord.Text.Length == 0)
            {
                MessageBox.Show("أدخل الكلمة المطلوب إضافتها في حقل الكلمة", "أدخل كلمة",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return;
            }
            if (!Tashkeel.CheckTashkeel(Tashkeel.Remove(txtWord.Text), txtDiacritics.Text))
            {
                MessageBox.Show("التشكيل المدخل غير متوافق مع حروف الكلمة، يرجى التأكد من التشكيل", "خطأ في التشكيل",
                    MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return;
            }
            Analyzer.con.Open();
            OleDbCommand com = new OleDbCommand();
            com.Connection = Analyzer.con;
            com.CommandText = "select count(word) from ArabizedWords where word = '" + txtWord.Text + "'";
            byte Result = byte.Parse(com.ExecuteScalar().ToString());
            if (Result > 0)
            {
                MessageBox.Show("الكلمة المدخلة موجودة بالفعل ضمن ذاكرة التعلم!", "كلمة موجودة",
                    MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK, MessageBoxOptions.RtlReading);
                return;
            }
            com.CommandText = string.Format("Insert into ArabizedWords values('{0}','{1}','{2}',{3})", Tashkeel.Remove(txtWord.Text), txtDiacritics.Text, WordMeaning, WordClass);
            com.ExecuteNonQuery();
            Analyzer.con.Close();
            DialogResult = true;
            Close();
        }


        private string WordMeaning
        {
            get
            {
                StringBuilder Meaning = new StringBuilder("N00");
                if (radSingle.IsChecked == true) Meaning.Append('1');
                else Meaning.Append('4');
                if (radMasc.IsChecked == true) Meaning.Append('1');
                else Meaning.Append('2');
                return Meaning.ToString();
            }
        }

        
    }
}
