using FakeItEasy;
using Microsoft.Extensions.Logging;
using Siccar.Platform;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using WalletService.Core;
using Xunit;
using WalletService.SQLRepository;
using Microsoft.EntityFrameworkCore;
using WalletService.Core.Interfaces;

namespace WalletService.UnitTests.V1.Repositories
{
    public class WalletRepositoryTest
    {
        private readonly ILogger<WalletRepository> _fakeLogger;
        private readonly WalletContext _dBContext;
        private readonly IWalletProtector _fakeWalletProtector;
        private readonly IWalletRepository _underTest;
        private readonly DbContextOptions<WalletContext> _walletDbOpts;

        private const string _encryptionKey = "WalletService.SQLRepository.dataprotector";

        readonly string expectedOwner = Guid.NewGuid().ToString();
        readonly string expectedOwnerAlt = Guid.NewGuid().ToString();
        readonly string delegateUser = Guid.NewGuid().ToString();
        readonly string delegateUserAlt = Guid.NewGuid().ToString();
        private const string wallet1Address = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa6";
        private const string wallet1Key = "BrZRtSCdr3Swz64TfyohvCsbfpQs6jNxyjBxzMgT4pBfnQMS9sZeSgK9FZxE7ZLfry8ttGR1NnVG8N2DHLcTi17LCzq2og";
        readonly string alternateOwner = Guid.NewGuid().ToString();
        private const string wallet2Address = "ws1junc7rlkesnf33mm5yz83zamamy244995kqys5jt0nndlxpdq4tcqp6d7xe";
        private const string wallet2Key = "BsARtSCdr3Swz64TfyohvCsbfpQs6jNxyjBxzMgT4pBfnQMS9sZeSgK9FZxE7ZLfry8ttGR1NnVG8N2DHLcTi17LCzq2og";
        private const string wallet4Address = "ws1jle65qnan70zm4jeuxtvqk9vf3rwaqlzvhmlg5949jmry4g9r376sca3nhv";
        private const string wallet4Key = "CfDJ8D3WIGalsetLt98nJXJdyvyfJh9S0LfkJweav7kv94AsvJ8Ladl_puOWTr6O9mT391N40wHqi76rAErOfoG2TRVKDAexFa6X_bo2VU1x2MNveRMnFB4J7LcFpIhJ5nl7ZVpr5sqHvQ6n1e5HUP_bRKu7Ujg2qSrMsQIw9ROoYodQ9vCyTZkMTYYJp-YgR7omjOTy5rOdZulRiGURNtv6lGMRi3OGAO0rQECHnVMUGzgI";

        /// <summary>
        /// These tests target the Wallet SQL Repository interface
        /// We are executing by using the EF In Memory Store
        /// </summary>
        public WalletRepositoryTest()
        {

            _fakeLogger = A.Fake<ILogger<WalletRepository>>();
            _fakeWalletProtector = A.Fake<IWalletProtector>();
            _fakeWalletProtector.MasterEncryptionKey = _encryptionKey;

            _walletDbOpts = new DbContextOptionsBuilder<WalletContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

            _dBContext = new WalletContext(_walletDbOpts);

            Wallet wallet2 = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet2Address);
            Wallet wallet4 = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet4Address);
            _underTest = new WalletRepository(_fakeLogger, _dBContext, _fakeWalletProtector);
        }

        public class CreateWallet : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_Create_Wallet_1()
            {
                SetupData();
                var randomAddr = Guid.NewGuid().ToString();

                var testWallet = BuildMockWalletObject(randomAddr);

                A.CallTo(() => _fakeWalletProtector.ProtectWallet(testWallet)).Returns<Wallet>(
                    new Wallet()
                    {
                        Address = testWallet.Address,
                        Name = testWallet.Name,
                        Addresses = testWallet.Addresses,
                        Delegates = testWallet.Delegates,
                        Owner = testWallet.Owner,
                        PrivateKey = testWallet.PrivateKey,
                        Tenant = testWallet.Tenant,
                        Transactions = testWallet.Transactions
                    });

                Assert.True(await _underTest.SaveWallet(testWallet, expectedOwner));
                Assert.Contains(testWallet, _dBContext.Wallets);
            }
        }

        public class GetAll : WalletRepositoryTest
        {
            [Fact]
            public async Task Simple_GetAll_Wallets()
            {
                SetupData();

                var wallets = await _underTest.GetAll();

                Assert.NotEmpty(wallets);
                int wc = wallets.Count();
                Assert.True(wc > 1); // should be atleast two from bootstrap
            }

            [Fact]
            public async Task GetAll_Wallet_ByOwner()
            {
                SetupData();

                var wallets = await _underTest.GetAll(expectedOwner);

                Assert.Single(wallets);

            }

            [Fact]
            public async Task Should_BeEmpty_WhenOwnerHasNoWallets()
            {
                SetupData();

                var wallets = await _underTest.GetAll("NotAnOwner");

                Assert.Empty(wallets);

            }
        }

        public class GetAllInTenant : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_ReturnSingleWallet()
            {
                _dBContext.Wallets.Add(new Wallet
                {
                    Address = wallet1Address,
                    Name = "Preset Test Wallet",
                    Owner = expectedOwner,
                    Tenant = "test-tenant",
                    PrivateKey = wallet1Key,
                });

                _dBContext.SaveChanges();

                var wallets = await _underTest.GetAllInTenant("test-tenant");
                
                Assert.Single(wallets);
            }

            [Fact]
            public async Task Should_ReturnNoWalletsWhenNoneInrequestedTenant()
            {
                _dBContext.Wallets.Add(new Wallet
                {
                    Address = wallet1Address,
                    Name = "Preset Test Wallet",
                    Owner = expectedOwner,
                    Tenant = "other-tenant",
                    PrivateKey = wallet1Key,
                });

                _dBContext.SaveChanges();

                var wallets = await _underTest.GetAllInTenant("test-tenant");

                Assert.Empty(wallets);
            }

            [Fact]
            public async Task Should_OnlyReturnWalletsFromRequestedTenant()
            {
                _dBContext.Wallets.Add(new Wallet
                {
                    Address = wallet1Address,
                    Name = "Preset Test Wallet",
                    Owner = expectedOwner,
                    Tenant = "test-tenant",
                    PrivateKey = wallet1Key,
                });
                
                _dBContext.Wallets.Add(new Wallet
                {
                    Address = wallet2Address,
                    Name = "Preset Test Wallet",
                    Owner = expectedOwner,
                    Tenant = "other-tenant",
                    PrivateKey = wallet1Key,
                });

                _dBContext.SaveChanges();

                var wallets = await _underTest.GetAllInTenant("test-tenant");

                Assert.Single(wallets);
            }
        }

        public class GetWallet : WalletRepositoryTest
        {
            [Fact]
            public async Task Get_Specified_Wallet_TestOwner()
            {
                SetupData();
                var wallet1 = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet1Address);

                A.CallTo(() => _fakeWalletProtector.UnProtectWallet(wallet1)).Returns(wallet1);

                var walletByOwner = await _underTest.GetWallet(wallet1Address, expectedOwner);

                Assert.Equal(wallet1, walletByOwner);
            }

            [Fact]
            public async Task Should_Throw_When_NotWalletOwner()
            {
                SetupData();

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.GetWallet(wallet1Address, alternateOwner));
                Assert.Equal("Unauthorized Wallet access.", result.Message);
            }

            [Fact]
            public async Task Should_ReturnWallet_WhenUserIs_InDelegates()
            {
                SetupData();
                var wallet1 = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet1Address);

                A.CallTo(() => _fakeWalletProtector.UnProtectWallet(wallet1)).Returns(wallet1);

                var wallet = await _underTest.GetWallet(wallet1Address, delegateUser);

                Assert.Equal(wallet1, wallet);
            }

            [Fact]
            public async Task Should_ThrowNotFound_When_GetState_Returns_Null()
            {
                SetupData();

                var walletThatDoesntExist = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa7";
                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.GetWallet(walletThatDoesntExist, alternateOwner));

                Assert.Equal("Not found.", result.Message);
            }

            [Fact]
            public async Task Should_Return_WalletDelegates()
            {
                SetupData();
                var wallet1 = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet1Address);

                A.CallTo(() => _fakeWalletProtector.UnProtectWallet(wallet1)).Returns(wallet1);
                var walletByOwner = await _underTest.GetWallet(wallet1Address, expectedOwner);

                Assert.Single(walletByOwner.Delegates);
            }
        }

        public class SaveWallet : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_SaveWallet()
            {
                SetupData();

                var expectedAddress = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa7";
                var expected = BuildMockWalletObject(expectedAddress);
                A.CallTo(() => _fakeWalletProtector.ProtectWallet(expected)).Returns(expected);

                await _underTest.SaveWallet(expected, expectedOwner);

                Assert.Contains(expected, _dBContext.Wallets);
            }

            [Fact]
            public async Task Should_Throw_When_WalletAlreadyExists()
            {
                SetupData();

                var expected = BuildMockWalletObject(wallet1Address);
                A.CallTo(() => _fakeWalletProtector.ProtectWallet(expected)).Returns(expected);

                await Assert.ThrowsAsync<WalletException>(() => _underTest.SaveWallet(expected, expectedOwner));
            }
        }
        public class WalletUpdateTransactions : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_ThrowWhenWalletDoesntExist()
            {
                SetupData();

                var addressOfNonExistentWallet = "ws1jaxrmlpll75sut3x4tltjyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa7";
                await Assert.ThrowsAsync<WalletException>(() => _underTest.WalletUpdateTransactions(addressOfNonExistentWallet, null, null));
            }

            [Fact]
            public async Task Should_ThrowWhenWalletTransactionDoesntExist()
            {
                SetupData();

                await Assert.ThrowsAsync<WalletException>(() => _underTest.WalletUpdateTransactions(wallet1Address, null, null));
            }

            [Fact]
            public async Task Should_UpdateWalletTransactionConfirmed()
            {
                SetupData();

                var transactionId = "e712db87a1eaae6c0548ba3f6ccf1a461f5ca2c3ccce47bf4fa0b23b69086870";
                var expected = new WalletTransaction
                {
                    TransactionId = transactionId,
                    WalletId = wallet1Address,
                    isConfirmed = false,
                    Sender = wallet2Address,
                    ReceivedAddress = wallet1Address,
                    PreviousId = ""
                };

                _dBContext.Transactions.Add(expected);
                _dBContext.SaveChanges();
                expected.isConfirmed = true;

                await _underTest.WalletUpdateTransactions(wallet1Address, transactionId, null);

                var tx = _dBContext.Transactions.FirstOrDefault(w => w.TransactionId == transactionId);
                Assert.Equal(expected.Id, tx.Id);
                Assert.True(tx.isConfirmed);
            }

            [Fact]
            public async Task Should_ChangePreviousTransaction_ToSpent_WhenRecipient_Is_SameAsSender()
            {
                SetupData();

                var transactionId = "e712db87a1eaae6c0548ba3f6ccf1a461f5ca2c3ccce47bf4fa0b23b69086870";
                var prevTxId = "0b55d333761415145ae7de79ac5e949cbf546cf92785f4d6d0df287578a1c6a6";

                var prevTransaction = new WalletTransaction
                {
                    TransactionId = prevTxId,
                    WalletId = wallet1Address,
                    isConfirmed = true,
                    Sender = wallet1Address,
                    ReceivedAddress = wallet1Address,
                    PreviousId = "",
                    isSpent = false,
                    MetaDataId = 1
                };
                var walletTransaction = new WalletTransaction
                {
                    TransactionId = transactionId,
                    WalletId = wallet1Address,
                    isConfirmed = false,
                    Sender = wallet1Address,
                    ReceivedAddress = wallet1Address,
                    PreviousId = prevTxId,
                    isSendingWallet = true,
                };

                _dBContext.TransactionMetaData.Add(new TransactionMetaData { Id = 1, TransactionType = TransactionTypes.Action });
                _dBContext.Transactions.Add(walletTransaction);
                _dBContext.Transactions.Add(prevTransaction);
                _dBContext.SaveChanges();
                walletTransaction.isConfirmed = true;

                await _underTest.WalletUpdateTransactions(wallet1Address, transactionId, prevTxId);

                var txs = _dBContext.Transactions.Where(w => w.TransactionId == prevTxId && w.Sender == wallet1Address).ToList();
                foreach (var tx in txs)
                {
                    Assert.Equal(prevTxId, tx.TransactionId);
                    Assert.True(tx.isSpent);
                }
            }
        }

        public class DeleteWallet : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_DeleteWallet()
            {
                SetupData();

                var walletAddressToDelete = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa7";
                var walletToDelete = BuildMockWalletObject(walletAddressToDelete);
                _dBContext.Wallets.Add(walletToDelete);
                _dBContext.SaveChanges();

                await _underTest.DeleteWallet(walletAddressToDelete, walletToDelete.Owner);

                Assert.DoesNotContain(walletToDelete, _dBContext.Wallets);

            }

            [Fact]
            public async Task Should_Throw_WhenNotFound()
            {
                SetupData();

                var nonExistantWalletAddress = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa7";

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.DeleteWallet(nonExistantWalletAddress, ""));

                Assert.Contains("Not found", result.Message);
            }

            [Fact]
            public async Task Should_Throw_WhenNotAuthorised()
            {
                SetupData();

                var unauthorisedUser = Guid.NewGuid().ToString();

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.DeleteWallet(wallet1Address, unauthorisedUser));

                Assert.Equal("Unauthorized Wallet access.", result.Message);
            }

            [Fact]
            public async Task Should_Throw_WhenUserDoesNot_HaveWriteAccess()
            {
                SetupData();

                var walletAddressToDelete = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9xa7";
                var userAccessingWallet = Guid.NewGuid().ToString();
                var walletToDelete = BuildMockWalletObject(walletAddressToDelete);
                walletToDelete.Delegates = new List<WalletAccess> { new WalletAccess() { Subject = userAccessingWallet, AccessType = AccessTypes.delegatero, Tenant = "Tenant" } };
                _dBContext.Wallets.Add(walletToDelete);
                _dBContext.SaveChanges();

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.DeleteWallet(walletAddressToDelete, userAccessingWallet));

                Assert.Contains("User does not have required access level", result.Message);
            }

        }

        public class AddWalletDelegates : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_AddSingleDelegate()
            {
                SetupData();

                var expected = new WalletAccess
                {
                    Subject = expectedOwner,
                    Tenant = "TenantName"
                };

                await _underTest.AddDelegates(wallet1Address, new List<WalletAccess> { expected }, expectedOwner);

                var wallet = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet1Address);
                Assert.Contains(expected, wallet.Delegates);
            }

            [Fact]
            public async Task Should_AddMultipleDelegates()
            {
                SetupData();

                var expected1 = new WalletAccess
                {
                    Subject = expectedOwner,
                    Tenant = "TenantName"
                };

                var expected2 = new WalletAccess
                {
                    Subject = new Guid().ToString(),
                    Tenant = "TenantName"
                };

                await _underTest.AddDelegates(wallet1Address, new List<WalletAccess> { expected1, expected2 }, expectedOwner);

                var wallet = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet1Address);

                Assert.Contains(expected1, wallet.Delegates);
                Assert.Contains(expected2, wallet.Delegates);
            }

            [Fact]
            public async Task Should_ThrowWhenOneOfTwoDelegatesWith_SameSubAlreadyExists()
            {
                SetupData();

                var sameSub = Guid.NewGuid().ToString();
                var existingDelegate = new WalletAccess
                {
                    Subject = sameSub,
                    Tenant = "TenantName",
                    AccessType = AccessTypes.delegaterw

                };
                var newDelegate = new WalletAccess
                {
                    Subject = new Guid().ToString(),
                    Tenant = "TenantName",
                };

                var walletAddress = "ws1jaxrmlpll75sut3x4ttljyttwkaeraua30xp2ejyvyphjysg3f9zstv9sdft";
                var wallet = BuildMockWalletObject(walletAddress);
                wallet.Delegates.Add(existingDelegate);

                _dBContext.Wallets.Add(wallet);
                _dBContext.SaveChanges();

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.AddDelegates(walletAddress, new List<WalletAccess> { existingDelegate }, sameSub));
                var updatedWallet = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet1Address);

                Assert.Contains("A delegate already exists for user", result.Message);

                Assert.DoesNotContain(newDelegate, wallet.Delegates);
            }

            [Fact]
            public async Task Should_ThrowWhenUser_IsNotAuthorised()
            {
                SetupData();

                var unauthorisedUser = Guid.NewGuid().ToString();
                var expected = new WalletAccess
                {
                    Subject = unauthorisedUser,
                    Tenant = "TenantName"
                };

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.AddDelegates(wallet1Address, new List<WalletAccess> { expected }, unauthorisedUser));

                Assert.Contains("Unauthorized Wallet access", result.Message);
            }

            [Fact]
            public async Task Should_ThrowWhen_WalletNotFound()
            {
                SetupData();

                var unauthorisedUser = Guid.NewGuid().ToString();
                var expected = new WalletAccess
                {
                    Subject = unauthorisedUser,
                    Tenant = "TenantName"
                };
                var nonExistantWalletAddress = "ws1jafdafjksdfkjweojfwefwfew";

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.AddDelegates(nonExistantWalletAddress, new List<WalletAccess> { expected }, unauthorisedUser));

                Assert.Contains("not found", result.Message);
            }
        }

        public class RemoveWalletDelegate : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_ThrowWhen_WalletNotFound()
            {
                SetupData();

                var nonExistantWalletAddress = "ws1jafdafjksdfkjweojfwefwfew";

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.RemoveDelegate(nonExistantWalletAddress, delegateUser, expectedOwner));

                Assert.Contains($"Wallet address with address: {nonExistantWalletAddress} not found", result.Message);
            }

            [Fact]
            public async Task Should_ThrowWhenUser_IsNotAuthorised()
            {
                SetupData();

                var unauthorisedUser = Guid.NewGuid().ToString();
                
                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.RemoveDelegate(wallet4Address, delegateUser, unauthorisedUser));

                Assert.Contains("Unauthorized Wallet access", result.Message);
            }

            [Fact]
            public async Task Should_ThrowWhenDelegate_NotExists()
            {
                SetupData();

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.RemoveDelegate(wallet4Address, alternateOwner, expectedOwnerAlt));

                Assert.Contains($"A delegate doesn't exists for user subject: {alternateOwner}", result.Message);
            }
            [Fact]
            public async Task Should_ThrowWhenUser_IsNotOwnerOrHaveNoWriteAccess()
            {
                SetupData();

                // delegateUserAlt has ro rights as per Wallet4 initialization
                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.RemoveDelegate(wallet4Address, delegateUser, delegateUserAlt));

                Assert.Contains($"User {delegateUserAlt} should have write access to modify Wallet Delegates.", result.Message);
            }
            [Fact]
            public async Task Should_RemoveDelegate()
            {
                SetupData();

                await _underTest.RemoveDelegate(wallet4Address, delegateUser, expectedOwnerAlt);

                var wallet = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet4Address);
                Assert.True(wallet.Delegates.Where(d => d.Subject == delegateUser).FirstOrDefault() == null);
                // check if doesn't delete all the delegates
                Assert.True(wallet.Delegates.Where(d => d.Subject == delegateUserAlt).FirstOrDefault() != null);
            }
        }

        public class UpdateWalletDelegate : WalletRepositoryTest
        {
            [Fact]
            public async Task Should_UpdateDelegate()
            {
                SetupData();

                var access = new WalletAccess
                {
                    Subject = delegateUser,
                    Tenant = "TenantName",
                    AccessType = AccessTypes.delegatero,
                    Reason = "Update Test"
                };
                var altDlg = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet4Address)
                    .Delegates.FirstOrDefault(d => d.Subject == delegateUserAlt);

                await _underTest.UpdateDelegate(wallet4Address, access, expectedOwnerAlt);

                var dlg = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet4Address)
                    .Delegates.FirstOrDefault(d => d.Subject == delegateUser);
                Assert.True((dlg.AccessType == AccessTypes.delegatero)
                            && (dlg.Reason == "Update Test")
                            );
                // check if other delegates are not updated
                dlg = _dBContext.Wallets.FirstOrDefault(w => w.Address == wallet4Address)
                    .Delegates.FirstOrDefault(d => d.Subject == delegateUserAlt);
                Assert.True((dlg.AccessType == altDlg.AccessType)
                            && (dlg.Reason == altDlg.Reason)
                            );
            }

            [Fact]
            public async Task Should_ThrowWhenDelegate_NotExists()
            {
                SetupData();

                var access = new WalletAccess
                {
                    Subject = alternateOwner,
                    Tenant = "TenantName",
                    AccessType = AccessTypes.delegaterw
                };

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.UpdateDelegate(wallet4Address, access, expectedOwnerAlt));

                Assert.Contains($"A delegate doesn't exists for user subject: {access.Subject}", result.Message);
            }

            [Fact]
            public async Task Should_ThrowWhenUser_IsNotAuthorised()
            {
                SetupData();

                var unauthorisedUser = Guid.NewGuid().ToString();
                var access = new WalletAccess
                {
                    Subject = unauthorisedUser,
                    Tenant = "TenantName",
                    AccessType = AccessTypes.delegaterw
                };

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.UpdateDelegate(wallet4Address, access, unauthorisedUser));

                Assert.Contains("Unauthorized Wallet access", result.Message);
            }

            [Fact]
            public async Task Should_ThrowWhenUser_IsNotOwnerOrHaveNoWriteAccess()
            {
                SetupData();

                var access = new WalletAccess
                {
                    Subject = delegateUser,
                    Tenant = "TenantName",
                    AccessType = AccessTypes.delegatero,
                    Reason = "Update Test"
                };

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.UpdateDelegate(wallet4Address, access, delegateUserAlt));
                
                Assert.Contains($"User {delegateUserAlt} should have write access to modify Wallet Delegates.", result.Message);
            }

            [Fact]
            public async Task Should_ThrowWhen_WalletNotFound()
            {
                SetupData();

                var unauthorisedUser = Guid.NewGuid().ToString();
                var access = new WalletAccess
                {
                    Subject = unauthorisedUser,
                    Tenant = "TenantName"
                };
                var nonExistantWalletAddress = "ws1jafdafjksdfkjweojfwefwfew";

                var result = await Assert.ThrowsAsync<WalletException>(() => _underTest.UpdateDelegate(nonExistantWalletAddress, access, expectedOwner));

                Assert.Contains($"Wallet address with address: {nonExistantWalletAddress} not found", result.Message);
            }
        }

        /// <summary>
        /// Preload the environmeny
        /// </summary>
        private void SetupData(string tenantId = "TestTenant")
        {
            _dBContext.Wallets.Add(new Wallet
            {
                Address = wallet1Address,
                Name = "Preset Test Wallet",
                Owner = expectedOwner,
                Tenant = tenantId,
                PrivateKey = wallet1Key,
                Delegates = new List<WalletAccess>
                {
                    new WalletAccess
                    {
                        AccessType = AccessTypes.delegatero,
                        AssignedTime = DateTime.UtcNow,
                        Tenant = tenantId,
                        Subject = delegateUser
                    }
                }
            });
            _dBContext.Wallets.Add(new Wallet
            {
                Address = wallet2Address,
                Name = "Preset Test Wallet - Alt User",
                Owner = alternateOwner,
                Tenant = tenantId,
                PrivateKey = wallet2Key,
                Delegates = new List<WalletAccess>
                {
                    new WalletAccess
                    {
                        AccessType = AccessTypes.delegatero,
                        AssignedTime = DateTime.UtcNow,
                        Tenant = tenantId,
                        Subject = delegateUser
                    }
                }
            });
            _dBContext.Wallets.Add(new Wallet
            {
                Address = wallet4Address,
                Name = "Preset Test Wallet for Delegats tests",
                Owner = expectedOwnerAlt,
                Tenant = tenantId,
                PrivateKey = wallet4Key,
                Delegates = new List<WalletAccess>
                {
                    new WalletAccess
                    {
                        AccessType = AccessTypes.delegatero,
                        AssignedTime = DateTime.UtcNow,
                        Tenant = tenantId,
                        Subject = delegateUser
                    },
                    new WalletAccess
                    {
                        AccessType = AccessTypes.delegatero,
                        AssignedTime = DateTime.UtcNow,
                        Tenant = tenantId,
                        Subject = delegateUserAlt
                    }
                }
            });

            _dBContext.SaveChanges();
        }


        private Wallet BuildMockWalletObject(string pubKey, string tenantId = "TestTenant")
        {
            return new Wallet
            {
                Address = pubKey,
                Name = "MyWallet",
                Owner = expectedOwner,
                Tenant = tenantId,
                Delegates = new List<WalletAccess>{
                    new WalletAccess
                    {
                        AccessType = AccessTypes.owner,
                        AssignedTime = DateTime.UtcNow,
                        Tenant = tenantId,
                        Subject = expectedOwner
                    } 
                },
                Addresses = new List<WalletAddress>(),
                PrivateKey = "a_cleartext_private_key",
                Transactions = new List<WalletTransaction>()
            };
        }
    }
}
