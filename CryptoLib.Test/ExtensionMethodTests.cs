using FluentAssertions;
using System.Collections;

namespace CryptoLib.Test
{
    public class ExtensionMethodTests
    {
        [Fact]
        public void ValidateNullArrayCheckExtension()
        {
            string[] nullArr = null;
            nullArr.IsNullOrEmpty().Should().BeTrue("null array did not return a true result");
        }

        [Fact]
        public void ValidateEmptyArrayCheckExtension()
        {
            int[] emptyArr = new int[] {  };
            emptyArr.IsNullOrEmpty().Should().BeTrue("empty array did not return a true result");
        }
        
        [Fact]
        public void ValidateArrayWithElementsCheckExtension()
        {
            object[] populated = new object[] { new object() };
            populated.IsNullOrEmpty().Should().BeFalse("populated array didn't return false result");
        }

        [Fact]
        public void ValidateBitArrayToByteArrayWithNull()
        {
            BitArray ba = null;
            Action act = () => ba.ToByteArray();

            string msg = "Value cannot be null. (Parameter 'bit array is null')";

            act.Should().Throw<ArgumentNullException>()
                .WithMessage(msg);
        }

        [Fact]
        public void ValidateBitArrayToByteArrayProperlyConverts()
        {
            byte[] byteValues = new byte[] { 0x22, 0x33, 0x44, 0x55, 0x66, 0x77 };
            BitArray ba = new BitArray(byteValues);

            ba.ToByteArray().Should().BeEquivalentTo(byteValues);
        }
    }
}
