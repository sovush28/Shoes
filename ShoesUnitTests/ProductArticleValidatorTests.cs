using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Shoes;

namespace ShoesUnitTests
{
    [TestClass]
    public class ProductArticleValidatorTests
    {
        [TestMethod]
        public void CheckArticle_6symbolsHasLettersNumbers_ReturnsNull()
        {
            string article = "A1B2c3", expected = null;
            
            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_6symbolsHasLettersOnly_ReturnsNull()
        {
            string article = "AbCDEf", expected = null;

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_6symbolsHasNumbersOnly_ReturnsNull()
        {
            string article = "123456", expected = null;

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_5symbols_ReturnsMsg()
        {
            string article = "A1Bc3", expected = "Длина артикула должна составлять 6 символов";

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_7symbols_ReturnsMsg()
        {
            string article = "Q2W54T6", expected = "Длина артикула должна составлять 6 символов";

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_HasSpaceInTheMiddle_ReturnsMsg()
        {
            string article = "A12 d5", expected = "Артикул может содержать только латинские буквы и цифры";

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_HasCyrillic_ReturnsMsg()
        {
            string article = "QWы785", expected = "Артикул может содержать только латинские буквы и цифры";

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void CheckArticle_HasSpecialCharacter_ReturnsMsg()
        {
            string article = "Q23!t9", expected = "Артикул может содержать только латинские буквы и цифры";

            ProductArticleValidator pav = new ProductArticleValidator();
            string actual = pav.CheckArticle(article);

            Assert.AreEqual(expected, actual);
        }
    }
}
