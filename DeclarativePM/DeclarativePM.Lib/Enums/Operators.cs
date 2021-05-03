namespace DeclarativePM.Lib.Enums
{
    public enum Operators
    {
        None = 0,
        //unary
        Not = 1,
        Next = 2,
        Subsequent = 3,
        Eventual = 4,
        //binary
        And = 256,
        Or = 257,
        Imply = 258,
        Equivalence = 259,
        Least = 260
    }
}