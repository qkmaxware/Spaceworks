using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {
    /// <summary>
    /// Represents a 2D matrix of boolean values
    /// </summary>
    public class BoolMatrix {

        [SerializeField]
        private bool[,] mtx;
        [SerializeField]
        private List<string> rowLabels = new List<string>();
        [SerializeField]
        private List<string> columnLabels = new List<string>();

        /// <summary>
        /// Rows in this matrix
        /// </summary>
        /// <returns></returns>
        public int rows { get; private set; }
        /// <summary>
        /// Columns in this matrix
        /// </summary>
        /// <returns></returns>
        public int columns { get; private set; }

        /// <summary>
        /// Index this matrix by numerical indices
        /// </summary>
        /// <returns></returns>
        public bool this[int x, int y] {
            get {
                return mtx[x, y];
            }
            set {
                mtx[x, y] = value;
            }
        }

        /// <summary>
        /// Index this matrix by column names
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get the name of a row at index
        /// </summary>
        /// <param name="i">row index</param>
        /// <returns>row name</returns>
        public string row(int i) {
            return rowLabels[i];
        }

        /// <summary>
        /// Get the name of a column at index
        /// </summary>
        /// <param name="i">column index</param>
        /// <returns>column name</returns>
        public string column(int i) {
            return columnLabels[i];
        }

        /// <summary>
        /// Create matrix of a given size
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
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

        /// <summary>
        /// Create matrix from a given 2d bool array
        /// </summary>
        /// <param name="mtx"></param>
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

        /// <summary>
        /// Create a matrix from row and column labels
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        public BoolMatrix(string[] rows, string[] columns) {
            this.rows = rows.Length;
            this.columns = columns.Length;
            mtx = new bool[this.rows, this.columns];
            this.rowLabels.AddRange(rows);
            this.columnLabels.AddRange(columns);
        }

        /// <summary>
        /// Add a row with a label
        /// </summary>
        /// <param name="label"></param>
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

        /// <summary>
        /// Add a column with a label
        /// </summary>
        /// <param name="label"></param>
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

        /// <summary>
        /// Remove a row
        /// </summary>
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

        /// <summary>
        /// Remove a column
        /// </summary>
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