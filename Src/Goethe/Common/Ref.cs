namespace Goethe.Common;

public class Ref<T> where T : struct
{
    public T Value;
    
    public Ref(T value) => Value = value;
}