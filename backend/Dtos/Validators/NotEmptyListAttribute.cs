using System.Collections;
using System.ComponentModel.DataAnnotations;

public class NotEmptyListAttribute : ValidationAttribute
{
    public NotEmptyListAttribute() : base("The list cannot be empty.")
    {
    }

    public override bool IsValid(object value)
    {
        // Check if the value is null
        if (value == null)
            return false;

        // Check if it's an IEnumerable
        if (value is IEnumerable collection)
        {
            // Use LINQ to check if the collection has any elements
            return collection.Cast<object>().Any();
        }

        // If not a collection or list, validation fails
        return false;
    }
}
