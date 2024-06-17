using CuiLib;
using CuiLib.Logging;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Test.Helpers;

namespace Test.CuiLib.Logging
{
    [TestFixture]
    public class LoggerTest : TestBase
    {
        private MethodReceivedNotifiyingTextWriter innerWriter;
        private Logger logger;

        [SetUp]
        public void SetUp()
        {
            innerWriter = new MethodReceivedNotifiyingTextWriter();
            logger = new Logger();
            logger.AddLog(innerWriter);
        }

        [TearDown]
        public void CleanUp()
        {
            logger.Dispose();
            innerWriter.Dispose();
        }

        #region Ctors

        [Test]
        public void Ctor_WithoutArgs()
        {
            Logger logger = default!;

            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() => logger = new Logger());
                Assert.That(logger.GetAllTargets(), Is.Empty);
            });
        }

        [Test]
        public void Ctor_WithStringAndDefaultEncodingAsAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target.Name, true, null);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.GreaterThan(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithStringAndSpecifiedEncodingAsAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                var encoding = Encoding.Unicode;
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target.Name, true, encoding);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.GreaterThan(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(encoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithStringAndDefaultEncodingAsNotAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target.Name, false, null);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.EqualTo(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithStringAndSpecifiedEncodingAsNotAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                var encoding = Encoding.Unicode;
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target.Name, false, encoding);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.EqualTo(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(encoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithFileInfoAndDefaultEncodingAsAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target, true, null);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.GreaterThan(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithFileInfoAndSpecifiedEncodingAsAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                var encoding = Encoding.Unicode;
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target, true, encoding);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.GreaterThan(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(encoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithFileInfoAndDefaultEncodingAsNotAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target, false, null);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.EqualTo(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        [Test]
        public void Ctor_WithFileInfoAndSpecifiedEncodingAsNotAppending()
        {
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                var encoding = Encoding.Unicode;
                using (StreamWriter writer = target.CreateText()) writer.Write("hoge");

                using Logger logger = new Logger(target, false, encoding);
                target.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.writers[0].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(target.Length, Is.EqualTo(0));
                    Assert.That(logger.writers[0].Writer.Encoding, Is.EqualTo(encoding));
                });
            }
            finally
            {
                target.Delete();
            }
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void Encoding_Get()
        {
            Assert.Throws<NotSupportedException>(() => _ = logger.Encoding);
        }

        [Test]
        public void ConsoleStdoutLogEnabled_Get_OnDefault()
        {
            Assert.That(new Logger().ConsoleStdoutLogEnabled, Is.False);
        }

        [Test]
        public void ConsoleStdoutLogEnabled_Get_AfterSetting()
        {
            var logger = new Logger
            {
                ConsoleStdoutLogEnabled = true
            };

            Assert.That(logger.ConsoleStdoutLogEnabled, Is.True);
        }

        [Test]
        public void ConsoleStdoutLogEnabled_Set()
        {
            logger.ConsoleStdoutLogEnabled = true;

            Assert.Multiple(() =>
            {
                Assert.That(logger.writers, Has.Count.EqualTo(2));
                Assert.That(logger.writers[1].Writer, Is.EqualTo(Console.Out));
                Assert.That(logger.writers[1].IsConsoleWriter, Is.True);
                Assert.That(logger.writers[1].MustDisposed, Is.False);
                Assert.That(logger.writers[1].Path, Is.Null);
            });
        }

        [Test]
        public void ConsoleErrorLogEnabled_Get_OnDefault()
        {
            Assert.That(new Logger().ConsoleErrorEnabled, Is.False);
        }

        [Test]
        public void ConsoleErrorLogEnabled_Get_AfterSetting()
        {
            var logger = new Logger
            {
                ConsoleErrorEnabled = true
            };

            Assert.That(logger.ConsoleErrorEnabled, Is.True);
        }

        [Test]
        public void ConsoleErrorLogEnabled_Set()
        {
            logger.ConsoleErrorEnabled = true;

            Assert.Multiple(() =>
            {
                Assert.That(logger.writers, Has.Count.EqualTo(2));
                Assert.That(logger.writers[1].Writer, Is.EqualTo(Console.Error));
                Assert.That(logger.writers[1].IsConsoleWriter, Is.True);
                Assert.That(logger.writers[1].MustDisposed, Is.False);
                Assert.That(logger.writers[1].Path, Is.Null);
            });
        }

        [Test]
        public void DefaultEncoding_Get_OnDefault()
        {
            Assert.That(logger.DefaultEncoding, Is.EqualTo(IOHelpers.UTF8N));
        }

        [Test]
        public void DefaultEncoding_Set_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => logger.DefaultEncoding = null!);
        }

        [Test]
        public void DefaultEncoding_Set_AsPositive()
        {
            Encoding encoding = Encoding.UTF32;
            logger.DefaultEncoding = encoding;

            Assert.That(logger.DefaultEncoding, Is.EqualTo(encoding));
        }

        #endregion Properties

        #region Methods

        [Test]
        public void GetAllTargets_AsExcludeStdoutAndError()
        {
            logger.ConsoleStdoutLogEnabled = true;
            logger.ConsoleErrorEnabled = true;

            Assert.Multiple(() =>
            {
                Assert.That(logger.GetAllTargets(false).Count(), Is.EqualTo(1));
                Assert.That(logger.GetAllTargets(false).First(), Is.InstanceOf<MethodReceivedNotifiyingTextWriter>());
            });
        }

        [Test]
        public void GetAllTargets_AsIncludeStdoutAndError()
        {
            logger.ConsoleStdoutLogEnabled = true;
            logger.ConsoleErrorEnabled = true;

            Assert.Multiple(() =>
            {
                Assert.That(logger.GetAllTargets(true).Count(), Is.EqualTo(3));
                Assert.That(logger.GetAllTargets(true).First(x => x != Console.Out && x != Console.Error), Is.InstanceOf<MethodReceivedNotifiyingTextWriter>());
                Assert.That(logger.GetAllTargets(true).Any(x => x == Console.Out), Is.True);
                Assert.That(logger.GetAllTargets(true).Any(x => x == Console.Error), Is.True);
            });
        }

        [Test]
        public void Dispose()
        {
            FileInfo targetFile = FileUtilHelpers.GetNoExistingFile();

            try
            {
                logger.AddLogFile(targetFile);
                TextWriter writer = logger.GetAllTargets().Last();

                logger.Dispose();

                Assert.Throws<ObjectDisposedException>(writer.WriteLine);
            }
            finally
            {
                targetFile.Delete();
            }
        }

        #region Collection Operations

        [Test]
        public void AddLogFile_WithPath_WithNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => logger.AddLogFile(path: null!));
        }

        [Test]
        public void AddLogFile_WithPath_WithEmptyPath()
        {
            Assert.Throws<ArgumentException>(() => logger.AddLogFile(string.Empty));
        }

        [Test]
        public void AddLogFile_WithPath_WithMisingDirectoryPath()
        {
            string path = Path.Combine(FileUtilHelpers.GetNoExistingDirectory().Name, "missing.txt");

            Assert.Throws<DirectoryNotFoundException>(() => logger.AddLogFile(path));
        }

        [Test]
        public void AddLogFile_WithPath_AsPositive_AsCreateAndDefaultEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                logger.AddLogFile(added.Name);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.writers[1].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].MustDisposed, Is.True);
                    Assert.That(logger.writers[1].Path, Is.EqualTo(added.FullName));
                });

                logger.Write("content");
                logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLogFile_WithPath_AsPositive_AsCreateAndSpeifiedEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();
            var encoding = new UnicodeEncoding();

            try
            {
                logger.AddLogFile(added.Name, encoding: encoding);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.writers[1].Writer.Encoding, Is.EqualTo(encoding));
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].MustDisposed, Is.True);
                    Assert.That(logger.writers[1].Path, Is.EqualTo(added.FullName));
                });

                logger.Write("content");
                logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLogFile_WithPath_AsPositive_AsAppendAndDefaultEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();
            try
            {
                using (FileStream stream = added.Create())
                {
#if NETCOREAPP3_1_OR_GREATER
                    stream.Write([1, 2, 3]);
#else
                    stream.Write([1, 2, 3], 0, 3);
#endif
                }

                logger.AddLogFile(added.Name, append: true);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.writers[1].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].MustDisposed, Is.True);
                    Assert.That(logger.writers[1].Path, Is.EqualTo(added.FullName));
                });

                logger.Write('t');
                logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(3));
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLogFile_WithFileInfo_WithNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => logger.AddLogFile(file: null!));
        }

        [Test]
        public void AddLogFile_WithFileInfo_WithMisingDirectoryPath()
        {
            string path = Path.Combine(FileUtilHelpers.GetNoExistingDirectory().Name, "missing.txt");

            Assert.Throws<DirectoryNotFoundException>(() => logger.AddLogFile(new FileInfo(path)));
        }

        [Test]
        public void AddLogFile_WithFileInfo_AsPositive_AsCreateAndDefaultEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                logger.AddLogFile(added);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.writers[1].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].MustDisposed, Is.True);
                    Assert.That(logger.writers[1].Path, Is.EqualTo(added.FullName));
                });

                logger.Write("content");
                logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLogFile_WithFileInfo_AsPositive_AsCreateAndSpecifiedEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();
            var encoding = new UnicodeEncoding();

            try
            {
                logger.AddLogFile(added, encoding: encoding);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.writers[1].Writer.Encoding, Is.EqualTo(encoding));
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].MustDisposed, Is.True);
                    Assert.That(logger.writers[1].Path, Is.EqualTo(added.FullName));
                });

                logger.Write("content");
                logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLogFile_WithFileInfo_AsPositive_AsAppendAndDefaultEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();
            try
            {
                using (FileStream stream = added.Create())
                {
#if NETCOREAPP3_1_OR_GREATER
                    stream.Write([1, 2, 3]);
#else
                    stream.Write([1, 2, 3], 0, 3);
#endif
                }

                logger.AddLogFile(added, append: true);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.writers[1].Writer.Encoding, Is.EqualTo(logger.DefaultEncoding));
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].MustDisposed, Is.True);
                    Assert.That(logger.writers[1].Path, Is.EqualTo(added.FullName));
                });

                logger.Write('t');
                logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(3));
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLog_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => logger.AddLog(null!));
        }

        [Test]
        public void AddLog_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();
            try
            {
                using StreamWriter writer = added.CreateText();
                logger.AddLog(writer);
                Assert.Multiple(() =>
                {
                    Assert.That(logger.writers, Has.Count.EqualTo(2));
                    Assert.That(logger.writers[1].Writer, Is.EqualTo(writer));
                    Assert.That(logger.writers[1].MustDisposed, Is.False);
                    Assert.That(logger.writers[1].IsConsoleWriter, Is.False);
                    Assert.That(logger.writers[1].Path, Is.Null);
                });

                logger.Write("content");
                logger.Flush();

                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                added.Delete();
            }
        }

        [Test]
        public void HasLogFile_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => logger.HasLogFile(null!));
        }

        [Test]
        public void HasLogFile_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => logger.HasLogFile(string.Empty));
        }

        [Test]
        public void HasLogFile_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                logger.AddLogFile(added);

                Assert.Multiple(() =>
                {
                    Assert.That(logger.HasLogFile(added.Name), Is.True);
                    Assert.That(logger.HasLogFile(added.FullName), Is.True);
                    Assert.That(logger.HasLogFile("missing.txt"), Is.False);
                });
            }
            finally
            {
                logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void RemoveLogFile_WithPath_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => logger.RemoveLogFile(null!));
        }

        [Test]
        public void RemoveLogFile_WithPath_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => logger.RemoveLogFile(string.Empty));
        }

        [Test]
        public void RemoveLogFile_WithPath_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                logger.AddLogFile(added);
                TextWriter writer = logger.GetAllTargets().Last();

                Assert.Multiple(() =>
                {
                    Assert.That(logger.RemoveLogFile("missing.txt"), Is.False);
                    Assert.That(logger.RemoveLogFile(added.FullName), Is.True);

                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.RemoveLogFile(added.Name), Is.False);
                    Assert.That(logger.RemoveLogFile(added.FullName), Is.False);

                    Assert.Throws<ObjectDisposedException>(() => writer.Flush());
                });

                logger.Dispose();
            }
            finally
            {
                added.Delete();
            }
        }

        [Test]
        public void RemoveLogFile_WithTextWriter_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => logger.RemoveLog(null!));
        }

        [Test]
        public void RemoveLogFile_WithTextWriter_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using TextWriter writer = added.CreateText();
                logger.AddLog(writer);

                Assert.Multiple(() =>
                {
                    Assert.That(logger.RemoveLog(new MethodReceivedNotifiyingTextWriter()), Is.False);
                    Assert.That(logger.RemoveLog(writer), Is.True);

                    Assert.That(logger.writers, Has.Count.EqualTo(1));
                    Assert.That(logger.RemoveLog(writer), Is.False);

                    Assert.DoesNotThrow(() => writer.Flush());
                });

                logger.Dispose();
            }
            finally
            {
                added.Delete();
            }
        }

        #endregion Collection Operations

        #region Write Operations

        [Test]
        public void Flush()
        {
            logger.Flush();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Flush()" }));
        }

        [Test]
        public void FlushAsync_WithoutArgs()
        {
            logger.FlushAsync().Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "FlushAsync()" }));
        }

#if NET8_0_OR_GREATER

        [Test]
        public void FlushAsync_WithCancellationToken()
        {
            logger.FlushAsync(CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "FlushAsync(CancellationToken)" }));
        }

#endif

        [Test]
        public void Write_WithChar()
        {
            logger.Write('t');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(char): t" }));
        }

#if NETCOREAPP3_1_OR_GREATER

        [Test]
        public void Write_WithReadOnlySpan_Char()
        {
            logger.Write("test".AsSpan());

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(ReadOnlySpan<char>): test" }));
        }

#endif

        [Test]
        public void Write_WithCharArray_Whole()
        {
            logger.Write(new[] { 't', 'e', 's', 't' });
            logger.Write(Array.Empty<char>());
            logger.Write(null as char[]);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(char[]?): test", "Write(char[]?): ", "Write(char[]?): " }));
        }

        [Test]
        public void Write_WithCharArray_Range()
        {
            logger.Write(['t', 'e', 's', 't'], 1, 2);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(char[], int, int): es" }));
        }

        [Test]
        public void Write_WithInt32()
        {
            logger.Write(100);
            logger.Write(-100);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(int): 100", "Write(int): -100" }));
        }

        [Test]
        public void Write_WithUInt32()
        {
            logger.Write(100u);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(uint): 100" }));
        }

        [Test]
        public void Write_WithInt64()
        {
            logger.Write(100L);
            logger.Write(-100L);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(long): 100", "Write(long): -100" }));
        }

        [Test]
        public void Write_WithUInt64()
        {
            logger.Write(100UL);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(ulong): 100" }));
        }

        [Test]
        public void Write_WithSingle()
        {
            logger.Write(100f);
            logger.Write(-100f);
            logger.Write(float.PositiveInfinity);
            logger.Write(float.NegativeInfinity);
            logger.Write(float.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(float): 100", "Write(float): -100", "Write(float): Infinity", "Write(float): -Infinity", "Write(float): NaN" }));
        }

        [Test]
        public void Write_WithDouble()
        {
            logger.Write(100d);
            logger.Write(-100d);
            logger.Write(double.PositiveInfinity);
            logger.Write(double.NegativeInfinity);
            logger.Write(double.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(double): 100", "Write(double): -100", "Write(double): Infinity", "Write(double): -Infinity", "Write(double): NaN" }));
        }

        [Test]
        public void Write_WithDecimal()
        {
            logger.Write(100m);
            logger.Write(-100m);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(decimal): 100", "Write(decimal): -100" }));
        }

        [Test]
        public void Write_WithBoolean()
        {
            logger.Write(true);
            logger.Write(false);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(bool): True", "Write(bool): False" }));
        }

        [Test]
        public void Write_WithObject()
        {
            logger.Write(null as object);
            logger.Write('t' as object);
            logger.Write("test" as object);
            logger.Write(1 as object);
            logger.Write(true as object);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(object?): ", "Write(object?): t", "Write(object?): test", "Write(object?): 1", "Write(object?): True" }));
        }

#if NET6_0_OR_GREATER

        [Test]
        public void Write_WithStringBuilder()
        {
            logger.Write(new StringBuilder("test", 4));
            logger.Write(null as StringBuilder);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(StringBuilder?): test", "Write(StringBuilder?): " }));
        }

#endif

        [Test]
        public void Write_WithRawString()
        {
            logger.Write("test");
            logger.Write(null as string);
            logger.Write(string.Empty);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string?): test", "Write(string?): ", "Write(string?): " }));
        }

        [Test]
        public void Write_WithFormattedStringAnd1Arg()
        {
            logger.Write("val={0}", 1);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?): val=1" }));
        }

        [Test]
        public void Write_WithFormattedStringAnd2Args()
        {
            logger.Write("val1={0}, val2={1}", 1, true);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?, object?): val1=1, val2=True" }));
        }

        [Test]
        public void Write_WithFormattedStringAnd3Args()
        {
            logger.Write("val1={0}, val2={1}, val3={2}", 1, true, "hoge");

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?, object?, object?): val1=1, val2=True, val3=hoge" }));
        }

        [Test]
        public void Write_WithFormattedStringAndMultiArgs()
        {
            logger.Write("val1={0}, val2={1}, val3={2}, val4={3}", 1, true, "hoge", 'v');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?[]): val1=1, val2=True, val3=hoge, val4=v" }));
        }

        [Test]
        public void WriteAsync_WithChar()
        {
            logger.WriteAsync('t').Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(char): t" }));
        }

#if NETCOREAPP3_1_OR_GREATER

        [Test]
        public void WriteAsync_WithReadOnlyMemory_Char()
        {
            logger.WriteAsync("test".AsMemory(), CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(ReadOnlyMemory<char>, CancellationToken): test" }));
        }

#endif

        [Test]
        public void WriteAsync_WithCharArray_Range()
        {
            logger.WriteAsync(['t', 'e', 's', 't'], 1, 2).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(char[], int, int): es" }));
        }

#if NET6_0_OR_GREATER

        [Test]
        public void WriteAsync_WithStringBuilder()
        {
            logger.WriteAsync(new StringBuilder("test", 4), CancellationToken.None).Wait();
            logger.WriteAsync(null as StringBuilder, CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(StringBuilder?, CancellationToken): test", "WriteAsync(StringBuilder?, CancellationToken): " }));
        }

#endif

        [Test]
        public void WriteAsync_WithRawString()
        {
            logger.WriteAsync("test").Wait();
            logger.WriteAsync(null as string).Wait();
            logger.WriteAsync(string.Empty).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(string?): test", "WriteAsync(string?): ", "WriteAsync(string?): " }));
        }

        [Test]
        public void WriteLine_WithoutArgs()
        {
            logger.WriteLine();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine()" }));
        }

        [Test]
        public void WriteLine_WithChar()
        {
            logger.WriteLine('t');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(char): t" }));
        }

#if NETCOREAPP3_1_OR_GREATER

        [Test]
        public void WriteLine_WithReadOnlySpan_Char()
        {
            logger.WriteLine("test".AsSpan());

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(ReadOnlySpan<char>): test" }));
        }

#endif

        [Test]
        public void WriteLine_WithCharArray_Whole()
        {
            logger.WriteLine(new[] { 't', 'e', 's', 't' });
            logger.WriteLine(Array.Empty<char>());
            logger.WriteLine(null as char[]);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(char[]?): test", "WriteLine(char[]?): ", "WriteLine(char[]?): " }));
        }

        [Test]
        public void WriteLine_WithCharArray_Range()
        {
            logger.WriteLine(['t', 'e', 's', 't'], 1, 2);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(char[], int, int): es" }));
        }

        [Test]
        public void WriteLine_WithInt32()
        {
            logger.WriteLine(100);
            logger.WriteLine(-100);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(int): 100", "WriteLine(int): -100" }));
        }

        [Test]
        public void WriteLine_WithUInt32()
        {
            logger.WriteLine(100u);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(uint): 100" }));
        }

        [Test]
        public void WriteLine_WithInt64()
        {
            logger.WriteLine(100L);
            logger.WriteLine(-100L);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(long): 100", "WriteLine(long): -100" }));
        }

        [Test]
        public void WriteLine_WithUInt64()
        {
            logger.WriteLine(100UL);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(ulong): 100" }));
        }

        [Test]
        public void WriteLine_WithSingle()
        {
            logger.WriteLine(100f);
            logger.WriteLine(-100f);
            logger.WriteLine(float.PositiveInfinity);
            logger.WriteLine(float.NegativeInfinity);
            logger.WriteLine(float.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(float): 100", "WriteLine(float): -100", "WriteLine(float): Infinity", "WriteLine(float): -Infinity", "WriteLine(float): NaN" }));
        }

        [Test]
        public void WriteLine_WithDouble()
        {
            logger.WriteLine(100d);
            logger.WriteLine(-100d);
            logger.WriteLine(double.PositiveInfinity);
            logger.WriteLine(double.NegativeInfinity);
            logger.WriteLine(double.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(double): 100", "WriteLine(double): -100", "WriteLine(double): Infinity", "WriteLine(double): -Infinity", "WriteLine(double): NaN" }));
        }

        [Test]
        public void WriteLine_WithDecimal()
        {
            logger.WriteLine(100m);
            logger.WriteLine(-100m);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(decimal): 100", "WriteLine(decimal): -100" }));
        }

        [Test]
        public void WriteLine_WithBoolean()
        {
            logger.WriteLine(true);
            logger.WriteLine(false);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(bool): True", "WriteLine(bool): False" }));
        }

        [Test]
        public void WriteLine_WithObject()
        {
            logger.WriteLine(null as object);
            logger.WriteLine('t' as object);
            logger.WriteLine("test" as object);
            logger.WriteLine(1 as object);
            logger.WriteLine(true as object);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(object?): ", "WriteLine(object?): t", "WriteLine(object?): test", "WriteLine(object?): 1", "WriteLine(object?): True" }));
        }

#if NET6_0_OR_GREATER

        [Test]
        public void WriteLine_WithStringBuilder()
        {
            logger.WriteLine(new StringBuilder("test", 4));
            logger.WriteLine(null as StringBuilder);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(StringBuilder?): test", "WriteLine(StringBuilder?): " }));
        }

#endif

        [Test]
        public void WriteLine_WithRawString()
        {
            logger.WriteLine("test");
            logger.WriteLine(null as string);
            logger.WriteLine(string.Empty);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string?): test", "WriteLine(string?): ", "WriteLine(string?): " }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAnd1Arg()
        {
            logger.WriteLine("val={0}", 1);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?): val=1" }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAnd2Args()
        {
            logger.WriteLine("val1={0}, val2={1}", 1, true);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?, object?): val1=1, val2=True" }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAnd3Args()
        {
            logger.WriteLine("val1={0}, val2={1}, val3={2}", 1, true, "hoge");

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?, object?, object?): val1=1, val2=True, val3=hoge" }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAndMultiArgs()
        {
            logger.WriteLine("val1={0}, val2={1}, val3={2}, val4={3}", 1, true, "hoge", 'v');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?[]): val1=1, val2=True, val3=hoge, val4=v" }));
        }

        [Test]
        public void WriteLineAsync_WithoutArgs()
        {
            logger.WriteLineAsync().Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync()" }));
        }

        [Test]
        public void WriteLineAsync_WithChar()
        {
            logger.WriteLineAsync('t').Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(char): t" }));
        }

#if NETCOREAPP3_1_OR_GREATER

        [Test]
        public void WriteLineAsync_WithReadOnlyMemory_Char()
        {
            logger.WriteLineAsync("test".AsMemory(), CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(ReadOnlyMemory<char>, CancellationToken): test" }));
        }

#endif

        [Test]
        public void WriteLineAsync_WithCharArray_Range()
        {
            logger.WriteLineAsync(['t', 'e', 's', 't'], 1, 2).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(char[], int, int): es" }));
        }

#if NET6_0_OR_GREATER

        [Test]
        public void WriteLineAsync_WithStringBuilder()
        {
            logger.WriteLineAsync(new StringBuilder("test", 4), CancellationToken.None).Wait();
            logger.WriteLineAsync(null as StringBuilder, CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(StringBuilder?, CancellationToken): test", "WriteLineAsync(StringBuilder?, CancellationToken): " }));
        }

#endif

        [Test]
        public void WriteLineAsync_WithRawString()
        {
            logger.WriteLineAsync("test").Wait();
            logger.WriteLineAsync(null as string).Wait();
            logger.WriteLineAsync(string.Empty).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(string?): test", "WriteLineAsync(string?): ", "WriteLineAsync(string?): " }));
        }

        #endregion Write Operations

        #endregion Methods
    }
}
