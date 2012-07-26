using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;


namespace ImpulseReSTCore.Models
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
        public List<TypeStruct> typeObjects;
        private string ModelName;
        public string[] htmlTitles;

        public HtmlModel(object objectToSerialize)
        {
            typeObjects = new List<TypeStruct>();
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
                    IEnumerable p = (IEnumerable) objectToSerialize;

                    foreach (object objectIter in p)
                    {
                        if (htmlTitles == null)
                        {
                            GetHtmlTitles(objectIter);
                        }

                        GetHtmlObjects(objectIter);
                    }
            }
            else
            {
                if (htmlTitles == null)
                {
                    GetHtmlTitles(objectToSerialize);
                }

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
                if (t != null)
                {
                    newStruct.htmlObjects[i].ObjectValue = t.ToString();
                }
                else
                {
                    newStruct.htmlObjects[i].ObjectValue = "NULL";
                }

                newStruct.htmlObjects[i].ObjectType = objectProperties[i].PropertyType.FullName;
            }

            typeObjects.Add(newStruct);
        }

        private void GetHtmlTitles(object objectToSerialize)
        {
            PropertyInfo[] objectProperties = objectToSerialize.GetType().GetProperties();
            htmlTitles = new string[objectProperties.Length];

            for (int i = 0; i < objectProperties.Length; ++i)
            {
                htmlTitles[i] = objectProperties[i].Name;
            }
        }
    }

}
