using System;
using System.Threading;
using Flixplorer.Helpers;
using Flixplorer.Commands;
using System.Windows.Input;
using System.Windows.Controls;

namespace Flixplorer.ViewModels
{
    /// <summary>
    /// Datacontext class for CreateAccountView. Inherits MainViewModel class.
    /// </summary>
    internal class CreateAccountViewModel : MainViewModel
    {
        private string _userName;
        public string UserName { get { return _userName; } set { _userName = value; OnPropertyChanged(); } }

        private string _userNameError;
        public string UserNameError { get { return _userNameError; } set { _userNameError = value; OnPropertyChanged(); } }

        private string _passwordError1;
        public string PasswordError1 { get { return _passwordError1; } set { _passwordError1 = value; OnPropertyChanged(); } }

        private string _passwordError2;
        public string PasswordError2 { get { return _passwordError2; } set { _passwordError2 = value; OnPropertyChanged(); } }

        /// <summary>
        /// Command that navigates the user back to the previous page.
        /// </summary>
        public ICommand GoBackCommand => new DelegateCommand(OpenLogInView);

        /// <summary>
        /// Command that is responsible for creating a new account.
        /// </summary>
        public ICommand CreateAccountCommand => new DelegateCommand(CreateAccount);

        /// <summary>
        /// Displays a success message in the MainWindowViewModel's LoggedInInfoText property using a new thread, and clears the property after 5 seconds.
        /// </summary>
        private void ShowSuccessMessage()
        {
            var thread = new Thread(() =>
            {
                MainWindowViewModel.LoggedInInfoText = "\nAccount\ncreated\nsuccesfully";
                Thread.Sleep(5000);
                MainWindowViewModel.LoggedInInfoText = string.Empty;
            });

            thread.Start();
        }

        /// <summary>
        /// Attempts to create a new account with the specified username and password.
        /// Validates input and checks if the desired username is already in use.
        /// If the input is valid and the desired username is not already in use,
        /// the account is added to the database and a success message is displayed.
        /// If the input is invalid, desired username is already in use or an exception is thrown,
        /// appropriate error messages are displayed.
        /// </summary>
        /// <param name="parameter">An object array containing the password and confirmation password as a PasswordBox</param>
        private void CreateAccount(object parameter)
        {
            var values = (object[])parameter;

            if (InputValidate(values))
            {
                try
                {
                    if (!SqlConnectionHelper.SearchUserFromDatabase(this.UserName))
                    {
                        string salt = SecurityHelper.GenerateSalt();
                        string password = SecurityHelper.HashPassword(((PasswordBox)values[0]).Password, salt);
                        SqlConnectionHelper.AddUserToDatabase(this.UserName, password, salt);
                        ShowSuccessMessage();
                        OpenLogInView();
                    }
                    else
                    {
                        this.UserNameError = "This username is not available";
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
        /// Validates the input fields for creating a new account.
        /// Checks if the username field is not empty, if the password fields are not empty, and if the passwords match.
        /// If any of the validation checks fail, the corresponding error message will be set and the method returns false.
        /// </summary>
        /// <param name="values">An object array containing the two PasswordBox controls.</param>
        /// <returns>True if all validation checks pass, false otherwise.</returns>
        private bool InputValidate(object[] values)
        {
            bool validInput = true;
            this.UserNameError = string.Empty;
            this.PasswordError1 = string.Empty;
            this.PasswordError2 = string.Empty;

            if (string.IsNullOrWhiteSpace(this.UserName))
            {
                this.UserNameError = "This field is required";
                validInput = false;
            }

            if (string.IsNullOrWhiteSpace(((PasswordBox)values[0]).Password))
            {
                this.PasswordError1 = "This field is required";
                validInput = false;
            }
            else if (((PasswordBox)values[0]).Password.Length < 8)
            {
                this.PasswordError1 = "The password needs to have at least 8 characters";
                validInput = false;
            }

            if (string.IsNullOrWhiteSpace(((PasswordBox)values[1]).Password))
            {
                this.PasswordError2 = "This field is required";
                validInput = false;
            }
            else if (((PasswordBox)values[0]).Password.Length > 0 && !((PasswordBox)values[0]).Password.Equals(((PasswordBox)values[1]).Password))
            {
                this.PasswordError2 = "The passwords you have entered do not match";
                validInput = false;
            }

            return validInput;
        }
    }
}