using Bunit;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Blazor;
using System.Linq;
using Siccar.UI.Admin.Shared.Components;
using Xunit;
using System.Collections.Generic;
using Siccar.Application;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.DropDowns;
using AngleSharp.Dom;
using System.Threading.Tasks;
using Siccar.UI.Admin.Models;
using Syncfusion.Blazor.Inputs;
using IdentityServer4.Models;
using Siccar.Common;

namespace AdminUiTest.Components
{
    public class ListViewWithDropDownTest : TestContext
    {
        // parameters prototypes
        public List<string> AddingSourceProto { get; set; }
        public ICollection<ClientClaim> DataSourceProto { get; set; }
        // for actual parameters
        public List<string> AddingSource { get; set; } = new();
        public ICollection<ClientClaim> DataSource { get; set; }


        public ListViewWithDropDownTest()
        {
            Services.AddSyncfusionBlazor();
            Services.AddOptions();

            AddingSourceProto = new List<string>(15)
            {
                Constants.BlueprintAdminRole,
                Constants.BlueprintAuthoriserRole,
                Constants.InstallationAdminRole,
                Constants.InstallationReaderRole,
                Constants.InstallationBillingRole,
                Constants.RegisterCreatorRole,
                Constants.RegisterMaintainerRole,
                Constants.RegisterReaderRole,
                Constants.TenantBillingRole,
                Constants.TenantAdminRole,
                Constants.TenantAppAdminRole,
                Constants.WalletUserRole,
                Constants.WalletOwnerRole,
                Constants.WalletDelegateRole
            };

            AddingSourceProto.Sort();

            DataSourceProto = new List<ClientClaim>
            {
                new ClientClaim { Type = "role", Value = Constants.InstallationAdminRole },
                new ClientClaim { Type = "role", Value = Constants.InstallationReaderRole },
                new ClientClaim { Type = "role", Value = Constants.InstallationBillingRole }
            };

            foreach (var el in DataSourceProto.Where(m => m.Type == "role"))
                AddingSourceProto.Remove(el.Value);

            AddingSource = new List<string>(AddingSourceProto);
            DataSource = new List<ClientClaim>(DataSourceProto);
        }

        [Fact]
        public void Should_RenderAllDataSourceMembersInDataGrid()
        {
            AddingSource = new List<string>(AddingSourceProto);
            DataSource = new List<ClientClaim>(DataSourceProto);

            var cut = RenderComponent<ListViewWithDropDown>(parameters => parameters.Add(p => p.DataSource, DataSource)
                                                                                    .Add(p => p.DropdownListDataSource, AddingSource));
            var claimsLabels = cut.FindAll("label");
            int _cnt = 0;
            foreach (var el in DataSource.Where(m => m.Type == "role"))
                if (claimsLabels.Where(l => l.InnerHtml.Contains(el.Value)) != null)
                {
                    _cnt++;
                    continue;
                }

            Assert.True(_cnt == DataSource.Count);
        }

        [Fact]
        public async void Should_ContainAllAddingSourceElementsInDropBox()
        {
            AddingSource = new List<string>(AddingSourceProto);
            DataSource = new List<ClientClaim>(DataSourceProto);
            Assert.True(11 == AddingSource.Count);

            var cut = RenderComponent<ListViewWithDropDown>(parameters => parameters.Add(p => p.DataSource, DataSource)
                                                                                    .Add(p => p.DropdownListDataSource, AddingSource));
            var ddlElements = await GetDropdownList(cut);
            Assert.NotNull(ddlElements);

            int _cnt = 0;
            foreach (var el in AddingSource)
                if (ddlElements.Where(l => l.InnerHtml.Contains(el)) != null)
                {
                    _cnt++;
                    continue;
                }
            Assert.True(_cnt == AddingSource.Count);
            Assert.True(11 == AddingSource.Count);
        }

        [Fact]
        public async void Should_CorrectlyAddAnElementChosen()
        {
            AddingSource = new List<string>(AddingSourceProto);
            DataSource = new List<ClientClaim>(DataSourceProto);
            int asdcInitial = AddingSource.Count;
            int dsInitial = DataSource.Count;
            
            var cut = RenderComponent<ListViewWithDropDown>(parameters => parameters.Add(p => p.DataSource, DataSource)
                                                                                    .Add(p => p.DropdownListDataSource, AddingSource));
            var dropdownList = cut.FindComponent<SfDropDownList<string, string>>();
            var ddlElements = await GetDropdownList(cut);
            Assert.NotNull(ddlElements);

            var ddlEl = ddlElements.Where(l => l.InnerHtml.Contains(Constants.WalletUserRole)).FirstOrDefault();
            Assert.NotNull(ddlEl);
            ddlEl.Click();

            Assert.True(dropdownList.Instance.Value == Constants.WalletUserRole);

            var addButton = cut.FindAll("button").First(input => input.TextContent.Contains("Add"));
            Assert.NotNull(addButton);
            addButton.Click();

            Assert.True(AddingSource.Count == asdcInitial - 1 && DataSource.Count == dsInitial + 1
                        && !AddingSource.Contains(Constants.WalletUserRole)
                        && DataSource.Contains(new ClientClaim { Type = "role", Value = Constants.WalletUserRole })
                        );

            Assert.True(await CheckArraysCorrespondency(cut));
        }

        [Fact]
        public async void Should_CorrectlyDeleteElements()
        {
            AddingSource = new List<string>(AddingSourceProto);
            DataSource = new List<ClientClaim>(DataSourceProto);
            int asdcInitial = AddingSource.Count;
            int dsInitial = DataSource.Count;

            var cut = RenderComponent<ListViewWithDropDown>(parameters => parameters.Add(p => p.DataSource, DataSource)
                                                                                    .Add(p => p.DropdownListDataSource, AddingSource));

            var delButton = cut.FindAll("button").First(e => e.TextContent.Contains("X"));
            Assert.NotNull(delButton);
            delButton.Click();

            Assert.True(AddingSource.Count == asdcInitial + 1 && DataSource.Count == dsInitial - 1);

            Assert.True(await CheckArraysCorrespondency(cut));
       }

        // check correspondence for arrays
        private async Task<bool> CheckArraysCorrespondency(IRenderedComponent<ListViewWithDropDown> cut)                                                    
        {
            var claimsLabels = cut.FindAll("label");
            int _cnt = 0;
            foreach (var el in DataSource.Where(m => m.Type == "role"))
                if (claimsLabels.Where(l => l.InnerHtml.Contains(el.Value)) != null)
                {
                    _cnt++;
                    continue;
                };

            var ddlElements = await GetDropdownList(cut);
            Assert.NotNull(ddlElements);
            int _cnt1 = 0;
            foreach (var el in AddingSource)
                if (ddlElements.Where(l => l.InnerHtml.Contains(el)) != null)
                {
                    _cnt1++;
                    continue;
                }

            return _cnt == DataSource.Count && _cnt1 == AddingSource.Count;
        }

        // Get all drop down list elements
        private static async Task<IHtmlCollection<IElement>> GetDropdownList(IRenderedComponent<ListViewWithDropDown> cut)
        {
            var dropdownList = cut.FindComponent<SfDropDownList<string, string>>();
            await dropdownList.Instance.ShowPopup();
            Assert.NotNull(dropdownList);
            var popupEle = dropdownList.Find(".e-popup");
            Assert.Contains("e-popup", popupEle.ClassName);
            var ulEle = popupEle.QuerySelector("ul");
            Assert.NotNull(ulEle);
            Assert.Contains("e-ul", ulEle.ClassName);
            var liCollection = ulEle.QuerySelectorAll("li.e-list-item");
            return liCollection;
        }
    }
}
