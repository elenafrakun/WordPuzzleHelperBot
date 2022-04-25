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

        [TestCase("אבג")]
        [TestCase("אבגדה")]
        [TestCase("אבגדהו¸")]
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
        [TestCase("אב")]
        [TestCase("אבגדהו¸זח")]
        [TestCase("אבגד5ו¸")]
        [TestCase("!בגדה")]
        [TestCase("sבדא")]
        public void ValidateLettersInput_ShoudReturnFalse(string letters)
        {
            // arrange
            var service = new WordsBuilder();

            // act
            var result = service.ValidateUserInput(letters);

            // assert
            Assert.IsFalse(result);
        }

        [TestCase("ךועקמנ")]
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