using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Mansour
{

    public class WordSuffix
    {
        public WordSuffix()
        {
            Meaning = Text = WordClass = Tashkeel = ConnectedLetter = "";
        }
        public string Meaning { get; set; }
        public string Text { get; set; }
        public string WordClass { get; set; }
        public string Tashkeel { get; set; }
        public string ConnectedLetter { get; set; }
        public static List<WordSuffix> CheckSuffixes(string Word)
        {
            string[] suffixes = { "ة", "ت", "ا", "ن", "تم", "ك", "كم", "هم", "ه", "ي" };
            List<WordSuffix> ValidSuffixes = new List<WordSuffix>();
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (Word.EndsWith(suffixes[i], StringComparison.Ordinal)) // =word.endwith()==suffixes[i]
                {
                    OleDbCommand com = new OleDbCommand();
                    com.Connection = Analyzer.con;
                    com.CommandText = "select * from suffixes where StrComp( Right( add , " + suffixes[i].Length + "),'" + suffixes[i] + "',0)=0";
                    OleDbDataReader dread = com.ExecuteReader();
                    while (dread.Read())
                    {
                        if (Word.EndsWith(dread[0].ToString(), StringComparison.Ordinal))
                        {
                            WordSuffix s = new WordSuffix();
                            s.Text = dread["Add"].ToString();
                            s.Tashkeel = dread["Diacritics"].ToString();
                            s.WordClass = dread["Class"].ToString();
                            s.Meaning = dread["Meaning"].ToString();
                            s.ConnectedLetter = dread["WordLetter"].ToString();
                            ValidSuffixes.Add(s);
                        }
                    }
                    dread.Close();
                    break;
                }
            }
            return ValidSuffixes;
        }
    }

}
