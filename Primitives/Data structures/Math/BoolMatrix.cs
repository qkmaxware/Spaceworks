using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {
    public class BoolMatrix {

        [SerializeField]
        private bool[,] mtx;
        [SerializeField]
        private List<string> rowLabels = new List<string>();
        [SerializeField]
        private List<string> columnLabels = new List<string>();
        public int rows { get; private set; }
        public int columns { get; private set; }

        public bool this[int x, int y] {
            get {
                return mtx[x, y];
            }
            set {
                mtx[x, y] = value;
            }
        }

        public bool this[string sx, string sy] {
            get {
                int x = rowLabels.IndexOf(sx);
                int y = columnLabels.IndexOf(sy);
                if (x == -1 || y == -1)
                    return false;
                return mtx[x, y];
            }
            set {
                int x = rowLabels.IndexOf(sx);
                int y = columnLabels.IndexOf(sy);
                if (x != -1 && y != -1)
                    mtx[x, y] = value;
            }
        }

        public string row(int i) {
            return rowLabels[i];
        }

        public string column(int i) {
            return columnLabels[i];
        }

        public BoolMatrix(int rows, int columns) {
            this.rows = rows;
            this.columns = columns;
            mtx = new bool[rows, columns];
            for (int i = 0; i < rows; i++) {
                rowLabels.Add("" + i);
            }
            for (int i = 0; i < columns; i++) {
                columnLabels.Add("" + i);
            }
        }

        public BoolMatrix(bool[,] mtx) {
            this.mtx = mtx;
            this.rows = mtx.GetLength(0);
            this.columns = mtx.GetLength(1);

            for (int i = 0; i < rows; i++) {
                rowLabels.Add("" + i);
            }
            for (int i = 0; i < columns; i++) {
                columnLabels.Add("" + i);
            }
        }

        public BoolMatrix(string[] rows, string[] columns) {
            this.rows = rows.Length;
            this.columns = columns.Length;
            mtx = new bool[this.rows, this.columns];
            this.rowLabels.AddRange(rows);
            this.columnLabels.AddRange(columns);
        }

        public void PushRow(string label) {
            bool[,] m = new bool[rows + 1, columns];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    m[i, j] = mtx[i, j];
                }
            }
            rows = rows + 1;
            mtx = m;
            rowLabels.Add(label);
        }

        public void PushColumn(string label) {
            bool[,] m = new bool[rows, columns + 1];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns; j++) {
                    m[i, j] = mtx[i, j];
                }
            }
            columns = columns + 1;
            mtx = m;
            rowLabels.Add(label);
        }

        public void PopRow() {
            if (rows < 2)
                return;

            bool[,] m = new bool[rows - 1, columns];
            for (int i = 0; i < rows - 1; i++) {
                for (int j = 0; j < columns; j++) {
                    m[i, j] = mtx[i, j];
                }
            }
            rows = rows - 1;
            mtx = m;
        }

        public void PopColumn() {
            if (columns < 2)
                return;

            bool[,] m = new bool[rows, columns - 1];
            for (int i = 0; i < rows; i++) {
                for (int j = 0; j < columns - 1; j++) {
                    m[i, j] = mtx[i, j];
                }
            }
            columns = columns - 1;
            mtx = m;
        }

    }

}