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
    /// Interaction logic for AnalysisDetails.xaml
    /// </summary>
    public partial class AnalysisDetails : Window
    {
        public AnalysisDetails()
        {
            InitializeComponent();
        }
        public string M1;
        public AnalysisDetails(WordInfo word)
        {
            InitializeComponent();
            txtWord.Text = word.ToString();
            txtTemplate.Text = word.Template;
            txtRoot.Text = word.Root.Root;
            
            txtRootCompatibility.Text = (word.Root.IsCompatible) ? "متوافق" : "غير مؤكد";
            txtDerivative.Text = word.Root.DerivationType.ToString().Replace('_', ' ');
            txtPrefix.Text = word.Prefix.Text;
            if (word.Prefix.Meaning.Length > 0)
            {
                txtPrefix.Text += "؛ " + Interpreter.MeaningOf(word.Prefix.Meaning);
            }
            txtSuffix.Text = word.Suffix.Text;
            if (word.Suffix.Meaning.Length > 0)
            {
                txtSuffix.Text += "؛ " + Interpreter.MeaningOf(word.Suffix.Meaning);
            }
            txtMeaning.Text = Interpreter.MeaningOf(word.Meaning) + " " + Interpreter.MeaningOf(word.SpecialClass);
            if (word.Word == "الله" && word.Meaning == "N211")
            {
                txtMeaning.Text += "(لفظ الجلالة الله)";
            }

            if (word.Interpretations != null && word.Interpretations.Count > 0)
            {
                txtInterpretation.Text = word.Interpretations[0].Description;
            }
            string M2 ;

             M1=word.Meaning;
            if (M1.StartsWith("V2")) txtInterpretation.Text = "فعل مضارع مرفوع بالضمه";
              
                 string word2 = Tashkeel.Remove(word.Word);
                 if (word2 == "كان") txtInterpretation.Text = "فعل ماض ناقص مبني على الفتح";
                 if (word2 == "إن") txtInterpretation.Text = ".حرف توكيد ونصب، مبني على الفتح ، لا محل له من الإعراب ";
                 if (txtPrefix.Text.StartsWith("و")) { txtInterpretation.Text += "معطوف "; }
                 if (word2.StartsWith("ت")) txtPrefix.Text= "ت";

                 if ((word2 == "من")||(word2 == "على")||(word2 == "الى")||(word2 == "إلى")||(word2 == "فى")||(word2 == "حتى")||(word2 == "عدا")||
                     (word2 == "الى") || (word2 == "لعل") ||(word2 == "متى") ||(word2 == "كى") ||(word2 == "منذ"))
                     txtInterpretation.Text = "حرف جر";
                 if ((word2 == "لم") || (word2 == "لما") || (txtPrefix.Text.StartsWith("ل")) || (word2 == "لا") || (word.Word == "إنْ"))
                 { txtInterpretation.Text = " جازمة ، حرف، مبني على السكون، لا محل له من الإعراب";
                 
                 }

                 if (txtPrefix.Text.StartsWith("ب") || txtPrefix.Text.StartsWith("ك") || txtPrefix.Text.StartsWith("ف")||(word2 == "الى"))
                     txtInterpretation.Text = "اسم مجرور بالكسره";

                

                for (int i = 1; i < Analyzer.ArabicWords.Count - 1; i++)
                 {
                     M2 = Analyzer.AllWordsInfo[i][0].Word;
                     if (M2 == word.Word&& M2!="كان")
                     {
                         M1 = Analyzer.AllWordsInfo[i - 1][0].Meaning; M2 = word.Meaning;
                         if (M1.StartsWith("N") && M2.StartsWith("V"))
                             txtInterpretation.Text = "جمله فعليه فى محل رفع خبر المبتدأ";
                     }

                     M2 = Analyzer.AllWordsInfo[i][0].Word;
                     if (M2 == word.Word)
                     {
                         M1 = Analyzer.AllWordsInfo[i - 1][0].Meaning; M2 = word.Meaning;
                         if (M1.StartsWith("N1") && M2.StartsWith("N2"))
                             txtInterpretation.Text = "مضاف اليه مجرور بالكسرة";
                     }

                     M2 = Analyzer.AllWordsInfo[i][0].Word;
                     if (M2 == word.Word)
                     {
                         M1 = Analyzer.AllWordsInfo[i - 1][0].Meaning; M2 = word.Meaning;
                         if (M1.StartsWith("T1") && M2.StartsWith("N"))
                             txtInterpretation.Text = "إسم مجرور بالكسرة";
                     }

                    M2 = Analyzer.AllWordsInfo[i][0].Word;
                     if (M2 == word.Word)
                     {
                         M1 = Analyzer.AllWordsInfo[i - 1][0].Meaning ;   M2 = word.Meaning;
                         if (M1 == "V113111" && M2.StartsWith("N"))
                             txtInterpretation.Text = "إسم" +" "+ Tashkeel.Remove(Analyzer.AllWordsInfo[i - 1][0].Word )+" "+ "مرفوع بالضمة ";
                     }

                     M2 = Analyzer.AllWordsInfo[i][0].Word;
                     if (M2 == word.Word && i>1)
                     {
                         M1 = Analyzer.AllWordsInfo[i - 2][0].Meaning; M2 = word.Meaning;
                         if (M1 == "V113111" && M2.StartsWith("N"))
                             txtInterpretation.Text = "خبر" + " " + Tashkeel.Remove(Analyzer.AllWordsInfo[i - 2][0].Word) + " " + "منصوب بالفتحة ";
                     }
                   


                     M2 = Analyzer.AllWordsInfo[i][0].Word;
                     if (M2 == word.Word)
                     {
                         M1 = Analyzer.AllWordsInfo[i - 1][0].Meaning  ; M2 = word.Meaning;

                         if (M1 == "T3" && M2.StartsWith("V2"))
                         {
                             txtInterpretation.Text = " .فعل مضارع مجزوم وعلامة جزمه السكون الظاهرة على آخره ";
                             
                                 txtWord.Text = txtWord.Text.Replace(txtWord.Text.Substring(txtWord.Text.Length - 1), "ْ");

                                 if (txtWord.Text.EndsWith("نْ") && txtWord.Text.Length>6)
                                 {
                                     txtWord.Text = txtWord.Text.Substring(0, txtWord.Text.Length - 2);
                                     txtInterpretation.Text = " .فعل مضارع مجزوم وعلامة جزمه حذف النون لأنه من الأفعال الخمسة والألف في محل رفع فاعل ";
                                 }

                                 if ( txtWord.Text.EndsWith("يْ"))
                                 {
                                     txtWord.Text = txtWord.Text.Substring(0, txtWord.Text.Length - 2);
                                     txtInterpretation.Text = " . فعل مضارع مجزوم وعلامة جزمه حذف حرف العلة من آخره. ";
                                 }
                             
                         }
                     }

                    
                 }

                if (txtInterpretation.Text.Contains("منصوب") && !txtWord.Text.EndsWith("َ") && !txtWord.Text.EndsWith("ً") && !word2.EndsWith("ا") && !word2.EndsWith("و") && !word2.EndsWith("ى") && !word2.EndsWith("ه"))
                    txtWord.Text = txtWord.Text.Replace(txtWord.Text.Substring(txtWord.Text.Length - 1), "َ");

                if (txtInterpretation.Text.Contains("مرفوع") && !txtWord.Text.EndsWith("ُ") && !txtWord.Text.EndsWith("ٌ") && !word2.EndsWith("ا") && !word2.EndsWith("و") && !word2.EndsWith("ى") && !word2.EndsWith("ه"))
                    txtWord.Text = txtWord.Text.Replace(txtWord.Text.Substring(txtWord.Text.Length - 1), "ُ");

                if (txtInterpretation.Text.Contains("مجرور") && !txtWord.Text.EndsWith("ِ") && !txtWord.Text.EndsWith("ٍ") && !word2.EndsWith("ا") && !word2.EndsWith("ى") && !word2.EndsWith("و") && !word2.EndsWith("ه"))
                    txtWord.Text = txtWord.Text.Replace(txtWord.Text.Substring(txtWord.Text.Length - 1), "ِ");

               //txtInterpretation.Text = word.Meaning;

        }
    }
}
