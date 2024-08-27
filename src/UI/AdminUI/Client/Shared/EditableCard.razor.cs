using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Inputs;
using System.Linq.Expressions;

namespace Siccar.UI.Admin.Shared
{
    public partial class EditableCard
    {
        [Parameter]
        public bool IsEditable { get; set; } = false;
        [Parameter]
        public Expression<Func<string>> CustomValueExpression { get; set; }

        private string _value { get; set; }

        [Parameter]
        public string ID { get; set; }

        [Parameter]
        public Expression<Func<string>> ValueExpression { get; set; }

#pragma warning disable BL0007
		[Parameter]
		public string Value
		{
            get => _value;
            set
            {
                if (!EqualityComparer<string>.Default.Equals(value, _value))
                {
                    _value = value;
                    ValueChanged.InvokeAsync(value);
                }
            }
        }
#pragma warning restore BL0007

		[Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public EventCallback<InputEventArgs> InputHandler { get; set; }

        [Parameter]
        public string CardTitle { get; set; }

        [Parameter]
        public Func<Task> DataUpdatedCallback { get; set; }

        private bool EditCardData { get; set; } = false;
        private void SetEditCardDataHandler()
        {
            EditCardData = !EditCardData;
        }

        public async Task OnBlurEventHandler()
        {
            EditCardData = false;
            await DataUpdatedCallback.Invoke();
        }
    }
}