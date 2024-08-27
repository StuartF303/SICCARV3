// TransactionBuilderV3 Class Unit Test File - The Welder/Skid Row
// 10.04.2021
// Wallet Services (Siccar)

using Xunit;
#nullable enable

namespace Siccar.Platform.Cryptography.Tests
{
	public class TransactionBuilderV3Tests
	{
		private SigningModule sm = new SigningModule(SigningModuleBase.SMSignMethod.USE_SODIUM);

		[Fact]
		public void BuildUnencryptedTxTest()
		{
			string data = @"{""txId"":""eeeff1e4b2c7732dbfc7d30a5fb742532ed9de64113abeb7a90bebdfa1d81c19""
			,""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
			""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
			""recipientsWallets"":[],""timestamp"":""2022-01-13T10:40:31.0000000Z"",""signature"":
			""0d717e0d2efe36da7ee5c17e1813a52bc961543e48688a1a203c04405f36f49956b22ce7a2b6d54eecc7462358d3d7044e27b918402efc147354a5bf6e85c107"",
			""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
			""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedTxTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[],""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":""bec57f393762d400b283feadec695978"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildUnencryptedTxPrevTxTest()
		{
			string data = @"{""txId"":""29feebe9a1799cf73e2b56bc010604f385f98842583aa0987d23bf325503469b"",
				""prevTxId"":""142af48f6cb3b9b795275e3bf41ed10dc8029ef091db810c596e62c7579bc39b"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],
				""timestamp"":""2022-01-24T02:43:40.0000000Z"",
				""signature"":""1ee82ec8e2b4562ea1696d30044023c0bc6e4d66ab2b9c64afb08c9ed45f48fcd2ac60ade3a3f01fc973bfcda8e22afaeb486c18aa68b633102faadedb7af609"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedTxPrevTxTest()
		{
			string data = @"{""txId"":""b397e786b3af14619033ce39b36692dc60f132e40d5182037ebbc4e4e73b035d"",
				""prevTxId"":""142af48f6cb3b9b795275e3bf41ed10dc8029ef091db810c596e62c7579bc39b"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],
				""timestamp"":""2022-01-24T02:39:26.0000000Z"",
				""signature"":""3c2f8c940fbeb75634eb8345e3b2c9c3b333889e22f36409f0346f343024d7adee4fd71b8d878766aed8ae5b45f31b33840eb4575bea3ed4355b3b1a1ac69b06"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],""challenges"":[{""size"":80,
				""hex"":""af19cb4a52a0937f817807a857e3c4041ffa77722da711d0ee5dee9a5114783a1ab04d77c1502df38d256dc8107138eb25b568fff3196bedb8a1e481fbaca5ed0faf8c08a992ca5e7d1ea7b96456cd62""}],
				""iV"":{""size"":16,""hex"":""7d10d6f52e03c1dab85945cc7d82923d""},""payloadSize"":16,""hash"":""c0bfe4fe4df3f2148273c1a48d1ddb91d7b515b7f58850709c678b9b05665b50"",
				""data"":""6b09461e9d9a2ad9b661d7e7dd5c2ad0"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildUnencryptedTxRecipientsTest()
		{
			string data = @"{""txId"":""f8ee0116e1b1167abd45ad4b452fcee0e9728fe64204476d07af1fd4baa7b448"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c"",
				""ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz""],""timestamp"":""2022-01-13T12:10:49.0000000Z"",
				""signature"":""34b5a1af38687babc76f5d13185db8ec4925e791b567ed101d14b898aa45e90790da26e88ab89df02abcd1d07dc82758009b20a68f4c3ddb04294b057f79e90a"",
				""payloadCount"":1,""payloads"":[{ ""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedTxRecipientsTest()
		{
			string data = @"{""txId"":""6fe67b8337844b78569afa7d7e6d91953c24ff17a2fe5f3f70347a06fd486f2d"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c"",
				""ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz""],""timestamp"":""2022-01-13T12:14:10.0000000Z"",
				""signature"":""e4f5c2912716369e3e01e64a7a212cfb6c514e958ef9d033a8f541d4128c8f0a9ebe79e1a8f050f6496fb35b0675fb9c1091654d7b5860d3f5e87798d7ed1304"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c"",""ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz""],""challenges"":[{
				""size"":80,""hex"":
				""cbba2ec7e20a4666b7dea7118ff8962c112243fc3b3f85dfda350028a7ea7f7669e411722f348081805e018bbbdff8c6ad515fcbff6452d351a8d6e0c4b048ac7db51cd1103ede72540e1777f68569d2""},{
				""size"":80,""hex"":
				""553e2be40ec186198a748f4ab3646f8dd8106b78fc892ce97e6bdb367b7d77646cfc1969c4c57f92f7f37de78477c9a36db5087e09ad55df3499e471fb4fdc1df532fc90bccd2b37ada39f418d94bab6""},{
				""size"":80,""hex"":
				""aa1c9da26260b119603f68254882f9d35872702a32f9a7972d962967916b994870f803bce36f8594fd68315cfca199c72bd1f2209b0e5fc682e49e2b34ccb2d61339fd907030ff325e1a50173207742d""}],
				""iV"":{""size"":16,""hex"":""dc7387b39772ab235f1e666dab123b8e""},""payloadSize"":16,""hash"":""cebe691c450a08ec56060d8e042a67b79fdbb7d4593a5e8ea5977ebbafd36157"",
				""data"":""d752514feed45d4756217c78ed6a244d"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildUnencryptedTxMultiplePayloadsTest()
		{
			string data = @"{""txId"":""de894aa81c9faabe67ee269d5f4ae89ae440c7fcd1d92e784a755b8938ace894"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:23:17.0000000Z"",
				""signature"":""500a8b4082bfca9f976802b48ff4aa8113561efd092eec7a5d2184910f205528b21cc4e0f1ba01873f319865a9858bf7c3f9ddccf0899911b396ec6b784cfc01"",
				""payloadCount"":2,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false},{""walletAccess"":[],""payloadSize"":4,""hash"":
				""a74a23de70edf7797ff707c0195293847ccf338d883bfe7251938880ef53ae2a"",""data"":""21314151"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedTxMultiplePayloadsTest()
		{
			string data = @"{""txId"":""161f4dcbe071703495753305e1f6a81ee81589c6c21ace290ca94ffe13dd542e"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:26:35.0000000Z"",
				""signature"":""2f8475d0a4f1d9d9bef2a19bf705a3caa2afe3098ba0bb614cda234ffc6e830f9a253da6ba8ec58ea30ffaf845c8e8bf18ad25dd5f0f6c99ef579151c7613207"",
				""payloadCount"":2,""payloads"":[{""walletAccess"":[""ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz"",
				""ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c""],""challenges"":[{""size"":80,""hex"":
				""116b5048f79428610a0d3114c145dccd5aa28081e872598943c48d4a6f01b527c99f09f50e9f2318b8bf63b0d80768dab8d18dd2ee5406669f83a28a6e08333dcfa35a400c4adae023aa6926ede43441""},{
				""size"":80,""hex"":
				""d082dbb63c5f899f3a711b2ef343dff7598107d0e21cfadf2eca77a2a4005b629f3f43e36796a7d62d0d22be47af8cc645be7e77d72bf5e17e2e54a19c09fa2fe5ba2af2b0f6d9e57a85d2379cc6c669""}],
				""iV"":{""size"":16,""hex"":""3752cb2f0475746cc7c30b80f9396673""},""payloadSize"":16,""hash"":""1bcb12b0b145b6da0006b4875a85b7672118e573108d7412a7c0e58a3a2473b9"",
				""data"":""8202a3316299efcc88d89f687b72e1dc"",""encrypted"":true},{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""fd652e72b0cc554248b0f4a1d5e794ea2cc41f432dfc6f8d4d56d24c3fc9e8395e91fa84ca83704632174826ebb35319bb831c57ac876372c23ec3500fc1113b62849c67cb07bb02693bdee187958f1c""}],
				""iV"":{""size"":16,""hex"":""bad0455953497bff0c9c98eb92fe3d81""},""payloadSize"":16,""hash"":""079904746af48222ab21fed59907a68130ce7d8484c85222313570e3ebf6df31"",
				""data"":""e5a0bdceca8ae1a99ae26c3fd75aef4b"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildUnencryptedTxPCountZeroTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedTxPCountZeroTest()
		{
			string data = @"{""txId"":""3f9c82fa3b3f7c7e79dc8a6fdfd13f15c86ea448050ae51d77da69cebd1943a0"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:43:41.0000000Z"",
				""signature"":""645744e0545096e80e9715a3822fc7d03b785d09f94c2082323ca43f915b28fc00626993824e745cbf69c9a4b1655d11d05a7b0becafe285907001caa8a03103"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],""challenges"":[{
				""size"":80,""hex"":
				""a4cc39e8815dcbd494250fbe3fcb05199acf1af53a821ebd20dc8029c50c3346fdae2fa9b647bd6dc7b1b5947805ed6737866406d896f09a86cf6df6a21756eb1c89b0d0f7fed91ddf27a72225175b80""}],
				""iV"":{""size"":16,""hex"":""4d0ca7faffb8254d3bbc62eaf0123549""},""payloadSize"":16,""hash"":
				""f7a925b4f7ffe3e90711dc3b5d106de4cbed19e679f0f3f6dc8127ce46ea91aa"",""data"":""19083f48245fd05dc5dea7ee40b8bc36"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildMissingTxIdTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildMissingPrevTxIdTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",
				""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildBadLowVersionTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":0,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildBadHighVersionTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":4,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoSenderTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,
				""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoRecipientsTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoTimeStampTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoSignatureTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoPayloadCountTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoPayloadsTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoPayloadSizeTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoPayloadHashTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoPayloadDataTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildNoEncryptedFlagTest()
		{
			string data = @"{""txId"":""fcef67158d82d2be502460db304a979f9d777b3a6212b5a9bcc50936d4391e96"",""prevTxId"":
				""0000000000000000000000000000000000000000000000000000000000000000"",""version"":3,""senderWallet"":
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",""recipientsWallets"":[],""timestamp"":""2022-01-13T12:40:17.0000000Z"",
				""signature"":""5686d51f63bc4476745f556cc0d702dfcaf5f43c8b4f95ce4b9da0e1fddc7ef39f1b64936e7d0fc0cf1c330efa9526e86ff23d29c33cd68c21e2b1b3a20fe00e"",
				""payloadCount"":0,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555""}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildUnencryptedInvalidJSONTxTest()
		{
			string data = @"{""txId"":""eeeff1e4b2c7732dbfc7d30a5fb742532ed9de64113abeb7a90bebdfa1d81c19""
			,""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
			""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
			""recipientsWallets"":[,""timestamp"":""2022-01-13T10:40:31.0000000Z"",""signature"":
			""0d717e0d2efe36da7ee5c17e1813a52bc961543e48688a1a203c04405f36f49956b22ce7a2b6d54eecc7462358d3d7044e27b918402efc147354a5bf6e85c107"",
			""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
			""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedInvalidJSONTxTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[,""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":""bec57f393762d400b283feadec695978"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildInvalidTxIdTest()
		{
			string data = @"{""txId"":""eeeff1e4b2c7732dbfc7d30a5b742532ed9de64113abeb7a90bebdfa1d81c19""
			,""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
			""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
			""recipientsWallets"":[],""timestamp"":""2022-01-13T10:40:31.0000000Z"",""signature"":
			""0d717e0d2efe36da7ee5c17e1813a52bc961543e48688a1a203c04405f36f49956b22ce7a2b6d54eecc7462358d3d7044e27b918402efc147354a5bf6e85c107"",
			""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
			""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildInvalidPrevTxIdTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59""
			,""prevTxId"":""0000000000000000000000000000000000000"",
			""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
			""recipientsWallets"":[],""timestamp"":""2022-01-13T10:40:31.0000000Z"",""signature"":
			""0d717e0d2efe36da7ee5c17e1813a52bc961543e48688a1a203c04405f36f49956b22ce7a2b6d54eecc7462358d3d7044e27b918402efc147354a5bf6e85c107"",
			""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
			""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildCorruptSenderWalletTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59""
			,""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
			""version"":3,""senderWallet"":""ts1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
			""recipientsWallets"":[],""timestamp"":""2022-01-13T10:40:31.0000000Z"",""signature"":
			""0d717e0d2efe36da7ee5c17e1813a52bc961543e48688a1a203c04405f36f49956b22ce7a2b6d54eecc7462358d3d7044e27b918402efc147354a5bf6e85c107"",
			""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
			""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildCorruptRecipientsTest()
		{
			string data = @"{""txId"":""f8ee0116e1b1167abd45ad4b452fcee0e9728fe64204476d07af1fd4baa7b448"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[""ts1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c"",
				""ws1jdvlej9527m5z46lwvaq4nz6ajznz0erleys4xekrht36ucmqpcws3rcwzz""],""timestamp"":""2022-01-13T12:10:49.0000000Z"",
				""signature"":""34b5a1af38687babc76f5d13185db8ec4925e791b567ed101d14b898aa45e90790da26e88ab89df02abcd1d07dc82758009b20a68f4c3ddb04294b057f79e90a"",
				""payloadCount"":1,""payloads"":[{ ""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildDuplicateRecipientsTest()
		{
			string data = @"{""txId"":""f8ee0116e1b1167abd45ad4b452fcee0e9728fe64204476d07af1fd4baa7b448"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""ws1j5fvq9x8x570er5eaktlju0kcz79934l8plddwv4f6r9x6hzq7nuqewqf0c"",
				""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],""timestamp"":""2022-01-13T12:10:49.0000000Z"",
				""signature"":""34b5a1af38687babc76f5d13185db8ec4925e791b567ed101d14b898aa45e90790da26e88ab89df02abcd1d07dc82758009b20a68f4c3ddb04294b057f79e90a"",
				""payloadCount"":1,""payloads"":[{ ""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
				""data"":""448955555555"",""encrypted"":false}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildInvalidSignatureTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[],""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb525d695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":""bec57f393762d400b283feadec695978"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildInvalidTimeStampTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[],""timestamp"":""2022"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":""bec57f393762d400b283feadec695978"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildCorruptPayloadWalletTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[],""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ts1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":""bec57f393762d400b283feadec695978"",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		//[Fact]
		//public void BuildBadPayloadSizeTest()
		//{
		//	string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
		//		""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
		//		""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
		//		""recipientsWallets"":[],""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
		//		""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
		//		""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
		//		""challenges"":[{""size"":80,""hex"":
		//		""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
		//		""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":0,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
		//		""data"":""bec57f393762d400b283feadec695978"",""encrypted"":true}]}";
		//	var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
		//	Assert.Equal(Status.TX_CORRUPT_PAYLOAD, result);
		//	Assert.Null(Transaction);
		//}
		[Fact]
		public void BuildNoDataTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[],""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":"""",""encrypted"":true}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildEncryptedNoEncryptionFlagTest()
		{
			string data = @"{""txId"":""215ccfde675f0ca208bf67c41a46f9b81ad371048e652215595bd2072dddff59"",
				""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
				""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
				""recipientsWallets"":[],""timestamp"":""2022-01-13T10:49:52.0000000Z"",""signature"":
				""ddb7a3c91706387b8e47dbf612445dcd7cb52588f2cd695ab808da5e53485647f9cb7f3a88985c94ecac2f694a0e20afbcec65646cbecacff06ff1f9b4bde60a"",
				""payloadCount"":1,""payloads"":[{""walletAccess"":[""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t""],
				""challenges"":[{""size"":80,""hex"":
				""43a39383e07b2f3b9e1d7ab0166aee182396d486ddf387664c36c0db8120383af82b2906b057a0c24380748dbfd1f4d159cab549f5f7cc2b47f855f719aebf7e1847bb49ab239d21434b706a574cca7d""}],
				""iV"":{""size"":16,""hex"":""7b030b44e370a59e20314b25a0472d1b""},""payloadSize"":16,""hash"":""c7723257d7c1703c091e96d64aac0ee689214eaff6e12a8b78266923221b14d9"",
				""data"":""bec57f393762d400b283feadec695978""}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
		[Fact]
		public void BuildUnencryptedNoEncryptionFlagTest()
		{
			string data = @"{""txId"":""eeeff1e4b2c7732dbfc7d30a5fb742532ed9de64113abeb7a90bebdfa1d81c19""
			,""prevTxId"":""0000000000000000000000000000000000000000000000000000000000000000"",
			""version"":3,""senderWallet"":""ws1jewlxg95hm6ejy3a3wx0wanplcfxgh0dvehclfpwph943jkpvwdjq409g2t"",
			""recipientsWallets"":[],""timestamp"":""2022-01-13T10:40:31.0000000Z"",""signature"":
			""0d717e0d2efe36da7ee5c17e1813a52bc961543e48688a1a203c04405f36f49956b22ce7a2b6d54eecc7462358d3d7044e27b918402efc147354a5bf6e85c107"",
			""payloadCount"":1,""payloads"":[{""walletAccess"":[],""payloadSize"":6,""hash"":""4e2841c9fb80d3fead8e6ef620e9d3ebf5e1904adc34e4832e09186842875fcd"",
			""data"":""448955555555""}]}";
			var (result, Transaction) = TransactionBuilder.Build(data, TransactionFormat.JSON, sm);
			Assert.Equal(Status.TX_BAD_FORMAT, result);
			Assert.Null(Transaction);
		}
	}
}
