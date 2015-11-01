using System;
using System.Windows.Controls;
using System.Windows.Input;


namespace NoelPush.Objects.ViewModel
{
    /// <summary>
    /// View model containing a <see cref="ICommand"/>.
    /// <para>
    /// This view model can be used to add information for the diplay of a
    /// command in a view.
    /// </para>
    /// <para>
    /// Each property of this view model can be binded as needed.
    /// </para>
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        private string iconUri;

        /// <summary>
        /// The command contained in the view model.
        /// </summary>
        public ICommand Command { get; set; }

        /// <summary>
        /// The icon to display for the command.
        /// </summary>
        public Image Icon { get; set; }

        /// <summary>
        /// The URI of the icon to diaply for the command.
        /// </summary>
        public string IconUri
        {
            get { return this.iconUri; }
            set
            {
                this.iconUri = value;
                this.OnPropertyChanged(NameOf<CommandViewModel>.Property(v => v.IconUri));
            }
        }

        /// <summary>
        /// Content of the dialog linked to the command.
        /// </summary>
        public Control DialogContent { get; set; }

        /// <summary>
        /// Indicates if the command is expected to take some time to execute.
        /// </summary>
        public bool IsHeavyCommand { get; set; }

        /// <summary>
        /// Indicates if the command can be executed.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Creates a new instance of CommandViewModel.
        /// </summary>
        /// <param name="displayName">The name to display for the command.</param>
        public CommandViewModel(string displayName)
        {
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Creates a new instance of CommandViewModel.
        /// </summary>
        /// <param name="displayName">The name to display for the command.</param>
        /// <param name="icon">The icon to display for the command.</param>
        public CommandViewModel(string displayName, Image icon)
            : this(displayName)
        {
            this.Icon = icon;
        }

        /// <summary>
        /// Creates a new instance of CommandViewModel.
        /// </summary>
        /// <param name="displayName">The name to display for the command.</param>
        /// <param name="command">The command to associate to the view model.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public CommandViewModel(string displayName, ICommand command)
            : this(displayName)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            this.Command = command;
        }

        /// <summary>
        /// Creates a new instance of CommandViewModel.
        /// </summary>
        /// <param name="displayName">The name to display for the command.</param>
        /// <param name="command">The command to associate to the view model.</param>
        /// <param name="icon">The icon to display for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public CommandViewModel(string displayName, ICommand command, Image icon)
            : this(displayName, command)
        {
            this.Icon = icon;
        }

        /// <summary>
        /// Creates a new instance of CommandViewModel.
        /// </summary>
        /// <param name="displayName">The name to display for the command.</param>
        /// <param name="command">The command to associate to the view model.</param>
        /// <param name="iconUri">The URI of the icon to display for the command.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="command"/> is null.</exception>
        public CommandViewModel(string displayName, ICommand command, string iconUri)
            : this(displayName, command)
        {
            this.IconUri = iconUri;
        }
    }
}