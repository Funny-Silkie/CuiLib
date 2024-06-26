﻿using CuiLib;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using Test.Helpers;

namespace Test.CuiLib
{
    [TestFixture]
    public class IOHelpersTest : TestBase
    {
        #region Properties

        [Test]
        public void UTF8N_Get()
        {
            Assert.That(IOHelpers.UTF8N, Is.InstanceOf<UTF8Encoding>());
        }

        #endregion Properties

        #region Methods

        [Test]
        public void EnsureDirectory_WithNoExistingDirectory()
        {
            DirectoryInfo directory = FileUtilHelpers.GetNoExistingDirectory();

            try
            {
                DirectoryInfo result = IOHelpers.EnsureDirectory(directory.FullName);
                directory.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(directory.Exists, Is.True);
                    Assert.That(result.FullName, Is.EqualTo(directory.FullName));
                });
            }
            finally
            {
                directory.Delete();
            }
        }

        [Test]
        public void EnsureDirectory_WithExistingDirectory()
        {
            DirectoryInfo directory = FileUtilHelpers.GetNoExistingDirectory();

            try
            {
                directory.Create();
                DirectoryInfo result = IOHelpers.EnsureDirectory(directory.FullName);
                directory.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(directory.Exists, Is.True);
                    Assert.That(result.FullName, Is.EqualTo(directory.FullName));
                });
            }
            finally
            {
                directory.Delete();
            }
        }

        [Test]
        public void OpenReadAllText_WithNull()
        {
            Assert.That(() => IOHelpers.OpenReadAllText(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void OpenReadAllText_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                Assert.That(file.OpenReadAllText(), Is.EqualTo($"hoge{Environment.NewLine}fuga{Environment.NewLine}"));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void OpenReadAllTextAsync_WithNull()
        {
            Assert.That(() => IOHelpers.OpenReadAllTextAsync(null!).GetAwaiter().GetResult(), Throws.ArgumentNullException);
        }

        [Test]
        public void OpenReadAllTextAsync_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                Assert.That(file.OpenReadAllTextAsync().GetAwaiter().GetResult(), Is.EqualTo($"hoge{Environment.NewLine}fuga{Environment.NewLine}"));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void OpenIterateLines_WithNull()
        {
            Assert.That(() => IOHelpers.OpenIterateLines(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void OpenIterateLines_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                Assert.That(file.OpenIterateLines(), Is.EqualTo(new[] { "hoge", "fuga" }));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void OpenIterateLinesAsync_WithNull()
        {
            Assert.That(() => IOHelpers.OpenIterateLinesAsync(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void OpenIterateLinesAsync_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                Assert.That(file.OpenIterateLinesAsync().ToArrayAsync().Result, Is.EqualTo(new[] { "hoge", "fuga" }));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void OpenReadLines_WithNull()
        {
            Assert.That(() => IOHelpers.OpenReadLines(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void OpenReadLines_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                Assert.That(file.OpenReadLines(), Is.EqualTo(new[] { "hoge", "fuga" }));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void OpenReadLinesAsync_WithNull()
        {
            Assert.That(() => IOHelpers.OpenReadLinesAsync(null!).GetAwaiter().GetResult(), Throws.ArgumentNullException);
        }

        [Test]
        public void OpenReadLinesAsync_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                Assert.That(file.OpenReadLinesAsync().GetAwaiter().GetResult(), Is.EqualTo(new[] { "hoge", "fuga" }));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void IterateLines_WithNull()
        {
            Assert.That(() => IOHelpers.IterateLines(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void IterateLines_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                using StreamReader reader = file.OpenText();
                Assert.That(reader.IterateLines(), Is.EqualTo(new[] { "hoge", "fuga" }));
            }
            finally
            {
                file.Delete();
            }
        }

        [Test]
        public void IterateLinesAsync_WithNull()
        {
            Assert.That(() => IOHelpers.IterateLinesAsync(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void IterateLinesAsync_AsPositive()
        {
            FileInfo file = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = file.CreateText())
                {
                    writer.WriteLine("hoge");
                    writer.WriteLine("fuga");
                }

                using StreamReader reader = file.OpenText();
                Assert.That(reader.IterateLinesAsync().ToArrayAsync().Result, Is.EqualTo(new[] { "hoge", "fuga" }));
            }
            finally
            {
                file.Delete();
            }
        }

        #endregion Methods
    }
}
