using Autofac;
using Xunit;

namespace Fast_Test.DB_Test
{
    public class Feature_t
    {
        [Fact]
        public void T1()
        {
            IContainer container = DI.DIContainer();
        }
    }
}