using System.Collections.Generic;
using System.Text;

namespace Norns.DestinyLoom.Symbols
{
    public class GenerateSymbolLinkedList : LinkedList<IGenerateSymbol>, IGenerateSymbol
    {
        public IGenerateSymbol Separator { get; set; }

        public void Generate(StringBuilder sb)
        {
            var node = First;
            if (node?.Value != null)
            {
                node.Value.Generate(sb);
                node = node.Next;
            }
            while (node?.Value != null)
            {
                Separator?.Generate(sb);
                node.Value.Generate(sb);
                node = node.Next;
            }
        }
    }
}