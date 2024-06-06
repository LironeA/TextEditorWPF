using TextEditorWPF.Model;

namespace TestProject1
{
    public class Tests
    {

        private ParserCore _parserCore;

        [SetUp]
        public void Setup()
        {
            _parserCore = new ParserCore();
        }

        [Test]
        public void SimleTestOfTokenisation()
        {

            var result = _parserCore.Tokenize("<root></root>");
            var expectedResult = new List<Token> { new Token() { RawData = "root", Type = TokenType.Open }, new Token() { RawData = "root", Type = TokenType.Close }, };
            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        [TestCase("<root></root>", new string[] { "root", "root" }, new TokenType[] { TokenType.Open, TokenType.Close })]
        [TestCase("<p></p>", new string[] { "p", "p" }, new TokenType[] { TokenType.Open, TokenType.Close })]
        [TestCase("<p>Текст</p>", new string[] { "p", "Текст", "p" }, new TokenType[] { TokenType.Open, TokenType.Content, TokenType.Close })]
        [TestCase("<root><p>Текст</p></root>", new string[] { "root", "p", "Текст", "p", "root" }, new TokenType[] { TokenType.Open, TokenType.Open, TokenType.Content, TokenType.Close, TokenType.Close })]
        public void SimpleTestOfTokenisation(string input, string[] expectedRawData, TokenType[] expectedTypes)
        {
            var expectedResult = expectedRawData.Zip(expectedTypes, (data, type) => new Token { RawData = data, Type = type }).ToList();

            var result = _parserCore.Tokenize(input);

            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        [Test]
        public void SimleTestOfAssigningLevel()
        {

            var result = _parserCore.Tokenize("<root></root>");
            _parserCore.AsignLevels(result);

            var expectedResult = new List<Token> { new Token() { RawData = "root", Type = TokenType.Open, Level = 0 }, new Token() { RawData = "root", Type = TokenType.Close, Level = 0 }, };
            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        [TestCase(
        new string[] { "root", "root" },
        new TokenType[] { TokenType.Open, TokenType.Close },
        new int[] { 0, 0 }
        )]
        [TestCase(
        new string[] { "root", "p", "текс", "p", "root" },
        new TokenType[] { TokenType.Open, TokenType.Open, TokenType.Content, TokenType.Close, TokenType.Close },
        new int[] { 0, 1, 1, 1, 0 }
        )]
        public void SimpleTestOfAssigningLevel(string[] rawData, TokenType[] types, int[] expectedLevels)
        {
            var tokens = rawData.Zip(types, (data, type) => new Token { RawData = data, Type = type }).ToList();
            var expectedResult = tokens.Zip(expectedLevels, (token, level) => { token.Level = level; return token; }).ToList();

            _parserCore.AsignLevels(tokens);

            Assert.IsTrue(tokens.SequenceEqual(expectedResult));
        }

        [Test]
        public void CreateTree_SimpleStructure_ShouldReturnCorrectTree()
        {
            // Arrange
            var tokens = new List<Token>
        {
            new Token { RawData = "root", Type = TokenType.Open, Level = 0 },
            new Token { RawData = "root", Type = TokenType.Close, Level = 0 }
        };

            var expectedResult = new List<Token>
        {
            new Token
            {
                RawData = "root",
                Type = TokenType.Open,
                Level = 0,
                Children = new List<Token>()
            }
        };

            // Act
            var result = _parserCore.CreateTree(tokens);

            // Assert
            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        [Test]
        public void CreateTree_NestedStructure_ShouldReturnCorrectTree()
        {
            // Arrange
                var tokens = new List<Token>
            {
                new Token { RawData = "root", Type = TokenType.Open, Level = 0 },
                new Token { RawData = "child", Type = TokenType.Open, Level = 1 },
                new Token { RawData = "child", Type = TokenType.Close, Level = 1 },
                new Token { RawData = "root", Type = TokenType.Close, Level = 0 }
            };

            var expectedResult = new List<Token>
            {
                new Token
                {
                    RawData = "root",
                    Type = TokenType.Open,
                    Level = 0,
                    Children = new List<Token>
                    {
                        new Token
                        {
                            RawData = "child",
                            Type = TokenType.Open,
                            Level = 1,
                            Children = new List<Token>()
                        }
                    }
                }
            };

            var result = _parserCore.CreateTree(tokens);

            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        [Test]
        public void CreateTree_MultipleNestedLevels_ShouldReturnCorrectTree()
        {
            var tokens = new List<Token>
            {
                new Token { RawData = "root", Type = TokenType.Open, Level = 0 },
                new Token { RawData = "child1", Type = TokenType.Open, Level = 1 },
                new Token { RawData = "child2", Type = TokenType.Open, Level = 2 },
                new Token { RawData = "child2", Type = TokenType.Close, Level = 2 },
                new Token { RawData = "child1", Type = TokenType.Close, Level = 1 },
                new Token { RawData = "root", Type = TokenType.Close, Level = 0 }
            };

            var expectedResult = new List<Token>
            {
                new Token
                {
                    RawData = "root",
                    Type = TokenType.Open,
                    Level = 0,
                    Children = new List<Token>
                    {
                        new Token
                        {
                            RawData = "child1",
                            Type = TokenType.Open,
                            Level = 1,
                            Children = new List<Token>
                            {
                                new Token
                                {
                                    RawData = "child2",
                                    Type = TokenType.Open,
                                    Level = 2,
                                    Children = new List<Token>()
                                }
                            }
                        }
                    }
                }
            };

            var result = _parserCore.CreateTree(tokens);

            Assert.IsTrue(result.SequenceEqual(expectedResult));
        }

        private bool AreEqual(List<Element> first, List<Element> second)
        {
            if (first == null && second == null)
                return true;

            if (first == null || second == null)
                return false;

            if (first.Count != second.Count)
                return false;

            for (int i = 0; i < first.Count; i++)
            {
                if (!first[i].Equals(second[i]))
                    return false;
            }

            return true;
        }

        [Test]
        public void CreateElementTree_NullTokens_ShouldReturnNull()
        {
            // Act
            var result = _parserCore.CreateElementTree(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void CreateElementTree_SimpleTokens_ShouldReturnCorrectElements()
        {
            var tokens = new List<Token>
            {
                new Token { RawData = "root", Type = TokenType.Open, Level = 0 },
            };

            var expectedResult = new List<Element>
            {
                new RootElement("root")
            };

            var result = _parserCore.CreateElementTree(tokens);

            Assert.IsTrue(AreEqual(result, expectedResult));
        }

        [Test]
        public void CreateElementTree_NestedTokens_ShouldReturnCorrectElementTree()
        {
            var tokens = new List<Token>
            {
                new Token { RawData = "root", Type = TokenType.Open, Level = 0, Children = new List<Token>
                    {
                        new Token { RawData = "p", Type = TokenType.Open, Level = 1 }
                    }
                }
            };

            var expectedResult = new List<Element>
            {
                new RootElement("root")
                {
                    ChildElements = new List<Element>
                    {
                        new ParagraphElement("p")
                    }
                }
            };

            var result = _parserCore.CreateElementTree(tokens);

            Assert.IsTrue(AreEqual(result, expectedResult));
        }

    }

}