using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;
using System.ComponentModel;

namespace Mansour
{
    class Analyzer
    {
        public static OleDbConnection con = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='Mansour.mdb'");

        static private int Corrections;
        static private int Sentences;
        static private int NotRecognized = 0;

        public static int NotRecognizedWords
        {
            get { return NotRecognized; }
        }
        public static int NumberOfSentences
        {
            get
            {
                return Sentences;
            }
        }
        static public int CorrectedWords
        {
            get { return Corrections; }
            set { Corrections = value; }
        }
        static string TextToParse = "";
        static public bool FastAnalysis { get; set; }
        static public bool IgnoreExistingDiacritics { get; set; }
        public static List<List<WordInfo>> AllWordsInfo;//قائمة معلومات الكلمات
        public static List<ArabicWord> ArabicWords; //قائمة بالكلمات العربية ضمن النص المعالج
        //public static List<List<Interpretation>> AllWordsInterpretations = new List<List<Interpretation>>();

        public static void LoadInterpreterData()
        {
            Interpretation.InitializeInterpretations();
            GrammarRelation.InitializeRules();
        }

        public static string AnalyzeText(string TextToParse, ref BackgroundWorker BackGroundProcess)
        {
            //TextToParse = Tashkeel.Remove(TextToParse);
            Analyzer.TextToParse = TextToParse;
            Corrections = 0;
            NotRecognized = 0;
            ArabicWords = ExtractArabicWords(TextToParse, out Sentences);
            //للاحتفاظ باحتمالات النطق المختلفة لكل كلمة لأغراض التعديل
            AllWordsInfo = new List<List<WordInfo>>();
            con.Open();
            LoadInterpreterData();
            for (int i = 0; i < ArabicWords.Count; i++) //التحليل الصرفي
            {
                if (ArabicWords[i].word == "EOS")//نهاية جملة
                {
                    List<WordInfo> EOS = new List<WordInfo>();
                    EOS.Add(new WordInfo { Word = "EOS" });
                    AllWordsInfo.Add(EOS);
                    continue;
                }
                List<WordInfo> NewWordInfo = ProcessWord(ArabicWords[i]); //ابدأ معالجة الكلمة الجديدة
                AllWordsInfo.Add(NewWordInfo); //أضف ناتج المعالجة لمعلومات الكلمات
                if (NewWordInfo.Count == 0) //إذا لم يجد تفسير للكلمة
                {
                    NewWordInfo = Morphology.LookForForignWord(ArabicWords[i].word, NewWordInfo);//ابحث في الكلمات الأعجمية المخزنة
                    if (NewWordInfo.Count == 0)
                    {
                        //فشل تفسير الكلمة والتشكيل بناء على تتابع الحروف -غالبا للكلمات الأعجمية
                        //هذه الخوارزمية تحتاج تحسين
                        //للتعامل مع الكلمات الأعجمية المضاف لها ألف ولام أو ياء نسب
                        WordInfo NewWord = new WordInfo();
                        NewWord.Word = ArabicWords[i].word;
                        NewWord.Diacritics = Tashkeel.Guess(ArabicWords[i].word);
                        NewWord.Meaning = "N";
                        NewWordInfo.Add(NewWord);
                        NotRecognized += 1;
                    }
                }
                BackGroundProcess.ReportProgress((i + 1) * 100 / ArabicWords.Count);//تحديث شريط التقدم
            }

            for (int i = 0; i < ArabicWords.Count; i++)
            {
                int Longest = 0, Index = 0;
                List<GrammarRelation> PossibleRelations = Interpreter.StartInterpreting(i, "", 1, false);
                for (int PR = 0; PR < PossibleRelations.Count; PR++)
                {
                    if (Longest < PossibleRelations[PR].WordsCovered)
                    {
                        Longest = PossibleRelations[PR].WordsCovered;
                        Index = PR;
                    }
                    //Interpreter.ApplyGrammarRelation(PossibleRelations[PR]);
                }
                i += Longest;
                if (PossibleRelations.Count > 0)
                {
                    GrammarRelation.ApplyGrammarRelation(PossibleRelations[Index]);
                    PossibleRelations[Index].ActivateRelation();
                }
            }
            con.Close();
            return GenerateOutput();
        }

        

        public static string GenerateOutput()
        {
            StringBuilder OutPutText = new StringBuilder(); ;
            WordInfo WordToAdd;
            int LastAdded = -1;
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                if (ArabicWords[i].word == "EOS") continue;
                WordToAdd = AllWordsInfo[i][0];
                OutPutText.Append(TextToParse.Substring(LastAdded + 1, ArabicWords[i].Start - (LastAdded + 1)));
                OutPutText.Append(WordToAdd.ToString());
                LastAdded = ArabicWords[i].End; //آخر حرف أضيف هو آخر حروف الكلمة 
                //إضافة الحروف غير العربية التالية إلى المخرجات كما هي
                if (i + 1 == ArabicWords.Count && LastAdded + 1 < TextToParse.Length)
                {
                    OutPutText.Append(TextToParse.Substring(LastAdded + 1));
                }
            }
            
            return OutPutText.ToString();
        }

        //استخراج الكلمات العربية المحتوية على الأحرف العربية المعتادة
        public static List<ArabicWord> ExtractArabicWords(string EntireText, out int NumberOfSentences)
        {
            NumberOfSentences = 0;
            int WordStartAt = -1;
            int WordEndAt = -1;
            int Unicode;
            List<ArabicWord> ArabicWords = new List<ArabicWord>();
            for (int i = 0; i < EntireText.Length; i++)
            {
                Unicode = EntireText[i];
                if (Unicode >= 1569 && Unicode <= 1618) //إذا كان الحرف من الحروف العربية المعتادة
                {
                    if (WordStartAt == -1) WordStartAt = i;
                    if (i + 1 == EntireText.Length)
                    {
                        WordEndAt = i;
                    }
                }
                else //حرف غير عربي
                {
                    if (WordStartAt >= 0)
                    {
                        WordEndAt = i - 1;
                    }
                }

                if (WordStartAt >= 0 && WordEndAt >= 0) //كلمة عربية كاملة
                {
                    ArabicWord NewWord = new ArabicWord();
                    NewWord.Start = WordStartAt;
                    NewWord.End = WordEndAt;
                    NewWord.Original = EntireText.Substring(WordStartAt, WordEndAt - WordStartAt + 1);
                    ArabicWords.Add(NewWord);
                    WordStartAt = -1;
                    WordEndAt = -1;
                }

                //هل يحدد نهاية جملة
                if (i > 0 && ArabicWords.Count > 0 && ArabicWords[ArabicWords.Count - 1].word != "EOS")
                {
                    if (i + 1 == EntireText.Length || Unicode == '\n' || (Unicode == '.' && (i + 1 == EntireText.Length || !char.IsDigit(EntireText[i + 1]))) || Unicode == '؛')
                    {
                        ArabicWord NewWord = new ArabicWord();
                        NewWord.Original = "EOS";
                        ArabicWords.Add(NewWord);
                        NumberOfSentences++;
                    }
                }
            }
            return ArabicWords;
        }


        private static List<WordInfo> ProcessWord(ArabicWord WordToProcess)
        {
            List<WordInfo> CurrentWordInfo = new List<WordInfo>();
            WordInfo NewInfo;

            Morphology.CheckForSpecialWord(WordToProcess, ref CurrentWordInfo);
            Morphology.CheckForPronoun(WordToProcess, ref CurrentWordInfo);
            if (FastAnalysis && CurrentWordInfo.Count > 0) //للاكتفاء باحتمالات الكلمات الخاصة وعدم البحث عن تحليل صرفي لأسماء أو أفعال
            {
                return CurrentWordInfo;
            }
            List<WordPrefix> ValidPrefixes = WordPrefix.CheckPrefix(WordToProcess.word);
            List<WordSuffix> ValidSuffixes = WordSuffix.CheckSuffixes(WordToProcess.word);
            //المعاني الافتراضية عند عدم وجود إضافات
            ValidPrefixes.Add(new WordPrefix() { WordClass = "N1" });//الأسماء نكرة
            ValidPrefixes.Add(new WordPrefix() { WordClass = "V1" });//فعل ماض
            ValidPrefixes.Add(new WordPrefix() { WordClass = "V3" });//فعل أمر

            ValidSuffixes.Add(new WordSuffix() { WordClass = "N0001" });//اسم مذكر
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V10311" });//فعل ماض غائب مفرد مذكر
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V20211" });//فعل مضارع مخاطب مفرد مذكر
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V201" });//فعل مضارع متكلم
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V2031" });//فعل مضارع غائب مفرد
            ValidSuffixes.Add(new WordSuffix() { WordClass = "V30011" });//فعل أمر مفرد مذكر
            List<string[]> Result = new List<string[]>();
            string Stem;
            for (int i = 0; i < ValidPrefixes.Count; i++)
            {
                for (int j = 0; j < ValidSuffixes.Count; j++)
                {
                    Result = new List<string[]>();

                    if (WordToProcess.word.Length <= (ValidSuffixes[j].Text.Length + ValidPrefixes[i].Text.Length))
                    {   //طول الإضافات يغطي طول الكلمة بأكملها
                        continue;
                    }
                    List<string> CompatibleAdditions = Morphology.CheckAdditionsCompatibility(ValidPrefixes[i].WordClass, ValidSuffixes[j].WordClass);
                    if (CompatibleAdditions.Count == 0)
                    {   //إضافات غير متوافقة
                        continue;
                    }
                    Stem = WordToProcess.word.Substring(ValidPrefixes[i].Text.Length, WordToProcess.word.Length - (ValidPrefixes[i].Text.Length + ValidSuffixes[j].Text.Length));
                    //ابحث عن الأوزان المتوافقة مع الإضافات المحددة
                    Result = Morphology.LookForTemplate(Stem, CompatibleAdditions, Result);
                    if (Result.Count == 0) continue;

                    /* اختبار وجود جذر للكلمة متوافق مع الوزن المحدد
                     * واختبار توافق الجذر الموجود مع هذا الوزن
                     * يمكن الاستغناء عن بعض هذه الخطوات عند إكمال قاعدة البيانات
                     * 
                     */

                    #region اختبار توافق الوزن والجذر
                    string[] CurrentResult;
                    ArabicRoot CurrentRoot = new ArabicRoot();
                    List<ArabicRoot> CheckRootResults = new List<ArabicRoot>();
                    for (int R = 0; R < Result.Count; R++)
                    {
                        CurrentResult = Result[R];
                        bool RootResult = Morphology.CheckRoot(Stem, CurrentResult[2], CurrentResult[4], CurrentResult[5], CurrentResult[6], CurrentResult[7], ref CurrentResult[3], ref CurrentRoot);
                        if (!RootResult) //اختبار وجود الجذر حسب الوزن 
                        {
                            Result.RemoveAt(R);
                            R--;
                        }
                        else
                        {
                            if (CurrentRoot.IsCompatible)
                            {
                                for (int prev = 0; prev < R; prev++)
                                {
                                    //عثر على جذر متوافق احذف كل الأوزان السابقة التي ليس لها جذور متوافقة
                                    if (!CheckRootResults[prev].IsCompatible)
                                    {
                                        Result.RemoveAt(prev);
                                        CheckRootResults.RemoveAt(prev);
                                        R--;
                                        prev--;
                                    }
                                }
                                CheckRootResults.Add(CurrentRoot);
                            }
                            else
                            {
                                bool AddThisOne = true;
                                for (int prev = 0; prev < R; prev++)
                                {
                                    if (CheckRootResults[prev].IsCompatible)
                                    {
                                        AddThisOne = false; //عثر من قبل على جذور متوافقة لها أولوية
                                        Result.RemoveAt(R--);
                                        break;
                                    }
                                    //مفاضلة الأوزان من نفس قاعدة الاشتقاق
                                    byte CompareResult = Morphology.CompareRules(CurrentResult[7], Result[prev][7]);
                                    if (CompareResult == 1)
                                    {
                                        CheckRootResults.RemoveAt(prev);
                                        Result.RemoveAt(prev--);
                                        R--;
                                    }
                                    else if (CompareResult == 2)//الوزن المضاف مسبقا أولى
                                    {
                                        AddThisOne = false;
                                        Result.RemoveAt(R--);
                                        break;
                                    }
                                }
                                if (AddThisOne)
                                {
                                    CheckRootResults.Add(CurrentRoot);
                                }
                            }
                        }
                    }
                    #endregion

                    for (int R = 0; R < Result.Count; R++)
                    {
                        NewInfo = new WordInfo();
                        NewInfo.Word = Stem;
                        NewInfo.Diacritics = Result[R][1];
                        NewInfo.Prefix = ValidPrefixes[i];
                        NewInfo.Suffix = ValidSuffixes[j];
                        Tashkeel.DiacritizeWord(NewInfo);
                        if (!IgnoreExistingDiacritics && !CheckOriginalDiacritics(WordToProcess.Original, NewInfo.FullDiacritics)) continue;

                        NewInfo.Template = Result[R][0];
                        NewInfo.Meaning = Result[R][3];
                        NewInfo.Root = CheckRootResults[R];
                        CurrentWordInfo.Add(NewInfo);
                    }
                }
            }

            for (int W = 0; W < CurrentWordInfo.Count; W++)
            {
                if (CurrentWordInfo[W].Root.IsCompatible)
                {
                    WordInfo Temp;
                    for (int prev = W; prev > 0; prev--)
                    {
                        Temp = CurrentWordInfo[prev];
                        CurrentWordInfo[prev] = CurrentWordInfo[prev - 1];
                        CurrentWordInfo[prev - 1] = Temp;
                    }
                }
            }
            CurrentWordInfo = RecallCorrections(WordToProcess, CurrentWordInfo);
            return CurrentWordInfo;
        }

        static List<WordInfo> RecallCorrections(ArabicWord Word, List<WordInfo> CurrentWordInfo)
        {
            OleDbCommand com = new OleDbCommand();
            com.Connection = con;
            com.CommandText = "Select * from corrections where word = '" + Word.word + "' order by Occured asc";
            OleDbDataReader dread = com.ExecuteReader();
            if (dread.Read())
            {
                Word.Corrected = true;
                Corrections++;
                do
                {
                    for (int W = 0; W < CurrentWordInfo.Count; W++)
                    {
                        if (CurrentWordInfo[W].Diacritics == dread["Diacritics"].ToString() && CurrentWordInfo[W].CompleteMeaning == dread["Meaning"].ToString())
                        {
                            WordInfo Temp;
                            for (int prev = W; prev > 0; prev--)
                            {
                                Temp = CurrentWordInfo[prev];
                                CurrentWordInfo[prev] = CurrentWordInfo[prev - 1];
                                CurrentWordInfo[prev - 1] = Temp;
                            }
                        }
                    }
                } while (dread.Read());
            }
            dread.Close();
            return CurrentWordInfo;
        }

        internal static void StoreCorrections()
        {
            con.Open();
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                if (!ArabicWords[i].Corrected) continue;
                WordInfo ThisWordInfo = AllWordsInfo[i][0];
                OleDbCommand com = new OleDbCommand();
                com.Connection = con;
                com.CommandText = string.Format("select occured from corrections where word = '{0}' and meaning = '{1}' and diacritics = '{2}'",
                    ArabicWords[i].word, ThisWordInfo.CompleteMeaning, ThisWordInfo.Diacritics);
                OleDbDataReader dread = com.ExecuteReader();
                byte Occured;
                if (dread.Read())
                {
                    Occured = byte.Parse(dread[0].ToString());
                    Occured++;
                    dread.Close();
                    com.CommandText = string.Format("update corrections set occured = '{0}' where word = '{1}' and meaning = '{2}' and diacritics = '{3}'",
                    Occured, ArabicWords[i].word, ThisWordInfo.CompleteMeaning, ThisWordInfo.Diacritics);
                    com.ExecuteNonQuery();
                }
                else
                {
                    dread.Close();
                    com.CommandText = string.Format("insert into corrections values('{0}','{1}','{2}',1)",
                        ArabicWords[i].word, ThisWordInfo.CompleteMeaning, ThisWordInfo.Diacritics);
                    com.ExecuteNonQuery();
                }
            }
            con.Close();
        }

        internal static void LearnNewWords()
        {
            for (int i = 0; i < ArabicWords.Count; i++)
            {
                if (AllWordsInfo[i][0].Template != "" || AllWordsInfo[i][0].Meaning != "N")//كلمة مفسرة
                {
                    continue;
                }
                ForeignWord LearnNewWord = new ForeignWord();
                LearnNewWord.txtWord.Text = ArabicWords[i].word;
                LearnNewWord.ShowDialog();
            }
        }

        internal static bool CheckOriginalDiacritics(string Original, string AppliedDiacritics)
        {
            string Diac = "ًٌٍَُِّْ";
            for (int i = 0, L = 0; i < Original.Length; i++, L++)//لكل حرف من حروف الكلمة الأصلية
            {
                string Current = "";//تشكيل الحرف الحالي
                for (int d = i + 1; d < Original.Length; d++)//تجميع تشكيل الحرف الحالي
                {
                    if (Diac.Contains(Original[d])) Current += Original[d];
                    else
                    {
                        break;
                    }
                }
                if (Current.Length > 0)//إذا كان للحرف تشكيل
                {
                    switch (AppliedDiacritics[L])//تأكد من مطابقة التشكيل
                    {
                        case '0':
                            if (Current != "ْ") return false;
                            break;
                        case '1':
                            if (Current != "َ") return false;
                            break;
                        case '2':
                            if (Current != "ُ") return false;
                            break;
                        case '3':
                            if (Current != "ِ") return false;
                            break;
                        case '4':
                            if (Current != "َّ" && Current != "ّ") return false;
                            break;
                        case '5':
                            if (Current != "ُّ" && Current != "ّ") return false;
                            break;
                        case '6':
                            if (Current != "ِّ" && Current != "ّ") return false;
                            break;
                        case '7':
                            return false;
                        case '9':
                            return false;
                        case 'A':
                            if (Current != "ً") return false;
                            break;
                        case 'B':
                            if (Current != "ٌ") return false;
                            break;
                        case 'C':
                            if (Current != "ٍ") return false;
                            break;
                        case 'D':
                            if (Current != "ًّ" && Current != "ّ") return false;
                            break;
                        case 'E':
                            if (Current != "ٌّ" && Current != "ّ") return false;
                            break;
                        case 'F':
                            if (Current != "ٍّ" && Current != "ّ") return false;
                            break;
                    }
                }
                i += Current.Length;
            }
            return true;
        }
    }
}
