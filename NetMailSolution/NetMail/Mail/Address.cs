using System;

namespace CoffeeBean.Mail
{
    [Serializable]
    public abstract class Address
    {
        static readonly long serialVersionUID = -3822459626751992278L;

        /// <summary>
        /// Return a type string that identifies this address type.
        /// </summary>
        /// <returns>address type</returns>
        /// <see cref="Internet.InternetAddress"/>
        public abstract string GetAddressType();

        /// <summary>
        /// Return a String representation of this address object.
        /// </summary>
        /// <returns>string representation of this address</returns>
        public override string ToString()
        {
            return toString();
        }

        /// <summary>
        /// Return a String representation of this address object.
        /// </summary>
        /// <returns>string representation of this address</returns>
        protected abstract string toString();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj">Address object</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return equals(obj);
        }

        protected abstract bool equals(object address);
    }
}
