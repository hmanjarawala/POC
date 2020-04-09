namespace CoffeeBean.Mail
{
    /// <summary>
    /// The Provider is a class that describes a protocol 
    /// implementation.The values typically come from the
    /// javamail.providers and javamail.default.providers
    /// resource files.An application may also create and
    /// register a Provider object to dynamically add support
    /// for a new provider.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class Provider
    {
        /// <summary>
        /// This inner class defines the Provider type.
        /// Currently, STORE and TRANSPORT are the only two provider types 
        /// supported.
        /// </summary>
        public class Type
        {
            public static readonly Type STORE = new Type("STORE");
            public static readonly Type TRANSPORT = new Type("TRANSPORT");

            private string type;

            private Type(string type)
            {
                this.type = type;
            }

            public override string ToString()
            {
                return type;
            }
        }

        private Type type;
        private string protocol, classname;

        /// <summary>
        /// Create a new provider of the specified type for the specified
        /// protocol.  The specified class implements the provider.
        /// </summary>
        /// <param name="type">Type.STORE or Type.TRANSPORT</param>
        /// <param name="protocol">valid protocol for the type</param>
        /// <param name="classname">class name that implements this protocol</param>
        public Provider(Type type, string protocol, string classname)
        {
            this.type = type;
            this.protocol = protocol;
            this.classname = classname;
        }

        /// <summary>
        /// Returns the type of this Provider.
        /// </summary>
        /// <returns>the provider type</returns>
        public Type ProviderType { get { return type; } }

        /// <summary>
        /// Returns the protocol supported by this Provider.
        /// </summary>
        /// <returns>the protocol</returns>
        public string Protocol { get { return protocol; } }

        /// <summary>
        /// Returns the name of the class that implements the protocol.
        /// </summary>
        /// <returns>the class name</returns>
        public string ClassName { get { return classname; } }

        /// <summary>
        /// Overrides Object.ToString()
        /// </summary>
        public override string ToString()
        {
            string s = string.Concat("CoffeeBean.Mail.Provider[", type,",",protocol,",",classname,"]");            
            return s;
        }
    }
}
