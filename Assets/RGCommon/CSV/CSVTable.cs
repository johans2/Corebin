using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

namespace RGCommon {

    /// Parse a simple csv-file where row0 contains headers
    /// then access data using myCsvTable.GetRow(2).GetColumn("someHeader");
    /// or typed: myCsvTable.GetRow(2).GetColumnInt("someOtherHeader");
    public class CSVTable {

        public struct Row {

            private int index;
            private Dictionary<string, int> headerIndexes;
            private string[] columns;

            internal Row(int index, Dictionary<string, int> headerIndexes, string[] columns) {
                this.index = index;
                this.headerIndexes = headerIndexes;
                this.columns = columns;
            }


            private void ValidateHeader(string header) {
                if(!headerIndexes.ContainsKey(header)) {
                    throw new KeyNotFoundException("CSVTable.GetRow(" + index + ").GetColumn(" + header + "), header name [" + header + "] invalid!");
                }
            }
            private void ValidateIndex(int i) {
                if(i < 0 || i > columns.Length - 1) {
                    throw new IndexOutOfRangeException("CSVTable.GetRow(" + index + ").GetColumn(" + i + "), column index [" + i + "] out of range!");
                }
            }


            public bool HasValue(int i) {
                ValidateIndex(i);
                return !String.IsNullOrEmpty(columns[i]);
            }
            public bool NotEmpty(string header) {
                ValidateHeader(header);
                return HasValue(headerIndexes[header]);
            }

            public string GetColumn(int i) {
                ValidateIndex(i);
                return columns[i];
            }

            public string GetColumn(string header) {
                ValidateHeader(header);
                return GetColumn(headerIndexes[header]);
            }

            public int GetColumnInt(string header) {
                return Int32.Parse(GetColumn(header));
            }

            public int GetColumnInt(int i) {
                return Int32.Parse(GetColumn(i));
            }

            public float GetColumnFloat(string header) {
                return float.Parse(GetColumn(header));
            }

            public float GetColumnFloat(int i) {
                return float.Parse(GetColumn(i));
            }

            public bool GetColumnBool(string header) {
                return GetColumnBool(headerIndexes[header]);
            }

            public bool GetColumnBool(int i) {
                string s = GetColumn(i).ToLower();
                return s == "true" || s == "yes";
            }


            public int Count { get { return columns.Length; } }

            public int Index() { return index; }

            override public string ToString() {
                string s = "Row " + index + ":: ";
                foreach (var c in columns)
                    s += c + ",";
                return s;
            }
        }


        private string path;
        private bool isLoaded;
        private Row[] rows;
        private string[] headers;
        private Dictionary<string, int> headerIndexes;


        public void LoadFromResources(string path) {

            if (path.EndsWith(".csv")) {
                throw new FileLoadException("Error opening file " + path + " from CSVTable, should not end in .csv when loading from resources");
            }

            this.path = path;
            string rawText;
            try {
                TextAsset txt = (TextAsset)Resources.Load(path, typeof(TextAsset));
                rawText = txt.text;
            }
            catch(Exception ex) {
                throw new FileLoadException("Could not load CSV file from " + path, ex);
            }

            string[] rowsText = rawText.Split('\n');

            headers = rowsText[0].Trim('\r').Split(',');

            headerIndexes = new Dictionary<string, int>(headers.Length);
            for (int i=0; i<headers.Length; i++) {
                headerIndexes[headers[i]] = i;
            }

            rows = new Row[rowsText.Length-1];

            for(int i = 1; i < rowsText.Length; i++) {
                if(rowsText[i] == "") {
                    continue;
                }

                string[] fields = rowsText[i].Trim('\r').Split(',');

                // Special case if field contains a , then the text will be wrapped in quotes "...".
                // since we split on "," we will have text elements in the array looking like
                // original: [abc, "def,ghi,jkl", mno] (3 elements)
                // after split: [abc, "def, ghi, jkl", mno] (5 elements)
                // if element starts with quotation mark we stitch it together again until
                // we reach final quotation mark

                if(rowsText[i].Contains("\"")) { // We only need to run this block if the un-splitted text contains a quotation mark somewhere
                    List<string> fieldsList = new List<string>(fields.Length);
                    bool openQuote = false;
                    string quotedText = "";

                    for(int f = 0; f < fields.Length; f++) {

                        string s = fields[f];

                        if(!openQuote) {
                            if(s.StartsWith("\"")) { // opening quote '"...'
                                openQuote = true;
                                quotedText = s.Substring(1) + ",";
                            } else {
                                fieldsList.Add(s); // just add text unmodified
                            }
                        }
                        else if (openQuote) {
                            if (s.EndsWith("\"")) { // closing quote '..."'
                                quotedText += s.Substring(0, s.Length - 1);
                                openQuote = false;

                                fieldsList.Add(quotedText);
                                quotedText = "";
                            } else {
                                quotedText += s + ",";
                            }
                        }
                    }

                    fields = fieldsList.ToArray();
                }


                Row row = new Row(i - 1, headerIndexes, fields);
                rows[i - 1] = row;
            }

            isLoaded = true;
        }

        public string GetHeader(int i) {
            if (i < 0 || i > headers.Length-1) {
                throw new IndexOutOfRangeException("CSVTable.GetHeader index out of range: " + i);
            }

            return headers[i];
        }

        public int GetHeaderIndex(string name) {
            return headerIndexes[name];
        }

        public bool HasHeader(string name) {
            return headerIndexes.ContainsKey(name);
        }

        public int NumRows {get {return rows.Length-1;}} // -1 as we don't count header row

        public int NumColumns { get {return headers.Length;}}

        public Row GetRow(int i) {
            if(i < 0 || i > rows.Length - 1) {
                throw new IndexOutOfRangeException("CSVTable.GeRow index out of range: " + i);
            }

            return rows[i];
        }

        public override string ToString() {
            return "CSVTable [path='"+path+"', isLoaded: "+isLoaded+"]";
        }
    }
}
