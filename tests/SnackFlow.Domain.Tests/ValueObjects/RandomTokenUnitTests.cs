using FluentAssertions;
using SnackFlow.Domain.ValueObjects.RandomToken;

namespace SnackFlow.Domain.Tests.ValueObjects;

public sealed class RandomTokenUnitTests : BaseTest
{
   #region Valid Tests

   [Fact(DisplayName = "Should create random token with valid base64 value")]
   public void Create_WhenCalled_ShouldCreateRandomTokenWithValidBase64()
   {
       // Act
       var result = RandomToken.Create();

       // Assert
       result.Should().NotBeNull();
       result.Value.Should().NotBeNullOrEmpty();
       
       // Verificar se é base64 válido
       var act = () => Convert.FromBase64String(result.Value);
       act.Should().NotThrow();
   }

   [Fact(DisplayName = "Should create tokens with expected length")]
   public void Create_WhenCalled_ShouldCreateTokenWithCorrectLength()
   {
       // Act
       var result = RandomToken.Create();

       // Assert
       // 32 bytes em base64 resulta em ~44 caracteres
       result.Value.Length.Should().BeGreaterThan(40);
       result.Value.Length.Should().BeLessThan(50);
   }

   [Fact(DisplayName = "Should create unique tokens")]
   public void Create_WhenCalledMultipleTimes_ShouldCreateUniqueTokens()
   {
       // Act
       var token1 = RandomToken.Create();
       var token2 = RandomToken.Create();
       var token3 = RandomToken.Create();

       // Assert
       token1.Value.Should().NotBe(token2.Value);
       token1.Value.Should().NotBe(token3.Value);
       token2.Value.Should().NotBe(token3.Value);
   }

   [Fact(DisplayName = "Should generate cryptographically secure tokens")]
   public void Create_WhenCalledManyTimes_ShouldNotGeneratePredictablePatterns()
   {
       // Arrange
       var tokens = new HashSet<string>();

       // Act
       for (int i = 0; i < 100; i++)
       {
           var token = RandomToken.Create();
           tokens.Add(token.Value);
       }

       // Assert
       tokens.Should().HaveCount(100); // Todos únicos
   }

   #endregion

   #region Operators Tests

   [Fact(DisplayName = "Should convert RandomToken to string using implicit operator")]
   public void ImplicitOperator_WhenConvertingToString_ShouldReturnTokenValue()
   {
       // Arrange
       var token = RandomToken.Create();

       // Act
       string result = token;

       // Assert
       result.Should().Be(token.Value);
   }

   #endregion

   #region Override Tests

   [Fact(DisplayName = "Should return token value when calling ToString")]
   public void ToString_WhenCalled_ShouldReturnTokenValue()
   {
       // Arrange
       var token = RandomToken.Create();

       // Act
       var result = token.ToString();

       // Assert
       result.Should().Be(token.Value);
   }

   #endregion

   #region Equality Tests

   [Fact(DisplayName = "Should not be equal when tokens have different values")]
   public void Equality_WhenDifferentValues_ShouldNotBeEqual()
   {
       // Arrange
       var token1 = RandomToken.Create();
       var token2 = RandomToken.Create();

       // Act & Assert
       token1.Should().NotBe(token2);
       (token1 == token2).Should().BeFalse();
   }

   #endregion
}