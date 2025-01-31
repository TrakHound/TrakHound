namespace TrakHound.Tables
{
    public class TrakHoundTableColumn
    {
        public string Name { get; set; }

        public string DefinitionUuid { get; set; }

        public TrakHoundTableDataType DataType { get; set; }

        public int? DataLength { get; set; }

        public bool FixedLength { get; set; }

        public bool AllowNull { get; set; }


        public TrakHoundTableColumn() { }

        public TrakHoundTableColumn(string name, TrakHoundTableDataType dataType, int? dataLength = null, bool fixedLength = false, bool allowNull = true)
        {
            Name = name;
            DataType = dataType;
            DataLength = dataLength;
            FixedLength = fixedLength;
            AllowNull = allowNull;
        }
    }

    public enum TrakHoundTableDataType
    {
        String,
        Byte,
        Int16,
        Int32,
        Int64,
        Float,
        Timestamp,
        Duration
    }
}
