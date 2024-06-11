using CuiLib.Checkers;
using NUnit.Framework;

namespace Test.CuiLib.Checkers
{
    [TestFixture]
    public class ValueCheckStateTest
    {
        #region Static Properties

        [Test]
        public void Success()
        {
            ValueCheckState state = ValueCheckState.Success;

            Assert.Multiple(() =>
            {
                Assert.That(state.IsValid, Is.True);
                Assert.That(state.Error, Is.Null);
            });
        }

        #endregion Static Properties

        #region Static Methods

        [Test]
        public void AsError_WithNull()
        {
            ValueCheckState state = ValueCheckState.AsError(null);

            Assert.Multiple(() =>
            {
                Assert.That(state.IsValid, Is.False);
                Assert.That(state.Error, Is.Not.Null);
            });
        }

        [Test]
        public void AsError_WithEmptyString()
        {
            ValueCheckState state = ValueCheckState.AsError(string.Empty);

            Assert.Multiple(() =>
            {
                Assert.That(state.IsValid, Is.False);
                Assert.That(state.Error, Is.EqualTo(string.Empty));
            });
        }

        [Test]
        public void AsError_WithString()
        {
            ValueCheckState state = ValueCheckState.AsError("ERROR!");

            Assert.Multiple(() =>
            {
                Assert.That(state.IsValid, Is.False);
                Assert.That(state.Error, Is.EqualTo("ERROR!"));
            });
        }

        #endregion Static Methods

        #region Methods

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void Equals()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueCheckState.Success.Equals(ValueCheckState.Success), Is.True);
                Assert.That(ValueCheckState.Success.Equals(ValueCheckState.AsError("ERROR!")), Is.False);
                Assert.That(ValueCheckState.AsError("ERROR!").Equals(ValueCheckState.AsError("ERROR!")), Is.True);
                Assert.That(ValueCheckState.AsError("ERROR!").Equals(ValueCheckState.AsError("other")), Is.False);
            });
        }

#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

#pragma warning disable NUnit2009 // The same value has been provided as both the actual and the expected argument

        [Test]
        public new void GetHashCode()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueCheckState.Success.GetHashCode(), Is.EqualTo(ValueCheckState.Success.GetHashCode()));
                Assert.That(ValueCheckState.AsError("ERROR!").GetHashCode(), Is.EqualTo(ValueCheckState.AsError("ERROR!").GetHashCode()));
            });
        }

#pragma warning restore NUnit2009 // The same value has been provided as both the actual and the expected argument

        #endregion Methods

        #region Operators

#pragma warning disable NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        [Test]
        public void Op_Equality()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueCheckState.Success == ValueCheckState.Success, Is.True);
                Assert.That(ValueCheckState.Success == ValueCheckState.AsError("ERROR!"), Is.False);
                Assert.That(ValueCheckState.AsError("ERROR!") == ValueCheckState.AsError("ERROR!"), Is.True);
                Assert.That(ValueCheckState.AsError("ERROR!") == ValueCheckState.AsError("other"), Is.False);
            });
        }

        [Test]
        public void Op_Inequality()
        {
            Assert.Multiple(() =>
            {
                Assert.That(ValueCheckState.Success != ValueCheckState.Success, Is.False);
                Assert.That(ValueCheckState.Success != ValueCheckState.AsError("ERROR!"), Is.True);
                Assert.That(ValueCheckState.AsError("ERROR!") != ValueCheckState.AsError("ERROR!"), Is.False);
                Assert.That(ValueCheckState.AsError("ERROR!") != ValueCheckState.AsError("other"), Is.True);
            });
        }

#pragma warning restore NUnit2010 // Use EqualConstraint for better assertion messages in case of failure

        #endregion Operators
    }
}
