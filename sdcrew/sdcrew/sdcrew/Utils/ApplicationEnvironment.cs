namespace sdcrew.Utils
{

    public enum ApplicationEnvironment
    {
        Dev = 0,
        Prod = 1,
        Test=2
    }

    public static class CurrentApplicationEnvironment
    {
        public static readonly ApplicationEnvironment Value = ApplicationEnvironment.Prod;
    }

}