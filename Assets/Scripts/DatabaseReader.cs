using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static UnityEngine.Rendering.DebugUI.Table;

public class CSVReader : MonoBehaviour
{
    public TextAsset csvFile;

    public List<string> headers = new List<string>();
    public List<Dictionary<string, string>> rowData = new List<Dictionary<string, string>>();

    public void ReadCSV()
    {
        rowData.Clear();
        headers.Clear();

        if (csvFile == null)
        {
            Debug.LogWarning("CSV file not assigned.");
            return;
        }

        StringReader reader = new StringReader(csvFile.text);
        string line;

        // 读取表头
        if ((line = reader.ReadLine()) != null)
        {
            headers.AddRange(line.Split(','));
        }

        // 读取内容
        while ((line = reader.ReadLine()) != null)
        {
            string[] values = line.Split(',');
            Dictionary<string, string> entry = new Dictionary<string, string>();

            Debug.Log("Headers: " + string.Join(",", headers));

            for (int i = 0; i < headers.Count && i < values.Length; i++)
            {
                entry[headers[i]] = values[i];
            }

            rowData.Add(entry);
        }

    }

    // ✅ 根据列名和行号获取值
    public string GetValue(int rowIndex, string columnName)
    {
        if (rowIndex >= 0 && rowIndex < rowData.Count && rowData[rowIndex].ContainsKey(columnName))
        {
            return rowData[rowIndex][columnName];
        }
        return null;
    }

    // ✅ 获取某列的所有行（用于绘制柱状图）
    public List<float> GetColumnValues(string columnName)
    {
        Debug.Log("游戏开始");
        
        List<float> values = new List<float>();

        foreach (var row in rowData)
        {
            Debug.Log("🧵 当前 row 的所有 key：" + string.Join(", ", row.Keys));
            if (row.ContainsKey(columnName))
            {
                string raw = row[columnName];
                Debug.Log($"🔍 原始值: {raw}");

                if (float.TryParse(row[columnName], out float val))
                {
                    values.Add(val);
                    Debug.Log($"✅ 转换成功: {val}");
                }
                else
                {
                    Debug.LogWarning($"❌ 转换失败: {raw}");
                    values.Add(0); // 或者跳过，或用 NaN
                }
            }
        }

        return values;
    }

}