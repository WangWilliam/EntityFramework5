namespace System.Data.Entity.Core.EntityModel.SchemaObjectModel
{
    using System.Xml;

    /// <summary>
    /// Summary description for Documentation.
    /// </summary>
    internal sealed class TextElement : SchemaElement
    {
        #region Instance Fields

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentElement"></param>
        public TextElement(SchemaElement parentElement)
            : base(parentElement)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; private set; }

        #endregion

        #region Protected Properties

        protected override bool HandleText(XmlReader reader)
        {
            TextElementTextHandler(reader);
            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        private void TextElementTextHandler(XmlReader reader)
        {
            var text = reader.Value;
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            if (string.IsNullOrEmpty(Value))
            {
                Value = text;
            }
            else
            {
                Value += text;
            }
        }

        #endregion
    }
}
