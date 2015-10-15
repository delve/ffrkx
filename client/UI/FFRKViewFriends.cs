using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFRKInspector.Proxy;
using FFRKInspector.GameData.Party;
using FFRKInspector.UI.ListViewFields;
using FFRKInspector.GameData;
using FFRKInspector.DataCache;
using FFRKInspector.Analyzer;
using System.Diagnostics;
using System.IO;

namespace FFRKInspector.UI
{
    public partial class FFRKViewFriends : UserControl
    {
        public FFRKViewFriends()
        {
            InitializeComponent();
        }
    }
}
