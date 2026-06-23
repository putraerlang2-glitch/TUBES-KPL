using System;
using System.Windows.Forms;

namespace TubesKPL
{
    public static class GenericHelper
    {
        public static void LoadEnumToComboBox<T>(ComboBox comboBox)
            where T : Enum
        {
            comboBox.DataSource = Enum.GetValues(typeof(T));
        }
    }
}