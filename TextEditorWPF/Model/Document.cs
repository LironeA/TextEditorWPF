using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextEditorWPF.Model
{
    public class Document
    {
        public string Name { get; set; }
        public string RawData { get; set; }
        public List<Element> Elements { get; set; }

    }
}
