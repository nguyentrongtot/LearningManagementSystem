using System.Dynamic;

class Program
{
    static void Main(string[] args)
    {
        dynamic employee = new ExpandoObject();
        employee.Name = "John Smith";
        employee.Age = 33;

        foreach (var property in (IDictionary<String, Object>)employee)
        {
            Console.WriteLine(property.Key + ": " + property.Value);
        }
        // This code example produces the following output:
        // Name: John Smith
        // Age: 33
    }

    // Event handler.
    static void SampleHandler(object sender, EventArgs e)
    {
        Console.WriteLine($"SampleHandler for {0} event", sender);
    }
}
// This code example produces the following output:
// SampleHandler for System.Dynamic.ExpandoObject event.