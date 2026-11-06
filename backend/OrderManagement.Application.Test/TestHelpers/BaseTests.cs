using AutoFixture;
using AutoFixture.AutoNSubstitute;

namespace OrderManagement.Application.Tests.TestHelpers
{
    public abstract class BaseTests
    {
        protected IFixture _fixture;

        protected BaseTests()
        {
            Setup();
        }

        public void Setup()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization()
            {
                ConfigureMembers = true
            });
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}
