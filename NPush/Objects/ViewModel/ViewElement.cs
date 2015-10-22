using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NPush.Objects.ViewModel
{
    [Serializable]
    public class ViewElement<T> : IDataErrorInfo, INotifyPropertyChanged, ISerializable
    {
        /// <summary>
        /// Occurs when a property value changes. (Inherited from <see cref="INotifyPropertyChanged"/>.)
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Event thrown when a property is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        private T value;
        private string description;
        private IEnumerable<T> valueList;
        private IEnumerable<T> selectedValues;
        private bool isEnabled;

        /// <summary>
        /// Bindable property usually containing the name to be displayed for the stored value(s).
        /// </summary>
        /// <remarks>This property doesn't notify when it is modified.</remarks>
        public string DisplayName { get; set; }

        /// <summary>
        /// A bindable string describing the stored value(s).
        /// </summary>
        /// <remarks>This property notifies when it is modified, contrarilly to <see cref="DisplayName"/></remarks>
        public string Description
        {
            get { return this.description; }
            set
            {
                this.OnPropertyChanging(NameOf<ViewElement<T>>.Property(v => v.Description));
                this.description = value;
                this.OnPropertyChanged(NameOf<ViewElement<T>>.Property(v => v.Description));
            }
        }

        /// <summary>
        /// A bindable list of values of type <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>This property notifies when the enumerable reference is modified, not when something is modified in the enumerable.</remarks>
        public IEnumerable<T> ValueList
        {
            get { return this.valueList; }
            set
            {
                this.OnPropertyChanging(NameOf<ViewElement<T>>.Property(v => v.ValueList));
                this.valueList = value;
                this.OnPropertyChanged(NameOf<ViewElement<T>>.Property(v => v.ValueList));
            }
        }

        /// <summary>
        /// A bindable list of values of type <typeparamref name="T"/>, usually containing selected values in a multiple selection control.
        /// </summary>
        /// <remarks>This property notifies when the enumerable reference is modified, not when something is modified in the enumerable.</remarks>
        public IEnumerable<T> SelectedValues
        {
            get { return this.selectedValues; }
            set
            {
                this.OnPropertyChanging(NameOf<ViewElement<T>>.Property(v => v.SelectedValues));
                this.selectedValues = value;
                this.OnPropertyChanged(NameOf<ViewElement<T>>.Property(v => v.SelectedValues));
            }
        }

        /// <summary>
        /// A bindable value of type <typeparamref name="T"/>. This can be used to store the selected value in a selection control.
        /// </summary>
        /// <remarks>This property notifies when it is modified.</remarks>
        public T Value
        {
            get { return this.value; }
            set
            {
                this.OnPropertyChanging(NameOf<ViewElement<T>>.Property(v => v.Value));
                this.value = value;
                this.OnPropertyChanged(NameOf<ViewElement<T>>.Property(v => v.Value));
            }
        }

        /// <summary>
        /// Validation predicate used when the <see cref="IDataErrorInfo"/> mechanism is used.
        /// <para>The predicate should return <c>true</c> if the data contained in the ViewElement is valid.</para>
        /// </summary>
        public Predicate<object> ValidationRule { get; set; }

        /// <summary>
        /// Bindable boolean value usually used to control the accessibility of the value.
        /// </summary>
        /// <remarks>This property notifies when it is modified.</remarks>
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.OnPropertyChanging(NameOf<ViewElement<T>>.Property(v => v.IsEnabled));
                this.isEnabled = value;
                this.OnPropertyChanged(NameOf<ViewElement<T>>.Property(v => v.IsEnabled));
            }
        }

        /// <summary>
        /// Property indicating if a value is stored in the <see cref="Value"/> property.
        /// </summary>
        /// <returns>Returns <c>true</c> if <see cref="Value"/> is different from the default value for type <typeparamref name="T"/>, <c>false</c> otherwise.</returns>
        public bool HasValue
        {
            get { return !Equals(this.Value, default(T)); }
        }

        /// <summary>
        /// Call on property changed for Value property
        /// </summary>
        public void RefreshValue()
        {
            this.OnPropertyChanged(NameOf<ViewElement<T>>.Property(v => v.Value));
        }

        /// <summary>
        /// Gets an error message indicating what is wrong with this object. (Inherited from <see cref="IDataErrorInfo"/>.)
        /// </summary>
        /// <returns>
        /// An error message indicating what is wrong with this object. The default is an empty string ("").
        /// </returns>
        public string Error { get; set; }

        /// <summary>
        /// Gets the error message for the property with the given name. (Inherited from <see cref="IDataErrorInfo"/>.)
        /// </summary>
        /// <returns>
        /// The error message for the property. The default is an empty string ("").
        /// </returns>
        /// <param name="columnName">The name of the property whose error message to get. </param>
        public string this[string columnName]
        {
            get
            {
                if (this.ValidationRule == null)
                    return null;

                if (columnName != NameOf<ViewElement<T>>.Property(v => v.Value))
                    return null;

                return this.ValidationRule.Invoke(this) ? null : this.Error;
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ViewElement() { }

        /// <summary>
        /// Constructor initializing <see cref="DisplayName"/>.
        /// </summary>
        /// <param name="displayName">The value to set in <see cref="DisplayName"/></param>
        public ViewElement(string displayName)
        {
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Constructor initializing <see cref="DisplayName"/> and <see cref="Value"/>.
        /// </summary>
        /// <param name="displayName">The value to set in <see cref="DisplayName"/></param>
        /// <param name="value">The value to set in <see cref="Value"/></param>
        public ViewElement(string displayName, T value)
            : this(displayName)
        {
            this.value = value;
        }

        /// <summary>
        /// Constructor initializing <see cref="DisplayName"/> and <see cref="ValueList"/>.
        /// </summary>
        /// <param name="displayName">The value to set in <see cref="DisplayName"/></param>
        /// <param name="valueList">The value to set in <see cref="ValueList"/></param>
        public ViewElement(string displayName, IEnumerable<T> valueList)
            : this(displayName)
        {
            this.valueList = valueList;
        }

        /// <summary>
        /// Constructor initializing <see cref="DisplayName"/>, <see cref="ValueList"/> and <see cref="Value"/>.
        /// </summary>
        /// <param name="displayName">The value to set in <see cref="DisplayName"/></param>
        /// <param name="valueList">The value to set in <see cref="ValueList"/></param>
        /// <param name="value">The value to set in <see cref="Value"/></param>
        public ViewElement(string displayName, IEnumerable<T> valueList, T value)
            : this(displayName, valueList)
        {
            this.value = value;
        }

        /// <summary>
        /// Populates the ViewElement with data stored in the given <see cref="SerializationInfo"/>.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> containing data to use.</param>
        /// <param name="context">The origin (see <see cref="StreamingContext"/>) for this deserialization.</param>
        protected ViewElement(SerializationInfo info, StreamingContext context)
        {
            this.Value = (T)info.GetValue(NameOf<ViewElement<T>>.Property(element => element.Value), typeof(T));
            this.valueList = (IEnumerable<T>)info.GetValue(NameOf<ViewElement<T>>.Property(element => element.ValueList), typeof(IEnumerable<T>));
            this.DisplayName = info.GetString(NameOf<ViewElement<T>>.Property(element => element.DisplayName));
            this.description = info.GetString(NameOf<ViewElement<T>>.Property(element => element.Description));
            this.IsEnabled = info.GetBoolean(NameOf<ViewElement<T>>.Property(element => element.IsEnabled));
            this.Error = info.GetString(NameOf<ViewElement<T>>.Property(element => element.Error));
        }

        /// <summary>
        /// Populates a <see cref="SerializationInfo"/> with the data needed to serialize the target object. (Inherited from <see cref="ISerializable"/>.)
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> to populate with data. </param>
        /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization. </param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission. </exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameOf<ViewElement<T>>.Property(element => element.Value), this.value);
            info.AddValue(NameOf<ViewElement<T>>.Property(element => element.ValueList), this.valueList);
            info.AddValue(NameOf<ViewElement<T>>.Property(element => element.DisplayName), this.DisplayName);
            info.AddValue(NameOf<ViewElement<T>>.Property(element => element.Description), this.description);
            info.AddValue(NameOf<ViewElement<T>>.Property(element => element.IsEnabled), this.IsEnabled);
            info.AddValue(NameOf<ViewElement<T>>.Property(element => element.Error), this.Error);
        }

        /// <summary>
        /// Implicit cast operator: casts the ViewElement to its inner type.
        /// </summary>
        /// <param name="element">The ViewElement to cast</param>
        /// <returns>The value stored in <see cref="Value"/></returns>
        public static implicit operator T(ViewElement<T> element)
        {
            return element.Value;
        }

        private void OnPropertyChanging(string property)
        {
            if (this.PropertyChanging != null)
                this.PropertyChanging(this, new PropertyChangingEventArgs(property));
        }

        private void OnPropertyChanged(string property)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(property));
        }
    }
}
