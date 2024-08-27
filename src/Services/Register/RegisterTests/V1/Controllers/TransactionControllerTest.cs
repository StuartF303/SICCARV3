using FakeItEasy;
using Microsoft.Extensions.Logging;
using Siccar.Common;
using Siccar.Platform.Cryptography;
using Siccar.Platform;
using Siccar.Registers.Core;
using Siccar.Registers.RegisterService.V1;
using System;
using System.Threading.Tasks;
using Xunit;
using Siccar.Common.Adaptors;
using Siccar.Registers.RegisterService.V1.Adapters;
using Siccar.Registers.RegisterService.V1.Services;
#nullable enable

namespace RegisterTests.V1.Controllers
{
	public class TransactionControllerTest
	{
		private readonly TransactionsController _underTest;
		private readonly IRegisterRepository _registerRepositoryFake;
		private readonly IRegisterResolver _registerResolver;
		private readonly IDaprClientAdaptor _daprClientAdaptorFake;
		private readonly ILogger<TransactionsController> _loggerFake;

		public TransactionControllerTest()
		{
			_registerRepositoryFake = A.Fake<IRegisterRepository>();
			_daprClientAdaptorFake = A.Fake<IDaprClientAdaptor>();
			_registerResolver = A.Fake<IRegisterResolver>();
			_loggerFake = A.Fake<ILogger<TransactionsController>>();
			_underTest = new TransactionsController(_loggerFake, _registerRepositoryFake, _registerResolver, _daprClientAdaptorFake, A.Fake<IHubContextAdapter>());
		}
		//public class PostLocalTransaction : TransactionControllerTest
		//{
		//    [Fact]
		//    public async Task Should_Call_Insert_With_TransactionInRequest()
		//    {
		//        var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
		//        var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
		//        var payload = Convert.FromBase64String("SGVsbG8K");
		//        var transaction = BuildTransaction(sendingWallet, privateKey, payload);
		//        var txJson = JsonDocument.Parse(JsonSerializer.Serialize(transaction));

		//        await _underTest.PostLocalTransaction(txJson);

		//        A.CallTo(() => _registerRepositoryFake.InsertTransactionAsync(A<Transaction>.That.Matches(tx => tx.TxId == transaction.TxId))).MustHaveHappenedOnceExactly();
		//    }
		//}

		public class PostPostRemoteTransaction : TransactionControllerTest
		{
			[Fact]
			public async Task Should_Call_Insert_With_TransactionInRequest()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				var payload = Convert.FromBase64String("SGVsbG8K");
				var transaction = BuildTransaction(sendingWallet, privateKey, payload);
				await _underTest.PostRemoteTransaction(transaction);
				A.CallTo(() => _registerRepositoryFake.InsertTransactionAsync(A<TransactionModel>.That.Matches(tx => tx.TxId == transaction.TxId))).MustHaveHappenedOnceExactly();
			}

			[Fact]
			public async Task Should_Call_PublishEvent_TransactionConfirmed()
			{
				var sendingWallet = "ws1jr8nj8k98ekrq23q43h9pmc2r3299m3nxc8vj9n57z264phlf96wq66r7k7";
				var privateKey = "BujfkmHjHT2HPEJCga7KWjDsAdmgCK5HR3W2WwpmwUGdZU1ryjNFWRYuPpy11eyWrGPdCpZ2rzXnx19EAL4TibUB9mbakM";
				var payload = Convert.FromBase64String("SGVsbG8K");
				var transaction = BuildTransaction(sendingWallet, privateKey, payload);
				transaction.MetaData = new TransactionMetaData { TransactionType = TransactionTypes.Action };

				await _underTest.PostRemoteTransaction(transaction);

				A.CallTo(() => _daprClientAdaptorFake.PublishEventAsync(
					A<string>.Ignored,
					Topics.TransactionConfirmedTopicName,
					A<TransactionConfirmed>.That.Matches(tx => tx.TransactionId == transaction.TxId)))
					.MustHaveHappenedOnceExactly();
			}
		}

		private static TransactionModel BuildTransaction(string sendingWallet, string sendingWalletPrivateKey, byte[] payload)
		{
			var transaction = TransactionBuilder.Build(TransactionVersion.TX_VERSION_LATEST);
			transaction.SetTxRecipients(new string[] { sendingWallet });
			var payloadMgr = transaction.GetTxPayloadManager();
			payloadMgr.AddPayload(payload, new string[] { sendingWallet });
			transaction.SignTx(sendingWalletPrivateKey);
			return TransactionFormatter.ToModel(transaction.GetTxTransport().transport)!;
		}
	}
}
