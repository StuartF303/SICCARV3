namespace Siccar.EndToEndTests.Action
{
    internal class ActionAssertions
    {
        public static async Task HasCount(int expectedCount, string registerId, string transactionId)
        {
            Assert.Equal(expectedCount, await SiccarServices.MongoDbRepository.GetTransactionCount(registerId, transactionId));
        }
    }
}
