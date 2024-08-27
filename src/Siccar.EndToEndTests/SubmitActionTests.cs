using System.Net;
using Siccar.EndToEndTests.Action;
using Siccar.EndToEndTests.ApiWrappers.Register;
using Siccar.EndToEndTests.Blueprint;
using Siccar.EndToEndTests.Blueprint.Models;
using Siccar.EndToEndTests.Wallet;
using Siccar.EndToEndTests.Wallet.Models;

namespace Siccar.EndToEndTests
{
    public class SubmitActionTests : EndToEndTestFixture, IClassFixture<EndToEndTestFixture>
    {
        [Fact]
        public async Task SubmitAction_ShouldSaveTransaction()
        {
            // Arrange
            await SiccarServices.Clear();

            var senderWallet = WalletTestData.NewDefault();
            senderWallet.Name = "Sender Wallet";
            var createdSenderWallet = await (await SiccarServices.Wallets.Create(senderWallet)).GetContent<CreateWalletResponse>();

            var receiverWallet = WalletTestData.NewDefault();
            receiverWallet.Name = "Receiver Wallet";
            receiverWallet.Mnemonic = null;
            var createdReceiverWallet = await (await SiccarServices.Wallets.Create(receiverWallet)).GetContent<CreateWalletResponse>();

            var register = RegisterTestData.NewDefault();
            await SiccarServices.Registers.Create(register);

            var blueprint = BlueprintTestData.NewDefault(createdSenderWallet!.Address!, createdReceiverWallet!.Address!);
            var blueprintPublishTransaction = await (await SiccarServices.Blueprints.CreateAndPublish(this, blueprint, createdSenderWallet.Address!, register.Id!)).GetContent<Transaction>();

            // Act
            var submitResponse = await SiccarServices.Actions.Submit(this, blueprintPublishTransaction?.Id!, blueprintPublishTransaction?.Id!, createdSenderWallet.Address!, 
                register.Id!, 
                new
                {
                    name = "Test-Name",
                    surname = "Test-Surname"
                });

            // Assert
            Assert.Equal(HttpStatusCode.Accepted, submitResponse.StatusCode);
            var submitTransaction = await submitResponse.GetContent<Transaction>();
            await ActionAssertions.HasCount(1, register.Id!, submitTransaction?.Id!);
        }
    }
}