using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace Siccar.UI.Admin.Shared.Components
{
    public partial class RegisterGraph
    {
        [Parameter]
        public string Title { get; set; } = "PERFORMANCE";
        [Parameter]
        public string ChartWidth { get; set; } = "320px";
        [Parameter]
        public string ChartHeight { get; set; } = "200px";
        [Parameter]
        public string LineColour { get; set; } = "blue";
        [Parameter]
        public string YAxisTitle { get; set; } = "Transactions/day";
        [Parameter]
        public string XAxisTitle { get; set; } = "Last 7 Days";


        public class ChartDummyData
        {
            public string Day { get; set; }
            public double Transactions { get; set; }
        }

        public List<ChartDummyData> data = new List<ChartDummyData> {
            new ChartDummyData { Day = "Day 1", Transactions = 250 },
            new ChartDummyData { Day = "Day 2", Transactions = 350 },
            new ChartDummyData { Day = "Day 3", Transactions = 650 },
            new ChartDummyData { Day = "Day 4", Transactions = 1210 },
            new ChartDummyData { Day = "Day 5", Transactions = 770 },
            new ChartDummyData { Day = "Day 6", Transactions = 880 },
            new ChartDummyData { Day = "Day 7", Transactions = 550 }
        };
    }
}