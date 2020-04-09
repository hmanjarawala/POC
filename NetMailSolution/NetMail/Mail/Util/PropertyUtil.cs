using System;
using CoffeeBean.Mail.Extension;

namespace CoffeeBean.Mail.Util
{
    /// <summary>
    /// Utilities to make it easier to get property values.
    /// Properties can be strings or type-specific value objects.
    /// </summary>
    /// <author>Himanshu Manjarawala</author>
    public class PropertyUtil
    {
        private PropertyUtil() { }

        /// <summary>
        /// Get an integer valued property.
        /// </summary>
        /// <param name="properties">the properties</param>
        /// <param name="name">the property name</param>
        /// <param name="defaultValue">default value if property not found</param>
        /// <returns>the property value</returns>
        public static int GetIntegerPropertyValue(Properties properties, string name, int defaultValue)
        {
            return getInteger(getProperty(properties, name), defaultValue);
        }

        /// <summary>
        /// Get an boolean valued property.
        /// </summary>
        /// <param name="properties">the properties</param>
        /// <param name="name">the property name</param>
        /// <param name="defaultValue">default value if property not found</param>
        /// <returns>the property value</returns>
        public static bool GetBooleanPropertyValue(Properties properties, string name, bool defaultValue)
        {
            return getBoolean(getProperty(properties, name), defaultValue);
        }

        /// <summary>
        /// Get an integer valued property.
        /// </summary>
        /// <param name="session">the Session</param>
        /// <param name="name">the property name</param>
        /// <param name="defaultValue">default value if property not found</param>
        /// <returns>the property value</returns>
        public static int GetIntegerSessionPropertyValue(Session session, string name, int defaultValue)
        {
            return getInteger(getProperty(session.Properties, name), defaultValue);
        }

        /// <summary>
        /// Get an boolean valued property.
        /// </summary>
        /// <param name="session">the Session</param>
        /// <param name="name">the property name</param>
        /// <param name="defaultValue">default value if property not found</param>
        /// <returns>the property value</returns>
        public static bool GetBooleanSessionPropertyValue(Session session, string name, bool defailtValue)
        {
            return getBoolean(getProperty(session.Properties, name), defailtValue);
        }

        public static bool GetBooleanSystemPropertyValue(string name, bool defaultValue)
        {
            try
            {
                return getBoolean(getProperty(Properties.GetSystemProperties(), name), defaultValue);
            }
            catch (AccessViolationException) { }

            // If we can't get the entire Environment Variables because
            // of an AccessViolationException, just ask for the specific variable.

            try
            {
                string value = Environment.GetEnvironmentVariable(name);

                if (string.IsNullOrEmpty(value)) return defaultValue;

                if (defaultValue)
                    return !value.EqualsIgnoreCase(bool.FalseString);
                else
                    return value.EqualsIgnoreCase(bool.TrueString);
            }
            catch
            {
                return defaultValue;                
            }
        }

        /// <summary>
        /// Get the value of the specified property.
        /// </summary>
        private static object getProperty(Properties properties, string name)
        {
            return properties.Get(name);
        }

        /// <summary>
        /// Interpret the value object as an integer,
        /// returning def if unable.
        /// </summary>
        private static int getInteger(object value, int defaultValue)
        {
            if (value.IsNull()) return defaultValue;
            if(value is string)
            {
                try
                {
                    return Convert.ToInt16((string)value);
                }
                catch { }
            }
            if(value is int)
            {
                return Convert.ToInt16(value);
            }
            return defaultValue;
        }

        /// <summary>
        /// Interpret the value object as a boolean,
        /// returning def if unable.
        /// </summary>
        private static bool getBoolean(object value, bool defaultValue)
        {
            if (value.IsNull()) return defaultValue;
            if(value is string)
            {
                //If the default is true, only "false" turns it off.
                //If the default is false, only "true" turns it on.
                if (defaultValue)
                    return !((string)value).EqualsIgnoreCase(bool.FalseString);
                else
                    return ((string)value).EqualsIgnoreCase(bool.TrueString);
            }
            if(value is bool)
            {
                return Convert.ToBoolean(value);
            }
            return defaultValue;
        }
    }
}
