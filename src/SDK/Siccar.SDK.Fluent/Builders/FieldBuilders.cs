// SPDX-License-Identifier: MIT
// Copyright (c) 2024 SICCAR Project Contributors

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for string fields
    /// </summary>
    public class StringFieldBuilder
    {
        private readonly string _fieldId;
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal StringFieldBuilder(string fieldId)
        {
            _fieldId = fieldId;
            _schema = new Dictionary<string, object>
            {
                ["type"] = "string",
                ["$id"] = fieldId
            };
        }

        public StringFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public StringFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public StringFieldBuilder WithMinLength(int minLength)
        {
            _schema["minLength"] = minLength;
            return this;
        }

        public StringFieldBuilder WithMaxLength(int maxLength)
        {
            _schema["maxLength"] = maxLength;
            return this;
        }

        public StringFieldBuilder WithPattern(string pattern)
        {
            _schema["pattern"] = pattern;
            return this;
        }

        public StringFieldBuilder WithFormat(string format)
        {
            _schema["format"] = format;
            return this;
        }

        public StringFieldBuilder WithEnum(params string[] values)
        {
            _schema["enum"] = values;
            return this;
        }

        public StringFieldBuilder WithDefault(string defaultValue)
        {
            _schema["default"] = defaultValue;
            return this;
        }

        public StringFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for number fields
    /// </summary>
    public class NumberFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal NumberFieldBuilder(string fieldId)
        {
            _schema = new Dictionary<string, object>
            {
                ["type"] = "number",
                ["$id"] = fieldId
            };
        }

        public NumberFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public NumberFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public NumberFieldBuilder WithMinimum(double minimum)
        {
            _schema["minimum"] = minimum;
            return this;
        }

        public NumberFieldBuilder WithMaximum(double maximum)
        {
            _schema["maximum"] = maximum;
            return this;
        }

        public NumberFieldBuilder WithMultipleOf(double multipleOf)
        {
            _schema["multipleOf"] = multipleOf;
            return this;
        }

        public NumberFieldBuilder WithDefault(double defaultValue)
        {
            _schema["default"] = defaultValue;
            return this;
        }

        public NumberFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for integer fields
    /// </summary>
    public class IntegerFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal IntegerFieldBuilder(string fieldId)
        {
            _schema = new Dictionary<string, object>
            {
                ["type"] = "integer",
                ["$id"] = fieldId
            };
        }

        public IntegerFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public IntegerFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public IntegerFieldBuilder WithMinimum(int minimum)
        {
            _schema["minimum"] = minimum;
            return this;
        }

        public IntegerFieldBuilder WithMaximum(int maximum)
        {
            _schema["maximum"] = maximum;
            return this;
        }

        public IntegerFieldBuilder WithMultipleOf(int multipleOf)
        {
            _schema["multipleOf"] = multipleOf;
            return this;
        }

        public IntegerFieldBuilder WithDefault(int defaultValue)
        {
            _schema["default"] = defaultValue;
            return this;
        }

        public IntegerFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for boolean fields
    /// </summary>
    public class BooleanFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal BooleanFieldBuilder(string fieldId)
        {
            _schema = new Dictionary<string, object>
            {
                ["type"] = "boolean",
                ["$id"] = fieldId
            };
        }

        public BooleanFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public BooleanFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public BooleanFieldBuilder WithDefault(bool defaultValue)
        {
            _schema["default"] = defaultValue;
            return this;
        }

        public BooleanFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for date/time fields
    /// </summary>
    public class DateFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal DateFieldBuilder(string fieldId)
        {
            _schema = new Dictionary<string, object>
            {
                ["type"] = "string",
                ["format"] = "date-time",
                ["$id"] = fieldId
            };
        }

        public DateFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public DateFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public DateFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for file fields
    /// </summary>
    public class FileFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal FileFieldBuilder(string fieldId)
        {
            _schema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["$id"] = fieldId,
                ["properties"] = new Dictionary<string, object>
                {
                    ["fileName"] = new Dictionary<string, object> { ["type"] = "string" },
                    ["fileType"] = new Dictionary<string, object> { ["type"] = "string" },
                    ["fileSize"] = new Dictionary<string, object> { ["type"] = "number" },
                    ["fileExtension"] = new Dictionary<string, object> { ["type"] = "string" }
                }
            };
        }

        public FileFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public FileFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public FileFieldBuilder WithMaxSize(int maxSizeBytes)
        {
            if (_schema["properties"] is Dictionary<string, object> props &&
                props["fileSize"] is Dictionary<string, object> sizeSchema)
            {
                sizeSchema["maximum"] = maxSizeBytes;
            }
            return this;
        }

        public FileFieldBuilder WithAllowedExtensions(params string[] extensions)
        {
            if (_schema["properties"] is Dictionary<string, object> props &&
                props["fileExtension"] is Dictionary<string, object> extSchema)
            {
                extSchema["enum"] = extensions;
            }
            return this;
        }

        public FileFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for object fields
    /// </summary>
    public class ObjectFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        private readonly Dictionary<string, object> _properties;
        internal bool IsRequiredField { get; private set; }

        internal ObjectFieldBuilder(string fieldId)
        {
            _properties = new Dictionary<string, object>();
            _schema = new Dictionary<string, object>
            {
                ["type"] = "object",
                ["$id"] = fieldId,
                ["properties"] = _properties
            };
        }

        public ObjectFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public ObjectFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public ObjectFieldBuilder AddProperty(string propName, string type)
        {
            _properties[propName] = new Dictionary<string, object> { ["type"] = type };
            return this;
        }

        public ObjectFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }

    /// <summary>
    /// Fluent builder for array fields
    /// </summary>
    public class ArrayFieldBuilder
    {
        private readonly Dictionary<string, object> _schema;
        internal bool IsRequiredField { get; private set; }

        internal ArrayFieldBuilder(string fieldId)
        {
            _schema = new Dictionary<string, object>
            {
                ["type"] = "array",
                ["$id"] = fieldId
            };
        }

        public ArrayFieldBuilder WithTitle(string title)
        {
            _schema["title"] = title;
            return this;
        }

        public ArrayFieldBuilder WithDescription(string description)
        {
            _schema["description"] = description;
            return this;
        }

        public ArrayFieldBuilder OfType(string itemType)
        {
            _schema["items"] = new Dictionary<string, object> { ["type"] = itemType };
            return this;
        }

        public ArrayFieldBuilder WithMinItems(int minItems)
        {
            _schema["minItems"] = minItems;
            return this;
        }

        public ArrayFieldBuilder WithMaxItems(int maxItems)
        {
            _schema["maxItems"] = maxItems;
            return this;
        }

        public ArrayFieldBuilder IsRequired()
        {
            IsRequiredField = true;
            return this;
        }

        internal Dictionary<string, object> Build() => _schema;
    }
}
