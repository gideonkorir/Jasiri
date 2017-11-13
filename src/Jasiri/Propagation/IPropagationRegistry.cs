namespace Jasiri.Propagation
{
    public interface IPropagationRegistry
    {
        bool TryGet(string format, out IPropagator propagator);  
    }
}
