using DeclarativePM.Lib.Models;

namespace DeclarativePM.Lib.Declare_Templates.TemplateInterfaces
{
    public interface IBiTemplate : ITemplate
    {
        public bool IsActivation(Event e);

        public string GetEventA();

        public string GetEventB();
    }
}