using System;
using System.Reflection;

public static class StringEnum
{
    public static string GetStringValue(Enum value)
    {
        string output = null;
        Type type = value.GetType();

        //Check first in our cached results...

        //Look for our 'StringValueAttribute'

        //in the field's custom attributes

        FieldInfo fi = type.GetField(value.ToString());
        StringValueAttribute[] attrs =
            fi.GetCustomAttributes(typeof(StringValueAttribute),
                false) as StringValueAttribute[];
        if (attrs.Length > 0)
        {
            output = attrs[0].StringValue;
        }

        return output;
    }
}

/// <summary>
/// This attribute is used to represent a string value
/// for a value in an enum.
/// </summary>
public class StringValueAttribute : Attribute {

    #region Properties

    /// <summary>
    /// Holds the stringvalue for a value in an enum.
    /// </summary>
    public string StringValue { get; protected set; }

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor used to init a StringValue Attribute
    /// </summary>
    /// <param name="value"></param>
    public StringValueAttribute(string value) {
        this.StringValue = value;
    }

    #endregion

}