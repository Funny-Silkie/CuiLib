using NUnit.Framework;
using System.Globalization;

namespace Test
{
    public abstract class TestBase
    {
#if NET481

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

#endif

        protected TestBase()
        {
        }
    }
}
