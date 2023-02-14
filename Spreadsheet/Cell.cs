using System;
using SpreadsheetUtilities;
namespace Spreadsheet
{
	public class Cell
	{
		private object contents;
        private object value;

        public Cell(object v)
        {
            contents = v;
            value = v;
        }
		public object GetValue()
		{
			return value;
		}
        public object GetContents()
        {
            return contents;
        }
        public void SetValue(object newValue)
        {
            value = newValue;
        }
        public void SetContents(object newContents)
        {
            contents = newContents;
        }
    }
}

