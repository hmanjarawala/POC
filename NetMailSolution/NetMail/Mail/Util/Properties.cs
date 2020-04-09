using System;
using System.Collections;
using System.Collections.Generic;

namespace CoffeeBean.Mail.Util
{
    public class Properties
    {
        private IDictionary _dict;

        public Properties()
        {
            _dict = new Dictionary<string, object>();
        }

        private Properties(IDictionary dict)
        {
            _dict = dict;
        }

        internal static Properties GetSystemProperties()
        {
            return new Properties(Environment.GetEnvironmentVariables());
        }

        public void Add(string key, object value)
        {
            if (_dict.Contains(key))
                _dict[key] = value;
            else
                _dict.Add(key, value);
        }

        public object Get(string key)
        {
            if (!_dict.Contains(key))
                return null;
            else
                return _dict[key];
        }

        ~Properties()
        {
            if(_dict != null)
            {
                _dict.Clear();
                _dict = null;
            }
        }
    }
}
