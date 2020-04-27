using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.XWPF.UserModel;
using SQLiteWPF.Dao;
using SQLiteWPF.Model;
using SQLiteWPF.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Reflection;
using System.Windows;

namespace SQLiteWPF.ViewModel
{
    class CompanyViewModel : ViewModelBase
    {
        /// <summary>
        /// 公司信息列表
        /// </summary>
        public static ObservableCollection<CompanyModel> companyData;

        /// <summary>
        /// 设置和获取公司信息列表
        /// </summary>
        public ObservableCollection<CompanyModel> CompanyData
        {
            get { return companyData; }
            set { companyData = value; RaisePropertyChanged(() => CompanyData); }
        }


        /// <summary>
        /// BPOIExcel导入数据
        /// </summary>
        void NPOIImportExecel()
        {
            string openFilePath = "";                                //导入时文件的路径
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Excel文件|*.xlsx;*.xls";           //文件名筛选字符串
            openDialog.ShowDialog();
            openFilePath = openDialog.FileName;

            IWorkbook workbook = null;
            ISheet sheet = null;
            System.Data.DataTable dataTable = null;
            NPOI.SS.UserModel.ICell cell = null;
            DataColumn column = null;
            DataRow dataRow = null;
            IRow row = null;

            try
            {
                using (FileStream fs = File.OpenRead(openFilePath))
                {
                    // 2007版本
                    if (openFilePath.IndexOf(".xlsx") > 0)
                        workbook = new XSSFWorkbook(fs);
                    // 2003版本
                    else if (openFilePath.IndexOf(".xls") > 0)
                        workbook = new HSSFWorkbook(fs);

                    if (workbook != null)
                    {
                        sheet = workbook.GetSheetAt(0);//读取第一个sheet，当然也可以循环读取每个sheet

                        if (sheet != null)
                        {
                            int rowCount = sheet.LastRowNum;    //总行数 

                            if (rowCount > 0)
                            {
                                IRow firstRow = sheet.GetRow(0);
                                int cellCount = firstRow.LastCellNum;
                                dataTable = new System.Data.DataTable();
                                //填充列
                                for (int i = 0; i <= cellCount; i++)
                                {
                                    column = new DataColumn("column" + i);
                                    dataTable.Columns.Add(column);
                                }

                                //填充行
                                for (int i = 0; i <= rowCount; ++i)
                                {
                                    row = sheet.GetRow(i);
                                    if (row == null) continue;

                                    dataRow = dataTable.NewRow();
                                    for (int j = row.FirstCellNum; j < cellCount; ++j)
                                    {
                                        cell = row.GetCell(j);
                                        if (cell == null)
                                        {
                                            dataRow[j] = "";
                                        }
                                        else
                                        {
                                            //CellType(Unknown = -1,Numeric = 0,String = 1,Formula = 2,Blank = 3,Boolean = 4,Error = 5,)
                                            switch (cell.CellType)
                                            {
                                                case CellType.Blank:
                                                    dataRow[j] = "";
                                                    break;
                                                case CellType.Numeric:
                                                    short format = cell.CellStyle.DataFormat;
                                                    //对时间格式（2015.12.5、2015/12/5、2015-12-5等）的处理
                                                    if (format == 14 || format == 31 || format == 57 || format == 58)
                                                        dataRow[j] = cell.DateCellValue;
                                                    else
                                                        dataRow[j] = cell.NumericCellValue;
                                                    break;
                                                case CellType.String:
                                                    dataRow[j] = cell.StringCellValue;
                                                    break;
                                            }
                                        }
                                    }
                                    dataTable.Rows.Add(dataRow);
                                }
                            }

                            for (int i = 0; i < dataTable.Rows.Count; i++)
                            {
                                CompanyModel companyModel = new CompanyModel();
                                companyModel.Name = dataTable.Rows[i][0].ToString();
                                companyModel.Address = dataTable.Rows[i][1].ToString();
                                companyModel.Telephone = dataTable.Rows[i][2].ToString();
                                companyModel.LegalPerson = dataTable.Rows[i][3].ToString();
                                companyModel.RegistrationDate = dataTable.Rows[i][4].ToString();
                                CompanyData.Add(companyModel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// NPOI测试导出数据
        /// </summary>
        void NPOIExportEexcel()
        {

            //创建文件
            string fileName = "NPOI导出EXcel";
            string saveFilePath = "";                       //导出时文件的路径
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = "xlsx";                 //默认文件扩展名
            saveDialog.Filter = "Excel文件|*.xlsx";         //文件名筛选字符串
            saveDialog.FileName = fileName;                 //导出文件名称
            saveDialog.ShowDialog();                        //显示窗口
            saveFilePath = saveDialog.FileName;             //文件路径 引用命名空间 using NPOI.HSSF.UserModel;using NPOI.SS.UserModel; using System.IO;

            //将List集合中的内容导出到Excel中
            //1、创建工作簿对象 2007版本
            IWorkbook wkBook = new XSSFWorkbook();
            //2、在该工作簿中创建工作表对象
            ISheet sheet = wkBook.CreateSheet("公司详细信息"); //Excel工作表的名称

            sheet.SetColumnWidth(0, 20 * 256);
            sheet.SetColumnWidth(1, 15 * 256);
            sheet.SetColumnWidth(2, 15 * 256);
            sheet.SetColumnWidth(3, 15 * 256);
            sheet.SetColumnWidth(4, 30 * 256);

            IRow row = sheet.CreateRow(0);

            row.CreateCell(0).SetCellValue("名称");
            row.CreateCell(1).SetCellValue("地址");
            row.CreateCell(2).SetCellValue("联系方式");
            row.CreateCell(3).SetCellValue("法人");
            row.CreateCell(4).SetCellValue("注册日期");

            //2.1向工作表中插入行与单元格
            for (int i = 0; i < companyData.Count; i++)
            {
                //在Sheet中插入创建一行
                row = sheet.CreateRow(i + 1);
                //在该行中创建单元格
                //方式一
                //ICell cell = row.CreateCell(0);
                //cell.SetCellValue(list[i].Name);
                //方式二
                row.CreateCell(0).SetCellValue(companyData[i].Name); //给单元格设置值：第一个参数(第几个单元格)；第二个参数(给当前单元格赋值)
                row.CreateCell(1).SetCellValue(companyData[i].Address);
                row.CreateCell(2).SetCellValue(companyData[i].Telephone);
                row.CreateCell(3).SetCellValue(companyData[i].LegalPerson);
                row.CreateCell(4).SetCellValue(companyData[i].RegistrationDate);
            }

            //3、写入，把内存中的workBook对象写入到磁盘上
            FileStream fsWrite = new FileStream(saveFilePath, FileMode.Create);
            wkBook.Write(fsWrite);
            MessageBox.Show("写入成功！", "提示");
            fsWrite.Close(); //关闭文件流
            wkBook.Close();  //关闭工作簿
            fsWrite.Dispose(); //释放文件流
        }

        /// <summary>
        /// 创建word文档中的段落对象和设置段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>
        /// <param name="document">document文档对象</param>
        /// <param name="fillContent">段落第一个文本对象填充的内容</param>
        /// <param name="isBold">是否加粗</param>
        /// <param name="fontSize">字体大小</param>
        /// <param name="fontFamily">字体</param>
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="isStatement">是否在同一段落创建第二个文本对象（解决同一段落里面需要填充两个或者多个文本值的情况，多个文本需要自己拓展，现在最多支持两个）</param>
        /// <param name="secondFillContent">第二次声明的文本对象填充的内容，样式与第一次的一致</param>
        /// <returns></returns>
        private static XWPFParagraph ParagraphInstanceSetting(XWPFDocument document, string fillContent, bool isBold, int fontSize, string fontFamily, ParagraphAlignment paragraphAlign, bool isStatement = false, string secondFillContent = "")
        {
            XWPFParagraph paragraph = document.CreateParagraph();//创建段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.IsBold = isBold;//文字加粗
            xwpfRun.SetText(fillContent);//填充内容
            xwpfRun.FontSize = fontSize;//设置文字大小
            xwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定

            if (isStatement)
            {
                XWPFRun secondxwpfRun = paragraph.CreateRun();//创建段落文本对象
                secondxwpfRun.IsBold = isBold;//文字加粗
                secondxwpfRun.SetText(secondFillContent);//填充内容
                secondxwpfRun.FontSize = fontSize;//设置文字大小
                secondxwpfRun.SetFontFamily(fontFamily, FontCharRange.None); //设置标题样式如：（微软雅黑，隶书，楷体）根据自己的需求而定
            }


            return paragraph;
        }

        /// <summary>  
        /// 创建Word文档中表格段落实例和设置表格段落文本的基本样式（字体大小，字体，字体颜色，字体对齐位置）
        /// </summary>  
        /// <param name="document">document文档对象</param>  
        /// <param name="table">表格对象</param>  
        /// <param name="fillContent">要填充的文字</param>  
        /// <param name="paragraphAlign">段落排列（左对齐，居中，右对齐）</param>
        /// <param name="rowsHeight">设置文本位置（设置两行之间的行间），从而实现table的高度设置效果  </param>
        /// <param name="isBold">是否加粗（true加粗，false不加粗）</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>  
        private static XWPFParagraph SetTableParagraphInstanceSetting(XWPFDocument document, XWPFTable table, string fillContent, ParagraphAlignment paragraphAlign, int rowsHeight, bool isBold, int fontSize = 10)
        {
            var para = new CT_P();
            XWPFParagraph paragraph = new XWPFParagraph(para, table.Body);//创建表格中的段落对象
            paragraph.Alignment = paragraphAlign;//文字显示位置,段落排列（左对齐，居中，右对齐）

            XWPFRun xwpfRun = paragraph.CreateRun();//创建段落文本对象
            xwpfRun.SetText(fillContent);
            xwpfRun.FontSize = fontSize;//字体大小
            xwpfRun.IsBold = isBold;//是否加粗
            xwpfRun.SetFontFamily("宋体", FontCharRange.None);//设置字体（如：微软雅黑,华文楷体,宋体）
            xwpfRun.SetTextPosition(rowsHeight);//设置文本位置（设置两行之间的行间），从而实现table的高度设置效果 
            return paragraph;
        }       
        /// <summary>
        /// NPOI导出World
        /// </summary>
        void NPOIExportWorld()
        {

            string savePath = "";
            try
            {
                string currentDate = DateTime.Now.ToString("yyyyMMdd");
                string checkTime = DateTime.Now.ToString("yyyy年MM月dd日");//检查时间
                                                                        //保存文件到静态资源wwwroot,使用绝对路径路径

                string openFilePath = "NPOIWord导出";                                //导出时文件的路径
                SaveFileDialog openDialog = new SaveFileDialog();
                openDialog.Filter = "Word文件|*.doc;*.dcox";            //文件名筛选字符串
                openDialog.FileName = "NPOIWord导出";
                openDialog.ShowDialog();
                openFilePath = openDialog.FileName;
                string workFileName = "NPOIWord导出";
                string fileName = string.Format("{0}.docx", workFileName, System.Text.Encoding.UTF8);

                if (!Directory.Exists(openFilePath))
                {
                    Directory.CreateDirectory(openFilePath);
                }

                //通过使用文件流，创建文件流对象，向文件流中写入内容，并保存为Word文档格式
                using (var stream = new FileStream(Path.Combine(openFilePath, fileName), FileMode.Create, FileAccess.Write))
                {
                    //创建document文档对象对象实例
                    XWPFDocument document = new XWPFDocument();

                    /**
                     *这里我通过设置公共的Word文档中SetParagraph（段落）实例创建和段落样式格式设置，大大减少了代码的冗余，
                     * 避免每使用一个段落而去创建一次段落实例和设置段落的基本样式
                     *(如下，ParagraphInstanceSetting为段落实例创建和样式设置，后面索引表示为当前是第几行段落,索引从0开始)
                     */
                    //文本标题
                    document.SetParagraph(ParagraphInstanceSetting(document, workFileName, true, 19, "宋体", ParagraphAlignment.CENTER), 0);

                    //TODO:这里一行需要显示两个文本
                    document.SetParagraph(ParagraphInstanceSetting(document, $"编号：20190927101120445887", false, 14, "宋体", ParagraphAlignment.CENTER, true, $"    检查时间：{checkTime}"), 1);


                    document.SetParagraph(ParagraphInstanceSetting(document, "登记机关：企业员工监督检查机构", false, 14, "宋体", ParagraphAlignment.LEFT), 2);


                    #region 文档第一个表格对象实例
                    //创建文档中的表格对象实例
                    XWPFTable firstXwpfTable = document.CreateTable(4, 4);//显示的行列数rows:3行,cols:4列
                    firstXwpfTable.Width = 5200;//总宽度
                    firstXwpfTable.SetColumnWidth(0, 1300); /* 设置列宽 */
                    firstXwpfTable.SetColumnWidth(1, 1100); /* 设置列宽 */
                    firstXwpfTable.SetColumnWidth(2, 1400); /* 设置列宽 */
                    firstXwpfTable.SetColumnWidth(3, 1400); /* 设置列宽 */

                    //Table 表格第一行展示...后面的都是一样，只改变GetRow中的行数
                    firstXwpfTable.GetRow(0).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "企业名称", ParagraphAlignment.CENTER, 40, true));
                    firstXwpfTable.GetRow(0).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "追逐时光", ParagraphAlignment.CENTER, 40, false));
                    firstXwpfTable.GetRow(0).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "企业地址", ParagraphAlignment.CENTER, 40, true));
                    firstXwpfTable.GetRow(0).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "湖南省-长沙市-岳麓区", ParagraphAlignment.CENTER, 40, false));

                    //Table 表格第二行
                    firstXwpfTable.GetRow(1).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "联系人", ParagraphAlignment.CENTER, 40, true));
                    firstXwpfTable.GetRow(1).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "小明同学", ParagraphAlignment.CENTER, 40, false));
                    firstXwpfTable.GetRow(1).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "联系方式", ParagraphAlignment.CENTER, 40, true));
                    firstXwpfTable.GetRow(1).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "151****0456", ParagraphAlignment.CENTER, 40, false));


                    //Table 表格第三行
                    firstXwpfTable.GetRow(2).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "企业许可证号", ParagraphAlignment.CENTER, 40, true));
                    firstXwpfTable.GetRow(2).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "XXXXX-66666666", ParagraphAlignment.CENTER, 40, false));
                    firstXwpfTable.GetRow(2).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "检查次数", ParagraphAlignment.CENTER, 40, true));
                    firstXwpfTable.GetRow(2).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, $"本年度检查8次", ParagraphAlignment.CENTER, 40, false));


                    firstXwpfTable.GetRow(3).MergeCells(0, 3);//合并3列
                    firstXwpfTable.GetRow(3).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "", ParagraphAlignment.LEFT, 10, false));

                    #endregion

                    var checkPeopleNum = 0;//检查人数
                    var totalScore = 0;//总得分

                    #region 文档第二个表格对象实例（遍历表格项）
                    //创建文档中的表格对象实例
                    XWPFTable secoedXwpfTable = document.CreateTable(5, 4);//显示的行列数rows:8行,cols:4列
                    secoedXwpfTable.Width = 5200;//总宽度
                    secoedXwpfTable.SetColumnWidth(0, 1300); /* 设置列宽 */
                    secoedXwpfTable.SetColumnWidth(1, 1100); /* 设置列宽 */
                    secoedXwpfTable.SetColumnWidth(2, 1400); /* 设置列宽 */
                    secoedXwpfTable.SetColumnWidth(3, 1400); /* 设置列宽 */

                    //遍历表格标题
                    secoedXwpfTable.GetRow(0).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "员工姓名", ParagraphAlignment.CENTER, 40, true));
                    secoedXwpfTable.GetRow(0).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "性别", ParagraphAlignment.CENTER, 40, true));
                    secoedXwpfTable.GetRow(0).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "年龄", ParagraphAlignment.CENTER, 40, true));
                    secoedXwpfTable.GetRow(0).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "综合评分", ParagraphAlignment.CENTER, 40, true));

                    //遍历四条数据
                    for (var i = 1; i < 5; i++)
                    {
                        secoedXwpfTable.GetRow(i).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "小明" + i + "号", ParagraphAlignment.CENTER, 40, false));
                        secoedXwpfTable.GetRow(i).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, "男", ParagraphAlignment.CENTER, 40, false));
                        secoedXwpfTable.GetRow(i).GetCell(2).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, 20 + i + "岁", ParagraphAlignment.CENTER, 40, false));
                        secoedXwpfTable.GetRow(i).GetCell(3).SetParagraph(SetTableParagraphInstanceSetting(document, firstXwpfTable, 90 + i + "分", ParagraphAlignment.CENTER, 40, false));

                        checkPeopleNum++;
                        totalScore += 90 + i;
                    }

                    #endregion

                    #region 文档第三个表格对象实例
                    //创建文档中的表格对象实例
                    XWPFTable thirdXwpfTable = document.CreateTable(5, 4);//显示的行列数rows:5行,cols:4列
                    thirdXwpfTable.Width = 5200;//总宽度
                    thirdXwpfTable.SetColumnWidth(0, 1300); /* 设置列宽 */
                    thirdXwpfTable.SetColumnWidth(1, 1100); /* 设置列宽 */
                    thirdXwpfTable.SetColumnWidth(2, 1400); /* 设置列宽 */
                    thirdXwpfTable.SetColumnWidth(3, 1400); /* 设置列宽 */
                    //Table 表格第一行，后面的合并3列(注意关于表格中行合并问题，先合并，后填充内容)
                    thirdXwpfTable.GetRow(0).MergeCells(0, 3);//从第一列起,合并3列
                    thirdXwpfTable.GetRow(0).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "检查内容: " +
                        $"于{checkTime}下午检查了追逐时光企业员工培训考核并对员工的相关信息进行了相关统计，统计结果如下：                                                                                                                                                                                                                " +
                        "-------------------------------------------------------------------------------------" +
                        $"共对该企业（{checkPeopleNum}）人进行了培训考核，培训考核总得分为（{totalScore}）分。 " + "", ParagraphAlignment.LEFT, 30, false));


                    //Table 表格第二行
                    thirdXwpfTable.GetRow(1).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "检查结果: ", ParagraphAlignment.CENTER, 40, true));
                    thirdXwpfTable.GetRow(1).MergeCells(1, 3);//从第二列起，合并三列
                    thirdXwpfTable.GetRow(1).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "该企业非常优秀，坚持每天学习打卡，具有蓬勃向上的活力。", ParagraphAlignment.LEFT, 40, false));

                    //Table 表格第三行
                    thirdXwpfTable.GetRow(2).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "处理结果: ", ParagraphAlignment.CENTER, 40, true));
                    thirdXwpfTable.GetRow(2).MergeCells(1, 3);
                    thirdXwpfTable.GetRow(2).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "通过检查，评分为优秀！", ParagraphAlignment.LEFT, 40, false));

                    //Table 表格第四行，后面的合并3列(注意关于表格中行合并问题，先合并，后填充内容),额外说明
                    thirdXwpfTable.GetRow(3).MergeCells(0, 3);//合并3列
                    thirdXwpfTable.GetRow(3).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "备注说明: 记住，坚持就是胜利，永远保持一种求知，好问的心理！", ParagraphAlignment.LEFT, 30, false));

                    //Table 表格第五行
                    thirdXwpfTable.GetRow(4).MergeCells(0, 1);
                    thirdXwpfTable.GetRow(4).GetCell(0).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "                                                                                                                                                                                                 检查人员签名：              年 月 日", ParagraphAlignment.LEFT, 40, false));
                    thirdXwpfTable.GetRow(4).MergeCells(1, 2);

                    thirdXwpfTable.GetRow(4).GetCell(1).SetParagraph(SetTableParagraphInstanceSetting(document, thirdXwpfTable, "                                                                                                                                                                                                 企业法人签名：              年 月 日", ParagraphAlignment.LEFT, 40, false));


                    #endregion

                    //向文档流中写入内容，生成word
                    document.Write(stream);

                    savePath = "/SaveWordFile/" + currentDate + "/" + fileName;
                    MessageBox.Show("导出成功","提示");


                }
            }
            catch (Exception ex)
            {
                //ignore
                savePath = ex.Message;

            }
        }

        /// <summary>
        /// 测试导出Excel
        /// </summary>
        //static void aExcel()
        //{

        //    var newFile = @"Test\test.xlsx";

        //    using (var fs = new FileStream(newFile, FileMode.Create, FileAccess.Write))
        //    {

        //        IWorkbook workbook = new XSSFWorkbook();

        //        ISheet sheet1 = workbook.CreateSheet("公司信息表");
        //        sheet1.DisplayGuts = true;
        //        IRow row = sheet1.CreateRow(0);

        //        var style1 = workbook.CreateCellStyle();
        //        style1.FillForegroundColor = HSSFColor.Tan.Index;
        //        style1.FillPattern = FillPattern.SolidForeground;
        //        style1.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
        //        sheet1.SetColumnWidth(0, 20 * 256);
        //        sheet1.SetColumnWidth(1, 15 * 256);
        //        sheet1.SetColumnWidth(2, 15 * 256);
        //        sheet1.SetColumnWidth(3, 15 * 256);
        //        sheet1.SetColumnWidth(4, 30 * 256);

        //        var
        //        cell1 = row.CreateCell(0); cell1.CellStyle = style1; cell1.SetCellValue("名称");
        //        cell1 = row.CreateCell(1); cell1.CellStyle = style1; cell1.SetCellValue("地址");
        //        cell1 = row.CreateCell(2); cell1.CellStyle = style1; cell1.SetCellValue("联系方式");
        //        cell1 = row.CreateCell(3); cell1.CellStyle = style1; cell1.SetCellValue("法人");
        //        cell1 = row.CreateCell(4); cell1.CellStyle = style1; cell1.SetCellValue("注册日期");

        //        CompanySQLiteDao companySQLiteDao = new CompanySQLiteDao();
        //        IList<CompanyModel> companyModel = companySQLiteDao.Qurey();

        //        for (int i = 0; i < companyModel.Count; i++)
        //        {
        //            IRow row1 = sheet1.CreateRow(i+1);
        //            var
        //            cell = row1.CreateCell(0); cell.CellStyle = style1; cell.SetCellValue(companyModel[i].Name);
        //            cell = row1.CreateCell(1); cell.CellStyle = style1; cell.SetCellValue(companyModel[i].Address);
        //            cell = row1.CreateCell(2); cell.CellStyle = style1; cell.SetCellValue(companyModel[i].Telephone);
        //            cell = row1.CreateCell(3); cell.CellStyle = style1; cell.SetCellValue(companyModel[i].LegalPerson);
        //            cell = row1.CreateCell(4); cell.CellStyle = style1; cell.SetCellValue(companyModel[i].RegistrationDate); 
        //        }
        //        workbook.Write(fs);

        //    }
        //}

        /// <summary>
        /// 测试插入数据
        /// </summary>
       
        void Insert()
        {
            CompanyModel companyModel = new CompanyModel();
            for (int i = 0; i < 100; i++)
            {
                companyModel.Name = "DingSheng" + i.ToString();
                companyModel.Address = "LiaoNingHuLudao";
                companyModel.Telephone = "0430-22223333";
                companyModel.LegalPerson = "MiaoMiao";
                companyModel.RegistrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                CompanySQLiteDao companySQLiteDao = new CompanySQLiteDao();

                companySQLiteDao.Insert(companyModel);
            }

        }
        /// <summary>
        /// 加载数据库内容
        /// </summary>
        void Reloade()
        {
            CompanySQLiteDao companySQLiteDao = new CompanySQLiteDao();
            IList<CompanyModel> companyModel = companySQLiteDao.Qurey(2);

            CompanyData.Clear();
            for (int i = 0; i < companyModel.Count; i++)
            {
                CompanyModel companyDataTemp = new CompanyModel();
                companyDataTemp.Number = i + 1;
                companyDataTemp.Name = companyModel[i].Name;
                companyDataTemp.Address = companyModel[i].Address;
                companyDataTemp.Telephone = companyModel[i].Telephone;
                companyDataTemp.LegalPerson = companyModel[i].LegalPerson;
                companyDataTemp.RegistrationDate = companyModel[i].RegistrationDate;
                CompanyData.Add(companyDataTemp);
            }
        }

        /// <summary>
        /// office读取Excel文件数据
        /// </summary>
        void MOImportExcel()
        {
            try
            {
                string openFilePath = "";                                //导出时文件的路径
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "Excel文件|*.xlsx;*.xls";           //文件名筛选字符串
                openDialog.ShowDialog();
                openFilePath = openDialog.FileName;

                //Driver={Driver do Microsoft Excel(*.xls)} 这种连接写法不需要创建一个数据源DSN，DRIVERID表示驱动ID，Excel2003后都使用790，FIL表示Excel文件类型，Excel2007用excel 8.0，MaxBufferSize表示缓存大小，DBQ表示读取Excel的文件名（全路径）
                //连接语句，读取文件路径
                string strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + openFilePath + ";Extended Properties='Excel 12.0; HDR=Yes; IMEX=1'";

                string sheetName = "公司详细信息";
                //string strExcel = "select * from [遥控$]";
                // 查询Excel表名，默认是Sheet1
                string strExcel = "select * from [" + sheetName.Replace('.', '#') + "$]";
                OleDbConnection ole = new OleDbConnection(strConn);
                // 打开连接
                ole.Open();
                System.Data.DataTable schemaTable = new System.Data.DataTable();
                OleDbDataAdapter odp = new OleDbDataAdapter(strExcel, strConn);
                odp.Fill(schemaTable);
                ole.Close();

                for (int i = 0; i < schemaTable.Rows.Count; i++)
                {
                    CompanyModel companyModel = new CompanyModel();
                    companyModel.Name = schemaTable.Rows[i][0].ToString();
                    companyModel.Address = schemaTable.Rows[i][1].ToString();
                    companyModel.Telephone = schemaTable.Rows[i][2].ToString();
                    companyModel.LegalPerson = schemaTable.Rows[i][3].ToString();
                    companyModel.RegistrationDate = schemaTable.Rows[i][4].ToString();
                    CompanyData.Add(companyModel);
                }
                // return schemaTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"提示");
            }
            
        }


        /// <summary>
        /// office导出Excel文件数据
        /// </summary>
        
        void M0ExportExcel()
        {
            string primaryname = "Office导出Excel";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel (*.XLSX)|*.xlsx";

            saveFileDialog.AddExtension = true;     // 自动添加扩展名
            saveFileDialog.OverwritePrompt = true;  // 文件已存在是否提示覆盖
            saveFileDialog.CheckPathExists = true;  // 提示输入的文件名无效
            saveFileDialog.FileName = primaryname;  // 文件初始名

            if (saveFileDialog.ShowDialog() == true)
            {
                // 首先模拟建立将要导出的数据,这些数据都存于DataTable中  
                System.Data.DataTable dtEx = new System.Data.DataTable();
                dtEx.Columns.Add("Name", typeof(string));
                dtEx.Columns.Add("Address", typeof(string));
                dtEx.Columns.Add("Telephone", typeof(string));
                dtEx.Columns.Add("LegalPerson", typeof(string));
                dtEx.Columns.Add("RegistrationDate", typeof(string));
                DataRow row;

                Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
                // 创建工作簿(WorkBook:即Excel文件主体本身)
                Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);
                excelWB.Worksheets.Add();
                excelWB.Worksheets.Add();
                #region 导出表1
                // 创建工作表(即Excel里的子表sheet) 1表示在子表sheet1里进行数据导出
                Worksheet excelWS1 = (Worksheet)excelWB.Worksheets[1];
                excelWS1.Columns.ColumnWidth = 30;
                excelWS1.Name = "云熙";
                // 如果数据中存在数字类型,可以让它变文本格式显示
                excelWS1.Cells.NumberFormat = "@";
                // 创建Excel  
                excelWS1.Cells[1, 1] = "名称";
                excelWS1.Cells[1, 2] = "地址";
                excelWS1.Cells[1, 3] = "联系方式";
                excelWS1.Cells[1, 4] = "法人";
                excelWS1.Cells[1, 5] = "注册时间";
                dtEx.Clear();

                // 读取数据
                for (int i = 0; i < CompanyData.Count; i++)
                {
                    row = dtEx.NewRow();
                    row["Name"] = CompanyData[i].Name;
                    row["Address"] = CompanyData[i].Address;
                    row["Telephone"] = CompanyData[i].Telephone;
                    row["LegalPerson"] = CompanyData[i].LegalPerson;
                    row["RegistrationDate"] = CompanyData[i].RegistrationDate;
                    dtEx.Rows.Add(row);
                }
                // 将数据导入到工作表的单元格  
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    for (int j = 0; j < dtEx.Columns.Count; j++)
                    {
                        // Excel单元格第一个从索引1开始
                        excelWS1.Cells[i + 2, j + 1] = dtEx.Rows[i][j].ToString();
                    }
                }
                #endregion

                #region 导出表2
                // 创建工作表(即Excel里的子表sheet) 1表示在子表sheet1里进行数据导出
                Worksheet excelWS2 = (Worksheet)excelWB.Worksheets[2];
                excelWS2.Name = "云哲";
                excelWS2.Columns.ColumnWidth = 30;
                // 如果数据中存在数字类型,可以让它变文本格式显示
                excelWS2.Cells.NumberFormat = "@";
                // 创建Excel  
                excelWS2.Cells[1, 1] = "名称";
                excelWS2.Cells[1, 2] = "地址";
                excelWS2.Cells[1, 3] = "联系方式";
                excelWS2.Cells[1, 4] = "法人";
                excelWS2.Cells[1, 5] = "注册时间";
                dtEx.Clear();

                // 读取数据
                for (int i = 0; i < CompanyData.Count; i++)
                {
                    row = dtEx.NewRow();
                    row["Name"] = CompanyData[i].Name;
                    row["Address"] = CompanyData[i].Address;
                    row["Telephone"] = CompanyData[i].Telephone;
                    row["LegalPerson"] = CompanyData[i].LegalPerson;
                    row["RegistrationDate"] = CompanyData[i].RegistrationDate;
                    dtEx.Rows.Add(row);
                }
                // 将数据导入到工作表的单元格  
                for (int i = 0; i < dtEx.Rows.Count; i++)
                {
                    for (int j = 0; j < dtEx.Columns.Count; j++)
                    {
                        // Excel单元格第一个从索引1开始
                        excelWS2.Cells[i + 2, j + 1] = dtEx.Rows[i][j].ToString();
                    }
                }


                #endregion
                // 保存表格
                // 将其进行保存到指定的路径
                excelWB.SaveAs(saveFileDialog.FileName);
                excelWB.Close();
                excelApp.Quit();
                MessageBox.Show("导出成功", "提示");
            }

        }


        /// <summary>
        /// office导出word
        /// </summary>
       
        void MOExportWord()
        {
            try
            {
                string primaryname = "Office导出Word";
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.DefaultExt = ".docx";
                saveFileDialog.Filter = "Word文档|*.doc;*.docx";
                // 是否自动添加扩展名
                saveFileDialog.AddExtension = true;
                // 文件已存在是否提示覆盖
                saveFileDialog.OverwritePrompt = true;
                // 提示输入的文件名无效
                saveFileDialog.CheckPathExists = true;
                // 文件初始名
                saveFileDialog.FileName = primaryname;
                if (saveFileDialog.ShowDialog() == true)
                {
                    // 文件保存的路径
                    object filename = saveFileDialog.FileName;
                    Object Nothing = Missing.Value;


                    // 创建Word文档
                    Microsoft.Office.Interop.Word.Application WordApp = new Microsoft.Office.Interop.Word.Application();
                    Microsoft.Office.Interop.Word.Document WordDoc = WordApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                    // 添加页眉                    
                    // 设置右对齐
                    WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    WordApp.ActiveWindow.View.Type = WdViewType.wdOutlineView;
                    WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekPrimaryHeader;

                    WordApp.ActiveWindow.ActivePane.Selection.InsertAfter("辽宁省沈阳市市政");


                    // 跳出页眉设置
                    WordApp.ActiveWindow.View.SeekView = WdSeekView.wdSeekMainDocument;
                    // 设置文档的行间距
                    WordApp.Selection.ParagraphFormat.LineSpacing = 15f;

                    // 移动焦点并换行
                    object count = 20;
                    // 换一行
                    object WdLine = Microsoft.Office.Interop.Word.WdUnits.wdLine;
                    object unite = Microsoft.Office.Interop.Word.WdUnits.wdStory;
                    // 移动焦点
                    // WordApp.Selection.MoveDown(ref WdLine, ref count, ref Nothing);
                    // 光标移动到未尾
                    WordApp.Selection.EndKey(ref unite, ref Nothing);

                    // 插入段落
                    Microsoft.Office.Interop.Word.Paragraph para = WordDoc.Content.Paragraphs.Add(ref Nothing);
                    int BigFontSize = 14;
                    int NormalFontSize = 10;
                    int FontBoldTrue = 2;
                    int FontBoldFalse = 0;

                    // 加入测试模块内容
                    para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;//对齐方式 
                    para.Range.Text = "公司注册信息表";
                    para.Range.Font.Size = BigFontSize;
                    para.Range.Font.Bold = FontBoldTrue;
                    para.Range.Font.Color = WdColor.wdColorRed;
                    para.Range.Font.Italic = 0;
                    para.Range.InsertParagraphAfter(); ;

                    WordApp.Selection.EndKey(ref unite, ref Nothing);
                    para.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;//对齐方式 
                    para.Range.Text = "表1 所有注册公司信息";
                    para.Range.Font.Size = NormalFontSize;
                    para.Range.Font.Bold = FontBoldFalse;
                    para.Range.Font.Color = WdColor.wdColorBlack;
                    para.Range.InsertParagraphAfter();

                    WordApp.Selection.EndKey(ref unite, ref Nothing);

                    // 文档中创建表格
                    Microsoft.Office.Interop.Word.Table newTable = WordDoc.Tables.Add(WordApp.Selection.Range, CompanyData.Count, 5, ref Nothing, ref Nothing);
                    //设置表格样式
                    newTable.Borders.OutsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    newTable.Borders.InsideLineStyle = Microsoft.Office.Interop.Word.WdLineStyle.wdLineStyleSingle;
                    // 垂直居中
                    WordApp.Selection.Cells.VerticalAlignment = Microsoft.Office.Interop.Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                    // 水平居中
                    WordApp.Selection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    // 填充表格内容
                    newTable.Cell(1, 1).Range.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorLightBlue;
                    newTable.Cell(1, 1).Range.Text = "名称";
                    newTable.Cell(1, 2).Range.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorLightBlue;
                    newTable.Cell(1, 2).Range.Text = "地址";
                    newTable.Cell(1, 3).Range.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorLightBlue;
                    newTable.Cell(1, 3).Range.Text = "联系方式";
                    newTable.Cell(1, 4).Range.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorLightBlue;
                    newTable.Cell(1, 4).Range.Text = "法人";
                    newTable.Cell(1, 5).Range.Shading.BackgroundPatternColor = Microsoft.Office.Interop.Word.WdColor.wdColorLightBlue;
                    newTable.Cell(1, 5).Range.Text = "注册日期";

                    for (int j = 0; j < CompanyData.Count; j++)
                    {
                        newTable.Cell(j + 2, 1).Range.Text = CompanyData[j].Name;
                        newTable.Cell(j + 2, 2).Range.Text = CompanyData[j].Address;
                        newTable.Cell(j + 2, 3).Range.Text = CompanyData[j].Telephone;
                        newTable.Cell(j + 2, 4).Range.Text = CompanyData[j].LegalPerson;
                        newTable.Cell(j + 2, 5).Range.Text = CompanyData[j].RegistrationDate;
                    }
                    para.Range.InsertParagraphAfter();



                    // 文件保存
                    WordDoc.SaveAs(ref filename, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                    WordDoc.Close(ref Nothing, ref Nothing, ref Nothing);
                    WordApp.Quit(ref Nothing, ref Nothing, ref Nothing);

                    MessageBox.Show("生成报表成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }


        /// <summary>
        /// itextsharp创建PDF
        /// </summary>
     
        private void ItxetsharpCreatePdf()
        {
            string primaryname = "itxetsharp导出PDF";
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = ".pdf";
            saveFileDialog.Filter = "PDF文档|*.pdf";
            // 是否自动添加扩展名
            saveFileDialog.AddExtension = true;
            // 文件已存在是否提示覆盖
            saveFileDialog.OverwritePrompt = true;
            // 提示输入的文件名无效
            saveFileDialog.CheckPathExists = true;
            // 文件初始名
            saveFileDialog.FileName = primaryname;
            saveFileDialog.ShowDialog();

            //创建一个pdf文档的对象，设置纸张大小为A4，页边距为0
            iTextSharp.text.Document docPDF = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4);
            //PageSize.A4.Rotate();当需要把PDF纸张设置为横向时，使用PageSize.A4.Rotate()
            PdfWriter write = PdfWriter.GetInstance(docPDF, new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate));
            //创建一个写入PDF的对象， 
            //在这里需要注意的是，itextsharp不支持中文字符，想要显示中文字符的话需要自己设置字体 
            BaseFont bsFont = BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            iTextSharp.text.Font font = new iTextSharp.text.Font(bsFont);



            docPDF.Open();//打开

            // 将测试结果写入文档中
            write.PageEvent = new GeneratePDFService();

            #region 页眉与页脚
            GeneratePDFService.PAGE_NUMBER = true;
            GeneratePDFService.tpl = write.DirectContent.CreateTemplate(500, 500);
            GeneratePDFService.HeaderLeft = "www.baidu.com";
            GeneratePDFService.HeaderRight = "xxxxxxxx公司";
            GeneratePDFService.FooterRight = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            #endregion 页眉与页脚

            // 设置字体
            GeneratePDFService.SetFont(BaseColor.BLACK, "黑体", 24, iTextSharp.text.Font.BOLD);
            docPDF.Add(GeneratePDFService.AddParagraph("状态序列测试报告", 1, 10, 0, 2f)); ;


            //将一句短语写入PDF中
            docPDF.Add(new iTextSharp.text.Paragraph("第一个PDF文件", font));
            docPDF.Add(new iTextSharp.text.Paragraph("        ", font));
            docPDF.Add(new iTextSharp.text.Paragraph("        ", font));
            docPDF.Add(new iTextSharp.text.Paragraph("        ", font));

            PdfPTable tablerow1 = new PdfPTable(new float[] { 20, 20, 20 ,20});
            //表格
            for (int i = 0; i <= 36; i++)  //表示创建一个4列9行的表格
            {
                PdfPCell cell = new PdfPCell(new iTextSharp.text.Paragraph(i.ToString(), font));

                // tablerow1.AddCell 
                tablerow1.AddCell(cell);//将单元格添加到表格中

            }
            docPDF.Add(tablerow1);//将表格添加到pdf文档中
            docPDF.Close();//关闭
            MessageBox.Show("导出成功", "提示");
        }

        /// <summary>
        /// 测试命令
        /// </summary>
        public RelayCommand<string> TestCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// 测试命令执行操作
        /// </summary>
        /// <param name="arg">参数</param>
        private void ExecuteTestCommand(string arg)
        {
            switch (arg)
            {
                case "N:E导入":
                    NPOIImportExecel();//导入
                    break;
                case "N:E导出":
                    NPOIExportEexcel();//导出
                    break;
                case "N:W导出":
                    NPOIExportWorld(); //导出
                    break;
                case "S:装载":
                    Reloade();
                    break;
                case "S:清空":
                    CompanyData.Clear();
                    break;
                case "M:E导入":
                    MOImportExcel();//导入
                    break;
                case "M:E导出":
                    M0ExportExcel();//导出                   
                    break;
                case "M:W导出":
                    MOExportWord();//导出
                    break;
                case "I:P导出":
                    ItxetsharpCreatePdf();//导出
                    break;
                default:
                    break;
            }
        }

         
        public CompanyViewModel()
        {
            CompanyData = new ObservableCollection<CompanyModel>();
            // 绑定保护模型测试命令执行操作
            TestCommand = new RelayCommand<string>(ExecuteTestCommand);
        }
    }
}
