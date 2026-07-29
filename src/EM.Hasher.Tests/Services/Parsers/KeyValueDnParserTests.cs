/*
 * EM Hasher
 * Copyright © 2026 Enda Mullally (em.apps@outlook.ie)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using EM.Hasher.Services.Parsers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EM.Hasher.Tests.Services.Parsers;

[TestClass]
public class KeyValueDnParserTests
{
    private static KeyValueDnParser CreateSut()
    {
        return new KeyValueDnParser();
    }

    [TestMethod]
    public void Load_ReturnsSameInstance_ForFluentChaining()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var result = sut.Load("CN=Contoso");

        // Assert
        result.Should().BeSameAs(sut);
    }

    [TestMethod]
    public void GetFirstFoundValue_SimpleKeyValue_ReturnsValue()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("CN=Contoso").GetFirstFoundValue("CN");

        // Assert
        value.Should().Be("Contoso");
    }

    [TestMethod]
    public void GetFirstFoundValue_MultipleParts_ReturnsRequestedValue()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("CN=Contoso, O=Contoso Ltd, C=IE").GetFirstFoundValue("O");

        // Assert
        value.Should().Be("Contoso Ltd");
    }

    [TestMethod]
    public void GetFirstFoundValue_KeyLookupIsCaseInsensitive()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("CN=Contoso").GetFirstFoundValue("cn");

        // Assert
        value.Should().Be("Contoso");
    }

    [TestMethod]
    public void GetFirstFoundValue_ReturnsFirstMatchingKeyInOrder()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("O=Contoso Ltd").GetFirstFoundValue("CN", "O");

        // Assert
        value.Should().Be("Contoso Ltd");
    }

    [TestMethod]
    public void GetFirstFoundValue_TrimsWhitespaceAroundKeysAndValues()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("  CN  =  Contoso  ").GetFirstFoundValue("CN");

        // Assert
        value.Should().Be("Contoso");
    }

    [TestMethod]
    public void GetFirstFoundValue_ValueContainingEquals_IsPreserved()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("CN=key=value").GetFirstFoundValue("CN");

        // Assert
        value.Should().Be("key=value");
    }

    [TestMethod]
    public void GetFirstFoundValue_PartWithoutEquals_AppendsToPreviousKey()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("O=Contoso, Inc").GetFirstFoundValue("O");

        // Assert
        value.Should().Be("Contoso, Inc");
    }

    [TestMethod]
    public void GetFirstFoundValue_KeyNotFound_ReturnsEmpty()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("CN=Contoso").GetFirstFoundValue("O");

        // Assert
        value.Should().BeEmpty();
    }

    [TestMethod]
    public void GetFirstFoundValue_SkipsNullOrEmptyKeys()
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load("CN=Contoso").GetFirstFoundValue("", "CN");

        // Assert
        value.Should().Be("Contoso");
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow((string?)null)]
    public void GetFirstFoundValue_NullOrWhiteSpaceInput_ReturnsEmpty(string? input)
    {
        // Arrange
        var sut = CreateSut();

        // Act
        var value = sut.Load(input!).GetFirstFoundValue("CN");

        // Assert
        value.Should().BeEmpty();
    }
}
