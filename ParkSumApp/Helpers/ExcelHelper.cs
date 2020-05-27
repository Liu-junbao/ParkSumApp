using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkSumApp.Helpers
{
    public class ExcelHelper
    {
        public async static Task<bool?> ExportWithDialog<TModel>(string defaultFileName, IEnumerable<TModel> models, string sheetName, Dictionary<string, Func<TModel, object>> columnsAndGetCellAction)
        {
            if (models == null || columnsAndGetCellAction == null) return false;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.FileName = defaultFileName;
            dialog.Filter = "Excel2007文件|*.xlsx|Excel2003文件|*.xls";
            if (dialog.ShowDialog() == true)
            {
                var fileName = dialog.FileName;
                var isXls = dialog.FilterIndex == 2;

                return await Task.Run(() =>
                {
                    try
                    {
                        IWorkbook workbook;
                        if (File.Exists(fileName) == false)
                            workbook = isXls ? new HSSFWorkbook() : (IWorkbook)new XSSFWorkbook();
                        else
                        {
                            try
                            {
                                using (var ms = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                                {
                                    workbook = isXls ? new HSSFWorkbook(ms) : (IWorkbook)new XSSFWorkbook(ms);
                                }
                            }
                            catch (Exception e)
                            {
                                //文件被占用
                                return false;
                            }
                        }

                        //删除表
                        var oldSheet = workbook.GetSheet(sheetName);
                        if (oldSheet != null)
                        {
                            var sheetIndex = workbook.GetSheetIndex(oldSheet);
                            workbook.RemoveSheetAt(sheetIndex);
                        }

                        //创建表
                        var sheet = workbook.CreateSheet(sheetName);
                        var rowIndex = 0;

                        //创建列
                        var columnRow = sheet.CreateRow(rowIndex);
                        var columnIndex = 0;
                        foreach (var item in columnsAndGetCellAction)
                        {
                            var headerText = item.Key;
                            var cell = columnRow.CreateCell(columnIndex);
                            cell.SetCellValue(headerText);
                            columnIndex++;
                        }

                        //表数据   
                        foreach (var model in models)
                        {
                            rowIndex++;
                            var row = sheet.CreateRow(rowIndex);
                            var colIndex = 0;
                            foreach (var item in columnsAndGetCellAction)
                            {
                                var getCellAction = item.Value;
                                if (getCellAction != null)
                                {
                                    var value = getCellAction.Invoke(model);
                                    string valueText;

                                    if (value is DateTime time)
                                        valueText = $"{time:yyyy-MM-dd HH:mm:ss.fff}";
                                    else if (value is TimeSpan timeLength)
                                        valueText = $"{timeLength:c}";
                                    else
                                        valueText = value?.ToString();
                                    var cell = row.CreateCell(colIndex);
                                    cell.SetCellValue(valueText);
                                }

                                colIndex++;
                            }
                        }

                        var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                        workbook.Write(fileStream);
                        fileStream.Close();
                        return true;
                    }
                    catch (Exception e)
                    {
                        //
                        return false;
                    }
                });
            }
            return null;
        }
    }
}
