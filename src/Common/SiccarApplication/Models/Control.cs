/*
* Copyright (c) 2024 Siccar (Registered co. Wallet.Services (Scotland) Ltd).
* All rights reserved.
*
* This file is part of a proprietary software product developed by Siccar.
*
* This source code is licensed under the Siccar Proprietary Limited Use License.
* Use, modification, and distribution of this software is subject to the terms
* and conditions of the license agreement. The full text of the license can be
* found in the LICENSE file or at https://github.com/siccar/SICCARV3/blob/main/LICENCE.txt.
*
* Unauthorized use, copying, modification, merger, publication, distribution,
* sublicensing, and/or sale of this software or any part thereof is strictly
* prohibited except as explicitly allowed by the license agreement.
*/

using Json.More;
using Json.Schema.Generation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MaxLengthAttribute = Json.Schema.Generation.MaxLengthAttribute;
#nullable enable

namespace Siccar.Application
{
    /// <summary>
    /// A Basic buildable element for UI form display
    /// links the render element type to a Data Scope
    /// </summary>
    public class Control   
    {
        /// <summary>
        /// The DisplayType for this element
        /// </summary>
        [JsonPropertyName("type")]
        public ControlTypes ControlType { get; set; } = ControlTypes.Label;

        /// <summary>
        /// A label for this control if 'false' then the control will not display
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// A Reference to the data values element likly a did
        /// </summary>
        [MaxLength(250)]
        public string Scope { get; set; } = string.Empty;

        /// <summary>
        /// Determins the format of this and embedded controls 
        /// </summary>
        public LayoutTypes Layout { get; set; } = LayoutTypes.VerticalLayout; 

        /// <summary>
        /// Determins the format of this and embedded controls 
        /// </summary>
        public JsonDocument? Properties { get; set; } 

        /// <summary>
        /// Sub Elements or sub controls of this control
        /// </summary>
        public List<Control> Elements { get; set; } = new List<Control>();

        /// <summary>
        /// Rules based on JSON Data Request as to Display this 
        /// </summary>
        public List<JsonNode> Conditions { get; set; } = new List<JsonNode>();
    }

    [JsonConverter(typeof(EnumStringConverter<LayoutTypes>))]
    public enum LayoutTypes
    {
        VerticalLayout,
        HorizontalLayout,
        Group, // many
        Categorization  // one of
    }

    [JsonConverter(typeof(EnumStringConverter<ControlTypes>))]
    public enum ControlTypes
    {
        /// <summary>
        /// A simpe layout
        /// </summary>
        [Display(Name = "Layout")]
        Layout,

        /// <summary>
        /// A Simple Label
        /// </summary>
        [Display(Name = "Label")]
        Label,

        /// <summary>
        /// A Single Line Text Entry
        /// </summary>
        [Display(Name = "Single Line Text")]
        TextLine,

        /// <summary>
        /// A Text Area
        /// </summary>
        /// 
        [Display(Name = "Text Area")]
        TextArea,

        /// <summary>
        /// A Numeric Input
        /// </summary>
        [Display(Name = "Number")]
        Numeric,

        /// <summary>
        /// A temporal entity
        /// </summary>
        [Display(Name = "Date/Time")]
        DateTime,

        /// <summary>
        /// Upload an attachment
        /// </summary>
        [Display(Name = "File")]
        File,

        /// <summary>
        /// One of Many, boolean is one from two
        /// </summary>
        [Display(Name = "Multiple Choice")]
        Choice,

        /// <summary>
        /// A single checkbox, bool value
        /// </summary>
        [Display(Name = "Checkbox")]
        Checkbox,

        /// <summary>
        /// Some of Many
        /// </summary>
        [Display(Name = "Selection")]
        Selection

    }
}
