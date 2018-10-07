using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Mansour
{
    public partial class AllDetails : Form
    {
        WordInfo selectedWord;

        public AllDetails()
        {
            InitializeComponent();

            AnalysisDetails Details;



            int i = 0;
            if (i < Analyzer.ArabicWords.Count - 1)
            {
                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word1.Text = Details.txtWord.Text;
                suf1.Text = Details.txtSuffix.Text;
                pre1.Text = Details.txtPrefix.Text;
                root1.Text = Details.txtRoot.Text;
                parse1.Text = Details.txtInterpretation.Text;
                pat1.Text = Details.txtTemplate.Text;

                i++;
                string word = Tashkeel.Remove(word1.Text);
                if (word == "كان") parse1.Text = "فعل ماض ناقص مبني على الفتح";
                if (word == "إن") parse1.Text = "حرف توكيد ونصب";
                if (pre1.Text.StartsWith("و")) { parse1.Text += "معطوف "; }
                if( word.StartsWith("ت")) pre1.Text="ت";
            }

            if (i < Analyzer.ArabicWords.Count - 1)
            {
                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);
                word2.Text = Details.txtWord.Text;
                suf2.Text = Details.txtSuffix.Text;
                pre2.Text = Details.txtPrefix.Text;
                root2.Text = Details.txtRoot.Text;
                parse2.Text = Details.txtInterpretation.Text;
                pat2.Text = Details.txtTemplate.Text;

                i++;
            }
            if (i < Analyzer.ArabicWords.Count - 1)
            {
                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word3.Text = Details.txtWord.Text;
                suf3.Text = Details.txtSuffix.Text;
                pre3.Text = Details.txtPrefix.Text;
                root3.Text = Details.txtRoot.Text;
                parse3.Text = Details.txtInterpretation.Text;
                pat3.Text = Details.txtTemplate.Text;

                i++;
            }

            if (i < Analyzer.ArabicWords.Count - 1)
            {
                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word4.Text = Details.txtWord.Text;
                suf4.Text = Details.txtSuffix.Text;
                pre4.Text = Details.txtPrefix.Text;
                root4.Text = Details.txtRoot.Text;
                parse4.Text = Details.txtInterpretation.Text;
                pat4.Text = Details.txtTemplate.Text;

                i++;
            }
            if (i < Analyzer.ArabicWords.Count - 1)
            {

                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word5.Text = Details.txtWord.Text;
                suf5.Text = Details.txtSuffix.Text;
                pre5.Text = Details.txtPrefix.Text;
                root5.Text = Details.txtRoot.Text;
                parse5.Text = Details.txtInterpretation.Text;
                pat5.Text = Details.txtTemplate.Text;
            }
            if (i < Analyzer.ArabicWords.Count - 1)
            {

                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word6.Text = Details.txtWord.Text;
                suf6.Text = Details.txtSuffix.Text;
                pre6.Text = Details.txtPrefix.Text;
                root6.Text = Details.txtRoot.Text;
                parse6.Text = Details.txtInterpretation.Text;
                pat6.Text = Details.txtTemplate.Text;
            }
            if (i < Analyzer.ArabicWords.Count - 1)
            {

                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word7.Text = Details.txtWord.Text;
                suf7.Text = Details.txtSuffix.Text;
                pre7.Text = Details.txtPrefix.Text;
                root7.Text = Details.txtRoot.Text;
                parse7.Text = Details.txtInterpretation.Text;
                pat7.Text = Details.txtTemplate.Text;
            }
            if (i < Analyzer.ArabicWords.Count - 1)
            {

                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);

                word8.Text = Details.txtWord.Text;
                suf8.Text = Details.txtSuffix.Text;
                pre8.Text = Details.txtPrefix.Text;
                root8.Text = Details.txtRoot.Text;
                parse8.Text = Details.txtInterpretation.Text;
                pat8.Text = Details.txtTemplate.Text;
            }
            string M1 = Analyzer.AllWordsInfo[0][0].Meaning;
            string M2 = Analyzer.AllWordsInfo[1][0].Meaning;

            if (M1.StartsWith("V") && M2 == ("N222112"))
                word2.BackColor = Color.Red;
            if (M1.StartsWith("V2")) parse1.Text = "فعل مضارع مرفوع بالضمه";
            if (M1.StartsWith("N") && M2.StartsWith("V"))
               parse2.Text = "جمله فعليه فى محل رفع خبر المبتدأ";
            
        }
    }
}
