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
            System.Windows.Automation.Automation.AddAutomationEventHandler(eventId: WindowPattern.WindowOpenedEvent,
                element: AutomationElement.RootElement, scope: TreeScope.Children, eventHandler: OnWindowOpened);


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
                    var child = topLevelWindow.FindAll(TreeScope.Children, Condition.TrueCondition);
                    Console.WriteLine("...child element");
                    foreach (var c in child)
                    {
                        var ch = (AutomationElement)c;
                        Console.WriteLine(ch.Current.Name);
                        Console.WriteLine(ch.Current.AutomationId);
                        //Reading text from AutomationElement
                        Console.WriteLine(AutomationExtensions.GetText(ch));

                    }
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
    public static class AutomationExtensions
    {
        public static string GetText(this AutomationElement element)
        {
            object patternObj;
            if (element.TryGetCurrentPattern(ValuePattern.Pattern, out patternObj))
            {
                var valuePattern = (ValuePattern)patternObj;
                return valuePattern.Current.Value;
            }
            else if (element.TryGetCurrentPattern(TextPattern.Pattern, out patternObj))
            {
                var textPattern = (TextPattern)patternObj;
                return textPattern.DocumentRange.GetText(-1).TrimEnd('\r'); // often there is an extra '\r' hanging off the end.
            }
            else
            {
                return element.Current.Name;
            }
        }
    }
}
