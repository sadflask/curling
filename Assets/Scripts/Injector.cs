using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    class Injector
    {
        private List<object> registeredObjects;

        public object Get(Type typeToGet)
        {
            object toReturn = TryRetrieve(typeToGet);
            if (toReturn != null)
                return toReturn;
            //If the object does not exist, create it and add it to the list.
            return null;
        }

        private object TryRetrieve(Type typeToGet)
        {
            foreach (object obj in registeredObjects)
            {
                //Find the object in the list
                if (obj.GetType() == typeToGet)
                    return obj;
            }
            return null;
        }
    }
}
