using TenantFlow.Infrastructure.Services;


namespace TenantFlow.Tests.Fakes
{
    public class FakeTenantContextSetter : ITenantContextSetter
    {
        public Guid? CalledWithValue { get; private set; }
        public bool WasCalled { get; private set; }
        public void SetTenantId(Guid tenantId)
        {
            WasCalled = true;
            CalledWithValue = tenantId;
        }

    }
}
