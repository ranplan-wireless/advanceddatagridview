using System.Data;
using System.Windows.Forms;
using NUnit.Framework;
using Zuby.ADGV;

namespace Zuby.ADGV.Tests
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class SortDiagnosticsTests
    {
        [Test]
        public void GivenFloatBracketColumnWithDbNull_WhenSortApplied_ThenNoThrow()
        {
            var grid = CreateGrid(out var table);

            Assert.DoesNotThrow(() => grid.LoadFilterAndSort(string.Empty, @"[WiFi RSSI [dBm\]] ASC"));
        }

        [Test]
        public void GivenFilterActive_WhenSortApplied_ThenNoThrow()
        {
            var grid = CreateGrid(out var table);

            Assert.DoesNotThrow(() => grid.LoadFilterAndSort(@"([WiFi RSSI [dBm\]] IN ('-44'))", @"[WiFi RSSI [dBm\]] ASC"));
        }

        [Test]
        public void GivenPlainFloatColumnSorted_WhenSortApplied_ThenNoThrow()
        {
            var grid = CreateGrid(out var table);

            Assert.DoesNotThrow(() => grid.LoadFilterAndSort(string.Empty, @"[X] DESC"));
        }

        private static AdvancedDataGridView CreateGrid(out DataTable table)
        {
            var grid = new AdvancedDataGridView();
            table = new DataTable();
            table.Columns.Add("X", typeof(float));
            // typed like iBuildNet's mapping (RX_LEVEL float), with a DBNull row like empty cells
            table.Columns.Add("WiFi RSSI [dBm]", typeof(float));
            table.Rows.Add(1f, -44f);
            table.Rows.Add(2f, -66f);
            table.Rows.Add(3f, -55f);
            table.Rows.Add(4f); // DBNull in the bracket column

            grid.AutoGenerateColumns = false;
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "X", DataPropertyName = "X" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "WiFi RSSI [dBm]", DataPropertyName = "WiFi RSSI [dBm]" });
            grid.DataSource = new BindingSource { DataSource = table };
            foreach (DataGridViewColumn column in grid.Columns)
                grid.EnableFilterAndSort(column);
            return grid;
        }
    }
}
