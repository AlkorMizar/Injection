using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionConteiner.Container
{
    public enum LifeCycle { SINGLETONE,PER_DEPENDENCY }
    public class ImplContext
    {
        public Type TImplementation { get; set; }
        public LifeCycle LCycle { get; set; }

        public Object Name { get; set; }

        public ImplContext() { }
        public ImplContext(Type tImplementation, LifeCycle lCycle,Object name=null) {
            TImplementation = tImplementation;
            LCycle = lCycle;
            Name = name;
        }

        public bool NameCheck(Object with) {
            if (Name == null)
            {
                return with == null;
            }
            else 
            {
                return Name.Equals(with);
            }
        }
    }
}
