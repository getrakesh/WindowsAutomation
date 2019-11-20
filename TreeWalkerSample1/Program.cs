using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace TreeWalkerSample1
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine();
            Condition paneCondition = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane);
            AutomationElementCollection desktopChildren =AutomationElement.RootElement.FindAll(TreeScope.Children, paneCondition);

            Console.WriteLine(desktopChildren.Count);
            foreach (var s in desktopChildren)
            {
                var childElement = (AutomationElement)s;
                Console.WriteLine(childElement.Current.Name);
                //Console.WriteLine(childElement.Current.ClassName);
                //Console.WriteLine(childElement.Current.ControlType.Id);

                AutomationElementCollection subChildren =childElement.FindAll(TreeScope.Children, paneCondition);

                foreach (var ch in subChildren)
                { 
                    var subChildElement = (AutomationElement)s;
                     Console.WriteLine(subChildElement.Current.Name);
                }


            }
            Console.ReadKey();


        }
    }
}
