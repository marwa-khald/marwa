using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Mansour
{
    public class WordPrefix
    {
        public WordPrefix()
        {
            Meaning = Text = WordClass = Tashkeel = "";
        }
        public string Meaning { get; set; }
        public string Text { get; set; }
        public string WordClass { get; set; }
        public string Tashkeel { get; set; }
        public static List<WordPrefix> CheckPrefix(string Word)
        {
            string[] prefixes = { "ا", "ي", "ت", "ن", "س", "ب", "ل", "ك", "ف", "و", "أ" };
            List<WordPrefix> ValidPrefixes = new List<WordPrefix>();
            for (int i = 0; i < prefixes.Length; i++) //هل تبدأ الكلمة بأحد حروف السوابق؟
            {
                if (Word.StartsWith(prefixes[i], StringComparison.Ordinal))  // the two strings must match character for character: "and" does not equal "And" or "AND"
                {
                    OleDbCommand com = new OleDbCommand();
                    com.Connection = Analyzer.con;
                    com.CommandText = "select * from prefixes where StrComp( Left( add , 1 ),'" + prefixes[i] + "',0)=0";
                    OleDbDataReader dread = com.ExecuteReader();

                    while (dread.Read())
                    {
                        if (Word.StartsWith(dread[0].ToString(), StringComparison.Ordinal))
                        {
                            WordPrefix p = new WordPrefix();
                            p.Text = dread["Add"].ToString();
                            p.Tashkeel = dread["Diacritics"].ToString();
                            p.WordClass = dread["Class"].ToString();
                            p.Meaning = dread["Meaning"].ToString();
                            ValidPrefixes.Add(p);
                        }
                    }
                    dread.Close();
                    break;
                }
            }
            return ValidPrefixes;
        }
    }


}
