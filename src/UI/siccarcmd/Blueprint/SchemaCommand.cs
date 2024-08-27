using System;
using System.Threading.Tasks;
using System.IO;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Json.Schema;
using Json.Schema.Generation;

namespace siccarcmd.Blueprint
{
    public class SchemaCommand : Command
    {
        public IConfiguration configuration = null;

        public SchemaCommand(string action, IServiceProvider services) : base(action)
        {
            // Initialise Commands
            this.Name = "schema";
            this.Description = "Outputs or save the current Bleuprint JSON Schema";

            configuration = services.GetService<IConfiguration>();

            //Add(new Argument<string>("name", description: "A name of the Register to use."));
            Add(new Option<string>(new string[] { "-f", "--file" }, description: "Output the schema to a file"));


            Handler = CommandHandler.Create<string>(BlueprintSchema);
        }

        private async Task BlueprintSchema(string file = "")
        {
            // if we dont have a filename the assume we get it from input?
            // in the mean time from a file

            // lets get the Blueprint JSON
            await Task.Run(() =>
            {
                JsonSerializerOptions opts = new(JsonSerializerDefaults.Web)
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                };
                // Prechache the Schema JSON

                var schemabuilder = new JsonSchemaBuilder();
                schemabuilder.Id("https://siccar.net/schemas/v1/blueprint.json");
                schemabuilder.Title("Siccar Blueprint");
                schemabuilder.Schema(new Uri("https://json-schema.org/draft/2020-12/schema"));
                schemabuilder.Description("The definition of a Siccar distributed process flow. \n Includes : \n \t Participants \n\t Data Definition \n\t and Actions.");

                // our base schema
                var bpSchema = schemabuilder.FromType<Siccar.Application.Blueprint>().Build();

                var acSchema = schemabuilder.FromType<Siccar.Application.Action>().Build();
                schemabuilder.AdditionalProperties(acSchema);
                var rlSchema = schemabuilder.FromType<Json.Logic.Rule>().Build();
                schemabuilder.AdditionalProperties(rlSchema);
                var prSchema = schemabuilder.FromType<Siccar.Application.Participant>().Build();
                schemabuilder.AdditionalProperties(prSchema);
                var cnSchema = schemabuilder.FromType<Siccar.Application.Condition>().Build();
                schemabuilder.AdditionalProperties(cnSchema);


                string schemaString = JsonSerializer.Serialize(bpSchema, opts);
                if (string.IsNullOrWhiteSpace(file))
                    Console.WriteLine(schemaString);
                else // save to file
                    File.WriteAllText(file, schemaString);
            });
        }
    }
}

