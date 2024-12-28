using System;
using Autodesk.Revit.UI;

namespace asBIM
{
    public class ViewModelEvent : IExternalEventHandler
    {
        private Action _doAction;
        private readonly ExternalEvent _externalEvent;
        public ViewModelEvent()
        {
            _externalEvent = ExternalEvent.Create(this);
        }
        public void Run(Action action)
        {
            _doAction = action;
            _externalEvent.Raise();
        }
        public void Execute(UIApplication uiApplication)
        {
            try
            {
                _doAction?.Invoke();
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
        }

        public string GetName()
        {
            return "IExternalEventHandler";
        }
    }
}