using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Mansour
{
    public class Interpretation
    {
        public string Description { get; set; }
        public string Meaning { get; set; }
        public GrammarRelation SuperRelation { get; set; }

        public static Dictionary<string, string[]> Interpretations;
        public static void InitializeInterpretations()
        {
            Interpretations = new Dictionary<string, string[]>();
            OleDbCommand cmd = new OleDbCommand("Select * from Interpretation", Analyzer.con);
            OleDbDataReader dreader = cmd.ExecuteReader();
            string[] Details;
            while (dreader.Read())
            {
                Details = new string[2];
                Details[0] = dreader["Description"].ToString();
                Details[1] = dreader["Sign"].ToString();
                Interpretations.Add(dreader["ID"].ToString(), Details);
            }
            dreader.Close();
        }
    }

    public class GrammarRelation : Interpretation
    {
        public Dictionary<int, Interpretation> Elements = new Dictionary<int, Interpretation>();
        public delegate void Activate();

        public void SendActivationSignal()
        {
            GrammarRelation GR = this;
            while (GR.SuperRelation != null)
            {
                GR = GR.SuperRelation;
            }
            GR.ActivateRelation();
        }

        public void ActivateRelation()
        {

            foreach (int WordIndex in Elements.Keys)
            {
                if (Elements[WordIndex] is GrammarRelation)
                {
                    ((GrammarRelation)Elements[WordIndex]).ActivateRelation();
                }
                else
                {
                    Interpretation Interp = Elements[WordIndex];
                    string Temp;
                    for (int i = 0; i < Analyzer.AllWordsInfo[WordIndex].Count; i++)
                    {
                        if (Interpreter.CompareMeanings(Analyzer.AllWordsInfo[WordIndex][i].Meaning, Interp.Meaning, out Temp))
                        {
                            if (i > 0)
                            {
                                WordInfo Top = Analyzer.AllWordsInfo[WordIndex][i];
                                Analyzer.AllWordsInfo[WordIndex][i] = Analyzer.AllWordsInfo[WordIndex][0];
                                Analyzer.AllWordsInfo[WordIndex][0] = Top;
                            }
                            break;
                        }
                    }
                }
            }

        }
        public static void ApplyGrammarRelation(GrammarRelation SuperRelation)
        {
            GrammarRelation GR;
            foreach (int WordIndex in SuperRelation.Elements.Keys)
            {
                GR = SuperRelation.Elements[WordIndex] as GrammarRelation;
                if (GR != null)
                {
                    ApplyGrammarRelation(GR);
                }
                else
                {
                    string Temp;
                    Interpretation Interp = SuperRelation.Elements[WordIndex];
                    for (int i = 0; i < Analyzer.AllWordsInfo[WordIndex].Count; i++)
                    {
                        if (Interpreter.CompareMeanings(Analyzer.AllWordsInfo[WordIndex][i].Meaning, Interp.Meaning, out Temp))
                        {
                            Interpretation NewInterp = new Interpretation 
                            { Meaning = Temp, Description = Interp.Description, SuperRelation = Interp.SuperRelation };
                            if (Analyzer.AllWordsInfo[WordIndex][i].Interpretations == null)
                            {
                                Analyzer.AllWordsInfo[WordIndex][i].Interpretations = new List<Interpretation>();
                            }
                            Analyzer.AllWordsInfo[WordIndex][i].Interpretations.Add(NewInterp);
                        }
                    }
                }
            }
        }

        public int WordsCovered
        {
            get
            {
                int Count = 0;
                foreach (Interpretation Element in Elements.Values)
                {
                    if (Element is GrammarRelation)
                    {
                        Count += ((GrammarRelation)Element).WordsCovered;
                    }
                    else
                    {
                        Count += 1;
                    }
                }
                return Count;
            }
        }
        public static List<string[]> GrammarRules;
        public static void InitializeRules()
        {
            GrammarRules = new List<string[]>();
            OleDbCommand cmd = new OleDbCommand("Select expression,result from GrammarRules order by priority asc", Analyzer.con);
            OleDbDataReader dreader = cmd.ExecuteReader();
            string[] Rule;
            while (dreader.Read())
            {
                Rule = new string[2];
                Rule[0] = dreader["result"].ToString();
                Rule[1] = dreader["expression"].ToString();
                GrammarRules.Add(Rule);
            }
            dreader.Close();
        }
    }

    public class Interpreter
    {
        #region OldMethod
        public static List<GrammarRelation> StartInterpreting(int StartIndex, string ExpectedMeaning, int MaximumRecursion, bool RecursiveCall)
        {
            bool ValidRule = true;
            List<GrammarRelation> AllRelations = new List<GrammarRelation>();
            foreach (string[] Rule in GrammarRelation.GrammarRules)
            {
                string RuleMeaning;
                if (!CompareMeanings(ExpectedMeaning, Rule[0], out RuleMeaning))//القاعدة لا تحقق المعنى المطلوب
                {
                    continue;
                }
                List<GrammarRelation> Relations = new List<GrammarRelation>();
                Relations.Add(new GrammarRelation());
                string[] expression = Rule[1].Split('+');//فصل مكونات علاقة القاعدة النحوية
                string ElementMeaning, ElementInterpretation = "";
                string[] InterpretFound = new string[2];
                ValidRule = true;

                for (int Element = 0; Element < expression.Length; Element++) //بدء مطابقة القاعدة
                {
                    string[] ElementRootRule = new string[2];
                    if (expression[Element].Contains(':'))
                    {
                        //فصل المعنى عن الإعراب
                        ElementMeaning = expression[Element].Substring(0, expression[Element].IndexOf(':')).Trim();
                        if (ElementMeaning.Contains('['))
                        {
                            ElementRootRule = ElementMeaning.Substring(ElementMeaning.IndexOf('[')).Trim('[', ']').Split(',');
                            ElementMeaning = ElementMeaning.Substring(0, ElementMeaning.IndexOf('['));
                        }
                        ElementInterpretation = expression[Element].Substring(expression[Element].IndexOf(':') + 1).Trim();
                    }
                    else
                    {
                        ElementMeaning = expression[Element].Trim();//حرف من الحروف ليس له إعراب
                    }
                    if (ElementMeaning[0] != 'T')
                    {
                        if (!Interpretation.Interpretations.ContainsKey(ElementInterpretation) && ElementMeaning[0] != 'T')//إذا كان الإعراب المخزن وصفه غير موجود 
                        {
                            //خطأ في قاعدة البيانات 
                            ValidRule = false;
                            break;
                        }
                        InterpretFound = Interpretation.Interpretations[ElementInterpretation];//البحث عن معنى رمز الإعراب
                    }
                    int Count = Relations.Count;
                    for (int GRelation = 0; GRelation < Count; GRelation++)
                    {
                        int Offset = Relations[GRelation].WordsCovered;
                        if (Relations.Count == 0 || Analyzer.AllWordsInfo[StartIndex + Offset][0].Word == "EOS")
                        {

                            Relations.RemoveAt(GRelation);
                            Count--;
                            GRelation--;
                            continue;
                        }
                        string InterpretedMeaning;

                        if (ElementRootRule[0] != null)//هل هناك شروط اشتقاق لهذه الكلمة
                        {
                            bool Found = false;
                            foreach (var item in Analyzer.AllWordsInfo[StartIndex + Offset])
                            {
                                if (CompareMeanings(item.Meaning, ElementMeaning, out InterpretedMeaning))
                                {
                                    if (item.Root.Root == ElementRootRule[0] && item.Root.DerivationRules.Contains(ElementRootRule[1]))
                                    {
                                        Interpretation NewInterpret = new Interpretation();//إعراب جديد
                                        GrammarRelation NewRelation = CloneRelation(Relations[GRelation]);
                                        NewInterpret.Description = InterpretFound[0];//نحميل وصف الإعراب
                                        NewInterpret.Meaning = ElementMeaning;
                                        NewRelation.Elements.Add(StartIndex + Offset, NewInterpret);
                                        NewInterpret.SuperRelation = NewRelation;
                                        Relations.Add(NewRelation);
                                        Found = true;
                                        break;
                                    }
                                }
                            }
                            if (!Found)
                            {
                                Relations.RemoveAt(GRelation);
                                Count--;
                                GRelation--;
                            }
                            continue;
                        }

                        List<string> WordMeanings = GetPossibleMeanings(Analyzer.AllWordsInfo[StartIndex + Offset]);

                        for (int Possibility = 0; Possibility < WordMeanings.Count; Possibility++)
                        {
                            bool ValidMeaning = CompareMeanings(WordMeanings[Possibility], ElementMeaning, out InterpretedMeaning);
                            if (ElementMeaning[0] == 'T')// إذا كان العنصر حرفا صالحا
                            {
                                if (ValidMeaning)
                                {
                                    Interpretation NewInterpret = new Interpretation();//إعراب جديد
                                    GrammarRelation NewRelation = CloneRelation(Relations[GRelation]);
                                    NewInterpret.Meaning = ElementMeaning;
                                    NewRelation.Elements.Add(StartIndex + Offset, NewInterpret);
                                    NewInterpret.SuperRelation = NewRelation;
                                    Relations.Add(NewRelation);
                                }
                                continue;
                            }

                            if (ValidMeaning)
                            {
                                Interpretation NewInterpret = new Interpretation();//إعراب جديد
                                GrammarRelation NewRelation = CloneRelation(Relations[GRelation]);
                                NewInterpret.Description = InterpretFound[0];//نحميل وصف الإعراب
                                NewInterpret.Meaning = ElementMeaning;
                                NewRelation.Elements.Add(StartIndex + Offset, NewInterpret);
                                NewInterpret.SuperRelation = NewRelation;
                                Relations.Add(NewRelation);
                            }
                        }

                        if (MaximumRecursion > 0 && Element > 0 && !Relations[GRelation].Elements.ContainsKey(-1))
                        {
                            //ابحث عن مجموعة كلمات تحقق المعنى المطلوب
                            List<GrammarRelation> SubRelations = new List<GrammarRelation>();
                            SubRelations = StartInterpreting(StartIndex + Offset, ElementMeaning, MaximumRecursion - 1, true);
                            if (SubRelations.Count > 0)
                            {
                                //إذا نجح البحث عن مجموعة كلمات
                                for (int R = 0; R < SubRelations.Count; R++)
                                {
                                    GrammarRelation NewSubRelation = CloneRelation(Relations[GRelation]);
                                    SubRelations[R].Description = InterpretFound[0];
                                    SubRelations[R].Meaning = ElementMeaning;
                                    SubRelations[R].SuperRelation = NewSubRelation;
                                    NewSubRelation.Elements.Add(-1, SubRelations[R]);
                                    Relations.Add(NewSubRelation);
                                }
                            }
                        }

                        Relations.RemoveAt(GRelation);
                        Count--;
                        GRelation--;
                    }
                    if (!ValidRule) break;
                }
                if (ValidRule)
                {
                    foreach (GrammarRelation R in Relations)
                    {
                        AllRelations.Add(R);
                    }
                }
            }
            return AllRelations;
        }
        #endregion

        public static void InterpretComposition(int StartIndex, string Meaning)
        {
            
        }

        public static GrammarRelation CloneRelation(GrammarRelation GR)
        {
            GrammarRelation NewGR = new GrammarRelation();
            foreach (var Key in GR.Elements.Keys)
            {
                NewGR.Elements.Add(Key, GR.Elements[Key]);
            }
            return NewGR;
        }

        public static List<string> GetPossibleMeanings(List<WordInfo> WordInfos)
        {
            List<string> AllMeanings = new List<string>();

            for (int i = 0; i < WordInfos.Count; i++)
            {
                bool NewMeaning = true;
                for (int j = 0; j < AllMeanings.Count; j++)
                {
                    if (WordInfos[i].CompleteMeaning == AllMeanings[j])
                    {
                        NewMeaning = false;
                        break;
                    }
                }
                if (NewMeaning) AllMeanings.Add(WordInfos[i].Meaning);
            }
            return AllMeanings;
        }

        public static string FinishDiacritics(string Diacritics, string Meaning)
        {
            if (Meaning.Length == 0) return Diacritics;
            if (Diacritics.IndexOf('!') < 0 && Diacritics.IndexOf('#') < 0) return Diacritics; //لا مكان للعلامات الإعرابية
            char[] Symbols = { '!', '#' };
            int position = Diacritics.IndexOfAny(Symbols);
           //string M2,parsing;
          
            switch (Meaning[0])
            {
                case 'N'://الكلمة اسم
                    if (Meaning[1] != '1' && Meaning[1] != '2' && Meaning[1] != '0')
                    {
                        return Diacritics; //التشكيل للأسماء المعربة فقط
                    }
                    if (Meaning.Length < 6 || Meaning[5] == '2' || Meaning[5] == '0')
                    {
                        if (Meaning[1] == '1' && (Meaning.Length > 6 && Meaning[6] != '1' || Meaning.Length <= 6))//نكرة وليس مضافا
                            {
                                Diacritics = Diacritics.Replace('#', 'E');//تنوين ضمتين وشده
                                return Diacritics.Replace('!', 'B');//تنوين ضمتين
                            }
                            Diacritics = Diacritics.Replace('#', '5');
                            return Diacritics.Replace('!', '2');
                    } 
                    switch(Meaning[5])
                    {
                        case '1'://اسم منصوب 
                            if (Meaning[2] == '3' && Meaning[3] == '2')//جمع مؤنث سالم
                            {
                                if (Meaning[1] == '1' && Meaning.Length > 6 && Meaning[6] != '1')//نكرة وليس مضافا
                                {
                                    return Diacritics.Replace('!', 'C');//تنوين كسرتين
                                }
                                return Diacritics.Replace('!', '3');//كسرة فقط
                            }
                            else
                            {
                                if (Meaning[1] == '1' && (Meaning.Length > 6 && Meaning[6] != '1' || Meaning.Length <= 6))//نكرة وليس مضافا
                                {
                                    Diacritics = Diacritics.Replace('#', 'D');
                                    return Diacritics.Replace('!', 'A');//تنوين فتحتين
                                }
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '1' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                        case '3':
                            if ((Meaning[1] == '1' || Meaning[1]=='0') && (Meaning.Length > 6 && Meaning[6] != '1' || Meaning.Length <= 6))//نكرة وليس مضافا
                            {
                                Diacritics = Diacritics.Replace('#', 'F');
                                return Diacritics.Replace('!', 'C');//تنوين كسرتين
                            }
                            Diacritics = Diacritics.Replace('#', '6');
                            return Diacritics.Replace('!', '3');
                    }
                    break;
                case 'V':
                    //if (Meaning[1] != '2') return Diacritics;//الإعراب للفعل المضارع فقط
                    switch (Meaning[1])
                    {
                        case '1':
                            if (Meaning.Length < 8 || Meaning[7] == '1' || Meaning[7] == '0')
                            {
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '1' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                            else if (Meaning[7] == '2')
                            {
                                Diacritics = Diacritics.Substring(0, position) + '0' + Diacritics.Substring(position + 1);
                            }
                            break;
                        case '2':
                            if (Meaning.Length < 8 || Meaning[7] == '2' || Meaning[7] == '0')
                            {
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '5' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '2' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                            switch (Meaning[7])
                            {
                                case '1':
                                    if (Diacritics[position] == '#')
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                    }
                                    else
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '1' + Diacritics.Substring(position + 1);
                                    }
                                    goto default;
                                case '3':
                                    if (Diacritics[position] == '#')
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                    }
                                    else
                                    {
                                        Diacritics = Diacritics.Substring(0, position) + '0' + Diacritics.Substring(position + 1);
                                    }
                                    goto default;
                                default:
                                    return Diacritics.Replace('!', '2');
                            }
                        case '3':
                            if (Meaning.Length < 8 || Meaning[7] == '1' || Meaning[7] == '0')
                            {
                                if (Diacritics[position] == '#')
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '4' + Diacritics.Substring(position + 1);
                                }
                                else
                                {
                                    Diacritics = Diacritics.Substring(0, position) + '0' + Diacritics.Substring(position + 1);
                                }
                                return Diacritics.Replace('!', '2');
                            }
                            else if (Meaning[7] == '2')
                            {
                                Diacritics = Diacritics.Substring(0, position) + '3' + Diacritics.Substring(position + 1);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            return Diacritics;
        }

        public static bool CompareMeanings(string first, string second, out string result)
        {
            //اختبار توافق المعنى وإرجاع المعنى الناتج
            string longest, shortest;
            StringBuilder merged = new StringBuilder();
            if (first.Length > second.Length)
            {
                longest = first;
                shortest = second;
            }
            else
            {
                longest = second;
                shortest = first;
            }

            for (int i = 0; i < shortest.Length; i++)
            {

                if (shortest[i] != longest[i] && longest[i] != '0' && shortest[i] != '0')
                {
                    result = "";
                    return false;
                }
                else if (longest[i] == '0')
                {
                    merged.Append(shortest[i]);
                }
                else
                {
                    merged.Append(longest[i]);
                }
            }
            merged.Append(longest.Substring(shortest.Length));
            result = merged.ToString();
            return true;
        }

        public static string MeaningOf(string Symbol) //تحويل المعنى المرمز إلى نص مقروء
        {
            if (Symbol == null || Symbol == "") return "";
            Symbol = Symbol.ToUpper();
            StringBuilder sb = new StringBuilder();
            if (Symbol.Length > 0)
                switch (Symbol[0])
                {
                    case 'N':
                        sb.Append("اسم");
                        if (Symbol.Length > 1)
                            switch (Symbol[1])
                            {
                                case '1':
                                    sb.Append(" نكرة");
                                    break;
                                case '2':
                                    sb.Append(" معرفة");
                                    break;
                                case '3':
                                    sb.Append(" إشارة للقريب");
                                    break;
                                case '4':
                                    sb.Append(" إشارة للبعيد");
                                    break;
                                case '5':
                                    sb.Append(" موصول");
                                    break;
                                case '6':
                                    sb.Append("، ضمير متكلم");
                                    break;
                                case '7':
                                    sb.Append("، ضمير مخاطب");
                                    break;
                                case '8':
                                    sb.Append("، ضمير غائب");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 3)
                            switch (Symbol[3])
                            {
                                case '1':
                                    sb.Append("، مفرد");
                                    break;
                                case '2':
                                    sb.Append("، مثنى");
                                    break;
                                case '3':
                                    sb.Append("، جمع سالم");
                                    break;
                                case '4':
                                    sb.Append("، جمع تكسير");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 4)
                            switch (Symbol[4])
                            {
                                case '1':
                                    sb.Append("، مذكر");
                                    break;
                                case '2':
                                    sb.Append("، مؤنث");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 5)
                            switch (Symbol[5])
                            {
                                case '1':
                                    sb.Append("، منصوب");
                                    break;
                                case '2':
                                    sb.Append("، مرفوع");
                                    break;
                                case '3':
                                    sb.Append("، مجرور");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 6)
                            switch (Symbol[6])
                            {
                                case '1':
                                    sb.Append("، مضاف");
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 7)
                            switch (Symbol[7])
                            {
                                case '1':
                                    sb.Append("، منصرف");
                                    break;
                                case '2':
                                    sb.Append("، ممنوع من الصرف");
                                    break;
                            }
                        sb.Append('.');
                        break;
                    case 'V':
                        sb.Append("فعل");
                        if (Symbol.Length > 1)
                            switch (Symbol[1])
                            {
                                case '1':
                                    sb.Append(" ماض");
                                    break;
                                case '2':
                                    sb.Append(" مضارع");
                                    break;
                                case '3':
                                    sb.Append(" أمر");
                                    break;
                                default:
                                    return sb.ToString();
                            }
                        else break;
                        if (Symbol.Length > 2)
                            switch (Symbol[2])
                            {
                                case '1':
                                    sb.Append(" لازم");
                                    break;
                                case '2':
                                    sb.Append(" متعد");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 3)
                        {
                            if (Symbol[1] != '3')//ليس فعل أمر
                            {
                                switch (Symbol[3])
                                {
                                    case '1':
                                        sb.Append(" والفاعل متكلم");
                                        break;
                                    case '2':
                                        sb.Append(" والفاعل مخاطب");
                                        break;
                                    case '3':
                                        sb.Append(" والفاعل غائب");
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        else break;
                        if (Symbol.Length > 4)
                            switch (Symbol[4])
                            {
                                case '1':
                                    sb.Append(" مفرد");
                                    break;
                                case '2':
                                    sb.Append(" مثنى");
                                    break;
                                case '3':
                                    sb.Append(" جمع");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 5)
                            switch (Symbol[5])
                            {
                                case '1':
                                    sb.Append(" مذكر");
                                    break;
                                case '2':
                                    sb.Append(" مؤنث");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 6)
                            switch (Symbol[6])
                            {
                                case '1':
                                    sb.Append(" مبني للمعلوم");
                                    break;
                                case '2':
                                    sb.Append(" مبني للمجهول");
                                    break;
                                default:
                                    break;
                            }
                        else break;
                        if (Symbol.Length > 7)
                            if (Symbol[1] == '2')//خاص بإعراب المضارع
                            {
                                switch (Symbol[7])
                                {
                                    case '1':
                                        sb.Append(" منصوب");
                                        break;
                                    case '2':
                                        sb.Append(" مرفوع");
                                        break;
                                    case '3':
                                        sb.Append(" مجزوم");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else break;
                        sb.Append(".");
                        break;
                    case 'T':
                        if (Symbol.Length > 1)
                            switch (Symbol[1])
                            {
                                case '1':
                                    sb.Append("حرف جر.");
                                    break;
                                case '2':
                                    sb.Append("حرف عطف.");
                                    break;
                                case '3':
                                    sb.Append("أداة نفي.");
                                    break;
                                case '4':
                                    sb.Append("أداة استفهام.");
                                    break;
                                case '5':
                                    sb.Append("أداة شرط.");
                                    break;
                                case '6':
                                    sb.Append("أداة استثناء.");
                                    break;
                                case '7':
                                    sb.Append("من إن وأخواتها.");
                                    break;
                                case '8':
                                    sb.Append("ظرف مكان أو زمان.");
                                    break;
                                default:
                                    sb.Append("حرف.");
                                    break;
                            }

                        break;
                    default:
                        switch (Symbol)
                        {
                            case "PN":
                                sb.Append("(اسم علم)");
                                break;
                            case "ST1":
                                sb.Append("(تجزم الفعل المضارع)");
                                break;
                            default:
                                sb.Append("فشل تفسير الكلمة.");
                                break;
                        }
                        break;
                }
            return sb.ToString();
        }
    }
}
