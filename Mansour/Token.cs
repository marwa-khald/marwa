using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mansour
{
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
