using BlackSharp.Core.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace BlackSharp.Core.Tests.Extensions
{
    [TestClass]
    public class AssemblyExtensionsTests
    {
        #region Public

        [TestMethod]
        public void GetGZipResourceStreamMissingResourceReturnsNull()
        {
            // Arrange
            Assembly assembly = typeof(AssemblyExtensionsTests).Assembly;

            // Act
            var result = assembly.GetGZipResourceStream("missing.resource.gz");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetGZipResourceStreamNullArgumentsReturnNull()
        {
            // Arrange
            Assembly assembly = null;

            // Act
            var missingAssemblyResult = assembly.GetGZipResourceStream("resource.gz");
            var missingNamesResult = typeof(AssemblyExtensionsTests).Assembly.GetGZipResourceStream(null);

            // Assert
            Assert.IsNull(missingAssemblyResult);
            Assert.IsNull(missingNamesResult);
        }

        #endregion
    }
}
