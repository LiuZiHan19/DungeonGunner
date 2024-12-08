using System.Collections;
using UnityEngine;

public static class HelperUtilities
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fieldName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fieldName + " is empty and must contain a value in object " + thisObject.name);
            return true;
        }

        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fieldName, IEnumerable enumerableToCheck)
    {
        bool error = false;
        int count = 0;

        foreach (var item in enumerableToCheck)
        {
            if (item == null)
            {
                Debug.Log(fieldName + " contains null value in object " + thisObject.name);
                error = true;
            }
            else
            {
                count++;
            }
        }

        if (count == 0)
        {
            Debug.Log(fieldName + " has no value in object " + thisObject.name);
        }

        return error;
    }
}