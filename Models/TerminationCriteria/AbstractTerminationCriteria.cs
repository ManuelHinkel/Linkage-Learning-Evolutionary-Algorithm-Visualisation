using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Models.TerminationCriteria
{
    public abstract class AbstractTerminationCriteria
    {
        public bool IsValid { get; private set; } = true;
        protected object argument;
        public AbstractTerminationCriteria(string parameter)
        {
            if (!Convert(parameter, GetArgumentType(), out argument))
            {
                IsValid = false;
            }
        }

        public abstract bool ShouldTerminate(IterationData iteration);
        public abstract Type GetArgumentType();

        private bool Convert(string value, Type type, out object output)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);

            if (converter.IsValid(value))
            {
                output = converter.ConvertTo(value, type);
                return true;
            }
            else
            {
                output = null;
                return false;
            }

        }
    }
}
