using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.ViewModels.Controls
{
    public class MessageBox: ReactiveObject
    {
        public ObservableCollection<string> Messages { get; private set; } = new ObservableCollection<string>();

        public void Clear()
        {
            Messages.Clear();
        }

        public void Insert(int index, string message)
        {
            Messages.Insert(index, message);
        }
    }
}
