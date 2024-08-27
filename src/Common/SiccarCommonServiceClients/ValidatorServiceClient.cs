namespace Siccar.Common.ServiceClients
{
    public class ValidatorServiceClient 
    {
        private readonly SiccarBaseClient _baseClient;

        public ValidatorServiceClient(SiccarBaseClient baseClient)
        {
            _baseClient = baseClient;
        }
    }
}
