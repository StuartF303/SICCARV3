namespace Siccar.EndToEndTests.ApiWrappers.Register
{
    internal class RegisterTestData
    {
        public static EndToEndTests.Register.Models.Register NewDefault()
        {
            return new EndToEndTests.Register.Models.Register
            {
                Name = "Test Register",
                Advertise = true,
                Id = "40daacacf4ef407cb5c4b9b7d0e7fe36",
                Status = "ONLINE",
                IsFullReplica = true
            };
        }
    }
}
