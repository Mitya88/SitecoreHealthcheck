using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Healthcheck.ExternalChecker.Configuration
{
    public class HealthcheckConfig : ConfigurationSection
    {
        // Create a property that lets us access the collection
        // of SageCRMInstanceElements

        // Specify the name of the element used for the property
        [ConfigurationProperty("components")]
        // Specify the type of elements found in the collection
        [ConfigurationCollection(typeof(HealthcheckCollection))]
        public HealthcheckCollection Healthchecks
        {
            get
            {
                // Get the collection and parse it
                return (HealthcheckCollection)this["components"];
            }
        }
    }

    public class HealthcheckCollection : ConfigurationElementCollection
    {
        // Create a property that lets us access an element in the
        // collection with the int index of the element
        public ComponentElement this[int index]
        {
            get
            {
                // Gets the SageCRMInstanceElement at the specified
                // index in the collection
                return (ComponentElement)BaseGet(index);
            }
            set
            {
                // Check if a SageCRMInstanceElement exists at the
                // specified index and delete it if it does
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
 
                // Add the new SageCRMInstanceElement at the specified
                // index
                BaseAdd(index, value);
            }
        }
 
        // Create a property that lets us access an element in the
        // colleciton with the name of the element
        public new ComponentElement this[string key]
        {
            get
            {
                // Gets the SageCRMInstanceElement where the name
                // matches the string key specified
                return (ComponentElement)BaseGet(key);
            }
            set
            {
                // Checks if a SageCRMInstanceElement exists with
                // the specified name and deletes it if it does
                if (BaseGet(key) != null)
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));
 
                // Adds the new SageCRMInstanceElement
                BaseAdd(value);
            }
        }
 
        // Method that must be overriden to create a new element
        // that can be stored in the collection
        protected override ConfigurationElement CreateNewElement()
        {
            return new ComponentElement();
        }
 
        // Method that must be overriden to get the key of a
        // specified element
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ComponentElement)element).Name;
        }
    }

    public class ComponentElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                // Return the value of the 'name' attribute as a string
                return (string)base["name"];
            }
            set
            {
                // Set the value of the 'name' attribute
                base["name"] = value;
            }
        }

        [ConfigurationProperty("sitecoreId", IsRequired = true)]
        public string SitecoreId
        {
            get
            {
                // Return the value of the 'name' attribute as a string
                return (string)base["sitecoreId"];
            }
            set
            {
                // Set the value of the 'name' attribute
                base["sitecoreId"] = value;
            }
        }

        [ConfigurationProperty("interval", IsRequired = true)]
        public string Interval
        {
            get
            {
                // Return the value of the 'name' attribute as a string
                return (string)base["interval"];
            }
            set
            {
                // Set the value of the 'name' attribute
                base["interval"] = value;
            }
        }
    }
}
