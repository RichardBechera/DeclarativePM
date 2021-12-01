using DeclarativePM.Lib.Declare_Templates.TemplateInterfaces;

namespace DeclarativePM.Lib.Declare_Templates
{
    public interface IExistenceTemplate : ITemplate
    {
        public string GetEvent();

        public int GetCount();
    }
}