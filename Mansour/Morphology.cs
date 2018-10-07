using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Mansour
{
    public class Morphology
    {

        public static void CheckForSpecialWord(ArabicWord WordToProcess, ref List<WordInfo> Possibilities)
        {
            WordInfo NewInfo;
            OleDbCommand com = new OleDbCommand("select Word, diacritics, Meaning, Class from SpecialWords where word='" + WordToProcess.word + "'");
            com.Connection = Analyzer.con;
            OleDbDataReader dread = com.ExecuteReader();
            while (dread.Read())
            {
                NewInfo = new WordInfo();
                NewInfo.Meaning = dread["meaning"].ToString();
                NewInfo.SpecialClass = dread["Class"].ToString();
                NewInfo.Word = WordToProcess.word;
                NewInfo.Diacritics = dread[1].ToString();
                Possibilities.Add(NewInfo);
            }
            dread.Close();
        }

        public static void CheckForPronoun(ArabicWord WordToProcess, ref List<WordInfo> Possibilities)
        {
            WordInfo NewInfo;
            OleDbCommand com = new OleDbCommand("Select * from ProperNouns where Word='" + WordToProcess.word + "'");
            com.Connection = Analyzer.con;
            OleDbDataReader dread = com.ExecuteReader();
            while (dread.Read())
            {
                NewInfo = new WordInfo();
                NewInfo.Meaning = dread["meaning"].ToString();
                NewInfo.SpecialClass = "PN";
                NewInfo.Word = WordToProcess.word;
                NewInfo.Diacritics = dread["Diacritics"].ToString();
                Possibilities.Add(NewInfo);
            }
            dread.Close();
        }

        public static List<WordInfo> LookForForignWord(string Word, List<WordInfo> Result)
        {
            OleDbCommand com = new OleDbCommand("Select * From ArabizedWords where Word='" + Word + "'");
            com.Connection = Analyzer.con;
            OleDbDataReader dread = com.ExecuteReader();
            while (dread.Read())
            {
                WordInfo WI = new WordInfo();
                WI.Word = Word;
                WI.Meaning = dread["Meaning"].ToString();
                WI.Diacritics = dread["diacritics"].ToString();
                Result.Add(WI);
            }
            return Result;
        }

        public static List<string[]> LookForTemplate(string OriginalWord,List<string> Meanings, List<string[]> Results)
        {
            if (OriginalWord.EndsWith("اؤ") || OriginalWord.EndsWith("ائ"))
            {
                OriginalWord = OriginalWord.Substring(0, OriginalWord.Length - 1) + "ء";
            }
            string Mask = OriginalWord.Replace("آ", "ءا");
            Mask = Mask.Replace('ى', 'ا');

            string additionals = "أءئؤستطمونيهاإ";
            byte RootLetters = 0;
            StringBuilder MaskBuilder = new StringBuilder();
            for (int i = 0; i < Mask.Length; i++)
            {
                if (additionals.Contains(Mask[i]))
                {
                    MaskBuilder.Append(Mask[i]);
                }
                else
                {
                    MaskBuilder.Append('#');
                    RootLetters++;
                }
            }
            Mask = MaskBuilder.ToString();
            List<string> Temps = new List<string>();
            if (RootLetters > 0)
            {
                Results = CheckWordMask(OriginalWord, Mask, Meanings, Results);
                if (RootLetters >= 4 || RootLetters == Mask.Length) //أقصى عدد حروف أصلية ممكن
                {
                    return Results;
                }
            }
            string TempMask;
            List<string> Masks = new List<string>();
            List<byte> RootLettersCount = new List<byte>();
            Masks.Add(Mask);
            RootLettersCount.Add(RootLetters);
            for (byte j = 0; j < Mask.Length; j++)
            {
                if (Mask[j] != '#') //إذا كان الحرف من الحروف الزائدة فيحتمل أن يكون أصليا أو زائدا
                {
                    int Count = Masks.Count;
                    for (int k = 0; k < Count; k++)
                    {
                        RootLetters = RootLettersCount[k];
                        if (++RootLetters > 4)
                        {
                            //لا تزيد الحروف الأصلية عن 4 فلا حاجة للاحتفاظ بهذا الاحتمال
                            Masks.RemoveAt(k);
                            RootLettersCount.RemoveAt(k--);
                            Count--;
                            continue;
                        }
                        TempMask = Masks[k].Substring(0, j) + '#' + Masks[k].Substring(j + 1);
                        Results = CheckWordMask(OriginalWord, TempMask, Meanings, Results);
                        Masks.Add(TempMask);
                        RootLettersCount.Add(RootLetters);
                    }
                }
            }
            return Results;
        }

        public static List<string[]> CheckWordMask(string word, string mask, List<string> Meanings, List<string[]> Results)
        {
            OleDbCommand com = new OleDbCommand("select * from WordTemplates where Mask='" + mask + "'", Analyzer.con);
            OleDbDataReader dread = com.ExecuteReader();
            string[] fields;
            while (dread.Read())
            {
                //اختبار توافق تشكيل الوزن مع حروف الكلمة
                if (dread["Class"].ToString().StartsWith("V2"))//إذا كان وزن فعل مضارع
                {
                    if (!Tashkeel.CheckTashkeel(word, dread[1].ToString().Substring(1))) continue;//استثناء حرف المضارعة
                }
                else if (!Tashkeel.CheckTashkeel(word, dread[1].ToString()))
                {
                    continue;
                }
                //اختبار توافق نوع الوزن مع النوع المتوقع
                foreach (string M in Meanings)
                {
                    string MergedMeaning;
                    if (Interpreter.CompareMeanings(dread["Class"].ToString(), M, out MergedMeaning))
                    {
                        fields = new string[8];
                        fields[0] = dread[0].ToString();
                        fields[1] = dread[1].ToString();
                        fields[2] = dread[2].ToString();
                        fields[3] = MergedMeaning;
                        fields[4] = dread[5].ToString();
                        fields[5] = dread[6].ToString();
                        fields[6] = dread[7].ToString();
                        fields[7] = dread[8].ToString();
                        Results.Add(fields);
                    }
                }
            }
            dread.Close();
            return Results;
        }

        public static byte CompareRules(string first, string second)
        {
            //مفاضلة الأوزان من نفس قاعدة الاشتقاق لتحسين النتائج
            //في حالة الجذور الغير مرتبطة بقواعد الاشتقاق في قاعدة البيانات
            if (first.Length < 2 || second.Length < 2) return 0;//تعريف القاعدة من حرفين على الأقل
            string[] FirstRules = first.Split(',');
            string[] SecondRules = second.Split(',');
            bool SecondGreater = false, FirstGreater = false;
            foreach (string fRule in FirstRules)
            {
                if (fRule[0] == 'X') continue;//أوزان الأسماء الغير مرتبطة بقاعدة
                foreach (string sRule in SecondRules)
                {
                    if (fRule[0] == sRule[0])//من نفس قاعدة الاشتقاق
                    {
                        if (fRule[1] != '1' && sRule[1] == '1')//الأولى حالة خاصة من الثانية
                        {
                            FirstGreater = true;
                        }
                        else if (sRule[1] != '1' && fRule[1] == '1')//الثانية حالة خاصة من الأولى
                        {
                            SecondGreater = true;
                        }
                    }
                }
            }
            if (FirstGreater && !SecondGreater && !second.Contains('X'))
            {
                return 1;
            }
            else if (SecondGreater && !FirstGreater && !first.Contains('X'))
            {
                return 2;
            }
            return 0; //لا صلة بينهما
        }

        public static List<string> CheckAdditionsCompatibility(string PrefixClass, string SuffixClass)
        {
            //اختبر توافق السابقة مع اللاحقة
            string[] PossiblePrefixes = PrefixClass.Split(',');
            string[] PossibleSuffixes = SuffixClass.Split(',');
            List<string> Compatibles = new List<string>();
            foreach (string PreClass in PossiblePrefixes)
            {
                foreach (string SufClass in PossibleSuffixes)
                {
                    string MergedMeaning;
                    if (Interpreter.CompareMeanings(PreClass, SufClass, out MergedMeaning))
                    {
                        Compatibles.Add(MergedMeaning);
                    }
                }
            }
            return Compatibles;
        }

        public static bool CheckRoot(string word, string Template, string ف, string ع, string ل, string rules, ref string Meaning, ref ArabicRoot WordRoot)
        {
            List<string> Roots = new List<string>();

            word = word.Replace('ء', 'أ');
            word = word.Replace('إ', 'أ');
            word = word.Replace('ؤ', 'أ');
            word = word.Replace('ئ', 'أ');
            word = word.Replace("آ", "أا");
            word = word.Replace("ى", "ا");

            List<char> First = new List<char>();
            List<char> Second = new List<char>();
            List<char> Third = new List<char>();
            List<char> Fourth = new List<char>();

            for (int i = 0; i < word.Length; i++)//تجميع حروف الجذر الموجودة في الكلمة
            {
                if (Template[i] == 'ف') First.Add(word[i]);
                if (Template[i] == 'ع') Second.Add(word[i]);
                if (Template[i] == 'ل')
                {
                    if (Third.Count == 0)
                    {
                        Third.Add(word[i]);
                        if (word[i] == 'و' || word[i] == 'ي')
                        {
                            Third.Add('ا');
                        }
                    }
                    else Fourth.Add(word[i]);
                }
            }
            
            #region فحص توافق الحروف
            if (ف.Length > 0)
            {
                if (First.Count == 0)
                {
                    foreach (char item in ف)
                    {
                        First.Add(item);
                    }
                }
                else
                {
                    bool match = false;
                    foreach (char item in ف)
                    {
                        if (First[0] == item)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return false;
                }

            }
            if (ع.Length > 0)
            {
                if (Second.Count == 0)
                {
                    foreach (char item in ع)
                    {
                        Second.Add(item);
                    }
                }
                else
                {
                    bool match = false;
                    foreach (char item in ع)
                    {
                        if (Second[0] == item)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return false;
                }

            }
            if (ل.Length > 0)
            {
                if (Third.Count == 0)
                {
                    if (ل == "ع")//جذر مضعف
                    {
                        Third.Add(Second[0]);
                    }
                    else
                    {
                        if (ل.Contains('و') ||ل.Contains('ي'))
                        {
                            Third.Add('ا');
                        }
                        foreach (char item in ل)
                        {
                            Third.Add(item);
                        }
                    }
                }
                else
                {
                    bool match = false;
                    foreach (char item in ل)
                    {
                        if (Third[0] == item)
                        {
                            match = true;
                            break;
                        }
                    }
                    if (!match) return false;
                }

            }
            #endregion

            StringBuilder RootBuilder;
            foreach (char L1 in First)
            {
                foreach (char L2 in Second)
                {
                    foreach (char L3 in Third)
                    {
                        RootBuilder = new StringBuilder();
                        RootBuilder.Append(L1);
                        RootBuilder.Append(L2);
                        RootBuilder.Append(L3);
                        if (Fourth.Count > 0) RootBuilder.Append(Fourth[0]);
                        Roots.Add(RootBuilder.ToString());
                    }
                }
            }

            OleDbCommand com = new OleDbCommand();
            com.Connection = Analyzer.con;
            OleDbDataReader dread;
            foreach (string root in Roots)
            {
                com.CommandText = "select * from roots where StrComp(Root,'" + root + "',0)=0";
                dread = com.ExecuteReader();
                if (dread.Read())
                {
                    string[] RootIntrans = dread["intrans"].ToString().Split(',');
                    string[] RootTrans1 = dread["trans1"].ToString().Split(',');
                    string[] RootTrans2 = dread["trans2"].ToString().Split(',');
                    string[] RootSingulars = dread["Singular"].ToString().Split(',');
                    string[] RootPlurals = dread["Plural"].ToString().Split(',');
                    string[] TemplateRules = rules.Split(',');
                    string Temp;//للتخزين المؤقت لناتج مقارنة المعنى
                    for (int X = 0; X < TemplateRules.Length; X++)
                    {
                        for (int Y = 0; Y < RootIntrans.Length; Y++)//البحث في الأفعال اللازمة
                        {
                            if (RootIntrans[Y] == TemplateRules[X])
                            {
                                if (Meaning.StartsWith("V"))
                                {
                                    Interpreter.CompareMeanings("V01", Meaning, out Meaning);
                                }
                                dread.Close();
                                WordRoot.Root = root;
                                WordRoot.IsCompatible = true;
                                WordRoot.DerivationRules = RootIntrans[Y];
                                if (Meaning.StartsWith("N") && Meaning.Length > 2)
                                {
                                    WordRoot.DerivationType = (ArabicRoot.Derivatives)Meaning[2] - 48;
                                }
                                return true;
                            }
                        }
                        for (int Y = 0; Y < RootTrans1.Length; Y++)//البحث في الأفعال المتعدية لمفعول
                        {
                            if (RootTrans1[Y] == TemplateRules[X])
                            {
                                if (Meaning.StartsWith("V"))
                                {
                                    Interpreter.CompareMeanings("V02", Meaning, out Meaning);
                                }
                                dread.Close();
                                WordRoot.Root = root;
                                WordRoot.IsCompatible = true;
                                WordRoot.DerivationRules = RootTrans1[Y];
                                if (Meaning.StartsWith("N") && Meaning.Length > 2)
                                {
                                    WordRoot.DerivationType = (ArabicRoot.Derivatives)Meaning[2] - 48;
                                }
                                return true;
                            }
                        }
                        for (int Y = 0; Y < RootTrans2.Length; Y++)//البحث في الأفعال المتعدية لمفعولين
                        {
                            if (RootTrans2[Y] == TemplateRules[X])
                            {
                                if (Meaning.StartsWith("V"))
                                {
                                    Interpreter.CompareMeanings("V03", Meaning, out Meaning);
                                }
                                dread.Close();
                                WordRoot.Root = root;
                                WordRoot.IsCompatible = true;
                                WordRoot.DerivationRules = RootTrans2[Y];
                                if (Meaning.StartsWith("N") && Meaning.Length > 2)
                                {
                                    WordRoot.DerivationType = (ArabicRoot.Derivatives)Meaning[2] - 48;
                                }
                                return true;
                            }
                        }
                        if (Interpreter.CompareMeanings("N004", Meaning, out Temp))
                        {
                            for (int Y = 0; Y < RootPlurals.Length; Y++)//البحث في الأسماء الجمع
                            {
                                if (RootPlurals[Y] == TemplateRules[X])
                                {
                                    dread.Close();
                                    WordRoot.Root = root;
                                    WordRoot.IsCompatible = true;
                                    WordRoot.DerivationRules = RootPlurals[Y];
                                    return true;
                                }
                            }
                        }
                        if (Interpreter.CompareMeanings("N001", Meaning, out Temp)
                            || Interpreter.CompareMeanings("N002", Meaning, out Temp)
                            || Interpreter.CompareMeanings("N003", Meaning, out Temp))
                        {
                            for (int Y = 0; Y < RootSingulars.Length; Y++)//البحث في الأسماء المفردة
                            {
                                Meaning = Temp;
                                if (RootSingulars[Y] == TemplateRules[X])
                                {
                                    dread.Close();
                                    WordRoot.Root = root;
                                    WordRoot.IsCompatible = true;
                                    WordRoot.DerivationRules = RootSingulars[Y];
                                    return true;
                                }
                            }
                        }
                    }
                    //جذر موجود لكن توافقه مع الوزن غير أكيد
                    dread.Close();
                    WordRoot.Root = root;
                    WordRoot.DerivationRules = rules;
                    WordRoot.IsCompatible = false;
                    if (Meaning.StartsWith("N") && Meaning.Length > 2)
                    {
                        WordRoot.DerivationType = (ArabicRoot.Derivatives)Meaning[2] - 48;
                    }
                    return true;
                }
                dread.Close();
            }
            //جذر غير موجود
            return false;
        }

    
    }
}
