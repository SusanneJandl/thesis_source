using ClosedXML.Excel;
using System.Collections.Generic;

namespace ExcelCreator
{
    public static class ExcelWriter
    {
        public static void Write(string outputPath, List<Values> values)
        {
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Results");

                int col = 1;

                // ===== HEADER =====

                // Identification
                sheet.Cell(1, col++).Value = "FilePath";
                sheet.Cell(1, col++).Value = "QuestionNo";
                sheet.Cell(1, col++).Value = "Language";
                sheet.Cell(1, col++).Value = "Accuracy";

                // Tokens
                sheet.Cell(1, col++).Value = "Tokens";

                // Timings
                sheet.Cell(1, col++).Value = "Context";
                sheet.Cell(1, col++).Value = "Stream";
                sheet.Cell(1, col++).Value = "ToEn";
                sheet.Cell(1, col++).Value = "ToDe";
                sheet.Cell(1, col++).Value = "GenerationOrAnswerEn";
                sheet.Cell(1, col++).Value = "Total";

                // RAM Total
                sheet.Cell(1, col++).Value = "RamTotalMin";
                sheet.Cell(1, col++).Value = "RamTotalMax";
                sheet.Cell(1, col++).Value = "RamTotalAvg";

                // RAM C#
                sheet.Cell(1, col++).Value = "RamCSharpMin";
                sheet.Cell(1, col++).Value = "RamCSharpMax";
                sheet.Cell(1, col++).Value = "RamCSharpAvg";

                // RAM Python (Flask / Langflow)
                sheet.Cell(1, col++).Value = "RamPythonMin";
                sheet.Cell(1, col++).Value = "RamPythonMax";
                sheet.Cell(1, col++).Value = "RamPythonAvg";

                // RAM Ollama
                sheet.Cell(1, col++).Value = "RamOllamaMin";
                sheet.Cell(1, col++).Value = "RamOllamaMax";
                sheet.Cell(1, col++).Value = "RamOllamaAvg";

                // ===== DATA =====

                int row = 2;

                foreach (Values v in values)
                {
                    col = 1;

                    // Identification
                    sheet.Cell(row, col++).Value = v.filePath;
                    sheet.Cell(row, col++).Value = v.questionNo;
                    sheet.Cell(row, col++).Value = v.language;

                    if (v.accuracy.HasValue)
                        sheet.Cell(row, col).Value = v.accuracy.Value;
                    col++;

                    // Tokens
                    if (v.tokens > 0)
                        sheet.Cell(row, col).Value = v.tokens;
                    col++;

                    // Timings
                    sheet.Cell(row, col++).Value = v.context.value;

                    if (v.stream != null)
                        sheet.Cell(row, col).Value = v.stream.value;
                    col++;

                    if (v.toEn != null)
                        sheet.Cell(row, col).Value = v.toEn.value;
                    col++;

                    if (v.toDe != null)
                        sheet.Cell(row, col).Value = v.toDe.value;
                    col++;

                    if (v.generation != null)
                        sheet.Cell(row, col).Value = v.generation.value;
                    col++;

                    sheet.Cell(row, col++).Value = v.total.value;

                    // RAM Total
                    sheet.Cell(row, col++).Value = v.ramTotal.min;
                    sheet.Cell(row, col++).Value = v.ramTotal.max;
                    sheet.Cell(row, col++).Value = v.ramTotal.avg;

                    // RAM C#
                    sheet.Cell(row, col++).Value = v.ramCSharp.min;
                    sheet.Cell(row, col++).Value = v.ramCSharp.max;
                    sheet.Cell(row, col++).Value = v.ramCSharp.avg;

                    // RAM Python
                    sheet.Cell(row, col++).Value = v.ramPython.min;
                    sheet.Cell(row, col++).Value = v.ramPython.max;
                    sheet.Cell(row, col++).Value = v.ramPython.avg;

                    // RAM Ollama
                    if (v.ramOllama != null)
                    {
                        sheet.Cell(row, col++).Value = v.ramOllama.min;
                        sheet.Cell(row, col++).Value = v.ramOllama.max;
                        sheet.Cell(row, col++).Value = v.ramOllama.avg;
                    }
                    else
                    {
                        col += 3; // skip columns
                    }

                    row++;
                }

                sheet.Columns().AdjustToContents();
                workbook.SaveAs(outputPath);
            }
        }
    }
}
