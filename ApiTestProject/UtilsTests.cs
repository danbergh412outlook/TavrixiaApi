using FluentAssertions;
using Microsoft.IdentityModel.Logging;
using PortfolioApp.Api.Helpers;

namespace ApiTestProject
{
    public class UtilsTests
    {
        #region Slugify Tests
        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("   Hello    World!   ", "hello-world")]
        [InlineData("C# Is Awesome", "c-is-awesome")]
        [InlineData("This -- is a test", "this-is-a-test")]
        [InlineData("Clean URL/Slug--Example!", "clean-url-slug-example")]
        [InlineData("Multiple     Spaces", "multiple-spaces")]
        [InlineData("Trailing and leading spaces    ", "trailing-and-leading-spaces")]
        [InlineData("--Already-Slugi*fied--", "already-slugified")]
        public void Slugify_ValidStrings_ReturnsExpectedSlug(string input, string expected)
        {
            var result = Utils.Slugify(input);
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Slugify_NullOrWhitespace_ReturnsEmptyString(string input)
        {
            var result = Utils.Slugify(input);
            result.Should().BeEmpty();
        }

        [Fact]
        public void Slugify_RemovesSpecialCharacters()
        {
            var input = "T@est! Str#ing$";
            var expected = "test-string";
            Utils.Slugify(input).Should().Be(expected);
        }

        [Fact]
        public void Slugify_ReplacesMultipleHyphensWithSingle()
        {
            var input = "a---b--c";
            var expected = "a-b-c";
            Utils.Slugify(input).Should().Be(expected);
        }
        #endregion

        #region GenerateUniqueSlug Tests

        [Fact]
        public async Task GenerateUniqueSlug_ShouldReturnOldSlug_WhenNewNameEqualsOldName()
        {
            // Arrange
            string newName = "My Survey";
            string oldName = "My Survey";
            string oldSlug = "my-survey";

            var result = await Utils.GenerateUniqueSlug(newName, oldName, oldSlug, null, (_, _) =>
                Task.FromResult(new HashSet<string>()));

            // Assert
            Assert.Equal(oldSlug, result);
        }

        [Fact]
        public async Task GenerateUniqueSlug_ShouldReturnSlugifiedNewName_WhenNoConflict()
        {
            // Arrange
            string newName = "New Survey";
            string oldName = "Old Survey";
            string oldSlug = "old-survey";

            var result = await Utils.GenerateUniqueSlug(newName, oldName, oldSlug, null, (_, _) =>
                Task.FromResult(new HashSet<string>()));

            // Assert
            Assert.Equal("new-survey", result);
        }

        [Fact]
        public async Task GenerateUniqueSlug_ShouldAppendNumber_WhenSlugConflictExists()
        {
            // Arrange
            string newName = "Test Survey";
            string oldName = "Old Survey";
            string oldSlug = "old-survey";

            var existingSlugs = new HashSet<string> { "test-survey", "test-survey-1" };

            var result = await Utils.GenerateUniqueSlug(newName, oldName, oldSlug, null, (_, _) =>
                Task.FromResult(existingSlugs));

            // Assert
            Assert.Equal("test-survey-2", result);
        }

        [Fact]
        public async Task GenerateUniqueSlug_ShouldSkipCurrentIdEntry()
        {
            // Arrange
            string newName = "Sample";
            string oldName = "Example";
            string oldSlug = "example";

            var existingSlugs = new HashSet<string> { "sample", "sample-1" };

            var result = await Utils.GenerateUniqueSlug(newName, oldName, oldSlug, 5, (_, _) =>
                Task.FromResult(existingSlugs));

            // Assert
            Assert.Equal("sample-2", result);
        }

        [Fact]
        public async Task GenerateUniqueSlug_ShouldReturnOldSlug_WhenSlugifiedNewNameEqualsOld()
        {
            // Arrange
            string newName = "Hello World!";
            string oldName = "Hello World";
            string oldSlug = "hello-world";

            var result = await Utils.GenerateUniqueSlug(newName, oldName, oldSlug, null, (_, _) =>
                Task.FromResult(new HashSet<string>()));

            // Assert
            Assert.Equal("hello-world", result);
        }

        #endregion
    }
}