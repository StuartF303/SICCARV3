using System;
using System.Threading.Tasks;
using System.IO;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Siccar.Application.Validation;

namespace siccarcmd.Blueprint
{
    public class ValidateCommand : Command
    {
        public readonly IConfiguration configuration = null;

        public ValidateCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "validate";
            this.Description = "Validates a blueprint, returning any syntactic errors ";

            configuration = services.GetService<IConfiguration>();

            Add(new Argument<string>("blueprint", description: "The filename of the Blueprint to validate"));
            //Add(new Option<string>(new string[] { "-b", "--blueprint" }, description: "Specify a Blueprint File to check"));


            Handler = CommandHandler.Create<string>(VerifyBlueprint);
        }

        private async Task VerifyBlueprint(string blueprint = "")
        {
            // if we dont have a filename the assume we get it from input?
            // in the mean time from a file
            await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(blueprint))
                {
                    Console.WriteLine("Please provide ablueprint file");
                    System.Environment.Exit(1);
                }

                var blueprintValidator = new BlueprintValidator();
                JsonSerializerOptions serializerOptions = new(JsonSerializerDefaults.Web);
                string json = File.ReadAllText(blueprint);
                var t = JsonSerializer.Deserialize<Siccar.Application.Blueprint>(json, serializerOptions);
                var test = blueprintValidator.Validate(t);

                Console.WriteLine($"Validator : Errors found {test.Errors.Count}");

                foreach (var e in test.Errors)
                    Console.WriteLine($" error: {e.ErrorMessage}");
            });
        }
    }
}

