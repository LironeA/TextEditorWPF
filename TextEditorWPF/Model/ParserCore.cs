using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TextEditorWPF.Model
{
    public static class ParserCore
    {

        public static Document ProccesText(string text)
        {
            try
            {
                var doc = new Document() { RawData = text };

                var tokens = Tokenize(doc.RawData);
                AsignLevels(tokens);

                var tree = CreateTree(tokens);
                doc.Elements = CreateElementTree(tree);

                return doc;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
           
        }




        private static readonly Regex DivadersRegex = new Regex(@"([\|\&\\])|([\(\)\^\/\<\>\\\s])");
        private static readonly Regex match = new Regex(@"[<][\/]?\w+[>]|[^<|^>|\s]+");
        private static readonly Regex match3 = new Regex(@"(?'content'[^<|>|\/]+)|(?'close'[<][\/]([^<|>|\/|\s]+)[>])|(?'open'[<]([^<|>|\/|\s]+)\s*([\s]*([^<|>|\/|\s]+)[=][""""]([^<|>|\/|\s]+)*[""""][\s]?)*[>])");
        private static readonly Regex matchPropTag = new Regex(@"(?'openprop'[<](?'tagname'[^<|>|\/|\s]+)(?>\s)*(?'prop'[\s]?(?'propName'[^<|>|\/|\s]+)[=][""""](?'propValue'[^<|>|\/|\s]+)*[""""](?>[\s]*))*[>])");
 
        private static Regex _elementOpen = new Regex("<[A-Za-z]>", RegexOptions.IgnoreCase);
        private static Regex _elementClose = new Regex("</[A-Za-z]>", RegexOptions.IgnoreCase);

        public static List<Token> Tokenize(string text)
        {
            var result = new List<Token>();
            text = Regex.Replace(text, @"\r|\n", " ");
            MatchCollection? matchges = match3.Matches(text);
            foreach (Match item in matchges)
            {
                if(String.IsNullOrEmpty(item.Value.Trim()))
                {
                    continue;
                }
                var token = new Token();
                result.Add(token);
                token.RawData = item.Value.Trim();

                var openGroup = item.Groups["open"];
                if(openGroup is not null && openGroup.Success)
                {
                    token.Type = TokenType.Open;
                    var propMatches = matchPropTag.Matches(token.RawData);

                    token.RawData = propMatches[0].Groups["tagname"].Value;
                    if(propMatches[0].Groups["prop"].Captures.Count > 0)
                    {
                        token.Properties = new List<TokenProperty>();
                        for (int i = 0; i < propMatches[0].Groups["prop"].Captures.Count; i++)
                        {
                            var prop = new TokenProperty();
                            prop.Name = propMatches[0].Groups["propName"].Captures[i].Value;
                            prop.Value = propMatches[0].Groups["propValue"].Captures[i].Value;
                            token.Properties.Add(prop);
                        }
                    }

                    continue;
                }

                var closeGroup = item.Groups["close"];
                if (closeGroup is not null && closeGroup.Success)
                {
                    token.Type = TokenType.Close;
                    token.RawData = token.RawData.Substring(2, token.RawData.Length - 3);
                    continue;
                }

                var contentGroup = item.Groups["content"];
                if (contentGroup is not null && contentGroup.Success)
                {
                    token.Type = TokenType.Content;
                    continue;
                }
            }
            return result;
        }

        public static void AsignLevels(List<Token> tokens)
        {
            int lavel = 0;
            foreach (var token in tokens)
            {
                if (token.Type == TokenType.Close)
                {
                    lavel--;
                }
                token.Level = lavel;
                if (token.Type == TokenType.Open)
                {
                    lavel++;
                }


            }
        }

        public static List<Token> CreateTree(List<Token> tokens, int level = 0)
        {
            var result = new List<Token>();

            foreach (var token in tokens.Where(x => x.Level == level && (x.Type == TokenType.Open || x.Type == TokenType.Content)))
            {
                if (token.Type == TokenType.Open)
                {
                    var indexStart = tokens.IndexOf(token);
                    var indexStop = tokens[indexStart..].FindIndex(x => x.Type == TokenType.Close && x.RawData == token.RawData);

                    token.Children = CreateTree(tokens[(indexStart + 1)..(indexStop + indexStart)], level + 1);
                }
                result.Add(token);
            }

            return result;
        }

        public static List<Element> CreateElementTree(List<Token> tokens)
        {
            if (tokens is null) return null;
            var result = new List<Element>();
            foreach (var token in tokens)
            {
                var element = Element.Create(token);
                element.ChildElements = CreateElementTree(token.Children);
                result.Add(element);
            }

            return result;
        }
    }
}


