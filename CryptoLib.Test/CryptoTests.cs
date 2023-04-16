using Xunit.Abstractions;
using FluentAssertions;
using System.Security.Cryptography;

namespace CryptoLib.Test;

public class CryptoTests
{
    private readonly ITestOutputHelper _output;

    public CryptoTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ValidateHelperWithInvalidAlgName()
    {
        CryptoHelper.SymmetricAlgorithmName = "blorf";

        Action act = () => { int ks= CryptoHelper.KeySize; };

        string msg = "Error Creating Algorithem 'blorf'";

        act.Should().Throw<CryptographicException>()
            .WithMessage(msg);
    }

    [Fact]
    public void ValidateHelperAlternativeAlgorithm()        
    {
        CryptoHelper.SymmetricAlgorithmName = "Rijndael";
        int ks = CryptoHelper.KeySize;

        ks.Should().NotBe(0);
    }

    [Fact]
    public void DefaultKeySize()
    {
        int expectedKeySize = 256;
        int keySize = CryptoHelper.KeySize;
        _output.WriteLine("KeySize: {0}", keySize);
        keySize.Should().Be(expectedKeySize, "Default size should be 256 bits");
    }


    [Fact]
    public void AesRandomizesKeyForEachInstance()
    {

        CryptoKey key1 = CryptoHelper.GenerateCryptoKey();
        CryptoKey key2 = CryptoHelper.GenerateCryptoKey();

        key1.IV.Should().NotBeEquivalentTo(key2.IV, "IV is not being randomized from within helper");
        key1.Key.Should().NotBeEquivalentTo(key2.Key, "keys are not being randomized");

        _output.WriteLine("keyLen:{0} IVLen:{1}", key1.Key.Length, key1.IV.Length);
    }

    [Fact]
    public void ValidateKeyCombination()
    {
        CryptoKey key1 = new CryptoKey(new byte[] { 0x1, 0x2, 0x3, 0x4 }, new byte[] { 0x5, 0x6, 0x7, 0x8 });

        _output.WriteLine("Original Key:{0}",string.Join(",", key1.Key));

        CryptoKey key2 = new CryptoKey(new byte[] { 0xA0, 0xB0, 0xC0, 0xD0 }, new byte[] { 0x10, 0x20, 0x30, 0x40 });

        byte[] expectedKey = new byte[] { 0xA1, 0xB2, 0xC3, 0xD4 };
        byte[] expectedIV = new byte[] { 0x15, 0x26, 0x37, 0x48 };

        key1.CombineKey(key2);

        key1.Key.Should().BeEquivalentTo(expectedKey);
        key1.IV.Should().BeEquivalentTo(expectedIV);

        //just wanted to visualize the values between combined and  expected
        _output.WriteLine("Combined Key:{0}", string.Join(",", key1.Key));
        _output.WriteLine("ExpectedKey Key:{0}", string.Join(",", expectedKey));

    }

    [Fact]
    public void ValidateCryptoStringMethods()
    {
        CryptoKey key = CryptoHelper.GenerateCryptoKey();

        string testString = "The Quick Brown Fox Jumped Over The Lazy Dog";
        string cipherText = CryptoHelper.EncryptStringToString(testString, key);

        string clearText = CryptoHelper.DecryptoStringToString(cipherText, key);

        cipherText.Should().NotBeNullOrEmpty();
        clearText.Should().Be(testString, "returned cleartext should match the original string");
    }
}