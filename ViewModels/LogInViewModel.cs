using System;
using System.Windows;
using Flixplorer.Models;
using Flixplorer.Helpers;
using Flixplorer.Commands;
using System.Windows.Input;
using System.Windows.Controls;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Datacontext class for LogInView. Inherits MainViewModel class.
    /// </summary>
    class LogInViewModel : MainViewModel
    {
        private string _userName;
        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged(); } }

        private string _userNameError;
        public string UserNameError { get { return _userNameError; } set { _userNameError = value; OnPropertyChanged(); } }

        private string _passwordError;
        public string PasswordError { get { return _passwordError; } set { _passwordError = value; OnPropertyChanged(); } }

        public static bool IsChecked
        {
            get
            {
                return (bool)Properties.Settings.Default["RememberMe"];
            }
            set
            {
                Properties.Settings.Default["RememberMe"] = value;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Command that is responsible for logging in the user.
        /// </summary>
        public ICommand LogInCommand => new DelegateCommand(LogIn);

        /// <summary>
        /// Command for opening the Create Account view.
        /// </summary>
        public ICommand OpenCreateAccountViewCommand => new DelegateCommand(OpenCreateAccountView);

        static LogInViewModel()
        {
            IsChecked = false;
            App.Current.MainWindow.Loaded += MainWindowLoaded;
        }

        /// <summary>
        /// If the application's settings contain a non-empty username and password,
        /// the method calls the AutoLogIn method to attempt an automatic login.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            if ((string)Properties.Settings.Default["UserName"] != string.Empty &&
                (string)Properties.Settings.Default["Password"] != string.Empty)
            {
                AutoLogIn();
            }
        }

        /// <summary>
        /// Opens CreateAccountView.
        /// </summary>
        private void OpenCreateAccountView()
        {
            SetViewModel(new CreateAccountViewModel());
        }

        /// <summary>
        /// Sets the LoggedInInfoText property of the MainWindowViewModel to a welcome message.
        /// Also sets the IsEnabled property of the MainWindowViewModel to true, enabling buttons in MainWindowView.
        /// </summary>
        private static void SetLoggedInValues()
        {
            MainWindowViewModel.LoggedInInfoText = $"Welcome\n{UserModel.UserName}";
            MainWindowViewModel.IsEnabled = true;
        }

        /// <summary>
        /// Attempts to automatically log in the user using the username and password stored in the application's settings.
        /// If the user is successfully authenticated, the method calls the SetLoggedInValues and OpenMoviesView methods.
        /// If an error occurs during the automatic login process, the method logs the error using the LogHelper class,
        /// resets the application's remember me values, and updates the UI to display an error message.
        /// </summary>
        private static void AutoLogIn()
        {
            try
            {
                if (SqlConnectionHelper.GetUserFromDatabase())
                {
                    SetLoggedInValues();
                    OpenMoviesView();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex);
                ResetRememberMeValues();
                MainWindowViewModel.LoggedInInfoText = "An unexpected\nerror occurred\nduring\nthe automatic\nlog in process";
            }
        }

        /// <summary>
        /// First validates the user's input using the InputValidate method. If the user's input is valid,
        /// the method attempts to authenticate the user by calling the GetUserFromDatabase method of the SqlConnectionHelper class.
        /// If the user is successfully authenticated, the method calls the SetLoggedInValues and OpenMoviesView methods.
        /// If the authentication fails, the method updates the UI with an error message indicating that the username or password is invalid.
        /// If an error occurs during the authentication process, the method logs the error using the LogHelper class
        /// and updates the UI with an error message indicating that an unexpected error occurred.
        /// </summary>
        /// <param name="parameter">An object containing PasswordBox control.</param>
        private void LogIn(object parameter)
        {
            if (InputValidate(parameter))
            {
                try
                {
                    if (SqlConnectionHelper.GetUserFromDatabase(this.UserName, parameter))
                    {
                        SetLoggedInValues();
                        OpenMoviesView();
                    }
                    else
                    {
                        this.UserNameError = "Invalid username or password";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.LogError(ex);
                    this.UserNameError = "An unexpected error occurred";
                }
            }
        }

        /// <summary>
        /// Validates the input fields for logging in.
        /// Checks that the username and password fields are not empty.
        /// If either of the validation checks fail, the corresponding error message will be set and the method returns false.
        /// </summary>
        /// <param name="parameter">An object containing PasswordBox control.</param>
        /// <returns>True if both validation checks pass, false otherwise.</returns>
        private bool InputValidate(object parameter)
        {
            bool validInput = true;
            this.UserNameError = string.Empty;
            this.PasswordError = string.Empty;

            if (string.IsNullOrWhiteSpace(this.UserName))
            {
                this.UserNameError = "This field is required";
                validInput = false;
            }

            if (string.IsNullOrWhiteSpace(((PasswordBox)parameter).Password))
            {
                this.PasswordError = "This field is required";
                validInput = false;
            }

            return validInput;
        }
    }
}