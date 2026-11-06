using CrystalLens.Models.Expression;

namespace CrystalLensTests
{
    [TestClass]
    public sealed class ExpressionTests
    {
        private static readonly string FailedMessage = "Expression not evaluated correctly";

        private void AssertExpression(string expression, int expected)
        {
            int actual = ExpressionHelper.Evaluate(ExpressionHelper.Tokenize(expression));
            Assert.AreEqual(expected, actual, FailedMessage);
        }

        [TestMethod]
        public void Evaluate_Addition_Binary()
        {
            AssertExpression("10 + 9", 19);
        }

        [TestMethod]
        public void Evaluate_Addition_Trinary()
        {
            AssertExpression("10 + 9 + 6", 25);
        }

        [TestMethod]
        public void Evaluate_Addition_WithMultiplication()
        {
            AssertExpression("2 + 5 * 4", 22);
        }

        [TestMethod]
        public void Evaluate_Subtraction_Binary()
        {
            AssertExpression("21 - 9", 12);
        }

        [TestMethod]
        public void Evaluate_Subtraction_Trinary()
        {
            AssertExpression("21 - 9 - 4", 8);
        }

        [TestMethod]
        public void Evaluate_Multiplication_Binary()
        {
            AssertExpression("6 * 9", 54);
        }

        [TestMethod]
        public void Evaluate_Multiplication_Quadrary()
        {
            AssertExpression("3 * 2 * 9 * 1", 54);
        }

        [TestMethod]
        public void Evaluate_Multiplication_WithAddition()
        {
            AssertExpression("(1 + 2) * (3 + 4)", 21);
        }

        [TestMethod]
        public void Evaluate_Division_Binary()
        {
            AssertExpression("54 / 9", 6);
        }

        [TestMethod]
        public void Evaluate_Division_Quadrary()
        {
            AssertExpression("54 / 9 / 1 / 3", 2);
        }

        [TestMethod]
        public void Evaluate_Division_WithSubtraction()
        {
            AssertExpression("(8 - 2) / (5 - 2)", 2);
        }

        [TestMethod]
        public void Evaluate_Division_Truncated()
        {
            AssertExpression("5 / 2", 2);
        }

        [TestMethod]
        public void Evaluate_LogicalOr_TrueTrue()
        {
            AssertExpression("1 || 2", 1);
        }

        [TestMethod]
        public void Evaluate_LogicalOr_TrueFalse()
        {
            AssertExpression("3 || 0", 1);
        }

        [TestMethod]
        public void Evaluate_LogicalOr_FalseFalse()
        {
            AssertExpression("0 || 0", 0);
        }

        [TestMethod]
        public void Evaluate_LogicalAnd_TrueTrue()
        {
            AssertExpression("1 && 2", 1);
        }

        [TestMethod]
        public void Evaluate_LogicalAnd_TrueFalse()
        {
            AssertExpression("3 && 0", 0);
        }

        [TestMethod]
        public void Evaluate_LogicalAnd_FalseFalse()
        {
            AssertExpression("0 && 0", 0);
        }

        [TestMethod]
        public void Evaluate_Equals_True()
        {
            AssertExpression("2 == 2", 1);
        }

        [TestMethod]
        public void Evaluate_Equals_False()
        {
            AssertExpression("2 == 1", 0);
        }

        [TestMethod]
        public void Evaluate_NotEquals_True()
        {
            AssertExpression("2 != 1", 1);
        }

        [TestMethod]
        public void Evaluate_NotEquals_False()
        {
            AssertExpression("2 != 2", 0);
        }

        [TestMethod]
        public void Evaluate_GreaterThan_True()
        {
            AssertExpression("2 > 1", 1);
        }

        [TestMethod]
        public void Evaluate_GreaterThan_False()
        {
            AssertExpression("2 > 2", 0);
        }

        [TestMethod]
        public void Evaluate_GreaterThanEqual_True()
        {
            AssertExpression("2 >= 2", 1);
        }

        [TestMethod]
        public void Evaluate_GreaterThanEqual_False()
        {
            AssertExpression("1 >= 2", 0);
        }

        [TestMethod]
        public void Evaluate_LessThan_True()
        {
            AssertExpression("1 < 2", 1);
        }

        [TestMethod]
        public void Evaluate_LessThan_False()
        {
            AssertExpression("2 < 2", 0);
        }

        [TestMethod]
        public void Evaluate_LessThanEqual_True()
        {
            AssertExpression("2 <= 2", 1);
        }

        [TestMethod]
        public void Evaluate_LessThanEqual_False()
        {
            AssertExpression("2 <= 1", 0);
        }

        [TestMethod]
        public void Evaluate_BitwiseAnd_Binary()
        {
            AssertExpression("7 & 13", 5);
        }

        [TestMethod]
        public void Evaluate_BitwiseAnd_Trinary()
        {
            AssertExpression("7 & 13 & 6", 4);
        }

        [TestMethod]
        public void Evaluate_BitwiseOr_Binary()
        {
            AssertExpression("10 | 3", 11);
        }

        [TestMethod]
        public void Evaluate_BitwiseOr_Trinary()
        {
            AssertExpression("10 | 3 | 4", 15);
        }

        [TestMethod]
        public void Evaluate_BitwiseXor_Binary()
        {
            AssertExpression("6 ^ 3", 5);
        }

        [TestMethod]
        public void Evaluate_BitwiseXor_Trinary()
        {
            AssertExpression("6 ^ 3 ^ 9", 12);
        }

        [TestMethod]
        public void Evaluate_UnaryPlus_Single()
        {
            AssertExpression("+4", 4);
        }

        [TestMethod]
        public void Evaluate_UnaryMinus_Single()
        {
            AssertExpression("-4", -4);
        }

        [TestMethod]
        public void Evaluate_Negation_True()
        {
            AssertExpression("!0", 1);
        }

        [TestMethod]
        public void Evaluate_Negation_False()
        {
            AssertExpression("!2", 0);
        }

        [TestMethod]
        public void Evaluate_Exponent_Binary()
        {
            AssertExpression("2 ** 3", 8);
        }

        [TestMethod]
        public void Evaluate_Exponent_Trinary()
        {
            AssertExpression("2 ** 3 ** 2", 512);
        }

        [TestMethod]
        public void Evaluate_HexNumber_Simple()
        {
            AssertExpression("$7f", 0x7F);
        }

        [TestMethod]
        public void Evaluate_BinaryNumber_Simple()
        {
            AssertExpression("%1011", 0b1011);
        }

        [TestMethod]
        public void Evaluate_High_Simple()
        {
            AssertExpression("HIGH($abcd)", 0xAB);
        }

        [TestMethod]
        public void Evaluate_Low_Simple()
        {
            AssertExpression("LOW($abcd)", 0xCD);
        }

        [TestMethod]
        public void Evaluate_Bitwidth_NonZero()
        {
            AssertExpression("BITWIDTH(%10001)", 5);
        }
    }
}
