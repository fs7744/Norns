using Norns.Destiny.Loom;
using Norns.Destiny.Notations;
using Norns.Destiny.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public abstract class AbstractNotationGenerator : INotationGenerator
    {
        protected HashSet<string> GeneratedSet = new HashSet<string>();

        public abstract bool Filter(ITypeSymbolInfo type);

        public virtual INotation GenerateNotations(ISymbolSource source)
        {
            return source.GetTypes()
                .Where(Filter)
                .Where(FilterGenerated)
                .Select(CreateImplement)
                .Combine();
        }

        public virtual bool FilterGenerated(ITypeSymbolInfo type)
        {
            var key = CreateImplementKey(type);
            if (GeneratedSet.Contains(key))
            {
                return false;
            }
            else
            {
                GeneratedSet.Add(key);
                return true;
            }
        }

        public virtual string CreateImplementKey(ITypeSymbolInfo type) 
        {
            return type.IsGenericType ? type.GenericDefinitionName : type.FullName;
        }

        public abstract INotation CreateImplement(ITypeSymbolInfo type);
    }
}