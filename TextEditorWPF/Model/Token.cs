using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditorWPF.Model
{
    public class Token
    {
        public string RawData { get; set; }
        public int Level { get; set; }
        public TokenType Type { get; set; }
        public List<Token> Children { get; set;}
    }

    public enum TokenType
    {
        None = 0,
        Open = 1,
        Close = 2,
        Content = 3
    }
}
