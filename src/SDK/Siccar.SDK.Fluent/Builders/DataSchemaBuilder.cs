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

using System.Text.Json;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating JSON Schemas for action data
    /// </summary>
    public class DataSchemaBuilder
    {
        private readonly Dictionary<string, object> _properties;
        private readonly List<string> _required;

        internal DataSchemaBuilder()
        {
            _properties = new Dictionary<string, object>();
            _required = new List<string>();
        }

        /// <summary>
        /// Adds a string field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Optional configuration for the field</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddString(string fieldId, Action<StringFieldBuilder>? configure = null)
        {
            var builder = new StringFieldBuilder(fieldId);
            configure?.Invoke(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds a number field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Optional configuration for the field</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddNumber(string fieldId, Action<NumberFieldBuilder>? configure = null)
        {
            var builder = new NumberFieldBuilder(fieldId);
            configure?.Invoke(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds an integer field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Optional configuration for the field</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddInteger(string fieldId, Action<IntegerFieldBuilder>? configure = null)
        {
            var builder = new IntegerFieldBuilder(fieldId);
            configure?.Invoke(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds a boolean field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Optional configuration for the field</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddBoolean(string fieldId, Action<BooleanFieldBuilder>? configure = null)
        {
            var builder = new BooleanFieldBuilder(fieldId);
            configure?.Invoke(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds a date field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Optional configuration for the field</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddDate(string fieldId, Action<DateFieldBuilder>? configure = null)
        {
            var builder = new DateFieldBuilder(fieldId);
            configure?.Invoke(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds a file field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Optional configuration for the field</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddFile(string fieldId, Action<FileFieldBuilder>? configure = null)
        {
            var builder = new FileFieldBuilder(fieldId);
            configure?.Invoke(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds an object field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Configuration for the object</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddObject(string fieldId, Action<ObjectFieldBuilder> configure)
        {
            var builder = new ObjectFieldBuilder(fieldId);
            configure(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        /// <summary>
        /// Adds an array field to the schema
        /// </summary>
        /// <param name="fieldId">The field identifier</param>
        /// <param name="configure">Configuration for the array</param>
        /// <returns>The builder instance for chaining</returns>
        public DataSchemaBuilder AddArray(string fieldId, Action<ArrayFieldBuilder> configure)
        {
            var builder = new ArrayFieldBuilder(fieldId);
            configure(builder);
            _properties[fieldId] = builder.Build();

            if (builder.IsRequiredField)
                _required.Add(fieldId);

            return this;
        }

        internal JsonDocument Build()
        {
            var schema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["properties"] = _properties
            };

            if (_required.Count > 0)
                schema["required"] = _required;

            var json = JsonSerializer.Serialize(schema, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            return JsonDocument.Parse(json);
        }
    }
}
