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

        public void Add(string value)
        {
            AddLast(value.ToSymbol());
        }

        public void Add(IGenerateSymbol value)
        {
            AddLast(value);
        }

        public void Add(params string[] bodys)
        {
            foreach (var item in bodys)
            {
                Add(item);
            }
        }

        public void Add(params IGenerateSymbol[] bodys)
        {
            foreach (var item in bodys)
            {
                Add(item);
            }
        }
    }
}