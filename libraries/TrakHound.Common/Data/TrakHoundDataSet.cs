// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace TrakHound.Data
{
    public class TrakHoundDataSet<TPrimaryKey>
    {
        private readonly string _primaryKeyName;
        private readonly Dictionary<string, int> _columnIndexes = new Dictionary<string, int>();
        private readonly Dictionary<TPrimaryKey, int> _rowIndexes = new Dictionary<TPrimaryKey, int>();
        private TrakHoundDataColumn[] _columns;
        private object[,] _rows;


        public TrakHoundDataColumn[] Columns => _columns;

        public object[,] Rows => _rows;

        public int RowCount => _rows.Length / _columns.Length;

        public int ColumnCount => _columns.Length;


        public TrakHoundDataSet(string primaryKeyId)
        {
            _primaryKeyName = primaryKeyId;

            if (!string.IsNullOrEmpty(primaryKeyId))
            {
                _columnIndexes.Add(primaryKeyId, 0);
                _columns = new TrakHoundDataColumn[1];
                _columns[0] = new TrakHoundDataColumn(0, primaryKeyId);
            }
        }


        public int GetColumnIndex(string columnId)
        {
            if (!string.IsNullOrEmpty(columnId))
            {
                if (_columnIndexes.TryGetValue(columnId, out var index))
                {
                    return index;
                }
            }

            return -1;
        }

        public object GetColumnValue(int rowIndex, string columnId)
        {
            if (!string.IsNullOrEmpty(columnId) && rowIndex < RowCount)
            {
                if (_columnIndexes.TryGetValue(columnId, out var columnIndex))
                {
                    return _rows[rowIndex, columnIndex];
                }
            }

            return null;
        }

        public void AddColumn(string columnId, string columnName = null)
        {
            if (!string.IsNullOrEmpty(columnId))
            {
                if (_columns == null) _columns = new TrakHoundDataColumn[1];

                if (!_columnIndexes.ContainsKey(columnId))
                {
                    var columns = new TrakHoundDataColumn[_columns.Length + 1];
                    for (var i = 0; i < _columns.Length; i++) columns[i] = _columns[i];

                    var index = columns.Length - 1;
                    columns[index] = new TrakHoundDataColumn(index, columnId, columnName);

                    _columns = columns;
                    _columnIndexes.Add(columnId, index);

                    if (_rows != null)
                    {
                        _rows = ResizeArray<object>(_rows, _rows.Length, _columns.Length);
                    }
                }
            }   
        }

        public void ClearColumns()
        {
            _columns = null;
            _columnIndexes.Clear();
        }


        public int? GetRowIndex(TPrimaryKey primaryKeyValue)
        {
            if (primaryKeyValue != null)
            {
                if (_rowIndexes.TryGetValue(primaryKeyValue, out var index))
                {
                    return index;
                }
            }

            return null;
        }

        public TPrimaryKey GetRowPrimaryKey(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < RowCount)
            {
                return (TPrimaryKey)_rows[rowIndex, 0];
            }

            return default;
        }

        public object[] GetRowValues(TPrimaryKey primaryKeyValue)
        {
            if (primaryKeyValue != null)
            {
                if (_rowIndexes.TryGetValue(primaryKeyValue, out var rowIndex))
                {
                    var rowValues = new object[ColumnCount];
                    for (var i = 0; i < ColumnCount; i++)
                    {
                        rowValues[i] = _rows[rowIndex, i];
                    }

                    return rowValues;
                }
            }

            return null;
        }

        public void AddRowValue(TPrimaryKey primaryKeyValue, string columnId, object cellValue)
        {
            var columnIndex = GetColumnIndex(columnId);
            if (columnIndex >= 0)
            {
                var rowIndex = GetRowIndex(primaryKeyValue) ?? -1;
                if (rowIndex >= 0)
                {
                    _rows[rowIndex, columnIndex] = cellValue;
                }
                else
                {
                    if (_rows != null)
                    {
                        var rowCount = _rows.Length / _columns.Length;
                        _rows = ResizeArray<object>(_rows, rowCount + 1, _columns.Length);
                    }
                    else
                    {
                        _rows = new object[1, _columns.Length];
                    }

                    rowIndex = _rows.Length - 1;
                    rowIndex = rowIndex / _columns.Length;

                    _rows[rowIndex, 0] = primaryKeyValue;
                    _rows[rowIndex, columnIndex] = cellValue;

                    _rowIndexes.Add(primaryKeyValue, rowIndex);
                }
            }
        }

        public void ClearRows()
        {
            _rows = null;
            _rowIndexes.Clear();
        }


        public void Sort()
        {
            if (_columns != null && _rows != null)
            {
                var rowCount = _rows.Length / _columns.Length;

                // Create Array of Primary Key Values
                var rowIndexes = new Dictionary<TPrimaryKey, int>(rowCount);
                var sortedKeys = new TPrimaryKey[rowCount];
                for (var i = 0; i < rowCount; i++)
                {
                    rowIndexes.Add((TPrimaryKey)_rows[i, 0], i);
                    sortedKeys[i] = (TPrimaryKey)_rows[i, 0];
                }

                // Sort Primary Keys
                Array.Sort(sortedKeys);

                var buffer = new object[_columns.Length];
                var existingRowIndex = 0;

                for (var i = 0; i < rowCount; i++)
                {
                    // Get Sorted Primary Key Value
                    var key = sortedKeys[i];

                    // Add Row[i] to buffer
                    existingRowIndex = rowIndexes[key];

                    // Get Original Primary Key Value
                    var originalKey = (TPrimaryKey)_rows[i, 0];

                    //existingRowIndex = GetRowIndex(x);
                    for (var j = 0; j < _columns.Length; j++)
                    {
                        buffer[j] = _rows[i, j];
                    }

                    // Swap row cells with one that was added to buffer
                    for (var j = 0; j < _columns.Length; j++)
                    {
                        _rows[i, j] = _rows[existingRowIndex, j];
                    }

                    // Update Existing Row from buffer
                    for (var j = 0; j < _columns.Length; j++)
                    {
                        _rows[existingRowIndex, j] = buffer[j];
                    }

                    // Update Row Index
                    rowIndexes[key] = i;
                    rowIndexes[originalKey] = existingRowIndex;
                }

                //for (var i = 0; i < rowCount; i++)
                //{
                //    // Get Sorted Primary Key Value
                //    var x = primaryKeyValues[i];

                //    // Add Existing Row to buffer
                //    existingRowIndex = GetRowIndex(x);
                //    bufferIndex = 0;
                //    for (var j = 1; j < _columns.Length; j++)
                //    {
                //        buffer[bufferIndex++] = _rows[existingRowIndex, j];
                //    }

                //    // Swap row cells with one that was added to buffer
                //    for (var j = 0; j < _columns.Length; j++)
                //    {
                //        _rows[existingRowIndex, j] = _rows[i, j];
                //    }

                //    // Update New Row (sorted) from buffer
                //    _rows[i, 0] = x;
                //    bufferIndex = 0;
                //    for (var j = 1; j < _columns.Length; j++)
                //    {
                //        _rows[i, j] = buffer[bufferIndex++];
                //    }
                //}
            }
        }

        public void Align()
        {
            if (_columns != null && _rows != null)
            {
                var rowCount = _rows.Length / _columns.Length;
                var buffer = new object[_columns.Length - 1];
                var bufferIndex = 0;

                for (var i = 0; i < rowCount; i++)
                {
                    bufferIndex = 0;
                    for (var j = 1; j < _columns.Length; j++)
                    {
                        var x = _rows[i, j];
                        if (x == null)
                        {
                            _rows[i, j] = buffer[bufferIndex];
                        }
                        else
                        {
                            buffer[bufferIndex] = _rows[i, j];
                        }

                        bufferIndex++;
                    }
                }
            }
        }


        private static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
        {
            var newArray = new T[rows, cols];
            int minRows = Math.Min(rows, original.GetLength(0));
            int minCols = Math.Min(cols, original.GetLength(1));
            for (int i = 0; i < minRows; i++)
                for (int j = 0; j < minCols; j++)
                    newArray[i, j] = original[i, j];
            return newArray;
        }
    }
}
