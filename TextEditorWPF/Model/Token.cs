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
        public List<TokenProperty> Properties { get; set; }
        public List<Token> Children { get; set; }

        public bool Equals(Token other)
        {
            if (other == null) return false;

            bool childrenEqual;
            if (Children == null && other.Children == null)
            {
                childrenEqual = true;
            }
            else if (Children == null || other.Children == null)
            {
                childrenEqual = false;
            }
            else
            {
                childrenEqual = Children.SequenceEqual(other.Children);
            }

            return RawData == other.RawData && Type == other.Type && Level == other.Level && childrenEqual;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Token);
        }


        public override int GetHashCode()
        {
            return (RawData, Type, Level).GetHashCode();
        }
    }

    public enum TokenType
    {
        None = 0,
        Open = 1,
        Close = 2,
        Content = 3,
    }

    public class TokenProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
