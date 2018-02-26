using System.IO;
using System;
using UnityEngine;
using System.Collections.Generic;

namespace WADinator.Structures.Textmap
{
    public abstract class BlockDef
    {
        protected List<Property> properties;

        public BlockDef(List<Property> properties)
        {
            this.properties = properties;
        }

        private string getProp(string prop)
        {
            for(var i = 0; i < properties.Count; i++)
            {
                if(properties[i].property == prop)
                {
                    return properties[i].value;
                }
            }

            return null;
        }

        public bool TryGetInt(string name, out int output, bool user = false)
        {
            name = user ? "user_" + name : name;
            var value = getProp(name);

            return Int32.TryParse(value, out output);
        }

        public bool TryGetFloat(string name, out float output, bool user = false)
        {
            name = user ? "user_" + name : name;
            var value = getProp(name);
            
            return float.TryParse(value, out output);
        }

        public bool TryGetString(string name, out string output, bool user = false)
        {
            name = user ? "user_" + name : name;
            output = getProp(name);

            if(output == null)
            {
                return false;
            }

            return true;
        }

        public bool TryGetBool(string name, out bool output, bool user = false)
        {
            name = user ? "user_" + name : name;
            var value = getProp(name);
            
            return bool.TryParse(value, out output);
        }

        public new string ToString()
        {
            var output = string.Empty;

            output += "{\n";

            foreach(var p in properties)
            {
                output += p.property + "=" + p.value + "\n";
            }

            output += "}";

            return output;
        }

        public bool Instantiated()
        {
            return properties.Count != 0;
        }

        protected void SetStuff(BlockDef def, Type type)
        {
            var props = type.GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType == typeof(int))
                {
                    int value;
                    var success = TryGetInt(prop.Name, out value);

                    if (success)
                    {
                        prop.SetValue(def, value, null);
                    }

                }
                else if (prop.PropertyType == typeof(bool))
                {
                    bool value;
                    var success = TryGetBool(prop.Name, out value);

                    if (success)
                    {
                        prop.SetValue(def, value, null);
                    }

                }
                else if(prop.PropertyType == typeof(string))
                {
                    string value;
                    var success = TryGetString(prop.Name, out value);

                    if (success)
                    {
                        prop.SetValue(def, value, null);
                    }

                }
                else
                {
                    float value;
                    var success = TryGetFloat(prop.Name, out value);

                    if (success)
                    {
                        prop.SetValue(def, value, null);
                    }

                }
            }
        }
    }
}
