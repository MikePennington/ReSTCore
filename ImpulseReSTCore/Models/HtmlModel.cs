using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;


namespace ImpulseReSTCore.Models
{

    public struct ObjectStruct
    {
        public string ObjectName;
        public string ObjectValue;
        public string ObjectType;
    }
    
    public class HtmlModel
    {
        public ObjectStruct[] htmlObjects;
        private string ModelName;

        public HtmlModel(object objectToSerialize)
        {
            Populate(objectToSerialize);
        }

        public string GetModelName()
        {
            return ModelName;
        }
        private void Populate(object objectToSerialize)
        {
            if (objectToSerialize == null)
                return;

            PropertyInfo[] objectProperties = objectToSerialize.GetType().GetProperties();
            ModelName = objectToSerialize.GetType().Name;

            htmlObjects = new ObjectStruct[objectProperties.Length];

            for (int i = 0; i < objectProperties.Length; ++i)
            {
                htmlObjects[i].ObjectName = objectProperties[i].Name;

                object t = objectProperties[i].GetValue(objectToSerialize, null);
                if (t != null)
                {
                    htmlObjects[i].ObjectValue = t.ToString();
                }
                else
                {
                    htmlObjects[i].ObjectValue = "NULL";
                }

                htmlObjects[i].ObjectType = objectProperties[i].PropertyType.FullName;
            }

        }
    }

}
