namespace CoffeeBean.Mail
{
    /// <summary>
    /// The Header class stores a name/value pair to represent headers.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class Header
    {
        protected string name;
        protected string value;

        /// <summary>
        /// Construct a Header object.
        /// </summary>
        /// <param name="name">name of the header</param>
        /// <param name="value">value of the header</param>
        public Header(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        /// <summary>
        /// Returns the name of this header.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Returns the value of this header.
        /// </summary>
        public virtual string Value
        {
            get { return value; }
        }
    }
}
