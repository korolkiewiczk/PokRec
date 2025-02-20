using System.Xml.Linq;
using CfrSolver.Model;

namespace CfrSolver.Utils
{
    public class NodeTraverser
    {
        public static void Traverse(Node node, Action<Node, int> nodeAction, int depth = 0)
        {
            nodeAction(node, depth);
            foreach (var nodeChild in node.Children)
            {
                Traverse(nodeChild, nodeAction, depth + 1);
            }
        }

        public static void TraverseToXml(XElement xElement, Node node)
        {
            xElement.Add(
                new XAttribute("player", node.Pos),
                new XAttribute("action", node.Action.ToShortString()),
                new XAttribute("round", node.Round)
            );

            if (node.PayOff != 0)
            {
                xElement.Add(new XAttribute("payoff", node.PayOff));
            }

            foreach (var nodeChild in node.Children)
            {
                XElement newxElement = new XElement("Node");
                TraverseToXml(newxElement, nodeChild);
                xElement.Add(newxElement);
            }
        }

        public static void TraverseWithAction(Node node, Action<Node, string> nodeAction, string action = "")
        {
            var shortActionName = node.Action.ToShortString();
            var newaction = action == "" ? shortActionName : action + "," + shortActionName;
            nodeAction(node, newaction);
            foreach (var nodeChild in node.Children)
            {
                TraverseWithAction(nodeChild, nodeAction, newaction);
            }
        }
    }
}