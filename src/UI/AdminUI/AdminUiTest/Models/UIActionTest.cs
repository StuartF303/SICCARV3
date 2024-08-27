using Siccar.Application;
using Siccar.UI.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace AdminUiTest.Models
{
    public class UIActionTest
    {
        private UIAction _underTest;
        public UIActionTest()
        {
            _underTest = new UIAction();
        }

        public class Contructor : UIActionTest
        {
            [Fact]
            public void Should_SetDataSchemaType_FromFormControl()
            {
                var propertyId = "data";
                var json = @"{
                              ""$schema"": ""http://json-schema.org/draft-07/schema"",
                              ""$id"": ""https://siccar.net/"",
                              ""type"": ""object"",
                              ""properties"": {
                                ""data"": {
                                  ""$id"": ""data"",
                                  ""type"": ""string"",
                                  ""title"": ""My Data"",
                                  ""description"": ""Some data""
                                }
                              }
                            }";
                var actionToConvert = new Siccar.Application.Action()
                {
                    DataSchemas = new List<JsonDocument> { JsonDocument.Parse(json) },
                    Form = new Control
                    {
                        Elements = new List<Control> { new Control
                        {
                            Elements = new List<Control>{ new Control
                            {
                                Scope = $"$.properties.{propertyId}",
                                ControlType = ControlTypes.TextLine
                            } }
                        } }
                    }
                };

                _underTest = new UIAction(actionToConvert);

                Assert.NotEmpty(_underTest.DataSchemas![0].Properties);
                Assert.Equal(ControlTypes.TextLine.ToString(), _underTest.DataSchemas[0].Properties[propertyId].Type);
            }

            [Fact]
            public void Should_SetRequiredDataSchemaProperty_FromDataSchemaRequiredList()
            {
                var propertyId = "data";
                var json = @"{
                              ""$schema"": ""http://json-schema.org/draft-07/schema"",
                              ""$id"": ""https://siccar.net/"",
                              ""type"": ""object"",
                              ""required"": [
                                ""data""
                              ],
                              ""properties"": {
                                ""data"": {
                                  ""$id"": ""data"",
                                  ""type"": ""string"",
                                  ""title"": ""My Data"",
                                  ""description"": ""Some data""
                                }
                              }
                            }";
                var actionToConvert = new Siccar.Application.Action()
                {
                    DataSchemas = new List<JsonDocument> { JsonDocument.Parse(json) },
                    Form = new Control
                    {
                        Elements = new List<Control> { new Control
                        {
                            Elements = new List<Control>{ new Control
                            {
                                Scope = $"$.properties.{propertyId}",
                                ControlType = ControlTypes.TextLine
                            } }
                        } }
                    }
                };

                _underTest = new UIAction(actionToConvert);

                Assert.NotEmpty(_underTest.DataSchemas![0].Properties);
                Assert.True(_underTest.DataSchemas[0].Properties[propertyId].Required);
            }

            [Fact]
            public void Should_SetDataSchemaPropertyDisclosureList_FromActionDisclosures()
            {
                var propertyId = "data";
                var disclosure1 = new Disclosure
                {
                    ParticipantAddress = "ws1jw3s4964su9aqgqfxh5c8gwrzuvx74h92gmpq5wln6334kzw2f0cq384wsj",
                    DataPointers = new List<string> { propertyId }
                };
                var disclosure2 = new Disclosure
                {
                    ParticipantAddress = "ws1j7x4vagssaje2r0knta6uln6qdczxlfyfcgj9g8p0ftsakxqje2dsdf35sg",
                    DataPointers = new List<string> { propertyId }
                };
                var json = @"{
                              ""$schema"": ""http://json-schema.org/draft-07/schema"",
                              ""$id"": ""https://siccar.net/"",
                              ""type"": ""object"",
                              ""required"": [
                                ""data""
                              ],
                              ""properties"": {
                                ""data"": {
                                  ""$id"": ""data"",
                                  ""type"": ""string"",
                                  ""title"": ""My Data"",
                                  ""description"": ""Some data""
                                }
                              }
                            }";
                var actionToConvert = new Siccar.Application.Action()
                {
                    Disclosures = new List<Disclosure>
                    {
                        disclosure1,
                        disclosure2
                    },
                    DataSchemas = new List<JsonDocument> { JsonDocument.Parse(json) },
                    Form = new Control
                    {
                        Elements = new List<Control> { new Control
                        {
                            Elements = new List<Control>{ new Control
                            {
                                Scope = $"$.properties.{propertyId}",
                                ControlType = ControlTypes.TextLine
                            } }
                        } }
                    }
                };

                _underTest = new UIAction(actionToConvert);

                Assert.NotEmpty(_underTest.DataSchemas![0].Properties);
                Assert.Contains(disclosure1.ParticipantAddress, _underTest.DataSchemas[0].Properties[propertyId].Disclosures);
                Assert.Contains(disclosure2.ParticipantAddress, _underTest.DataSchemas[0].Properties[propertyId].Disclosures);
            }
        }

        public class UpdateSchemaAndRelatedProperties : UIActionTest
        {
            [Fact]
            public void Should_AddDataSchemaProperty()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "Data"
                };
                _underTest = new UIAction();

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                Assert.Contains(expected, _underTest.DataSchemas![0].Properties.Values);
            }

            [Fact]
            public void Should_ReplacePropertyWhenIDChanges()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId"
                };
                var originalId = "OriginalDataId";
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.DataSchemas[0].Properties.Add(originalId, new DataSchemaProperty { Id = originalId });

                _underTest.UpdateSchemaAndRelatedProperties(expected, originalId);

                Assert.Contains(expected, _underTest.DataSchemas![0].Properties.Values);
                Assert.DoesNotContain(originalId, _underTest.DataSchemas![0].Properties.Keys);
            }

            [Fact]
            public void Should_AddDataSchemaPropertyId_ToDataSchemaRequiredList()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                    Required = true
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                Assert.Contains(expected.Id, _underTest.DataSchemas![0].Required);
            }

            [Fact]
            public void Should_RemoveDataSchemaId_FromDataSchemaRequiredList()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                    Required = false
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.DataSchemas[0].Required.Add(expected.Id);

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                Assert.DoesNotContain(expected.Id, _underTest.DataSchemas![0].Required);
            }

            [Fact]
            public void Should_AddId_ToEachDisclosure()
            {
                var participant1 = Guid.NewGuid().ToString();
                var participant2 = Guid.NewGuid().ToString();
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                    Disclosures = new() { participant1, participant2 }
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.Disclosures = new List<Disclosure>() {
                    new Disclosure {
                        ParticipantAddress = participant2,
                        DataPointers = new List<string>() { "SomeOtherDataPointer" }
                    }
                };

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                Assert.True(_underTest.Disclosures.Count == 2);
                foreach (var disclosure in _underTest.Disclosures)
                {
                    Assert.Contains(expected.Id, disclosure.DataPointers);
                }
            }

            [Fact]
            public void Should_NotAddId_OnceAlreadyAdded()
            {
                var participant1 = Guid.NewGuid().ToString();
                var participant2 = Guid.NewGuid().ToString();
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                    Disclosures = new() { participant1, participant2 }
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.Disclosures = new List<Disclosure>() {
                    new Disclosure {
                        ParticipantAddress = participant2,
                        DataPointers = new List<string>() { expected.Id }
                    }
                };

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                var disclosure = _underTest.Disclosures.Find(disclosure => disclosure.ParticipantAddress == participant2);
                Assert.Single(disclosure!.DataPointers);
                Assert.Equal(expected.Id, disclosure!.DataPointers[0]);
            }


            [Fact]
            public void Should_NotAddId_ToExistingDisclosures_WhenNotSetInDataSchemaProperty()
            {
                var participant1 = Guid.NewGuid().ToString();
                var participant2 = Guid.NewGuid().ToString();
                var participantWithOriginalData = Guid.NewGuid().ToString();
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                    Disclosures = new() { participant1, participant2 }
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.Disclosures = new List<Disclosure>() {
                    new Disclosure {
                        ParticipantAddress = participantWithOriginalData,
                        DataPointers = new List<string>() { "SomeOriginalData" }
                    }
                };

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                var disclosure = _underTest.Disclosures.Find(disclosure => disclosure.ParticipantAddress == participantWithOriginalData);
                Assert.DoesNotContain(expected.Id, disclosure!.DataPointers);
            }

            [Fact]
            public void Should_AddForm()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId"
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());


                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                var dataSchemaElements = _underTest.Form!.Elements[0].Elements;
                var newFormElement = dataSchemaElements!.Find(element => element.Scope.Contains(expected.Id));
                Assert.NotNull(newFormElement);
            }

            [Fact]
            public void Should_UpdateForm()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.Form!.Elements[0].Elements.Add(new Control { Scope = $"$.properties.{expected.Id}", ControlType = ControlTypes.TextLine });

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                var dataSchemaElements = _underTest.Form!.Elements[0].Elements;
                var formElement = dataSchemaElements!.Find(element => element.Scope.Contains(expected.Id));
                Assert.NotNull(formElement);
                Assert.Equal(expected.Type, formElement.ControlType.ToString());
            }

            [Fact]
            public void Should_UpdateFormId_WhenIdChanges()
            {
                var originalId = "OriginalId";
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                };
                _underTest = new UIAction();
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.Form!.Elements[0].Elements.Add(new Control { Scope = $"$.properties.{originalId}", ControlType = ControlTypes.TextLine });

                _underTest.UpdateSchemaAndRelatedProperties(expected, null);

                var dataSchemaElements = _underTest.Form!.Elements[0].Elements;
                var formElement = dataSchemaElements!.Find(element => element.Scope.Contains(expected.Id));
                Assert.NotNull(formElement);
                Assert.Contains(expected.Id, formElement.Scope);
                Assert.Equal(expected.Type, formElement.ControlType.ToString());
            }
        }

        public class SetPropertyTypesFromControlType : UIActionTest
        {
            [Fact]
            public void Should_ChangeDataSchemType_ToPropertyType()
            {
                var expected = new DataSchemaProperty()
                {
                    Type = ControlTypes.TextArea.ToString(),
                    Id = "NewDataId",
                };
                _underTest.DataSchemas!.Add(new DataSchema());
                _underTest.DataSchemas[0].Properties.Add(expected.Id, expected);

                _underTest.SetPropertyTypesFromControlType();

                Assert.Equal(expected.PropertyType, _underTest.DataSchemas[0].Properties[expected.Id].Type);
            }
        }

    }

}
