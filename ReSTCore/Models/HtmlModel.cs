using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;


namespace ReSTCore.Models
{

    public struct TypeStruct
    {
        public ObjectStruct[] htmlObjects;
    }

    public struct ObjectStruct
    {
        public string ObjectValue;
        public string ObjectType;
    }

    public class HtmlModel
    {
        public List<TypeStruct> TypeObjects { get; private set; }
        private string ModelName { get; set; }
        public string[] HTMLTitles { get; private set; }

        public HtmlModel(object objectToSerialize)
        {
            TypeObjects = new List<TypeStruct>();
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

            PropertyInfo countProperty = objectToSerialize.GetType().GetProperty("Count");

            if (countProperty != null)
            {
                var p = (IEnumerable) objectToSerialize;
                foreach (object objectIter in p)
                {
                    if (HTMLTitles == null)
                        GetHtmlTitles(objectIter);

                    GetHtmlObjects(objectIter);
                }
            }
            else
            {
                if (HTMLTitles == null)
                    GetHtmlTitles(objectToSerialize);

                GetHtmlObjects(objectToSerialize);
            }
        }

        private void GetHtmlObjects(object objectToSerialize)
        {
            PropertyInfo[] objectProperties = objectToSerialize.GetType().GetProperties();
            ModelName = objectToSerialize.GetType().Name;

            TypeStruct newStruct;
            newStruct.htmlObjects = new ObjectStruct[objectProperties.Length];

            for (int i = 0; i < objectProperties.Length; ++i)
            {
                object t = objectProperties[i].GetValue(objectToSerialize, null);
                newStruct.htmlObjects[i].ObjectValue = (t != null ? t.ToString() : "NULL");
                newStruct.htmlObjects[i].ObjectType = objectProperties[i].PropertyType.FullName;
            }

            TypeObjects.Add(newStruct);
        }

        private void GetHtmlTitles(object objectToSerialize)
        {
            PropertyInfo[] objectProperties = objectToSerialize.GetType().GetProperties();
            HTMLTitles = new string[objectProperties.Length];

            for (int i = 0; i < objectProperties.Length; ++i)
            {
                HTMLTitles[i] = objectProperties[i].Name;
            }
        }
    }
}
