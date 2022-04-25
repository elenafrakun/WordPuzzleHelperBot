using NUnit.Framework;
using WordPuzzleHelperBot.Classes;

namespace WordPuzzleHelperBot.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("���")]
        [TestCase("�����")]
        [TestCase("������")]
        public void ValidateLettersInput_ShoudReturnTrue(string letters)
        {
            // arrange
            var service = new WordsBuilder();

            // act
            var result = service.ValidateUserInput(letters);

            // assert
            Assert.IsTrue(result);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("��")]
        [TestCase("��������")]
        [TestCase("����5�")]
        [TestCase("!����")]
        [TestCase("s���")]
        public void ValidateLettersInput_ShoudReturnFalse(string letters)
        {
            // arrange
            var service = new WordsBuilder();

            // act
            var result = service.ValidateUserInput(letters);

            // assert
            Assert.IsFalse(result);
        }

        [TestCase("������")]
        public void FindWords_ShoudReturnTrue(string letters)
        {
            // arrange
            var service = new WordsBuilder();

            // act
            var result = service.FindWords(letters);

            // assert
            Assert.IsTrue(result);
        }
    }
}