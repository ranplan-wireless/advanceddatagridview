using System.Data;
using System.Windows.Forms;
using NUnit.Framework;
using Zuby.ADGV;

namespace Zuby.ADGV.Tests
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class AdvancedDataGridViewBracketColumnTests
    {
        [Test]
        public void GivenBoundBracketColumn_WhenLoadFilterAndSortWithEscapedFilter_ThenFilterApplied()
        {
            var grid = CreateGridBoundToBracketTable(out var table);

            grid.LoadFilterAndSort(@"([WiFi RSSI [dBm\]] IN ('-44'))", string.Empty);

            Assert.That(table.DefaultView.Count, Is.EqualTo(1));
            Assert.That(grid.FilterString, Is.EqualTo(@"([WiFi RSSI [dBm\]] IN ('-44'))"));
        }

        [Test]
        public void GivenBoundBracketColumn_WhenLoadFilterAndSortWithEscapedSort_ThenSortApplied()
        {
            var grid = CreateGridBoundToBracketTable(out var table);

            grid.LoadFilterAndSort(string.Empty, @"[WiFi RSSI [dBm\]] ASC");

            // string ASC order of -44/-66/-55 is -44,-55,-66, i.e. X order 1,3,2
            Assert.That(table.DefaultView[0]["X"], Is.EqualTo(1f));
            Assert.That(table.DefaultView[1]["X"], Is.EqualTo(3f));
            Assert.That(table.DefaultView[2]["X"], Is.EqualTo(2f));
            Assert.That(grid.SortString, Is.EqualTo(@"[WiFi RSSI [dBm\]] ASC"));
        }

        [Test]
        public void GivenBoundBracketColumn_WhenLoadFilterAndSortWithEscapedStrings_ThenColumnMenusRestored()
        {
            var grid = CreateGridBoundToBracketTable(out var table);

            grid.LoadFilterAndSort(@"([WiFi RSSI [dBm\]] IN ('-44'))", @"[WiFi RSSI [dBm\]] ASC");

            var menu = ((ColumnHeaderCell)grid.Columns["WiFi RSSI [dBm]"].HeaderCell).MenuStrip;
            Assert.That(menu.ActiveFilterType, Is.EqualTo(MenuStrip.FilterType.CheckList));
            Assert.That(menu.ActiveSortType, Is.EqualTo(MenuStrip.SortType.ASC));
        }

        [Test]
        public void GivenBoundBracketColumn_WhenLoadFilterAndSortWithLegacyUnescapedFilter_ThenNoThrowAndViewUnfiltered()
        {
            var grid = CreateGridBoundToBracketTable(out var table);

            Assert.DoesNotThrow(() => grid.LoadFilterAndSort("([WiFi RSSI [dBm]] IN ('-44'))", string.Empty));

            Assert.That(table.DefaultView.Count, Is.EqualTo(3),
                "an expression the DataView parser cannot address must be skipped, not applied");
        }

        private static AdvancedDataGridView CreateGridBoundToBracketTable(out DataTable table)
        {
            var grid = new AdvancedDataGridView();
            table = new DataTable();
            table.Columns.Add("X", typeof(float));
            table.Columns.Add("WiFi RSSI [dBm]", typeof(string));
            // insertion order chosen so sorted order differs from insertion order
            table.Rows.Add(1f, "-44");
            table.Rows.Add(2f, "-66");
            table.Rows.Add(3f, "-55");

            // explicit columns: auto-generation does not run on a headless control
            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "X", DataPropertyName = "X" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "WiFi RSSI [dBm]", DataPropertyName = "WiFi RSSI [dBm]" });
            // the sort pipeline only applies through a BindingSource (as in the host app)
            grid.DataSource = new BindingSource { DataSource = table };
            foreach (DataGridViewColumn column in grid.Columns)
                grid.EnableFilterAndSort(column);
            return grid;
        }
    }
}
