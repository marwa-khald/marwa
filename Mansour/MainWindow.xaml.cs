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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.IO;

namespace Mansour
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker BackGroundProcess = new BackgroundWorker();
        FileInfo CurrentFile;
        bool FileChanged = false;
        
    WordInfo selectedWord;
    WordInfo selectedWord2;
        
        public MainWindow()
        {
            InitializeComponent();
            BackGroundProcess.DoWork += new DoWorkEventHandler(BackGroundProcess_DoWork);
            BackGroundProcess.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackGroundProcess_RunWorkerCompleted);
            BackGroundProcess.ProgressChanged += new ProgressChangedEventHandler(BackGroundProcess_ProgressChanged);

          
        }

        void BackGroundProcess_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress.Value = e.ProgressPercentage;
        }

        void BackGroundProcess_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            AnalysisDetails Details;
            // txtOut.Text = e.Result.ToString();
            ProcessingFinished(Analyzer.ArabicWords.Count - Analyzer.NumberOfSentences, Analyzer.NumberOfSentences, Analyzer.NotRecognizedWords, Analyzer.CorrectedWords);
            for (int i = 0; i < Analyzer.ArabicWords.Count - 1; i++)
            {
                selectedWord2 = Analyzer.AllWordsInfo[i][0];
                Details = new AnalysisDetails(selectedWord2);
                txtOut.Text += Details.txtWord.Text + " ";
            }
            string M2, M1;
            for (int i = 1; i < Analyzer.ArabicWords.Count - 1; i++)
            {
                M2 = Analyzer.AllWordsInfo[i][0].Meaning;
                M1 = Analyzer.AllWordsInfo[i - 1][0].Meaning;

                if (M1.StartsWith("V") && M2 == ("N222112"))
                {
                    MessageBox.Show(".يوجد خطأ نحوى فالجمله ",
                        "error Message");
                }

                if (M1.StartsWith("T1") && M2 == ("N223122"))
                {
                    MessageBox.Show(".يوجد خطأ نحوى فالجمله ",
                        "error Message");
                }
                       
                
            }
        }
        void BackGroundProcess_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgp = sender as BackgroundWorker;
            e.Result = Analyzer.AnalyzeText(e.Argument.ToString(), ref bgp);


        }

        void ProcessingFinished(int WordCount, int SentenceCount, int Fuzzy, int Corrected)
        {
            stsStatus.Text = "Status: processing done.";
            stsWords.Text = "words number: " + WordCount;
            stsSentences.Text = "sentence number: " + SentenceCount;
            stsFuzzy.Text = "Unexplained: " + Fuzzy;
            stsCorrections.Text = "Corrected: " + Corrected;
            stsPercent.Text = "correctness precentage: " + ((WordCount > 0) ? Corrected * 100 / (WordCount) : WordCount) + " %";
            btnStart.IsEnabled = true;
            Progress.Visibility = System.Windows.Visibility.Collapsed;
        }

        int SelectedWordIndex = -1;

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            txtOriginal.Text = Clipboard.GetText();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            txtOut.Text = "";
                if (Analyzer.CorrectedWords > 0 && MessageBox.Show(
                    "لقد قمت بإجراء تصحيحات على مخرجات المعالجة، هل ترغب بتذكر الكلمات المصححة تلقائيا في المستقبل ؟",
                    "تذكر التصحيحات", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading)
                    == MessageBoxResult.Yes)
                {
                    Analyzer.StoreCorrections();
                }

                if (Analyzer.NotRecognizedWords > 0)
                {
                    if (MessageBox.Show("توجد كلمات غير مفسرة في النص المعالج؛ هل ترغب بإضافة هذه الكلمات لذاكرة التعلم؟",
                        "إضافة كلمات أعجمية", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading)
                        == MessageBoxResult.Yes)
                    {
                        Analyzer.LearnNewWords();
                    }

                    btnDetails.IsEnabled = true;
            }

            btnStart.IsEnabled = false;
            Progress.Visibility = System.Windows.Visibility.Visible;
            stsStatus.Text = "state : processing run";
            Progress.Value = 0;
            BackGroundProcess.WorkerReportsProgress = true;
            BackGroundProcess.RunWorkerAsync(txtOriginal.Text);
        }

        private void btnRemoveDiacrits_Click(object sender, RoutedEventArgs e)
        {
           txtOriginal.Text = Tashkeel.Remove(txtOriginal.Text);
        }


        private void txtOut_SelectionChanged(object sender, RoutedEventArgs e)
        {
            btnDetails.IsEnabled = true;
            btnModify.IsEnabled = false;
            if (txtOut.SelectionLength == 0)
            {
                lstPossibilities.Items.Clear();
                SelectedWordIndex = -1;
                return;
            }
            int temp;
            List<ArabicWord> prevWords = Analyzer.ExtractArabicWords(txtOut.Text.Substring(0, txtOut.SelectionStart), out temp);
            SelectedWordIndex = (prevWords.Count > 0) ? prevWords.Count - 1 : 0;//عدد الكلمات السابقة باستثناء نهاية الجملة
            if (Analyzer.ArabicWords[SelectedWordIndex].word == "EOS") SelectedWordIndex++;
            lstPossibilities.Items.Clear();
            //إذا لم تكن الكلمة منتقاة بشكل صحيح لا تعدل
            if (SelectedWordIndex == Analyzer.ArabicWords.Count || Tashkeel.Remove(txtOut.SelectedText.Trim()) != Analyzer.ArabicWords[SelectedWordIndex].word) return;

            for (int i = 0; i < Analyzer.AllWordsInfo[SelectedWordIndex].Count; i++)
            {
                lstPossibilities.Items.Add(Analyzer.AllWordsInfo[SelectedWordIndex][i]);
            }
            txtAnalysis.Text = "Analysis: " + Interpreter.MeaningOf(Analyzer.AllWordsInfo[SelectedWordIndex][0].Meaning);
            btnModify.IsEnabled = true;
            btnDetails.IsEnabled = true;
        }

        private void btnModify_Click(object sender, RoutedEventArgs e)
        {
            if (lstPossibilities.SelectedItem == null) return;
            int CorrectionIndex = lstPossibilities.SelectedIndex;

            //إسقاط المسافات السابقة واللاحقة من التحديد
            if (txtOut.SelectedText.EndsWith(" "))
            {
                txtOut.SelectionLength--;
                lstPossibilities.SelectedIndex = CorrectionIndex;
            }
            if (txtOut.SelectedText.StartsWith(" "))
            {
                txtOut.SelectionStart++;
                txtOut.SelectionLength--;
                lstPossibilities.SelectedIndex = CorrectionIndex;
            }
            if (!Analyzer.ArabicWords[SelectedWordIndex].Corrected)
            {
                Analyzer.ArabicWords[SelectedWordIndex].Corrected = true;
                Analyzer.CorrectedWords++;
            }
            WordInfo winf = Analyzer.AllWordsInfo[SelectedWordIndex][0];
            Analyzer.AllWordsInfo[SelectedWordIndex][0] = Analyzer.AllWordsInfo[SelectedWordIndex][lstPossibilities.SelectedIndex];
            Analyzer.AllWordsInfo[SelectedWordIndex][lstPossibilities.SelectedIndex] = winf;
            stsCorrections.Text = "corrected : " + Analyzer.CorrectedWords;
            double CorrectionsPercent = 0;
            if (Analyzer.ArabicWords.Count > 0)
                CorrectionsPercent = Analyzer.CorrectedWords * 100.0 / (Analyzer.ArabicWords.Count - Analyzer.NumberOfSentences);
            stsPercent.Text = string.Format("correctness precentage: : {0:f2} %", CorrectionsPercent);
            txtOut.SelectedText = lstPossibilities.SelectedItem.ToString();
            //txtOut.Text = Analyzer.GenerateOutput();
            btnModify.IsEnabled = false;
            
        }


        private void mnuAddForeignWord_Click(object sender, RoutedEventArgs e)
        {
            ForeignWord fw = new ForeignWord();
            fw.ShowDialog();
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOut.Text);
        }

        private void btnDetails_Click(object sender, RoutedEventArgs e)
        {
            dataGrid1.Items.Clear();
            dataGrid1.Columns.Clear();
            AnalysisDetails Details ;
            
            
             

            DataGridTextColumn word = new DataGridTextColumn();
            word.Header = "Word";
            word.Binding = new Binding("word");
            word.Width = 100;
            dataGrid1.Columns.Add(word);
            DataGridTextColumn pattern = new DataGridTextColumn();
            pattern.Header = "Pattern";
            pattern.Width = 100;
            pattern.Binding = new Binding("pattern");
            dataGrid1.Columns.Add(pattern);
            DataGridTextColumn suffix = new DataGridTextColumn();
            suffix.Header = "Suffix";
            suffix.Width = 100;
            suffix.Binding = new Binding("suffix");
            dataGrid1.Columns.Add(suffix);
            DataGridTextColumn prefix = new DataGridTextColumn();
            prefix.Header = "Prefix";
            prefix.Width = 100;
            prefix.Binding = new Binding("prefix");
            dataGrid1.Columns.Add(prefix);
            DataGridTextColumn root = new DataGridTextColumn();
            root.Header = "Root";
            root.Width = 100;
            root.Binding = new Binding("root");
            dataGrid1.Columns.Add(root);
            DataGridTextColumn interpertation = new DataGridTextColumn();
            interpertation.Header = "Interpertation";
            interpertation.Width = 300;
            interpertation.Binding = new Binding("parsing");
            dataGrid1.Columns.Add(interpertation);
            DataGridTextColumn analysis = new DataGridTextColumn();
            analysis.Header = "Analysis";
            analysis.Width = 300;
            analysis.Binding = new Binding("analysis");
            dataGrid1.Columns.Add(analysis);

            string word1, suf1, prf1, root1, parse1, pattern1, analysis1;


            for (int i = 0; i < Analyzer.ArabicWords.Count-1; i++)
            {
                selectedWord = Analyzer.AllWordsInfo[i][0];

                Details = new AnalysisDetails(selectedWord);
                
                word1 = Details.txtWord.Text;
                suf1 = Details.txtSuffix.Text;
                prf1 = Details.txtPrefix.Text;
                root1 = Details.txtRoot.Text;
                analysis1 = Details.txtMeaning.Text;
                 parse1 = Details.txtInterpretation.Text;
                 
                 pattern1 = Details.txtTemplate.Text;

                 string M1 = Analyzer.AllWordsInfo[0][0].Meaning;
                 string M2 = Analyzer.AllWordsInfo[1][0].Meaning;

             
                 dataGrid1.Items.Add(new AnalysisItem(word1, suf1, prf1, root1, pattern1, parse1,analysis1));

                
            }
                   
        }



        private void lstPossibilities_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPossibilities.SelectedIndex >= 0)
            {
                txtAnalysis.Text = "Analysis: " + Interpreter.MeaningOf(((WordInfo)lstPossibilities.SelectedItem).Meaning);
            }
        }

        private void mnuAbout_Click(object sender, RoutedEventArgs e)
        {
            About ME = new About();
            ME.ShowDialog();
        }

        private void chkQuick_Unchecked(object sender, RoutedEventArgs e)
        {
            Analyzer.FastAnalysis = false;
        }

        private void chkQuick_Checked(object sender, RoutedEventArgs e)
        {
            Analyzer.FastAnalysis = true;
        }

        private void chkIgnore_Checked(object sender, RoutedEventArgs e)
        {
            Analyzer.IgnoreExistingDiacritics = true;
        }

        private void chkIgnore_Unchecked(object sender, RoutedEventArgs e)
        {
            Analyzer.IgnoreExistingDiacritics = false;
        }

        private void Command_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (FileChanged && ConfirmSave() == MessageBoxResult.Cancel) return;
            Microsoft.Win32.OpenFileDialog OpDlg = new Microsoft.Win32.OpenFileDialog();
            OpDlg.DefaultExt = ".txt";
            OpDlg.Filter = "ملف نصي (.txt)|*.txt";
            if (OpDlg.ShowDialog() == true)
            {
                CurrentFile = new FileInfo(OpDlg.FileName);
                StreamReader sr = new StreamReader(OpDlg.FileName);
                StringBuilder FileContent = new StringBuilder();
                while (!sr.EndOfStream)
                {
                    FileContent.Append(sr.ReadLine());
                    FileContent.Append("\r\n");
                }
                txtOriginal.Text = FileContent.ToString();
                FileChanged = false;
                sr.Close();

            }

        }

        private void Command_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Command_SaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveCurrentFile(true);
        }

        private void Command_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveCurrentFile(false);
        }

        private void Command_SaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Command_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (CurrentFile != null);
        }

        private void Command_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Command_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (FileChanged && ConfirmSave() == MessageBoxResult.Cancel) return;
            txtOriginal.Clear();
            txtOut.Clear();
            CurrentFile = null;
            FileChanged = false;
            Analyzer.AllWordsInfo = new List<List<WordInfo>>();
            Analyzer.ArabicWords = new List<ArabicWord>();
            stsStatus.Text = "State:Ready.";
            stsWords.Text = "Words number :0";
            stsSentences.Text = "Sentence number: 0";
            stsFuzzy.Text = "Unexplained: 0";
            stsCorrections.Text = "Corrected: 0";
            stsPercent.Text = "correctness precentage: 0 %";
        }

        private void SaveCurrentFile(bool New)
        {
            if (New || CurrentFile == null)
            {
                Microsoft.Win32.SaveFileDialog SvDlg = new Microsoft.Win32.SaveFileDialog();
                SvDlg.DefaultExt = ".txt";
                SvDlg.Filter = "File (.txt)|*.txt";
                SvDlg.ShowDialog();
                if (SvDlg.FileName == "") return;
                CurrentFile = new FileInfo(SvDlg.FileName);
            }
            StreamWriter file = new StreamWriter(CurrentFile.FullName, false);
            file.Write(txtOriginal.Text);
            file.Close();
        }

        private MessageBoxResult ConfirmSave()
        {
            MessageBoxResult Response = MessageBox.Show(
                "Do you want to save the changes?", "Save changes",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question, MessageBoxResult.Yes, MessageBoxOptions.RtlReading);
            if (Response == MessageBoxResult.Yes)
            {
                SaveCurrentFile(false);
            }
            return Response;
        }

        private void txtOriginal_TextChanged(object sender, TextChangedEventArgs e)
        {
            FileChanged = true;
        }

        private void mnuPasteInput_Click(object sender, RoutedEventArgs e)
        {
            txtOriginal.Text = Clipboard.GetText();
        }

        private void mnuCopyOutput_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtOut.Text);
        }

        private void mnuExportOutput_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog SvDlg = new Microsoft.Win32.SaveFileDialog();
            SvDlg.DefaultExt = ".txt";
            SvDlg.Filter = "File (.txt)|*.txt";
            SvDlg.ShowDialog();
            if (SvDlg.FileName == "") return;
            StreamWriter file = new StreamWriter(SvDlg.FileName, false);
            file.Write(txtOut.Text);
            file.Close();
        }

        private void Command_Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Command_Close_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (FileChanged)
            {
                if (ConfirmSave() != MessageBoxResult.Cancel)
                {
                    Analyzer.con.Close();
                }
                else e.Cancel = true;
            }
        }
        private void mnuOptions_Click(object sender, RoutedEventArgs e)
        {
            wndOptions Options = new wndOptions();
            Options.InputFontFamily = txtOriginal.FontFamily;
            Options.InputFontSize = txtOriginal.FontSize;
            Options.InputBackground = txtOriginal.Background;
            Options.InputTextColor = txtOriginal.Foreground;
            Options.OutputFontFamily = txtOut.FontFamily;
            Options.OutputFontSize = txtOut.FontSize;
            Options.OutputBackground = txtOut.Background;
            Options.OutputTextColor = txtOut.Foreground;
            if (Options.ShowDialog() == true)
            {
                txtOriginal.FontFamily = Options.InputFontFamily;
                txtOriginal.FontSize = Options.InputFontSize;
                txtOriginal.Background = Options.InputBackground;
                txtOriginal.Foreground = Options.InputTextColor;
                txtOut.FontFamily = Options.OutputFontFamily;
                txtOut.FontSize = Options.OutputFontSize;
                txtOut.Background = Options.OutputBackground;
                txtOut.Foreground = Options.OutputTextColor;
            }
        }

        private void Command_Heplp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://");
        }

        private void Command_Help_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        
        }
        class Token
        {
            private string data, delimeter;
            private string[] tokens;
            private int index;
            public Token(string strdata)
            {
                init(strdata, " ");
            }

            private void init(string strdata, string delim)
            {

                data = strdata;
                delimeter = delim;
                tokens = data.Split(delimeter.ToCharArray());
                index = 0;
            }

            public bool hasElements()
            {
                return (index < (tokens.Length));
            }

            public string nextElement()
            {
                if (index < tokens.Length)
                {
                    return tokens[index++];
                }
                else
                {
                    return "";
                }
            }
          
        }
    }
}
