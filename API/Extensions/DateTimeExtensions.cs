namespace API.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalculateAge(this DateOnly dob)
        {
            // This will get the Date ONLY, without the time
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Get the difference in years from todays date to the year of the DOB parameter
            var age = today.Year - dob.Year;

            // Take into account if their Birthday for the current year has not passed. This does NOT take into account
            //  leap years or any other time fluctuations that can be thought of. Would require more complicated logic
            if (dob > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}