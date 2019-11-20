﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace WindowsAutomationAPIForBrowser
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
                int counter = 1;
                foreach (var c in parents)
                {
                    Console.WriteLine("SR NO-{0}", counter);
                    Console.WriteLine("ID {0}", c.Current.AutomationId);
                    Console.WriteLine("Name {0}", c.Current.Name);
                    Console.WriteLine("Type {0}", c.Current.ControlType.ProgrammaticName);
                    Console.WriteLine("IsControlEmenent {0}", c.Current.IsControlElement);

                    Console.WriteLine("Text {0}", AutomationExtensions.GetText(c));
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

            Program p = new Program();

            if (elementFocused == null)
                return;

            try
            {
                Console.WriteLine("Current Control type and Name", elementFocused.Current.ItemType, elementFocused.Current.LocalizedControlType);

                AutomationElement topLevelWindow = GetParentWindow(elementFocused);
                if (topLevelWindow == null)
                    return;

                if (topLevelWindow != _lastWindow)
                {
                    _lastWindow = topLevelWindow;
                    Console.WriteLine("OnFocusChanged() if TW=LW- Focus moved to window: {0}", topLevelWindow.Current.ControlType.ProgrammaticName);
                    var child = p.GetChildren(topLevelWindow);
                    
                    int counter = 1;
                    foreach (var c in child)
                    {
                        Console.WriteLine("Wait");
                        //Thread.Sleep(2000);
                        Console.WriteLine("SR NO-{0}", counter);
                        Console.WriteLine("ID {0}", c.Current.AutomationId);
                        Console.WriteLine("Name {0}", c.Current.Name);
                        Console.WriteLine("Type {0}", c.Current.ControlType.ProgrammaticName);
                        Console.WriteLine("IsControlEmenent {0}", c.Current.IsControlElement);

                        Console.WriteLine("Text {0}", AutomationExtensions.GetText(c));
                        Console.WriteLine("********************");
                        counter = counter + 1;
                    }

                }
                else
                {
                    Console.WriteLine("OnFocusChanged() if TW!=LW Focused element: Type: '{0}', Name:'{1}'",
                    elementFocused.Current.ControlType, elementFocused.Current.ControlType.ProgrammaticName);
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

            try
            {
                Condition conditions = new AndCondition(new PropertyCondition(AutomationElement.IsContentElementProperty, true),
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                //HERE WE CAN PUT EDIT OR TEXT

                AutomationElementCollection collection = parent.FindAll(TreeScope.Descendants, conditions);

                Console.WriteLine("Find All Executed");

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
            catch(Exception e)
            {
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
