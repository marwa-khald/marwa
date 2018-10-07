using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mansour
{
    public class ArabicWord
    {
        public string word
        {
            get
            {
                return Tashkeel.Remove(Original.Replace("ـ", ""));
            }
        }
        public string Original { get; set; }
        public int Start, End;
        public bool Corrected = false;
    }

    public class WordInfo
    {
        public WordInfo()
        {
            Prefix = new WordPrefix();
            Suffix = new WordSuffix();
            Word = "";
            Meaning = "";
            Template = "";
        }
        public WordPrefix Prefix { get; set; }
        public WordSuffix Suffix { get; set; }
        public ArabicRoot Root { get; set; }
        public string Diacritics { get; set; }
        public String Word { get; set; }
        public string Template { get; set; }
        public string Meaning { get; set; }
        public List<Interpretation> Interpretations { get; set; }
        public string InterpretedDiacritics
        {
            get
            {
                if (Interpretations != null && Interpretations.Count > 0)
                {
                    return Interpreter.FinishDiacritics(Prefix.Tashkeel + Diacritics + Suffix.Tashkeel, Interpretations[0].Meaning);
                }
                return FullDiacritics;
            }
        }

        public string FullDiacritics
        {
            get
            {
                return Interpreter.FinishDiacritics(Prefix.Tashkeel + Diacritics + Suffix.Tashkeel, Meaning);
            }
        }

        public override string ToString()
        {
            return Tashkeel.SetTashkeel(Prefix.Text + Word + Suffix.Text, InterpretedDiacritics);
        }
        public string CompleteMeaning
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.Append((Prefix.Meaning.Length > 0) ? Prefix.Meaning + "+" : "");
                sb.Append(Meaning);
                sb.Append((Suffix.Meaning.Length > 0) ? "+" + Suffix.Meaning : "");
                return sb.ToString();
            }
        }
        public string SpecialClass { get; set; }
    }

    public struct ArabicRoot
    {
        public enum Derivatives : byte
        {
            غير_محدد,
            مصدر,
            اسم_فاعل,
            اسم_مفعول,
            اسم_مفرد,
            اسم_جمع
        }
        public Derivatives DerivationType { get; set; }
        public string Root { get; set; }
        public bool IsCompatible { get; set; }
        public string DerivationRules { get; set; }
    }
}
