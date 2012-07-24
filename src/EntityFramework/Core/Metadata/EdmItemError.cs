namespace System.Data.Entity.Core.Metadata.Edm
{
    /// <summary>
    /// Class representing Edm error for an inmemory EdmItem
    /// </summary>
    internal class EdmItemError : EdmError
    {
        #region Constructors

        /// <summary>
        /// Construct the EdmItemError with an error message
        /// </summary>
        /// <param name="message">The error message for this validation error</param>
        public EdmItemError(string message)
            : base(message)
        {
        }

        #endregion

        #region Fields

        #endregion
    }
}
