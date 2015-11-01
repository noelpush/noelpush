using System.ComponentModel;

namespace NoelPush.Objects.ViewModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// The name to display for the view model.
        /// </summary>
        string DisplayName { get; set; }

        /// <summary>
        /// Verifies that <paramref name="propertyName"/> corresponds to an existing property.
        /// </summary>
        /// <param name="propertyName">The property name to verify.</param>
        void VerifyPropertyName(string propertyName);
    }
}
