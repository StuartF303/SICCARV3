// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using System;
using System.Collections.Generic;
using System.Text.Json;
#nullable enable

namespace Siccar.Application
{
    public class SustainabilityModel
    {
        public SustainabilityModel(Action action)
        {
            var previousData = JsonSerializer.Deserialize<Dictionary<string, object?>>(action.PreviousData!.RootElement.GetRawText());

            if (previousData == null)
            {
                return;
            }

            Name = action.Title;

            foreach (var kvp in previousData)
            {
                //There be dragons : Order matters here
                if (kvp.Key.Contains(nameof(SustainabilityModel.OrganizationalUnit), StringComparison.OrdinalIgnoreCase))
                {
                    OrganizationalUnit = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.DataQualityType), StringComparison.OrdinalIgnoreCase))
                {
                    DataQualityType = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.IndustrialProcessType), StringComparison.OrdinalIgnoreCase))
                {
                    IndustrialProcessType = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.FuelQuantityUnit), StringComparison.OrdinalIgnoreCase))
                {
                    FuelQuantityUnit = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.ConsumptionStartDate), StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value != null)
                    {
                        ConsumptionStartDate = DateTime.Parse(kvp.Value.ToString()!).ToString("MM/dd/yy");
                    }
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.ConsumptionEndDate), StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value != null)
                    {
                        ConsumptionEndDate = DateTime.Parse(kvp.Value.ToString()!).ToString("MM/dd/yy");
                    }
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.GoodsQuantity), StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value != null)
                    {
                        GoodsQuantity = int.Parse(kvp.Value.ToString()!);
                    }
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.GoodsUnit), StringComparison.OrdinalIgnoreCase))
                {
                    GoodsUnit = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.SpendType), StringComparison.OrdinalIgnoreCase))
                {
                    SpendType = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.FuelQuantity), StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value != null)
                    {
                        FuelQuantity = int.Parse(kvp.Value.ToString()!);
                    }
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.FuelType), StringComparison.OrdinalIgnoreCase))
                {
                    FuelType = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.Facility), StringComparison.OrdinalIgnoreCase))
                {
                    Facility = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.QuantityUnit), StringComparison.OrdinalIgnoreCase))
                {
                    QuantityUnit = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.ActivityType), StringComparison.OrdinalIgnoreCase))
                {
                    ActivityType = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.ActivityName), StringComparison.OrdinalIgnoreCase))
                {
                    ActivityName = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.Greenhousegas), StringComparison.OrdinalIgnoreCase))
                {
                    Greenhousegas = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.OriginCorrelationId), StringComparison.OrdinalIgnoreCase))
                {
                    OriginCorrelationId = kvp.Value?.ToString();
                    continue;
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.TransactionDate), StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value != null)
                    {
                        TransactionDate = DateTime.Parse(kvp.Value.ToString()!).ToString("MM/dd/yy");
                        continue;
                    }
                }
                if (kvp.Key.Contains(nameof(SustainabilityModel.Quantity), StringComparison.OrdinalIgnoreCase))
                {
                    if (kvp.Value != null)
                    {
                        Quantity = int.Parse(kvp.Value.ToString()!);
                        continue;
                    }
                }
            }
        }
        public string? Name { get; set; }  = String.Empty;
        public string? OrganizationalUnit { get; set; } = String.Empty;
        public string? DataQualityType { get; set; } = String.Empty;
        public string ConsumptionStartDate { get; set; } = DateTime.MinValue.ToString("MM/dd/yy");
        public string ConsumptionEndDate { get; set; } = DateTime.MinValue.ToString("MM/dd/yy");
        public int? GoodsQuantity { get; set; } = 0;
        public string? GoodsUnit { get; set; } = String.Empty;
        public string? SpendType { get; set; } = String.Empty;
        public int? FuelQuantity { get; set; } = 0;
        public string? FuelQuantityUnit { get; set; } = String.Empty;
        public string? FuelType { get; set; } = String.Empty;
        public string? Facility { get; set; } = String.Empty;
        public string? IndustrialProcessType { get; set; } = String.Empty;
        public int? Quantity { get; set; } = 0;
        public string? QuantityUnit { get; set; } = String.Empty;
        public string? Greenhousegas { get; set; } = String.Empty;
        public string? ActivityType { get; set; } = String.Empty;
        public string? ActivityName { get; set; } = String.Empty;
        public string TransactionDate { get; set; } = DateTime.MinValue.ToString("MM/dd/yy");
        public string? OriginCorrelationId { get; set; } = String.Empty;
    }
}
