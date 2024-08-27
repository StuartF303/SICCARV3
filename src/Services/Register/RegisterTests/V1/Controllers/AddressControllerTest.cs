using FakeItEasy;
using Siccar.Common.Adaptors;
using Siccar.Platform;
using Siccar.Registers.RegisterService.V1;
using System.Threading.Tasks;
using Xunit;
#nullable enable

namespace RegisterTests.V1.Controllers
{
    public class AddressControllerTest
    {
        private readonly AddressController _underTest;
        private readonly IDaprClientAdaptor _client;
        public AddressControllerTest()
        {
            _client = A.Fake<IDaprClientAdaptor>();
            _underTest = new AddressController(_client);
        }

        public class PostLocalAddress : AddressControllerTest
        {
            [Fact]
            public async Task Should_Call_SaveStateAsync()
            {
                var address = new WalletAddress() { Address = "some-address", WalletId = "some-address" };
                await _underTest.PostLocalAddress(address);
                A.CallTo(() => _client.SaveStateAsync("registerstore", address.WalletId, address)).MustHaveHappenedOnceExactly();
            }
        }
    }
}
