// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DataAnnotations = System.ComponentModel.DataAnnotations;
using Json.Schema.Generation;
#nullable enable

namespace Siccar.Application
{
    /// <summary>
    /// Blueprint - 
    /// 
    /// FluentValidation in Validation/BlueprintValidator 
    /// </summary>
    public class Blueprint : IEquatable<Blueprint>
    {
        /// <summary>
        /// Id - String Identifier for the Blueprint
        /// </summary>
        [Required]
        [MaxLength(64)]
       // [RegularExpression(@"^[{]?[0-9a-fA-F]{8}-([0-9a-fA-F]{4}-){3}[0-9a-fA-F]{12}[}]?$", ErrorMessage = "Blueprint Id must be a guid")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The Blueprint Title
        /// </summary>
        [DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Blueprint title must be populated.")]
        [MinLength(3)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// A Description of the Blueprints purpose
        /// </summary>
        [DataAnnotations.Required(AllowEmptyStrings = false, ErrorMessage = "Blueprint description must be populated.")]
        [MinLength(5)]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// A Version Number
        /// </summary>
        [Required]
        public int Version { get; set; } = 1;

        /// <summary>
        /// Embedded Reference Data Schema Documents
        /// </summary>
        public List<JsonDocument>? DataSchemas { get; set; }

        /// <summary>
        /// List of Participants 
        /// </summary>
        [Required]
        [MinLength(2)]
        [MinItems(2)]
        public List<Participant> Participants { get; set; } = new List<Participant>();

        /// <summary>
        /// Actions - Where Data Paths, values and display collide 
        /// </summary>
        [Required]
        [MinLength(1)]
        [MinItems(1)]
        public List<Action> Actions { get; set; } = new List<Action>();

        public Blueprint()
        {

        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Blueprint);
        }

        public bool Equals(Blueprint? other)
        {
            return other != null &&
                   Id == other.Id &&
                   Title == other.Title &&
                   Description == other.Description &&
                   (


                    Participants == other.Participants ||
                    Participants != null &&
                    Participants.SequenceEqual(other.Participants)
                    ) &&
                    (
                    Actions == other.Actions ||
                    Actions != null &&
                    Actions.SequenceEqual(other.Actions)
                    );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Title, Description, Participants, Actions, DataSchemas);
        }
    }
}
