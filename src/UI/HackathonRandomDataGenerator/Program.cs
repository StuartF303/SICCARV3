using System.Reflection.Metadata.Ecma335;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HackathonRandomDataGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var blueprintRuns = new List<Dictionary<string, Dictionary<string, object>>>();

            for (int j = 0; j < 10; j++)
            {
                var prevDateTime = GetDateTime(new DateTime(2020, 01, 01), 4);

                for (int i = 0; i < 60; i++)
                {
                    prevDateTime = GetDateTime(prevDateTime, 1);

                    var currentRun = new Dictionary<string, Dictionary<string, object>>();

                    currentRun.Add("Request to bring Rig", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDatebringrig", prevDateTime },
                        { "ConsumptionEndDatebringrig", GetDateTime(prevDateTime, 4) },
                        { "recordedbybringrig", GetPerson() },
                        { "performedbybringrig", GetPerson() },
                        { "FuelQuantitybringrig", GetInt(30, 110) },
                        { "FuelQuantityUnitbringrig", "Cubic Metres" },
                        { "FuelTypebringrig", "Diesel" },
                        { "DataQualityTypebringrig", "Actual" },
                        { "OrganizationalUnitbringrig", "Tow and Anchor Handling Vessel" },
                        { "ActivityTypebringrig", "Mobile Combustion" },
                        { "ActivityNamebringrig", "Request to bring Rig" },
                        { "RigName", "ShellFinder" },
                        { "OperationalArea", "Safaniya Oil Field" },
                        { "PONumber", GetPoNumber() },
                        { "OriginCorrelationIdbringrig", GetOrigin() },
                    });

                    currentRun.Add("Validate Rig Emission Data", new Dictionary<string, object>
                    {
                        { "ValidateRigData", true }
                    });

                    prevDateTime = GetDateTime(prevDateTime, 2);
                    currentRun.Add("Fly out drilling team", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDateflyoutteam", prevDateTime },
                        { "ConsumptionEndDateflyoutteam", GetDateTime(prevDateTime, 4) },
                        { "recordedbyflyoutteam", GetPerson() },
                        { "performedbyflyoutteam", GetPerson() },
                        { "FuelQuantityflyoutteam", GetInt(200, 500) },
                        { "FuelTypeflyoutteam", "Aviation Gasoline" },
                        { "DataQualityTypeflyoutteam", "Actual" },
                        { "OrganizationalUnitflyoutteam", "Helicopter Owner" },
                        { "ActivityTypeflyoutteam", "Mobile Combustion" },
                        { "FuelQuantityUnitflyoutteam", "Cubic Metres" },
                        { "ActivityNameflyoutteam", "Fly out drilling team" },
                        { "OriginCorrelationIdflyoutteam", GetOrigin() }
                    });

                    currentRun.Add("Validate Transportation Emission Data", new Dictionary<string, object>
                    {
                        { "ValidateTransportData", true }
                    });

                    prevDateTime = GetDateTime(prevDateTime, 5);
                    currentRun.Add("Well Drilled", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDatewelldrilled", prevDateTime },
                        { "ConsumptionEndDatewelldrilled", GetDateTime(prevDateTime, 30) },
                        { "recordedbywelldrilled", GetPerson() },
                        { "performedbywelldrilled", GetPerson() },
                        { "FuelQuantitywelldrilled", GetInt(20000, 100000) },
                        { "FuelQuantityUnitwelldrilled", "Cubic metres" },
                        { "FuelTypewelldrilled", "Diesel" },
                        { "DataQualityTypewelldrilled", "Actual" },
                        { "OrganizationalUnitwelldrilled", "Drilling Contractor" },
                        { "ActivityTypewelldrilled", "Mobile Combustion" },
                        { "ActivityNamewelldrilled", "Well Drilled" },
                        { "OriginCorrelationIdwelldrilled", GetOrigin() }
                    });

                    prevDateTime = GetDateTime(prevDateTime, 10);
                    currentRun.Add("Casing of hole", new Dictionary<string, object>
                    {
                        { "TransactionDatecasingofhole", prevDateTime },
                        { "recordedbycasingofhole", GetPerson() },
                        { "performedbycasingofhole", GetPerson() },
                        { "DataQualityTypecasingofhole", "Actual" },
                        { "OrganizationalUnitcasingofhole", "Casing Company" },
                        { "SpendTypecasingofhole", "Steel" },
                        { "FuelQuantitycasingofhole", GetInt(6000, 30000) },
                        { "FuelTypecasingofhole", "Diesel" },
                        { "ActivityTypecasingofhole", "Purchased good and service" },
                        { "ActivityNamecasingofhole", "Casing of hole" },
                        { "GoodsQuantitycasingofhole", GetInt(15000, 125000) },
                        { "GoodsUnitcasingofhole", "Cubic Metres" },
                        { "OriginCorrelationIdcasingofhole", GetOrigin() }

                    });

                    prevDateTime = GetDateTime(prevDateTime, 7);
                    currentRun.Add("Cementing of hole", new Dictionary<string, object>
                    {
                        { "TransactionDatecementingofhole", prevDateTime },
                        { "recordedbycementingofhole", GetPerson() },
                        { "performedbycementingofhole", GetPerson() },
                        { "DataQualityTypecementingofhole", "Actual" },
                        { "OrganizationalUnitcementingofhole", "Cement Provider" },
                        { "SpendTypecementingofhole", "Cement" },
                        { "ActivityTypecementingofhole", "Purchased good and service" },
                        { "ActivityNamecementingofhole", "Cementing of hole" },
                        { "GoodsQuantitycementingofhole", GetInt(15000, 125000) },
                        { "GoodsUnitcementingofhole", "ton" },
                        { "OriginCorrelationIdcementingofhole", GetOrigin() }

                    });

                    prevDateTime = GetDateTime(prevDateTime, 3);
                    currentRun.Add("Cuttings delivered to waste transfer", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDatecuttingsdelivered", prevDateTime },
                        { "ConsumptionEndDatecuttingsdelivered", GetDateTime(prevDateTime, 3) },
                        { "recordedbycuttingsdelivered", GetPerson() },
                        { "performedbycuttingsdelivered", GetPerson() },
                        { "FuelQuantitycuttingsdelivered", GetInt(20, 200) },
                        { "FuelQuantityUnitcuttingsdelivered", "Cubic Metres" },
                        { "FuelTypecuttingsdelivered", "Diesel" },
                        { "DataQualityTypecuttingsdelivered", "Actual" },
                        { "OrganizationalUnitcuttingsdelivered", "Supply Vessel" },
                        { "ActivityTypecuttingsdelivered", "Mobile Combustion" },
                        { "ActivityNamecuttingsdelivered", "Cuttings delivered to waste transfer" },
                        { "OriginCorrelationcuttingsdelivered", GetOrigin() }

                    });

                    prevDateTime = GetDateTime(prevDateTime, 3);
                    currentRun.Add("Treatment of waste", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDatewastetreatment", prevDateTime },
                        { "ConsumptionEndDatewastetreatment", GetDateTime(prevDateTime, 6) },
                        { "recordedbywastetreatment", GetPerson() },
                        { "performedbywastetreatment", GetPerson() },
                        { "DataQualityTypewastetreatment", "Actual" },
                        { "Facilitywastetreatment", "Waste Treatment Facility" },
                        { "IndustrialProcessTypewastetreatment", "Waste Treatment" },
                        { "OrganizationalUnitwastetreatment", "Cuttings Management" },
                        { "FuelQuantitywastetreatment", GetInt(40000, 150000) },
                        { "FuelQuantityUnitwastetreatment", "ton" },
                        { "ActivityTypewastetreatment", "Industrial Process" },
                        { "ActivityNamewastetreatment", "Treatment of waste" },
                        { "GoodsQuantitywastetreatment", GetInt(40000, 150000) },
                        { "GoodsUnitwastetreatment", "ton" },
                        { "OriginCorrelationIdwastetreatment", GetOrigin() }
                    });

                    prevDateTime = GetDateTime(prevDateTime, 3);
                    currentRun.Add("Plug & Abandon", new Dictionary<string, object>
                    {
                        { "TransactionDateplugandabandon", prevDateTime },
                        { "recordedbyplugandabandon", GetPerson() },
                        { "performedbyplugandabandon", GetPerson() },
                        { "DataQualityTypeplugandabandon", "Actual" },
                        { "OrganizationalUnitplugandabandon", "Cement Provider" },
                        { "SpendTypeplugandabandon", "Cement" },
                        { "ActivityTypeplugandabandon", "Purchased good and service" },
                        { "ActivityNameplugandabandon", "Plug & Abandon" },
                        { "GoodsQuantityplugandabandon", GetInt(20000, 90000) },
                        { "GoodsUnitplugandabandon", "ton" },
                        { "OriginCorrelationIdplugandabandon", GetOrigin() }

                    });

                    prevDateTime = GetDateTime(prevDateTime, 3);
                    currentRun.Add("All non-essentials removed from rig", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDateremovenonessentials", prevDateTime },
                        { "ConsumptionEndDateremovenonessentials", GetDateTime(prevDateTime, 8) },
                        { "recordedbyremovenonessentials", GetPerson() },
                        { "performedbyremovenonessentials", GetPerson() },
                        { "FuelQuantityremovenonessentials", GetInt(40000, 150000) },
                        { "FuelQuantityUnitremovenonessentials", "Cubic Metres" },
                        { "FuelTyperemovenonessentials", "Aviation Gasoline" },
                        { "DataQualityTyperemovenonessentials", "Actual" },
                        { "OrganizationalUnitremovenonessentials", "Helicopter Owner" },
                        { "ActivityTyperemovenonessentials", "Mobile Combustion" },
                        { "ActivityNameremovenonessentials", "All non-essentials removed from rig" },
                        { "OriginCorrelationIdremovenonessentials", GetOrigin() }
                    });

                    prevDateTime = GetDateTime(prevDateTime, 3);
                    currentRun.Add("Rig towed to shore", new Dictionary<string, object>
                    {
                        { "ConsumptionStartDatetowedtoshore", prevDateTime },
                        { "ConsumptionEndDatetowedtoshore", GetDateTime(prevDateTime, 8) },
                        { "recordedbytowedtoshore", GetPerson() },
                        { "performedbytowedtoshore", GetPerson() },
                        { "FuelQuantitytowedtoshore", GetInt(20, 120) },
                        { "FuelQuantityUnittowedtoshore", "Cubic Metres" },
                        { "FuelTypetowedtoshore", "Diesel" },
                        { "DataQualityTypetowedtoshore", "Actual" },
                        { "OrganizationalUnittowedtoshore", "Tow and Anchor Handling Vessel" },
                        { "ActivityTypetowedtoshore", "Mobile Combustion" },
                        { "ActivityNametowedtoshore", "Rig towed to shore" },
                        { "OriginCorrelationIdtowedtoshore", GetOrigin() }
                    });

                    currentRun.Add("Report all data to Rig Operator", new Dictionary<string, object>
                    {
                        { "reportsignedoff", true }
                    });

                    blueprintRuns.Add(currentRun);
                }

                var json = JsonSerializer.Serialize(blueprintRuns, new JsonSerializerOptions { WriteIndented = true });
            }
        }

        static string GetOrigin()
        {
            var dataOrigin = new List<string>
            {
                "Sap Integration",
                "Epicor Integration",
                "Manual Entry"
            };

            var rand = new Random();
            var randIndex = rand.Next(0, dataOrigin.Count);
            return dataOrigin[randIndex];
        }

        static DateTime GetDateTime(DateTime previousDateTime, int daysRange)
        {
            var rand = new Random();
            var daysToAdd = rand.Next(0, daysRange);
            return previousDateTime.AddDays(daysToAdd);
        }

        static string GetPerson()
        {
            var availablePeople = new List<string>
            {
                "Mike Andrews",
                "Chris Hunter",
                "Chris Dixon",
                "Velerii Danilov",
                "Alan McGowan",
                "Stuart Fraser",
                "Chris Warburton",
                "Peter Ferry",
                "Dominic McCann"
            };
            var rand = new Random();
            var randIndex = rand.Next(0, availablePeople.Count - 1);
            return availablePeople[randIndex];
        }

        static int GetInt(int from, int to)
        {
            var rand = new Random();
            var randomNumber = rand.Next(from, to);
            return randomNumber;
        }

        static string GetPoNumber()
        {
            var numbers = "1234567890";
            var letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Random rand = new Random();

            return $"{letters[rand.Next(0, letters.Length)]}" +
                           $"{letters[rand.Next(0, letters.Length)]}" +
                           $"{numbers[rand.Next(0, numbers.Length)]}" +
                           $"{numbers[rand.Next(0, numbers.Length)]}" +
                           $"{numbers[rand.Next(0, numbers.Length)]}" +
                           $"{numbers[rand.Next(0, numbers.Length)]}" +
                           $"{numbers[rand.Next(0, numbers.Length)]}" +
                           $"{numbers[rand.Next(0, numbers.Length)]}";
        }
    }
}

