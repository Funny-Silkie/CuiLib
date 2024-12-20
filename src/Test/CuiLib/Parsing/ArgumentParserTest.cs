﻿using CuiLib;
using CuiLib.Commands;
using CuiLib.Options;
using CuiLib.Parameters;
using CuiLib.Parsing;
using NUnit.Framework;
using System;

namespace Test.CuiLib.Parsing
{
    public class ArgumentParserTest
    {
        private ArgumentParser parser;

        [SetUp]
        public void SetUp()
        {
            parser = new ArgumentParser(["child", "--num", "100", "-f", "value1", "value2"]);
        }

        #region Ctors

        [Test]
        public void Ctor_WithNull()
        {
            Assert.That(() => new ArgumentParser(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_WithNullContainingArray()
        {
            Assert.That(() => new ArgumentParser([null!]), Throws.ArgumentException);
        }

        [Test]
        public void Ctor_AsPositive()
        {
            Assert.That(() => new ArgumentParser(["hoge"]), Throws.Nothing);
        }

        #endregion Ctors

        #region Properties

        [Test]
        public void EndOfArguments_Get_OnEmptyArgs()
        {
            var parser = new ArgumentParser([]);

            Assert.That(parser.EndOfArguments, Is.True);
        }

        [Test]
        public void EndOfArguments_Get_OnNotEnd()
        {
            Assert.That(parser.EndOfArguments, Is.False);
        }

        [Test]
        public void EndOfArguments_Get_OnEnd()
        {
            var parser = new ArgumentParser(["hoge"]);
            parser.SkipArguments(6);

            Assert.That(parser.EndOfArguments, Is.True);
        }

        [Test]
        public void Index_Get_OnDefault()
        {
            Assert.That(parser.Index, Is.EqualTo(0));
        }

        [Test]
        public void ForcingParameter_Get_OnDefault()
        {
            Assert.That(parser.ForcingParameter, Is.False);
        }

        #endregion Properties

        #region Methods

        [Test]
        public void SkipArguments_WithNegative()
        {
            Assert.That(() => parser.SkipArguments(-1), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void SkipArguments_AsPositive_WithZero()
        {
            parser.SkipArguments(0);

            Assert.That(parser.Index, Is.EqualTo(0));
        }

        [Test]
        public void SkipArguments_AsPositive_WithNonOverflow()
        {
            parser.SkipArguments(2);

            Assert.That(parser.Index, Is.EqualTo(2));
        }

        [Test]
        public void SkipArguments_AsPositive_WithOverflow()
        {
            parser.SkipArguments(int.MaxValue);

            Assert.That(parser.Index, Is.EqualTo(6));
        }

        [Test]
        public void SkipArguments_AsPositive_OnEndOfArguments()
        {
            parser.SkipArguments(int.MaxValue);
            parser.SkipArguments(0);

            Assert.That(parser.Index, Is.EqualTo(6));
        }

        [Test]
        public void GetTargetCommand_WithNull()
        {
            Assert.That(() => parser.GetTargetCommand(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void GetTargetCommand_AsPositive_OnEndOfArguments()
        {
            parser.SkipArguments(6);

            Assert.That(parser.GetTargetCommand([new Command("child")]), Is.Null);
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithMissingCommand()
        {
            var child = new Command("1-2");
            child.Children.Add(new Command("2-1"));
            child.Children.Add(new Command("2-2"));
            var target = new Command("2-3");
            child.Children.Add(target);
            var commandCollection = new CommandCollection()
            {
                new Command("1-1"),
                child,
                new Command("1-3"),
            };

            Assert.Multiple(() =>
            {
                Assert.Multiple(() =>
                {
                    var parser = new ArgumentParser(["1-4"]);
                    Assert.That(parser.GetTargetCommand(commandCollection), Is.Null);
                    Assert.That(parser.Index, Is.EqualTo(0));
                });
                Assert.Multiple(() =>
                {
                    var parser = new ArgumentParser(["2-3", "1-2"]);
                    Assert.That(parser.GetTargetCommand(commandCollection), Is.Null);
                    Assert.That(parser.Index, Is.EqualTo(0));
                });
                Assert.Multiple(() =>
                {
                    var parser = new ArgumentParser(["1-2", "2-0"]);
                    Assert.That(parser.GetTargetCommand(commandCollection), Is.EqualTo(child));
                    Assert.That(parser.Index, Is.EqualTo(1));
                });
            });
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithExistingCommand()
        {
            var parser = new ArgumentParser(["hoge"]);
            var target = new Command("hoge");

            Assert.Multiple(() =>
            {
                Assert.That(parser.GetTargetCommand([target, new Command("fuga")]), Is.EqualTo(target));
                Assert.That(parser.Index, Is.EqualTo(1));
            });
        }

        [Test]
        public void GetTargetCommand_AsPositive_WithNestedCommand()
        {
            var parser = new ArgumentParser(["1-2", "2-3"]);
            var child = new Command("1-2");
            child.Children.Add(new Command("2-1"));
            child.Children.Add(new Command("2-2"));
            var target = new Command("2-3");
            child.Children.Add(target);

            Assert.Multiple(() =>
            {
                Assert.That(parser.GetTargetCommand([new Command("1-1"), child, new Command("1-3")]), Is.EqualTo(target));
                Assert.That(parser.Index, Is.EqualTo(2));
            });
        }

        [Test]
        public void ParseOption_WithNull()
        {
            Assert.That(() => parser.ParseOption(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void ParseOption_AsPositive_WithEmpty()
        {
            Assert.That(parser.ParseOption([]), Is.Null);
        }

        [Test]
        public void ParseOption_AsPositive_OnEndOfArguments()
        {
            var parser = new ArgumentParser(["-f", "value"]);
            var flag = new FlagOption('f');
            parser.SkipArguments(2);

            Assert.That(parser.ParseOption([flag]), Is.Null);
        }

        [Test]
        public void ParseOption_AsPositive_OnForcingParameter()
        {
            var parser = new ArgumentParser(["-s", "--", "value", "-f"]);
            var flag = new FlagOption('f');
            var valued = new SingleValueOption<string>('s');
            parser.ParseOption([flag, valued]);

            Assert.That(parser.ParseOption([flag, valued]), Is.Null);
        }

        [Test]
        public void ParseOption_AsPositive_OnNoOptionString()
        {
            var parser = new ArgumentParser(["-f", "value"]);
            var flag = new FlagOption('f');
            parser.SkipArguments(1);

            Assert.That(parser.ParseOption([flag]), Is.Null);
        }

        [Test]
        public void ParseOption_AsMissingOption()
        {
            var parser = new ArgumentParser(["-!", "value"]);
            var flag = new FlagOption('f');

            Assert.That(() => parser.ParseOption([flag]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ParseOption_AsPositive_OnNonValuedOptionWithShortName()
        {
            var parser = new ArgumentParser(["-f", "value"]);
            var flag = new FlagOption('f', "flag");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([flag]), Is.EquivalentTo(new[] { flag }));
                Assert.That(parser.Index, Is.EqualTo(1));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(flag.ValueAvailable, Is.True);
                Assert.That(flag.Value, Is.True);
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnNonValuedOptionWithFullName()
        {
            var parser = new ArgumentParser(["--flag", "value"]);
            var flag = new FlagOption('f', "flag");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([flag]), Is.EqualTo(new[] { flag }));
                Assert.That(parser.Index, Is.EqualTo(1));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(flag.ValueAvailable, Is.True);
                Assert.That(flag.Value, Is.True);
            });
        }

        [Test]
        public void ParseOption_AsDuplicatedNonValuedOption()
        {
            var parser = new ArgumentParser(["-f", "-f", "value"]);
            var flag = new FlagOption('f');
            var options = new OptionCollection() { flag };

            Assert.That(parser.ParseOption(options), Is.EquivalentTo(new[] { flag }));
            Assert.That(() => parser.ParseOption(options), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ParseOption_AsPositive_OnSingleValuedOptionWithShortName()
        {
            var parser = new ArgumentParser(["-n", "100", "value"]);
            var valued = new SingleValueOption<int>('n', "num");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(2));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnSingleValuedOptionWithFullName()
        {
            var parser = new ArgumentParser(["--num", "100", "value"]);
            var valued = new SingleValueOption<int>('n', "num");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(2));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnSingleValuedOptionWithForcingToken()
        {
            var parser = new ArgumentParser(["-n", "--", "100", "value"]);
            var valued = new SingleValueOption<int>('n', "num");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(3));
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnMultipleValuedOption_WithOneValue()
        {
            var parser = new ArgumentParser(["-n", "100", "-n", "200", "300", "value"]);
            var valued = new MultipleValueOption<int>('n', "num")
            {
                ValueCount = 1,
            };
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(2));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(new[] { 100 }));
            });

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(4));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(new[] { 100, 200 }));
            });

            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "300", "value" }));
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnMultipleValuedOption_WithThreeValues()
        {
            var parser = new ArgumentParser(["-n", "100", "200", "300", "value"]);
            var valued = new MultipleValueOption<int>('n', "num")
            {
                ValueCount = 3,
            };
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(4));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(new[] { 100, 200, 300 }));
            });

            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value" }));
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnMultipleValuedOption_WithAllValues1()
        {
            var parser = new ArgumentParser(["-n", "100", "200", "300", "-f", "value"]);
            var valued = new MultipleValueOption<int>('n', "num")
            {
                ValueCount = 0,
            };
            var flag = new FlagOption('f');
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued, flag]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(4));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(new[] { 100, 200, 300 }));
            });

            parser.ParseOption([valued, flag]);
            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value" }));
            });
        }

        [Test]
        public void ParseOption_AsPositive_OnMultipleValuedOption_WithAllValues2()
        {
            var parser = new ArgumentParser(["-n", "100", "--", "200", "300"]);
            var valued = new MultipleValueOption<int>('n', "num")
            {
                ValueCount = 0,
            };

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
                Assert.That(parser.Index, Is.EqualTo(5));
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(new[] { 100, 200, 300 }));
            });
        }

        [Test]
        public void ParseOption_AsMissingValueOfValuedOption()
        {
            var parser = new ArgumentParser(["-n"]);
            var valued = new SingleValueOption<int>('n', "num");

            Assert.That(() => parser.ParseOption([valued]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ParseOption_AsDuplicatedSingleValuedOption()
        {
            var parser = new ArgumentParser(["-n", "100", "-n", "200", "value"]);
            var valued = new SingleValueOption<int>('n', "num");

            Assert.That(parser.ParseOption([valued]), Is.EquivalentTo(new[] { valued }));
            Assert.That(() => parser.ParseOption([valued]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ParseOption_AsCombinedOptionsWithFlagAndValued_OnMissingValue()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');
            var parser = new ArgumentParser(["-acb", "100"]);

            Assert.That(() => parser.ParseOption([flag1, flag2, valued]), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ParseOption_AsCombinedOptionsWithFlagAndValued_AsPositive()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');
            var options = new OptionCollection() { flag1, flag2, valued };
            var parser = new ArgumentParser(["-abc", "100"]);

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption(options), Is.EquivalentTo(options));
                Assert.That(parser.Index, Is.EqualTo(2));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(flag1.ValueAvailable, Is.True);
                Assert.That(flag1.Value, Is.True);
                Assert.That(flag2.ValueAvailable, Is.True);
                Assert.That(flag2.Value, Is.True);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void ParseOption_AsCombinedOptionsWithFlagAndValued_WithForcingToken_AsPositive()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var valued = new SingleValueOption<int>('c');
            var options = new OptionCollection() { flag1, flag2, valued };
            var parser = new ArgumentParser(["-abc", "--", "100"]);

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption(options), Is.EquivalentTo(options));
                Assert.That(parser.Index, Is.EqualTo(3));
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(flag1.ValueAvailable, Is.True);
                Assert.That(flag1.Value, Is.True);
                Assert.That(flag2.ValueAvailable, Is.True);
                Assert.That(flag2.Value, Is.True);
                Assert.That(valued.ValueAvailable, Is.True);
                Assert.That(valued.Value, Is.EqualTo(100));
            });
        }

        [Test]
        public void ParseOption_AsCombinedOptionsWithFlags_AsPositive()
        {
            var flag1 = new FlagOption('a');
            var flag2 = new FlagOption('b');
            var flag3 = new FlagOption('c');
            var options = new OptionCollection() { flag1, flag2, flag3 };
            var parser = new ArgumentParser(["-acb"]);

            Assert.Multiple(() =>
            {
                Assert.That(parser.ParseOption(options), Is.EquivalentTo(options));
                Assert.That(parser.Index, Is.EqualTo(1));
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(flag1.ValueAvailable, Is.True);
                Assert.That(flag1.Value, Is.True);
                Assert.That(flag2.ValueAvailable, Is.True);
                Assert.That(flag2.Value, Is.True);
                Assert.That(flag3.ValueAvailable, Is.True);
                Assert.That(flag3.Value, Is.True);
            });
        }

        [Test]
        public void ParseParameters_WithNull()
        {
            Assert.That(() => parser.ParseParameters(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void ParseParameters_AsPositive_WithEmpty()
        {
            parser.ParseParameters([]);

            Assert.Multiple(() =>
            {
                Assert.That(parser.Index, Is.EqualTo(0));
                Assert.That(parser.EndOfArguments, Is.False);
            });
        }

        [Test]
        public void ParseParameters_AsPositive_OnEndOfArguments()
        {
            var parameters = new ParameterCollection();
            SingleValueParameter<string> param1 = parameters.CreateAndAdd<string>("p1");
            SingleValueParameter<string> param2 = parameters.CreateAndAdd<string>("p2");

            parser.SkipArguments(6);
            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(param1.ValueAvailable, Is.False);
                Assert.That(param2.ValueAvailable, Is.False);
            });
        }

        [Test]
        public void ParseParameters_AsPositive_WithSingleValuesAsNoRedundancy()
        {
            var parameters = new ParameterCollection();
            SingleValueParameter<string> param1 = parameters.CreateAndAdd<string>("p1");
            SingleValueParameter<string> param2 = parameters.CreateAndAdd<string>("p2");

            parser.SkipArguments(4);
            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.EndOfArguments, Is.True);
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(param1.ValueAvailable, Is.True);
                Assert.That(param1.Value, Is.EqualTo("value1"));
                Assert.That(param2.ValueAvailable, Is.True);
                Assert.That(param2.Value, Is.EqualTo("value2"));
            });
        }

        [Test]
        public void ParseParameters_AsPositive_WithSingleValuesAsRedundant()
        {
            var parameters = new ParameterCollection();
            SingleValueParameter<string> param = parameters.CreateAndAdd<string>("p1");

            parser.SkipArguments(4);
            Assert.That(() => parser.ParseParameters(parameters), Throws.TypeOf<ArgumentAnalysisException>());
        }

        [Test]
        public void ParseParameters_AsPositive_WithMultipleValue()
        {
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            parser.SkipArguments(4);
            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.EndOfArguments, Is.True);
                Assert.That(parser.ForcingParameter, Is.False);
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value1", "value2" }));
            });
        }

        [Test]
        public void ParseParameters_AsPositive_WithFirstForcingToken()
        {
            var parser = new ArgumentParser(["--", "value1", "value2"]);
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.EndOfArguments, Is.True);
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value1", "value2" }));
            });
        }

        [Test]
        public void ParseParameters_AsPositive_WithMiddleForcingToken()
        {
            var parser = new ArgumentParser(["value1", "--", "value2"]);
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.EndOfArguments, Is.True);
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value1", "value2" }));
            });
        }

        [Test]
        public void ParseParameters_AsPositive_WithLastForcingToken()
        {
            var parser = new ArgumentParser(["value1", "value2", "--"]);
            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.EndOfArguments, Is.True);
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value1", "value2" }));
            });
        }

        [Test]
        public void ParseParameters_AsPositive_WithForcingToken_OnForcingParameter()
        {
            var parser = new ArgumentParser(["-n", "--", "100", "value1", "--", "value2"]);
            var option = new SingleValueOption<int>('n');

            parser.ParseOption([option]);

            var parameters = new ParameterCollection();
            MultipleValueParameter<string> param = parameters.CreateAndAddAsArray<string>("param");

            parser.ParseParameters(parameters);

            Assert.Multiple(() =>
            {
                Assert.That(parser.EndOfArguments, Is.True);
                Assert.That(parser.ForcingParameter, Is.True);
                Assert.That(param.ValueAvailable, Is.True);
                Assert.That(param.Value, Is.EqualTo(new[] { "value1", "--", "value2" }));
            });
        }

        #endregion Methods
    }
}
