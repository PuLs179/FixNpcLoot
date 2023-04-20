using System.Windows;
using System.Windows.Controls;

namespace FixNpcLoot
{
    public partial class FixNpcLootControl : UserControl
    {

        private FixNpcLoot Plugin { get; }

        private FixNpcLootControl()
        {
            InitializeComponent();
        }

        public FixNpcLootControl(FixNpcLoot plugin) : this()
        {
            Plugin = plugin;
            DataContext = plugin.Config;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin.Save();
        }
    }
}
