using DeclarativePM.Lib.Enums;

namespace DeclarativePM.Lib.Models.ConformanceModels
{
    public record WrappedEventActivation
    {
        
        public EventActivationType Activation { get; }
        public Event Event { get; }
        
        public WrappedEventActivation(Event @event, EventActivationType activation)
        {
            Event = @event;
            Activation = activation;
        }
    }
}