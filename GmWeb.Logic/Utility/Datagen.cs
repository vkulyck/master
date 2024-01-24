using RandomDataGenerator.FieldOptions;
using RandomDataGenerator.Randomizers;

namespace GmWeb.Logic.Utility
{
    public static class Datagen
    {
        private static readonly RandomizerEmailAddress Email;

        static Datagen()
        {
            Email = new RandomizerEmailAddress(new FieldOptionsEmailAddress
            {
                Female = true,
                Male = true,
                //UseNullValues = false, // Only want valid emails
                //ValueAsString = true, // Only applicable to SQL generator
                //Left2Right = false // Does nothing
            });
        }

        public static string GetEmail() => Email.Generate();
    }
}
