namespace Jasiri.Propagation
{
    public interface IPropagator
    {
        void Inject(SpanContext spanContext, IPropagatorMap propagatorMap);

        SpanContext Extract(IPropagatorMap propagatorMap);
    }
}
