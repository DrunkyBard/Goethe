namespace Goethe.Common;

public readonly struct Either<TLeft, TRight>
{
    public readonly bool IsLeft;

    public readonly TLeft?  Left;
    public readonly TRight? Right;

    public Either(TLeft left)
    {
        Left   = left;
        Right  = default;
        IsLeft = true;
    }

    public Either(TRight right)
    {
        Left   = default;
        Right  = right;
        IsLeft = false;
    }
}

public static class Either
{
    public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left) => new(left);

    public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right) => new(right);
}