using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WIndowsAutomationAPI
{

    class Program
    {
        static void Main(string[] args)
        {
            //Registers a method that handles UI Automation events
            System.Windows.Automation.Automation.AddAutomationEventHandler(
            eventId: WindowPattern.WindowOpenedEvent,
            element: AutomationElement.RootElement,
            scope: TreeScope.Children,
            eventHandler: OnWindowOpened);


            System.Windows.Automation.Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);

            Console.ReadLine();
            Automation.RemoveAllEventHandlers();
        }
        private static void OnWindowOpened(object sender, AutomationEventArgs automationEventArgs)
        {
            try
            {
                var element = sender as AutomationElement;
                if (element != null)
                    Console.WriteLine("New Window opened: {0}", element.Current.Name);
            }
            catch (ElementNotAvailableException)
            {
            }
        }
        static AutomationElement _lastWindow;
        private static void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            AutomationElement elementFocused = sender as AutomationElement;
            if (elementFocused == null)
                return;

            try
            {
                AutomationElement topLevelWindow = GetParentWindow(elementFocused);
                if (topLevelWindow == null)
                    return;

                if (topLevelWindow != _lastWindow)
                {
                    _lastWindow = topLevelWindow;
                    Console.WriteLine("Focus moved to window: {0}", topLevelWindow.Current.Name);
                }
                else
                {
                    Console.WriteLine("Focused element: Type: '{0}', Name:'{1}'",
                    elementFocused.Current.LocalizedControlType, elementFocused.Current.Name);
                }
            }
            catch (ElementNotAvailableException)
            {
            }
        }

        private static AutomationElement GetParentWindow(AutomationElement element)
        {
            AutomationElement node = element;
            try
            {
                while (!Equals(node.Current.ControlType, ControlType.Window))
                {
                    node = TreeWalker.ControlViewWalker.GetParent(node);
                    if (node == null)
                        return null;
                }

                return node;
            }
            catch (ElementNotAvailableException)
            {
            }

            return null;
        }
    }
}
