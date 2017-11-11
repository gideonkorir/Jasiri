namespace Jasiri.Propagation
{
    public interface IPropagator
    {
        void Inject(ZipkinTraceContext spanContext, IPropagatorMap propagatorMap);

        ZipkinTraceContext Extract(IPropagatorMap propagatorMap);
    }
}
