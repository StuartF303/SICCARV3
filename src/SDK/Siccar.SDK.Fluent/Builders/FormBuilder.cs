// SPDX-License-Identifier: MIT
// Copyleft 2026 Sorcha Project Contributors

using Siccar.Application;

namespace Siccar.SDK.Fluent.Builders
{
    /// <summary>
    /// Fluent builder for creating UI forms for actions
    /// </summary>
    public class FormBuilder
    {
        private readonly Control _form;

        internal FormBuilder()
        {
            _form = new Control
            {
                ControlType = ControlTypes.Layout,
                Layout = LayoutTypes.VerticalLayout,
                Elements = new List<Control>()
            };
        }

        /// <summary>
        /// Sets the form layout type
        /// </summary>
        public FormBuilder WithLayout(LayoutTypes layout)
        {
            _form.Layout = layout;
            return this;
        }

        /// <summary>
        /// Sets the form title
        /// </summary>
        public FormBuilder WithTitle(string title)
        {
            _form.Title = title;
            return this;
        }

        /// <summary>
        /// Adds a control element to the form
        /// </summary>
        public FormBuilder AddControl(Action<ControlBuilder> configure)
        {
            var controlBuilder = new ControlBuilder();
            configure(controlBuilder);
            _form.Elements.Add(controlBuilder.Build());
            return this;
        }

        internal Control Build() => _form;
    }

    /// <summary>
    /// Fluent builder for creating form controls
    /// </summary>
    public class ControlBuilder
    {
        private readonly Control _control;

        internal ControlBuilder()
        {
            _control = new Control
            {
                Elements = new List<Control>()
            };
        }

        /// <summary>
        /// Sets the control type
        /// </summary>
        public ControlBuilder OfType(ControlTypes type)
        {
            _control.ControlType = type;
            return this;
        }

        /// <summary>
        /// Sets the control title/label
        /// </summary>
        public ControlBuilder WithTitle(string title)
        {
            _control.Title = title;
            return this;
        }

        /// <summary>
        /// Binds the control to a data field
        /// </summary>
        public ControlBuilder BoundTo(string scope)
        {
            _control.Scope = scope;
            return this;
        }

        /// <summary>
        /// Sets the layout for container controls
        /// </summary>
        public ControlBuilder WithLayout(LayoutTypes layout)
        {
            _control.Layout = layout;
            return this;
        }

        /// <summary>
        /// Adds child controls (for layout controls)
        /// </summary>
        public ControlBuilder AddChild(Action<ControlBuilder> configure)
        {
            var childBuilder = new ControlBuilder();
            configure(childBuilder);
            _control.Elements.Add(childBuilder.Build());
            return this;
        }

        internal Control Build() => _control;
    }
}
