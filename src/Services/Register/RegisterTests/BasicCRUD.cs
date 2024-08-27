using System;
using System.Net.Http;

namespace RegisterTests
{
    /// <summary>
    /// Basic CRUD Integration Tests for the RegisterAPI
    /// Erm dont know how to do this yet...
    /// </summary>
    public class BasicAPI : IDisposable
    {
        public HttpClient httpClient { get; set; }

        public BasicAPI() 
        {
            httpClient = new HttpClient();
        }

        //[Fact]
        //public async Task CheckMetaData()
        //{
        //    // Get the /api/$metadata xml configuration

        //    // make sure its correct
        //}

        //[Fact]
        //public async Task RegisterCreate()
        //{
        //    // Create a Register with known parameters

        //    // Read that the Reister 
        //}

        //[Fact]
        //public async Task RegisterUpdate()
        //{

        //}

        //[Fact]
        //public async Task QueryUpdate()
        //{

        //}

        //[Fact]
        //public async Task RegisterDelete()
        //{

        //}

        // Helpers just so we dont need to repeat code

        private static void CreateReg(string registerName)
        {

        }

        private static void DeleteReg(string registerName)
        {

        }

        public void Dispose()
        {
            // Tear down

        }
    }
}
