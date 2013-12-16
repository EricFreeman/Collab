namespace Extensions
{
    public static class BoolExtensions
    {
        public static bool IsTrue(this bool? b)
        {
            return b.HasValue && b.Value;
        }

        public static bool IsFalse(this bool? b)
        {
            return b.HasValue && !b.Value;
        }

        public static bool IsNotTrue(this bool? b)
        {
            return !b.HasValue || !b.Value;
        }

        public static bool IsNotFalse(this bool? b)
        {
            return !b.HasValue || b.Value;
        }
    }
}
