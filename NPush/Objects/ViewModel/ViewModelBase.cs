using System;
using System.ComponentModel;


namespace NPush.Objects.ViewModel
{
    /// <summary>
    /// Abstract base class for view models.
    /// </summary>
    [Serializable]
    public abstract class ViewModelBase : IViewModel
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// The name to display for the view model.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Verifies that <paramref name="propertyName"/> corresponds to an existing property.
        /// </summary>
        /// <param name="propertyName">The property name to verify.</param>
        public void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new Exception(string.Format("InvalidPropertyNameFormat"));
        }

        /// <summary>
        /// Notifies that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}