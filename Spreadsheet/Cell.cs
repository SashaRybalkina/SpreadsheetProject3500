using System;
using SpreadsheetUtilities;
namespace Spreadsheet
{
	public class Cell
	{
		object contents;
        object value;
		public Cell(double number)
		{
			contents = number;
			value = number;
		}
        public Cell(string text)
        {
			contents = text;
            value = text;
        }
        public Cell(Formula formula)
        {
            contents = formula;
			value = formula.Evaluate();
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

