namespace backend.Utils
{
    public abstract class UniqueSequenceGenerator
    {
        public static string GenerateUniqueString(int randomStringLength)
        {
            string datePart = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

            Random random = new();
            string stringPart = "";

            char[] characters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
                              'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                              '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            for (int i = 0; i < randomStringLength; i++)
            {
                int randomNumber = random.Next(0, characters.Length);
                stringPart += characters[randomNumber];
            }

            string uniqueString = datePart + stringPart;
            return uniqueString;
        }

        public static long GenerateLongIdUsingTicks()
        {
            long ticks = DateTime.UtcNow.Ticks;
            return ticks;
        }

        public static long GenerateUniqueLongIdToNeo4j()
        {
            // Get the current timestamp in seconds since Unix epoch (1970-01-01)
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // Generate a small random component
            Random random = new Random();
            int randomComponent = random.Next(1000, 9999); // Generate a random number between 1000 and 9999

            // Combine the timestamp and random component into a unique ID
            return long.Parse($"{timestamp}{randomComponent}");
        }

    }
}
