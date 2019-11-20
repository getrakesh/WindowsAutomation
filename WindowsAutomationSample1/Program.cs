using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WindowsAutomationSample1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Registers a method that handles UI Automation events
            System.Windows.Automation.Automation.AddAutomationEventHandler(eventId: WindowPattern.WindowOpenedEvent, element: AutomationElement.RootElement,
                scope: TreeScope.Children, eventHandler: OnWindowOpened);

            //Registers a method that handles OnFocus changed
            System.Windows.Automation.Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);

            Console.ReadLine();
            Automation.RemoveAllEventHandlers();
        }

        private static void OnWindowOpened(object sender, AutomationEventArgs automationEventArgs)
        {
            Program p = new Program();
            try
            {
                var element = sender as AutomationElement;
                if (element != null)
                    Console.WriteLine("OnWindowOpened()-New Window opened: {0}", element.Current.Name);

                var parents = p.GetChildren(element);
                Console.WriteLine("All children count-" + parents.Count);
                int counter =1 ;
                foreach (var c in parents)
                {
                    Console.WriteLine("SR NO-{0}", counter);
                    Console.WriteLine("ID {0}", c.Current.AutomationId);
                    Console.WriteLine("Name {0}", c.Current.Name);
                    Console.WriteLine("Type {0}", c.Current.ControlType.ProgrammaticName);
                    Console.WriteLine("IsControlEmenent {0}", c.Current.IsControlElement);


                    Console.WriteLine( "Text {0}",AutomationExtensions.GetText(c));
                    Console.WriteLine("********************");
                    counter = counter + 1;
                }
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
                Console.WriteLine("Current Control type and Name",elementFocused.Current.ControlType, elementFocused.Current.Name);

                AutomationElement topLevelWindow = GetParentWindow(elementFocused);
                if (topLevelWindow == null)
                    return;

                if (topLevelWindow != _lastWindow)
                {
                    _lastWindow = topLevelWindow;
                    Console.WriteLine("OnFocusChanged() if TW=LW- Focus moved to window: {0}", topLevelWindow.Current.Name);
                }
                else
                {
                    Console.WriteLine("OnFocusChanged() if TW!=LW Focused element: Type: '{0}', Name:'{1}'",
                    elementFocused.Current.ControlType, elementFocused.Current.Name);
                }

            }
            catch (ElementNotAvailableException)
            {
            }
        }

        // using FindAll function
        private List<AutomationElement> GetChildren(AutomationElement parent)
    {
        if (parent == null)
        {
            // null parameter
            throw new ArgumentException();
        }

            //TO DO Condition need to Test with samples? 
            Condition conditions = new AndCondition(new PropertyCondition(AutomationElement.IsControlElementProperty, true),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
            //Thread.Sleep(1000);

            AutomationElementCollection collection = parent.FindAll(TreeScope.Subtree, conditions);

        if (collection != null)
        {
            List<AutomationElement> result = new List<AutomationElement>(collection.Cast<AutomationElement>());
            return result;
        }
        else
        {
            // some error occured
            return null;
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

        // using predefined TreeWalker.ControlViewWalker
        //private List<AutomationElement> GetChildren(AutomationElement parent)
        //{
        //    if (parent == null)
        //    {
        //        // null parameter
        //        throw new ArgumentException();
        //    }

        //    List<AutomationElement> result = new List<AutomationElement>();

        //    // the predefined tree walker wich iterates through controls
        //    TreeWalker tw = TreeWalker.ControlViewWalker;
        //    AutomationElement child = tw.GetFirstChild(parent);

        //    while (child != null)
        //    {
        //        result.Add(child);
        //        child = tw.GetNextSibling(child);
        //    }

        //    return result;
        //}
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
            else if (element.TryGetCurrentPattern(TextPat   tern.Pattern, out patternObj))
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
