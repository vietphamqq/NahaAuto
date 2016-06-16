using System;
using System.Collections.Generic;
using Syncfusion.XlsIO;
using System.Linq;

namespace NahaAuto.Code
{
    public abstract class ParseExcelBase<T> : IDisposable
    {
        protected IWorksheet Worksheet;
        protected ExcelEngine ExcelEngine;
        protected IApplication Application;
        protected IWorkbook Workbook;

        protected Dictionary<string, int> HeaderMapping;

        protected int RowData;
        protected ParseExcelBase(string filePath)
        {
            try
            {
                ExcelEngine = new ExcelEngine();
                Application = ExcelEngine.Excel;

                Workbook = Application.Workbooks.Open(filePath, ExcelOpenType.Automatic);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ExcelEngine = null;
                Application = null;
                Workbook = null;
            }
        }

        public abstract T Parse(int row);

        public virtual IWorksheet SelectWorkSheet()
        {
            return Workbook.Worksheets[0];
        }

        public virtual IEnumerable<string> DefinedMapping()
        {
            return null;
        }

        public virtual IEnumerable<T> Load()
        {
            if (ExcelEngine == null)
                return null;

            Worksheet = SelectWorkSheet();

            if (Worksheet == null)
                return null;

            var headerMaping = DefinedMapping();

            if (headerMaping != null)
            {
                HeaderMapping = new Dictionary<string, int>();
                foreach (var mapping in headerMaping)
                {
                    HeaderMapping[mapping] = Worksheet[mapping].Column;
                }
            }

            var items = new List<T>();

            for (var i = RowData; i < Worksheet.Rows.Length; i++)
            {
                var item = Parse(i);
                if (item != null)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public void Dispose()
        {
            Worksheet = null;
            Workbook = null;
            Application = null;
            ExcelEngine?.Dispose();
            ExcelEngine = null;
        }
    }
}