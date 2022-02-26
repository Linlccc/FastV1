using System;
using Xunit;

namespace Fast_Test.Extensions_Test
{
    public class String_t
    {
        #region length

        [Fact]
        public void FullHalfLength_T1()
        {
            int a1 = "asdas".FullHalfLength();//5

            Assert.True(a1 == 5);
        }

        [Fact]
        public void FullHalfLength_T2()
        {
            int a2 = "1231".FullHalfLength();//4

            Assert.True(a2 == 4);
        }

        [Fact]
        public void FullHalfLength_T3()
        {
            int a3 = ",..';.".FullHalfLength();//6

            Assert.True(a3 == 6);
        }

        [Fact]
        public void FullHalfLength_T4()
        {
            int a4 = "你好".FullHalfLength();//4

            Assert.True(a4 == 4);
        }

        #endregion length

        #region chineseText

        [Fact]
        public void GetChineseText_T1()
        {
            string a1 = "asdas".GetChineseText();

            Assert.True(a1 == "");
        }

        [Fact]
        public void GetChineseText_T2()
        {
            string a2 = "1231".GetChineseText();//4

            Assert.True(a2 == "");
        }

        [Fact]
        public void GetChineseText_T3()
        {
            string a3 = ",..';.".GetChineseText();//6

            Assert.True(a3 == "");
        }

        [Fact]
        public void GetChineseText_T4()
        {
            string a4 = "你好".GetChineseText();//4

            Assert.True(a4 == "你好");
        }

        [Fact]
        public void GetChineseText_T5()
        {
            string a4 = "vcv你asda好..".GetChineseText();//4

            Assert.True(a4 == "你好");
        }

        #endregion chineseText

        [Fact]
        public void T1()
        {
            string str = "asdasd.aa";
            string a = str[str.LastIndexOf('.')..^0];

            int start = str.LastIndexOf(".");
            int length = str.Length;
            string postfix = str.Substring(start, length - start);

            Assert.Equal("sda", a);
        }
    }
}