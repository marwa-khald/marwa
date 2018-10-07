using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.OleDb;

namespace Mansour
{
    
    class Tashkeel
    {
        public static string Remove(string text)
        {
            StringBuilder temp = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case 'َ':
                    case 'ً':
                    case 'ُ':
                    case 'ٌ':
                    case 'ِ':
                    case 'ٍ':
                    case 'ْ':
                    case 'ّ':
                        continue;
                    default:
                        temp.Append(text[i]);
                        break;
                }
            }
            return temp.ToString();
        }

        public static string Guess(string WordToParse)// التشكيل بناء على تتابع الحروف
        {
            StringBuilder sb = new StringBuilder();
            byte syl = 0;
            for (int i = 0; i < WordToParse.Length; i++)
            {
                if (WordToParse[i] == 'إ')
                {
                    syl++;
                    sb.Append('3');
                    continue;
                }
                if (i < WordToParse.Length - 1)
                {
                    if (WordToParse[i + 1] == 'و')
                    {
                        sb.Append('2');
                        continue;
                    }
                    else if (WordToParse[i + 1] == 'ي')
                    {
                        sb.Append('3');
                        continue;
                    }
                    else if (WordToParse[i + 1] == 'ا')
                    {
                        sb.Append('1');
                        continue;
                    }
                }
                if (syl++ % 2 != 0 || (i < WordToParse.Length - 2 && (WordToParse[i + 2] == 'ي' || WordToParse[i + 2] == 'و' || WordToParse[i + 2] == 'ا'))
                    && i > 0 && (WordToParse[i - 1] == 'ي' || WordToParse[i - 1] == 'و' || WordToParse[i - 1] == 'ا'))
                {
                    sb.Append('0');
                    continue;
                }
                if ((sb.Length >= 1 && (sb[sb.Length - 1] == '1' || sb[sb.Length - 1] == '3' || sb[sb.Length - 1] == '2'))
                    && (WordToParse[i] == 'ي' || WordToParse[i] == 'و' || WordToParse[i] == 'ا'))
                {
                    sb.Append('7');
                    syl = 0;
                    continue;
                }
                sb.Append('1');
            }
            return sb.ToString();
        }

        public static bool CheckTashkeel(string word, string diacritics)
        {
            //اختبار توافق التشكيل مع حروف الكلمة
            //هذه الخوارزمية ليست كاملة
            word = word.Replace("آ", "أا");
            diacritics.Replace("8", "");
            if (word.Length != diacritics.Length) return false;
            for (int i = 0; i < word.Length; i++)
            {
                if ((word[i] == 'ا') && (i > 0) && ((diacritics[i] != '7') || ((diacritics[i - 1] != '1') && (diacritics[i - 1] != '4'))))
                {
                    return false;
                }
                if ((word[i] == 'ى') && diacritics[i - 1] != '1' && diacritics[i - 1] != '4')
                {
                    return false;
                }
                //if ((word[i] == 'ي') && (i == word.Length - 1) && (diacritics[i - 1] != '3'))
                //{
                //    return false;
                //}
                //if ((word[i] == 'و') && (i == word.Length - 1) && (diacritics[i - 1] != '2'))
                //{
                //    return false;
                //}
                if ((word[i] == 'ئ') && ((diacritics[i - 1] != '3') && (diacritics[i - 1] != '0') && (diacritics[i - 1] != '7')))
                {
                    return false;
                }
                if ((word[i] == 'ؤ') && ((diacritics[i - 1] != '2') && (diacritics[i - 1] != '0') && (diacritics[i - 1] != '7')))
                {
                    return false;
                }
                if ((word[i] == 'إ') && (diacritics[i] != '3'))
                {
                    return false;
                }
                if ((word[i] == 'أ') && ((diacritics[i] == '3') || ((i > 0) && (diacritics[i - 1] != '1') && (diacritics[i - 1] != '4'))))
                {
                    return false;
                }
                if (i < word.Length - 1 && word[i] == word[i + 1] && diacritics[i] == '0')//حرفين متشابهين أولهما ساكن
                    return false;
                if (i < word.Length - 1 && word[i] == 'و' && word[i + 1] == 'ي' && diacritics[i] == '0')
                    return false;
                if (i < word.Length - 1 && word[i] == 'ي' && word[i + 1] == 'و' && diacritics[i] == '0')
                    return false;
                if (i > 0 && i == word.Length - 2 && !IsArabicVowel(word[i + 1]))//لم تختبر جيدا
                {
                    if ((word[i] == 'و' || word[i] == 'ي') && diacritics[i] == '1' && diacritics[i - 1] == '1')
                    {
                        return false;
                    }
                    if ((word[i] == 'و' || word[i] == 'ي') && (diacritics[i] == '2' || diacritics[i] == '3') && diacritics[i - 1] == '0')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsArabicVowel(char Letter)
        {
            if (Letter == 'ي' || Letter == 'و' || Letter == 'ا' || Letter == 'ى')
                return true;
            return false;
        }

        public static bool SilentLam(char Letter)
        {
            if(("تثدذرزسشصضطظن").Contains(Letter)) return true;
            return false;
        }

        public static string DiacritizeWord(WordInfo Word)
        {
            StringBuilder sb = new StringBuilder();

            if (Word.Prefix.WordClass.StartsWith("N2"))
            {
                if (Tashkeel.SilentLam(Word.Word[0]))
                {
                    //لام شمسية
                    Word.Prefix.Tashkeel = Word.Prefix.Tashkeel.Substring(0, Word.Prefix.Tashkeel.Length - 1) + '9';
                    Word.Diacritics = (char)(Word.Diacritics[0] + 3) + Word.Diacritics.Substring(1);
                }
            }
            sb.Append(Word.Prefix.Tashkeel);
            if (Word.Suffix.ConnectedLetter != "")
            {
                if (Word.Diacritics.EndsWith("#"))
                {
                    Word.Diacritics = Word.Diacritics.Replace('#', (char)(Word.Suffix.ConnectedLetter[0] + 3));
                }
                else
                {
                    Word.Diacritics = Word.Diacritics.Replace('!', Word.Suffix.ConnectedLetter[0]);
                }
            }
            sb.Append(Word.Diacritics);
            sb.Append(Word.Suffix.Tashkeel);
            return sb.ToString();
        }
        public static string SetTashkeel(string word, string diacritics)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < word.Length; i++)
            {
                if (i == diacritics.Length)
                {
                    sb.Append(word.Substring(i));
                    break;
                }
                sb.Append(word[i]);
                if (word[i] == 'آ' || diacritics[i] == '8')
                {
                    diacritics = diacritics.Remove(i, 1);
                    continue;
                }
                if (word[i] == 'ا') continue;
                switch (diacritics[i])
                {
                    case '0':
                        sb.Append('ْ');
                        break;
                    case '1':
                        sb.Append('َ');
                        break;
                    case '2':
                        sb.Append('ُ');
                        break;
                    case '3':
                        sb.Append('ِ');
                        break;
                    case '4':
                        sb.Append("َّ");
                        break;
                    case '5':
                        sb.Append("ُّ");
                        break;
                    case '6':
                        sb.Append("ِّ");
                        break;
                    case 'A':
                        sb.Append('ً');
                        break;
                    case 'B':
                        sb.Append('ٌ');
                        break;
                    case 'C':
                        sb.Append('ٍ');
                        break;
                    case 'D':
                        sb.Append("ًّ");
                        break;
                    case 'E':
                        sb.Append("ٌّ");
                        break;
                    case 'F':
                        sb.Append("ٍّ");
                        break;
                    default:
                        break;
                }
            }
            return sb.ToString();
        }

        

    }
}
