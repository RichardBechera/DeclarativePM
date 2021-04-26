namespace DeclarativePM.Lib.Declare_Templates
{
    public struct Existence
    {
        public int Occurances;
        public int LogEvent;
        
        public Existence(int occurances, int logEvent)
        {
            this.Occurances = occurances;
            this.LogEvent = logEvent;
        }
    }
}