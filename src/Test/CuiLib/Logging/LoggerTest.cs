using CuiLib;
using CuiLib.Log;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Test.Helpers;

namespace Test.CuiLib.Log
{
    [TestFixture]
    public class LoggerTest
    {
        private MethodReceivedNotifiyingTextWriter innerWriter;
        private Logger Logger;

        [SetUp]
        public void SetUp()
        {
            innerWriter = new MethodReceivedNotifiyingTextWriter();
            Logger = new Logger();
            Logger.AddLog(innerWriter);
        }

        [TearDown]
        public void CleanUp()
        {
            Logger.Dispose();
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
        public void Ctor_WithTarget()
        {
            Logger logger = default!;
            FileInfo target = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Assert.Multiple(() =>
                {
                    Assert.DoesNotThrow(() => logger = new Logger(target.Name));
                    Assert.That(logger.GetAllTargets().Count(), Is.EqualTo(1));
                    Assert.That(logger.GetAllTargets().First(), Is.InstanceOf<StreamWriter>());
                    Assert.That(logger.GetAllTargets().First().Encoding, Is.InstanceOf<UTF8Encoding>());
                });
            }
            finally
            {
                logger?.Dispose();
                target.Delete();
            }
        }

        #endregion Ctors

        #region Properties

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
            var logger = new Logger
            {
                ConsoleStdoutLogEnabled = true
            };

            Assert.Multiple(() =>
            {
                Assert.That(logger.GetAllTargets().Count(), Is.EqualTo(1));
                Assert.That(logger.GetAllTargets().First(), Is.EqualTo(Console.Out));
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
            var logger = new Logger
            {
                ConsoleErrorEnabled = true
            };

            Assert.Multiple(() =>
            {
                Assert.That(logger.GetAllTargets().Count(), Is.EqualTo(1));
                Assert.That(logger.GetAllTargets().First(), Is.EqualTo(Console.Error));
            });
        }

        #endregion Properties

        #region Methods

        [Test]
        public void GetAllTargets_AsExcludeStdoutAndError()
        {
            Logger.ConsoleStdoutLogEnabled = true;
            Logger.ConsoleErrorEnabled = true;

            Assert.Multiple(() =>
            {
                Assert.That(Logger.GetAllTargets(false).Count(), Is.EqualTo(1));
                Assert.That(Logger.GetAllTargets(false).First(), Is.InstanceOf<MethodReceivedNotifiyingTextWriter>());
            });
        }

        [Test]
        public void GetAllTargets_AsIncludeStdoutAndError()
        {
            Logger.ConsoleStdoutLogEnabled = true;
            Logger.ConsoleErrorEnabled = true;

            Assert.Multiple(() =>
            {
                Assert.That(Logger.GetAllTargets(true).Count(), Is.EqualTo(3));
                Assert.That(Logger.GetAllTargets(true).First(x => x != Console.Out && x != Console.Error), Is.InstanceOf<MethodReceivedNotifiyingTextWriter>());
                Assert.That(Logger.GetAllTargets(true).Any(x => x == Console.Out), Is.True);
                Assert.That(Logger.GetAllTargets(true).Any(x => x == Console.Error), Is.True);
            });
        }

        [Test]
        public void Dispose()
        {
            FileInfo targetFile = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Logger.AddLogFile(targetFile);
                TextWriter writer = Logger.GetAllTargets().Last();

                Logger.Dispose();

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
            Assert.Throws<ArgumentNullException>(() => Logger.AddLogFile(path: null!));
        }

        [Test]
        public void AddLogFile_WithPath_WithEmptyPath()
        {
            Assert.Throws<ArgumentException>(() => Logger.AddLogFile(string.Empty));
        }

        [Test]
        public void AddLogFile_WithPath_WithMisingDirectoryPath()
        {
            string path = Path.Combine(FileUtilHelpers.GetNoExistingDirectory().Name, "missing.txt");

            Assert.Throws<DirectoryNotFoundException>(() => Logger.AddLogFile(path));
        }

        [Test]
        public void AddLogFile_WithPath_AsPositive_AsCreateAndDefaultEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Logger.AddLogFile(added.Name);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.InstanceOf<StreamWriter>());
                    Assert.That(Logger.GetAllTargets().Last().Encoding, Is.EqualTo(IOHelpers.UTF8N));
                });

                Logger.Write("content");
                Logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                Logger.Dispose();
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
                Logger.AddLogFile(added.Name, encoding: encoding);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.InstanceOf<StreamWriter>());
                    Assert.That(Logger.GetAllTargets().Last().Encoding, Is.EqualTo(encoding));
                });

                Logger.Write("content");
                Logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                Logger.Dispose();
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
                    stream.Write(new byte[] { 1, 2, 3 });
                }

                Logger.AddLogFile(added.Name, append: true);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.InstanceOf<StreamWriter>());
                    Assert.That(Logger.GetAllTargets().Last().Encoding, Is.EqualTo(IOHelpers.UTF8N));
                });

                Logger.Write('t');
                Logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(3));
            }
            finally
            {
                Logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLogFile_WithFileInfo_WithNullPath()
        {
            Assert.Throws<ArgumentNullException>(() => Logger.AddLogFile(file: null!));
        }

        [Test]
        public void AddLogFile_WithFileInfo_WithMisingDirectoryPath()
        {
            string path = Path.Combine(FileUtilHelpers.GetNoExistingDirectory().Name, "missing.txt");

            Assert.Throws<DirectoryNotFoundException>(() => Logger.AddLogFile(new FileInfo(path)));
        }

        [Test]
        public void AddLogFile_WithFileInfo_AsPositive_AsCreateAndDefaultEncoding()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Logger.AddLogFile(added);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.InstanceOf<StreamWriter>());
                    Assert.That(Logger.GetAllTargets().Last().Encoding, Is.EqualTo(IOHelpers.UTF8N));
                });

                Logger.Write("content");
                Logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                Logger.Dispose();
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
                Logger.AddLogFile(added, encoding: encoding);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(added.Exists, Is.True);
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.InstanceOf<StreamWriter>());
                    Assert.That(Logger.GetAllTargets().Last().Encoding, Is.EqualTo(encoding));
                });

                Logger.Write("content");
                Logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(0));
            }
            finally
            {
                Logger.Dispose();
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
                    stream.Write(new byte[] { 1, 2, 3 });
                }

                Logger.AddLogFile(added, append: true);
                added.Refresh();

                Assert.Multiple(() =>
                {
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.InstanceOf<StreamWriter>());
                    Assert.That(Logger.GetAllTargets().Last().Encoding, Is.EqualTo(IOHelpers.UTF8N));
                });

                Logger.Write('t');
                Logger.Flush();
                added.Refresh();

                Assert.That(added.Length, Is.GreaterThan(3));
            }
            finally
            {
                Logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void AddLog_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => Logger.AddLog(null!));
        }

        [Test]
        public void AddLog_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();
            try
            {
                using StreamWriter writer = added.CreateText();
                Logger.AddLog(writer);
                Assert.Multiple(() =>
                {
                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(2));
                    Assert.That(Logger.GetAllTargets().Last(), Is.EqualTo(writer));
                });

                Logger.Write("content");
                Logger.Flush();

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
            Assert.Throws<ArgumentNullException>(() => Logger.HasLogFile(null!));
        }

        [Test]
        public void HasLogFile_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => Logger.HasLogFile(string.Empty));
        }

        [Test]
        public void HasLogFile_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Logger.AddLogFile(added);

                Assert.Multiple(() =>
                {
                    Assert.That(Logger.HasLogFile(added.Name), Is.True);
                    Assert.That(Logger.HasLogFile(added.FullName), Is.True);
                    Assert.That(Logger.HasLogFile("missing.txt"), Is.False);
                });
            }
            finally
            {
                Logger.Dispose();
                added.Delete();
            }
        }

        [Test]
        public void RemoveLogFile_WithPath_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => Logger.RemoveLogFile(null!));
        }

        [Test]
        public void RemoveLogFile_WithPath_WithEmpty()
        {
            Assert.Throws<ArgumentException>(() => Logger.RemoveLogFile(string.Empty));
        }

        [Test]
        public void RemoveLogFile_WithPath_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                Logger.AddLogFile(added);
                TextWriter writer = Logger.GetAllTargets().Last();

                Assert.Multiple(() =>
                {
                    Assert.That(Logger.RemoveLogFile("missing.txt"), Is.False);
                    Assert.That(Logger.RemoveLogFile(added.FullName), Is.True);

                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(1));
                    Assert.That(Logger.RemoveLogFile(added.Name), Is.False);
                    Assert.That(Logger.RemoveLogFile(added.FullName), Is.False);

                    Assert.Throws<ObjectDisposedException>(() => writer.Flush());
                });

                Logger.Dispose();
            }
            finally
            {
                added.Delete();
            }
        }

        [Test]
        public void RemoveLogFile_WithTextWriter_WithNull()
        {
            Assert.Throws<ArgumentNullException>(() => Logger.RemoveLog(null!));
        }

        [Test]
        public void RemoveLogFile_WithTextWriter_AsPositive()
        {
            FileInfo added = FileUtilHelpers.GetNoExistingFile();

            try
            {
                using TextWriter writer = added.CreateText();
                Logger.AddLog(writer);

                Assert.Multiple(() =>
                {
                    Assert.That(Logger.RemoveLog(new MethodReceivedNotifiyingTextWriter()), Is.False);
                    Assert.That(Logger.RemoveLog(writer), Is.True);

                    Assert.That(Logger.GetAllTargets().Count(), Is.EqualTo(1));
                    Assert.That(Logger.RemoveLog(writer), Is.False);

                    Assert.DoesNotThrow(() => writer.Flush());
                });

                Logger.Dispose();
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
            Logger.Flush();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Flush()" }));
        }

        [Test]
        public void FlushAsync_WithoutArgs()
        {
            Logger.FlushAsync().Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "FlushAsync()" }));
        }

#if NET8_0_OR_GREATER

        [Test]
        public void FlushAsync_WithCancellationToken()
        {
            Logger.FlushAsync(CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "FlushAsync(CancellationToken)" }));
        }

#endif

        [Test]
        public void Write_WithChar()
        {
            Logger.Write('t');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(char): t" }));
        }

        [Test]
        public void Write_WithReadOnlySpan_Char()
        {
            Logger.Write("test".AsSpan());

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(ReadOnlySpan<char>): test" }));
        }

        [Test]
        public void Write_WithCharArray_Whole()
        {
            Logger.Write(new[] { 't', 'e', 's', 't' });
            Logger.Write(Array.Empty<char>());
            Logger.Write(null as char[]);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(char[]?): test", "Write(char[]?): ", "Write(char[]?): " }));
        }

        [Test]
        public void Write_WithCharArray_Range()
        {
            Logger.Write(['t', 'e', 's', 't'], 1, 2);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(char[], int, int): es" }));
        }

        [Test]
        public void Write_WithInt32()
        {
            Logger.Write(100);
            Logger.Write(-100);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(int): 100", "Write(int): -100" }));
        }

        [Test]
        public void Write_WithUInt32()
        {
            Logger.Write(100u);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(uint): 100" }));
        }

        [Test]
        public void Write_WithInt64()
        {
            Logger.Write(100L);
            Logger.Write(-100L);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(long): 100", "Write(long): -100" }));
        }

        [Test]
        public void Write_WithUInt64()
        {
            Logger.Write(100UL);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(ulong): 100" }));
        }

        [Test]
        public void Write_WithSingle()
        {
            Logger.Write(100f);
            Logger.Write(-100f);
            Logger.Write(float.PositiveInfinity);
            Logger.Write(float.NegativeInfinity);
            Logger.Write(float.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(float): 100", "Write(float): -100", "Write(float): Infinity", "Write(float): -Infinity", "Write(float): NaN" }));
        }

        [Test]
        public void Write_WithDouble()
        {
            Logger.Write(100d);
            Logger.Write(-100d);
            Logger.Write(double.PositiveInfinity);
            Logger.Write(double.NegativeInfinity);
            Logger.Write(double.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(double): 100", "Write(double): -100", "Write(double): Infinity", "Write(double): -Infinity", "Write(double): NaN" }));
        }

        [Test]
        public void Write_WithDecimal()
        {
            Logger.Write(100m);
            Logger.Write(-100m);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(decimal): 100", "Write(decimal): -100" }));
        }

        [Test]
        public void Write_WithBoolean()
        {
            Logger.Write(true);
            Logger.Write(false);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(bool): True", "Write(bool): False" }));
        }

        [Test]
        public void Write_WithObject()
        {
            Logger.Write(null as object);
            Logger.Write('t' as object);
            Logger.Write("test" as object);
            Logger.Write(1 as object);
            Logger.Write(true as object);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(object?): ", "Write(object?): t", "Write(object?): test", "Write(object?): 1", "Write(object?): True" }));
        }

        [Test]
        public void Write_WithStringBuilder()
        {
            Logger.Write(new StringBuilder("test", 4));
            Logger.Write(null as StringBuilder);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(StringBuilder?): test", "Write(StringBuilder?): " }));
        }

        [Test]
        public void Write_WithRawString()
        {
            Logger.Write("test");
            Logger.Write(null as string);
            Logger.Write(string.Empty);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string?): test", "Write(string?): ", "Write(string?): " }));
        }

        [Test]
        public void Write_WithFormattedStringAnd1Arg()
        {
            Logger.Write("val={0}", 1);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?): val=1" }));
        }

        [Test]
        public void Write_WithFormattedStringAnd2Args()
        {
            Logger.Write("val1={0}, val2={1}", 1, true);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?, object?): val1=1, val2=True" }));
        }

        [Test]
        public void Write_WithFormattedStringAnd3Args()
        {
            Logger.Write("val1={0}, val2={1}, val3={2}", 1, true, "hoge");

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?, object?, object?): val1=1, val2=True, val3=hoge" }));
        }

        [Test]
        public void Write_WithFormattedStringAndMultiArgs()
        {
            Logger.Write("val1={0}, val2={1}, val3={2}, val4={3}", 1, true, "hoge", 'v');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "Write(string, object?[]): val1=1, val2=True, val3=hoge, val4=v" }));
        }

        [Test]
        public void WriteAsync_WithChar()
        {
            Logger.WriteAsync('t').Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(char): t" }));
        }

        [Test]
        public void WriteAsync_WithReadOnlyMemory_Char()
        {
            Logger.WriteAsync("test".AsMemory(), CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(ReadOnlyMemory<char>, CancellationToken): test" }));
        }

        [Test]
        public void WriteAsync_WithCharArray_Range()
        {
            Logger.WriteAsync(['t', 'e', 's', 't'], 1, 2).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(char[], int, int): es" }));
        }

        [Test]
        public void WriteAsync_WithStringBuilder()
        {
            Logger.WriteAsync(new StringBuilder("test", 4), CancellationToken.None).Wait();
            Logger.WriteAsync(null as StringBuilder, CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(StringBuilder?, CancellationToken): test", "WriteAsync(StringBuilder?, CancellationToken): " }));
        }

        [Test]
        public void WriteAsync_WithRawString()
        {
            Logger.WriteAsync("test").Wait();
            Logger.WriteAsync(null as string).Wait();
            Logger.WriteAsync(string.Empty).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteAsync(string?): test", "WriteAsync(string?): ", "WriteAsync(string?): " }));
        }

        [Test]
        public void WriteLine_WithoutArgs()
        {
            Logger.WriteLine();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine()" }));
        }

        [Test]
        public void WriteLine_WithChar()
        {
            Logger.WriteLine('t');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(char): t" }));
        }

        [Test]
        public void WriteLine_WithReadOnlySpan_Char()
        {
            Logger.WriteLine("test".AsSpan());

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(ReadOnlySpan<char>): test" }));
        }

        [Test]
        public void WriteLine_WithCharArray_Whole()
        {
            Logger.WriteLine(new[] { 't', 'e', 's', 't' });
            Logger.WriteLine(Array.Empty<char>());
            Logger.WriteLine(null as char[]);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(char[]?): test", "WriteLine(char[]?): ", "WriteLine(char[]?): " }));
        }

        [Test]
        public void WriteLine_WithCharArray_Range()
        {
            Logger.WriteLine(['t', 'e', 's', 't'], 1, 2);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(char[], int, int): es" }));
        }

        [Test]
        public void WriteLine_WithInt32()
        {
            Logger.WriteLine(100);
            Logger.WriteLine(-100);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(int): 100", "WriteLine(int): -100" }));
        }

        [Test]
        public void WriteLine_WithUInt32()
        {
            Logger.WriteLine(100u);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(uint): 100" }));
        }

        [Test]
        public void WriteLine_WithInt64()
        {
            Logger.WriteLine(100L);
            Logger.WriteLine(-100L);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(long): 100", "WriteLine(long): -100" }));
        }

        [Test]
        public void WriteLine_WithUInt64()
        {
            Logger.WriteLine(100UL);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(ulong): 100" }));
        }

        [Test]
        public void WriteLine_WithSingle()
        {
            Logger.WriteLine(100f);
            Logger.WriteLine(-100f);
            Logger.WriteLine(float.PositiveInfinity);
            Logger.WriteLine(float.NegativeInfinity);
            Logger.WriteLine(float.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(float): 100", "WriteLine(float): -100", "WriteLine(float): Infinity", "WriteLine(float): -Infinity", "WriteLine(float): NaN" }));
        }

        [Test]
        public void WriteLine_WithDouble()
        {
            Logger.WriteLine(100d);
            Logger.WriteLine(-100d);
            Logger.WriteLine(double.PositiveInfinity);
            Logger.WriteLine(double.NegativeInfinity);
            Logger.WriteLine(double.NaN);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(double): 100", "WriteLine(double): -100", "WriteLine(double): Infinity", "WriteLine(double): -Infinity", "WriteLine(double): NaN" }));
        }

        [Test]
        public void WriteLine_WithDecimal()
        {
            Logger.WriteLine(100m);
            Logger.WriteLine(-100m);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(decimal): 100", "WriteLine(decimal): -100" }));
        }

        [Test]
        public void WriteLine_WithBoolean()
        {
            Logger.WriteLine(true);
            Logger.WriteLine(false);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(bool): True", "WriteLine(bool): False" }));
        }

        [Test]
        public void WriteLine_WithObject()
        {
            Logger.WriteLine(null as object);
            Logger.WriteLine('t' as object);
            Logger.WriteLine("test" as object);
            Logger.WriteLine(1 as object);
            Logger.WriteLine(true as object);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(object?): ", "WriteLine(object?): t", "WriteLine(object?): test", "WriteLine(object?): 1", "WriteLine(object?): True" }));
        }

        [Test]
        public void WriteLine_WithStringBuilder()
        {
            Logger.WriteLine(new StringBuilder("test", 4));
            Logger.WriteLine(null as StringBuilder);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(StringBuilder?): test", "WriteLine(StringBuilder?): " }));
        }

        [Test]
        public void WriteLine_WithRawString()
        {
            Logger.WriteLine("test");
            Logger.WriteLine(null as string);
            Logger.WriteLine(string.Empty);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string?): test", "WriteLine(string?): ", "WriteLine(string?): " }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAnd1Arg()
        {
            Logger.WriteLine("val={0}", 1);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?): val=1" }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAnd2Args()
        {
            Logger.WriteLine("val1={0}, val2={1}", 1, true);

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?, object?): val1=1, val2=True" }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAnd3Args()
        {
            Logger.WriteLine("val1={0}, val2={1}, val3={2}", 1, true, "hoge");

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?, object?, object?): val1=1, val2=True, val3=hoge" }));
        }

        [Test]
        public void WriteLine_WithFormattedStringAndMultiArgs()
        {
            Logger.WriteLine("val1={0}, val2={1}, val3={2}, val4={3}", 1, true, "hoge", 'v');

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLine(string, object?[]): val1=1, val2=True, val3=hoge, val4=v" }));
        }

        [Test]
        public void WriteLineAsync_WithoutArgs()
        {
            Logger.WriteLineAsync().Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync()" }));
        }

        [Test]
        public void WriteLineAsync_WithChar()
        {
            Logger.WriteLineAsync('t').Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(char): t" }));
        }

        [Test]
        public void WriteLineAsync_WithReadOnlyMemory_Char()
        {
            Logger.WriteLineAsync("test".AsMemory(), CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(ReadOnlyMemory<char>, CancellationToken): test" }));
        }

        [Test]
        public void WriteLineAsync_WithCharArray_Range()
        {
            Logger.WriteLineAsync(['t', 'e', 's', 't'], 1, 2).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(char[], int, int): es" }));
        }

        [Test]
        public void WriteLineAsync_WithStringBuilder()
        {
            Logger.WriteLineAsync(new StringBuilder("test", 4), CancellationToken.None).Wait();
            Logger.WriteLineAsync(null as StringBuilder, CancellationToken.None).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(StringBuilder?, CancellationToken): test", "WriteLineAsync(StringBuilder?, CancellationToken): " }));
        }

        [Test]
        public void WriteLineAsync_WithRawString()
        {
            Logger.WriteLineAsync("test").Wait();
            Logger.WriteLineAsync(null as string).Wait();
            Logger.WriteLineAsync(string.Empty).Wait();

            Assert.That(innerWriter.GetData(), Is.EqualTo(new[] { "WriteLineAsync(string?): test", "WriteLineAsync(string?): ", "WriteLineAsync(string?): " }));
        }

        #endregion Write Operations

        #endregion Methods
    }
}
