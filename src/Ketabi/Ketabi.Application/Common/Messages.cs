namespace Ketabi.Application.Common;

public static class Messages
{
    public static class Auth
    {
        public const string RegisterSuccess = "Welcome to Ketabi! Your account has been created.";
        public const string LoginFailed = "Invalid email or password.";
        public const string AccountLocked = "Your account is locked due to multiple failed attempts.";
        public const string LogoutSuccess = "You have been logged out successfully.";
    }

    public static class Validation
    {
        public const string ProfilePicSize = "Profile picture must be less than 10MB.";
        public const string InvalidFile = "The uploaded file is not valid.";
        public const string RequiredField = "This field is required.";
    }

    public static class Notifications
    {
        public const string SuccessHeader = "SuccessMessage";
        public const string ErrorHeader = "ErrorMessage";
    }
}

public static class AppConstants
{
    public const string DefaultProfilePic = "profile-picture.png";
    public const string AuthCookieName = "AuthToken";

    // for ViewData keys
    public const string SuccessMessageKey = "SuccessMessage";
    public const string ErrorMessageKey = "ErrorMessage";
    public static class Folders
    {
        public const string Uploads = "uploads";
        public const string UserUploads = "users";
        public const string BookCovers = "books";
    }
}